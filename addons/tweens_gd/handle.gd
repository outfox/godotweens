# SPDX-License-Identifier: MIT
# SPDX-FileCopyrightText: 2026 Moritz Voss
class_name TweensGdHandle
extends RefCounted
## A single playback. Await wait(), which also works after playback has ended.

signal ended(reason: int)

const Types = preload("types.gd")
const Definition = preload("definition.gd")
const Playback = preload("playback.gd")
const Easing = preload("easing.gd")
const Interpolation = preload("interpolation.gd")

var target: Object:
	get: return _target
var state: int:
	get: return _state
var completion_reason: int:
	get: return _reason
var error: String:
	get: return _error
var value: Variant:
	get: return _value
var progress: float:
	get: return _clock.progress if _clock != null else 0.0
var is_terminal: bool:
	get: return _state >= Types.State.COMPLETED
var is_settled: bool:
	get: return _settled
var is_paused: bool = false:
	set(paused):
		if _main_thread(): is_paused = paused

var _target: Object
var _owner: Node
var _tree: SceneTree
var _scheduler: WeakRef
var _options: Definition
var _clock: Playback
var _initial: Variant
var _from: Variant
var _to: Variant
var _value: Variant
var _state: int = Types.State.DELAYED
var _reason: int = -1
var _error: String = ""
var _mode: int
var _pause_mode: int
var _unscaled: bool
var _started: bool = false
var _settled: bool = false
var _depth: int = 0
var _stamp: Dictionary = {}
var _target_is_node: bool
var _target_is_owner: bool

## A rejected start is already settled and owns no target, callbacks or scheduler.
static func rejected(message: String) -> TweensGdHandle:
	var detail := message if not message.is_empty() else "The tween could not be started."
	return TweensGdHandle.new(null, null, null, null, null, null, detail)

func _init(scheduler: RefCounted, target_object: Object, options: Definition,
		initial: Variant, lifetime_owner: Node, tree: SceneTree, rejection: String = "") -> void:
	if not rejection.is_empty():
		_state = Types.State.FAULTED
		_reason = Types.Reason.FAILED
		_error = rejection
		_settled = true
		return
	_scheduler = weakref(scheduler)
	_target = target_object
	_owner = lifetime_owner
	_target_is_node = target_object is Node
	_target_is_owner = target_object == lifetime_owner
	_tree = tree
	_options = options
	_clock = Playback.new(options)
	_initial = initial
	_value = initial
	_from = initial if options.from_value == null else options.from_value
	_to = initial if options.to_value == null else options.to_value
	_mode = options.process_mode
	_pause_mode = options.pause_mode
	_unscaled = options.use_unscaled_time

func pause() -> void:
	is_paused = true

func resume() -> void:
	is_paused = false

func cancel() -> void:
	if _main_thread(): _finish(Types.Reason.CANCELLED)

func wait() -> int:
	if not _main_thread(): return Types.Reason.FAILED
	if _settled: return _reason
	return await ended

func _main_thread() -> bool:
	if OS.get_thread_caller_id() == OS.get_main_thread_id(): return true
	push_error("Use tweens.gd on Godot's main thread.")
	return false

func _initialize() -> void:
	_depth += 1
	if _check_target():
		if is_instance_valid(_owner): _owner.tree_exiting.connect(_owner_exiting)
		_invoke(_options.on_add)
		if _check_target() and _options.delay > 0.0 and _options.fill & Types.Fill.APPLY_FROM_DURING_DELAY:
			_apply(_from)
	_end_operation()

func _advance(delta: float) -> void:
	_depth += 1
	_advance_inner(delta)
	_end_operation()

func _advance_inner(delta: float) -> void:
	_clock.advance(delta)
	_state = Types.State.PLAYING if _clock.completed else _clock.state
	if not _clock.started: return
	if not _started:
		_started = true
		if not _options.on_start.is_null():
			_invoke(_options.on_start)
			if not _check_target(): return
	var time := clampf(_clock.progress, 0.0, 1.0)
	if _options.skew != 1.0: time = pow(time, _options.skew)
	var weight: Variant
	if _options.curve != null:
		weight = _options.curve.sample(time)
	elif not _options.ease_function.is_null():
		if not _options.ease_function.is_valid():
			_fail("The easing Callable is no longer valid.")
			return
		weight = _options.ease_function.call(time)
		if not _check_target(): return
	else:
		weight = Easing.evaluate(_options.ease, time)
	var weight_type := typeof(weight)
	if (weight_type != TYPE_FLOAT and weight_type != TYPE_INT) or not is_finite(float(weight)):
		_fail("Easing must return a finite number.")
		return
	var sample: Variant = Interpolation.interpolate(_from, _to, weight, typeof(_initial))
	if not Interpolation.finite(sample):
		_fail("Interpolation produced a non-finite value.")
		return
	_apply(sample)
	if is_terminal or not _clock.completed: return
	if not _options.fill & Types.Fill.RETAIN_FINAL_VALUE: _apply(_initial)
	if not is_terminal: _finish(Types.Reason.COMPLETED)

