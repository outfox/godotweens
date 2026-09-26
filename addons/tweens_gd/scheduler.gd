# SPDX-License-Identifier: MIT
# SPDX-FileCopyrightText: 2026 Moritz Voss
class_name TweensGdScheduler
extends RefCounted
## Deterministic manual scheduler. Call dispose() when its lifetime ends.

signal error_reported(message: String)

const Types = preload("types.gd")
const Definition = preload("definition.gd")
const Handle = preload("handle.gd")
const Interpolation = preload("interpolation.gd")

var last_error: String = ""
var active_count: int:
	get:
		var count := 0
		for instance in _instances:
			if not instance.is_terminal: count += 1
		return count
var is_disposed: bool:
	get: return _disposed

var _instances: Array[Handle] = []
var _updating: bool = false
var _disposed: bool = false
var _ticks: Array[int] = [0, 0]
var _carry: Dictionary = {}

## Node targets bind to themselves. Other Objects can optionally bind to an owner.
## Always returns a handle. Invalid starts are already settled with Reason.FAILED.
func add(target: Variant, definition: Definition, owner: Variant = null) -> Handle:
	if not _main_thread(): return Handle.rejected("Use tweens.gd on Godot's main thread.")
	last_error = ""
	if _disposed: return _reject("The scheduler is disposed.")
	if typeof(target) != TYPE_OBJECT or not is_instance_valid(target): return _reject("The target is invalid.")
	if definition == null: return _reject("A definition is required.")
	var validation := definition.validate()
	if not validation.is_empty(): return _reject(validation)
	if typeof(owner) != TYPE_NIL and (typeof(owner) != TYPE_OBJECT or not is_instance_valid(owner) or not owner is Node):
		return _reject("The owner must be a valid Node.")
	if target is Node:
		if typeof(owner) != TYPE_NIL and owner != target: return _reject("Node targets use their own lifetime.")
		owner = target
	if typeof(owner) != TYPE_NIL and (not is_instance_valid(owner) or not owner.is_inside_tree() or owner.is_queued_for_deletion()):
		return _reject("The owner must be alive and inside the scene tree.")
	var snapshot := definition.copy()
	var initial: Variant = snapshot.initial_value if snapshot.property.is_empty() else Interpolation.read_property(target, snapshot.property)
	if not is_instance_valid(target) or (typeof(owner) != TYPE_NIL and (not is_instance_valid(owner) or not owner.is_inside_tree() or owner.is_queued_for_deletion())):
		return _reject("The target or owner became invalid while reading the property.")
	if not Interpolation.supported(initial): return _reject("The property is missing or its value type is unsupported.")
	for endpoint in [initial, snapshot.from_value, snapshot.to_value]:
		if endpoint == null: continue
		if not Interpolation.compatible(initial, endpoint) or not Interpolation.finite(endpoint):
			return _reject("Endpoints must be finite and match the property's value type.")
		if typeof(endpoint) == TYPE_QUATERNION and endpoint.length_squared() == 0.0:
			return _reject("Quaternion endpoints must have nonzero length.")
	var tree: SceneTree = owner.get_tree() if is_instance_valid(owner) else null
	var instance := Handle.new(self, target, snapshot, initial, owner, tree)
	if not _carry.is_empty() and _carry.mode == instance._mode and _carry.unscaled == instance._unscaled and _carry.tick == _ticks[instance._mode]:
		instance._clock.elapsed = _carry.seconds
	if _disposed:
		instance._finish(Types.Reason.RUNNER_DISPOSED)
		return instance
	_instances.append(instance)
	instance._initialize()
	return instance

## New playback created inside callbacks is first sampled on the next update.
func update(delta: float, unscaled_delta: float = -1.0, mode: int = Types.Process.PROCESS) -> void:
	if not _main_thread(): return
	if _disposed:
		_report("The scheduler is disposed.")
		return
	if _updating:
		_report("Recursive scheduler updates are not supported.")
		return
	if unscaled_delta == -1.0: unscaled_delta = delta
	if not is_finite(delta) or delta < 0.0 or not is_finite(unscaled_delta) or unscaled_delta < 0.0 or not Types.Process.values().has(mode):
		_report("Update requires finite nonnegative deltas and a valid process mode.")
		return
	_updating = true
	_ticks[mode] += 1
	var count := _instances.size()
	for index in range(count):
		if _disposed: break
		var instance := _instances[index]
		# Lifetime checks run even for paused tweens and the other process lane.
		if instance._check_target() and instance._mode == mode and instance._can_advance():
			instance._advance(unscaled_delta if instance._unscaled else delta)
	_updating = false
	_compact()

func cancel_all() -> void:
	if not _main_thread(): return
	for instance in _instances.duplicate(): instance.cancel()
	if not _updating: _compact()

func cancel_owner(owner: Node, include_children: bool = false) -> void:
	if not _main_thread() or not is_instance_valid(owner): return
	for instance in _instances.duplicate():
		if instance._owner == owner or (include_children and is_instance_valid(instance._owner) and owner.is_ancestor_of(instance._owner)):
			instance.cancel()
	if not _updating: _compact()

func dispose() -> void:
	if not _main_thread() or _disposed: return
	_disposed = true
	for instance in _instances.duplicate(): instance._finish(Types.Reason.RUNNER_DISPOSED)
	if not _updating: _instances.clear()
	_carry = {}

func _compact() -> void:
	var kept := 0
	for instance in _instances:
		if not instance.is_terminal:
			_instances[kept] = instance
			kept += 1
	_instances.resize(kept)

func _main_thread() -> bool:
	if OS.get_thread_caller_id() == OS.get_main_thread_id(): return true
	push_error("Use tweens.gd on Godot's main thread.")
	return false

func _reject(message: String) -> Handle:
	_report(message)
	return Handle.rejected(message)

func _report(message: String) -> void:
	last_error = message
	error_reported.emit(message)