func _apply(sample: Variant) -> void:
	# Callers have checked lifetimes after every preceding user callback.
	if not _options.property.is_empty():
		_target.set_indexed(_options.property, sample)
		_value = sample
		# Script setters may cancel or free the target.
		if not _check_target(): return
	_value = sample
	if not _options.on_update.is_null():
		if not _options.on_update.is_valid():
			_fail("A callback Callable is no longer valid.")
			return
		_options.on_update.call(self, sample)
		_check_target()

func _invalid_target() -> bool:
	return not is_instance_valid(_target) or (_target_is_node and _target.is_queued_for_deletion())

func _invalid_owner() -> bool:
	return _owner != null and (not is_instance_valid(_owner) or _owner.is_queued_for_deletion() or not _owner.is_inside_tree())

func _check_target() -> bool:
	if _state >= Types.State.COMPLETED: return false
	if _invalid_target(): _finish(Types.Reason.TARGET_FREED)
	elif not _target_is_owner and _invalid_owner(): _finish(Types.Reason.OWNER_EXITED)
	return _state < Types.State.COMPLETED

func _can_advance() -> bool:
	if is_paused: return false
	if _pause_mode == Types.Pause.ALWAYS: return true
	if is_instance_valid(_owner) and _pause_mode == Types.Pause.BOUND: return _owner.can_process()
	return not is_instance_valid(_tree) or not _tree.paused

func _owner_exiting() -> void:
	_finish(Types.Reason.TARGET_FREED if _owner == _target and _owner.is_queued_for_deletion() else Types.Reason.OWNER_EXITED)

func _invoke(callback: Callable, args: Array = []) -> void:
	if callback.is_null(): return
	if not callback.is_valid():
		_fail("A callback Callable is no longer valid.")
		return
	callback.callv([self] if args.is_empty() else args)

func _fail(message: String) -> void:
	if _settled: return
	_error = message
	if is_terminal:
		_state = Types.State.FAULTED
		_reason = Types.Reason.FAILED
	else:
		_finish(Types.Reason.FAILED)

func _finish(reason: int) -> void:
	if is_terminal: return
	_reason = reason
	_state = Types.State.COMPLETED if reason == Types.Reason.COMPLETED else Types.State.CANCELLED
	if reason == Types.Reason.FAILED: _state = Types.State.FAULTED
	var scheduler = _scheduler.get_ref()
	if reason == Types.Reason.COMPLETED and scheduler != null:
		_stamp = {"mode": _mode, "unscaled": _unscaled, "tick": scheduler._ticks[_mode], "seconds": _clock.overshoot}
	var previous: Dictionary = _enter_carry()
	var suppress := _options.suppress_callbacks_when_target_invalid and (
		_invalid_target() or _invalid_owner() or reason in [Types.Reason.TARGET_FREED, Types.Reason.OWNER_EXITED])
	if not suppress:
		if reason == Types.Reason.COMPLETED: _invoke(_options.on_end)
		elif reason != Types.Reason.FAILED: _invoke(_options.on_cancel)
		_invoke(_options.on_finally)
	if is_instance_valid(_owner) and _owner.tree_exiting.is_connected(_owner_exiting):
		_owner.tree_exiting.disconnect(_owner_exiting)
	_options = null
	_leave_carry(previous)
	if _depth == 0: _settle()

func _end_operation() -> void:
	_depth -= 1
	if is_terminal and _depth == 0: _settle()

func _settle() -> void:
	if _settled: return
	_settled = true
	var scheduler = _scheduler.get_ref()
	if not _error.is_empty() and scheduler != null: scheduler._report(_error)
	var previous: Dictionary = _enter_carry()
	ended.emit(_reason)
	# Completion is one-shot. Release observers, including closures capturing this handle.
	for connection in ended.get_connections():
		ended.disconnect(connection.callable)
	_leave_carry(previous)

func _enter_carry() -> Dictionary:
	var scheduler = _scheduler.get_ref()
	if scheduler == null: return {}
	var previous: Dictionary = scheduler._carry
	scheduler._carry = _stamp if _reason == Types.Reason.COMPLETED else {}
	return previous

func _leave_carry(previous: Dictionary) -> void:
	var scheduler = _scheduler.get_ref()
	if scheduler != null: scheduler._carry = previous
