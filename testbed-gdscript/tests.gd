# SPDX-License-Identifier: MIT
# SPDX-FileCopyrightText: 2026 Moritz Voss
extends Node

const T = preload("res://addons/tweens_gd/tweens.gd")
const Playback = preload("res://addons/tweens_gd/playback.gd")
const ErrorCollector = preload("error_collector.gd")
const Benchmark = preload("benchmark.gd")

var finished := false
var checks := 0
var failures: Array[String] = []
var _collector := ErrorCollector.new()
var _wait_results: Array[int] = []
var _sequence_handles: Array = []

class PropertyProbe extends Node:
	var when_written: Callable
	var when_read: Callable
	var amount: float = 0.0:
		set(next):
			amount = next
			if when_written.is_valid(): when_written.call()
		get:
			if when_read.is_valid(): when_read.call()
			return amount

class ResourceProbe extends RefCounted:
	var when_read: Callable
	var amount: float:
		get:
			when_read.call()
			return 0.0

func _ready() -> void:
	if "--run-tests" in OS.get_cmdline_user_args(): _run_standalone.call_deferred()

func _run_standalone() -> void:
	get_tree().create_timer(20.0).timeout.connect(func():
		if not finished:
			push_error("GDScript suite timed out.")
			get_tree().quit(1))
	await run_tests()
	for failure in failures: printerr(failure)
	print("GDScript: %d checks, %d failures." % [checks, failures.size()])
	get_tree().quit(0 if failures.is_empty() else 1)

func benchmark() -> String:
	OS.add_logger(_collector)
	var json := Benchmark.run(self)
	var errors := _collector.take_errors()
	OS.remove_logger(_collector)
	for message in errors: printerr(message)
	return json if errors.is_empty() else ""

func check(condition: bool, message: String) -> void:
	checks += 1
	if not condition: failures.append(message)

func near(actual: float, expected: float, message: String) -> void:
	check(absf(actual - expected) <= 0.000001, "%s: got %s, expected %s" % [message, actual, expected])

func run_tests() -> void:
	OS.add_logger(_collector)
	for test in [_conformance, _validation, _snapshots_and_fill, _interpolation,
			_callbacks, _setter_reentrancy, _lifetime, _pause_and_lanes, _detected_faults, _reference_cleanup]:
		check(test.call() == true, "Test returned normally: " + test.get_method())
	await _await_and_carry()
	await _automatic_runner()
	await _rejected_starts()
	failures.append_array(_collector.take_errors())
	OS.remove_logger(_collector)
	finished = true

func _conformance() -> bool:
	var data: Dictionary = JSON.parse_string(FileAccess.get_file_as_string("res://conformance/timelines.json"))
	for test in data.cases:
		var definition := T.Definition.new()
		for key in test.options: definition.set(key, test.options[key])
		check(definition.validate().is_empty(), test.name + " validates")
		var clock := Playback.new(definition)
		clock.elapsed = test.get("credit", 0.0)
		for sample in test.samples:
			clock.advance(sample.delta)
			near(clock.progress, sample.progress, test.name)
			check(clock.state == int(sample.state), test.name + " state")
			if sample.has("overshoot"): near(clock.overshoot, sample.overshoot, test.name + " overshoot")
	for sample in data.easing:
		near(T.Easing.evaluate(int(sample.ease), sample.t), sample.value, "ease %d" % sample.ease)
	for ease in T.Ease.values():
		near(T.Easing.evaluate(ease, 0.0), 0.0, "ease start")
		near(T.Easing.evaluate(ease, 1.0), 1.0, "ease end")
	var infinite := T.value(0.0, 1.0, 1.0)
	infinite.repeats = T.INFINITE
	var huge := Playback.new(infinite)
	huge.advance(Playback.MAX_TIME)
	huge.advance(Playback.MAX_TIME)
	check(not huge.completed and is_finite(huge.progress), "huge infinite deltas saturate")
	return true

func _validation() -> bool:
	var scheduler := T.Scheduler.new()
	var target := RefCounted.new()
	for field in ["duration", "delay", "offset", "ping_pong_interval", "repeat_interval"]:
		for invalid in [-1.0, INF, NAN]:
			var definition := T.value(0.0, 1.0, 1.0)
			definition.set(field, invalid)
			check(scheduler.add(target, definition).completion_reason == T.Reason.FAILED, "reject invalid " + field)
	for pair in [["offset", 2.0], ["repeats", -2], ["skew", 0.0], ["skew", INF],
			["fill", 4], ["ease", 999], ["pause_mode", 99], ["process_mode", 99]]:
		var definition := T.value(0.0, 1.0, 1.0)
		definition.set(pair[0], pair[1])
		check(scheduler.add(target, definition).completion_reason == T.Reason.FAILED, "reject " + pair[0])
	var zero := T.value(0.0, 1.0)
	zero.repeats = T.INFINITE
	check(scheduler.add(target, zero).completion_reason == T.Reason.FAILED, "reject zero infinite timeline")
	check(scheduler.add(target, T.value(Vector2.ZERO, Color.WHITE, 1.0)).completion_reason == T.Reason.FAILED, "reject mixed types")
	check(scheduler.add(target, T.value([], [], 1.0)).completion_reason == T.Reason.FAILED, "reject shared mutable endpoints")
	check(scheduler.add(target, T.value(Quaternion(0, 0, 0, 0), Quaternion.IDENTITY)).completion_reason == T.Reason.FAILED, "reject zero quaternion")
	check(scheduler.add(target, T.property(^"missing", 1.0)).completion_reason == T.Reason.FAILED, "reject missing property")
	check(scheduler.active_count == 0, "invalid starts do not register work")
	var valid := scheduler.add(target, T.value(0.0, 1.0, 1.0))
	scheduler.update(-0.1)
	near(valid.value, 0.0, "invalid update does not advance")
	scheduler.dispose()
	check(valid.completion_reason == T.Reason.RUNNER_DISPOSED, "dispose settles handles")
	check(scheduler.add(target, T.value(0.0, 1.0)).completion_reason == T.Reason.FAILED, "disposed scheduler rejects starts")
	return true

func _snapshots_and_fill() -> bool:
	var scheduler := T.Scheduler.new()
	var first := Node2D.new()
	var second := Node2D.new()
	add_child(first)
	add_child(second)
	first.position.x = 2.0
	second.position.x = 4.0
	var definition := T.property(^"position:x", 10.0, 1.0)
	var a := scheduler.add(first, definition)
	var b := scheduler.add(second, definition)
	definition.to_value = 100.0
	definition.duration = 100.0
	scheduler.update(0.5)
	near(first.position.x, 6.0, "first captures own initial value")
	near(second.position.x, 7.0, "second captures own initial value")
	a.cancel()
	b.cancel()
	var updates: Array = []
	var fill := T.property(^"position:x", 20.0, 1.0)
	fill.delay = 0.5
	fill.from_value = 1.0
	fill.fill = T.Fill.APPLY_FROM_DURING_DELAY
	fill.on_update = func(_h, v): updates.append(v)
	var filled := scheduler.add(first, fill)
	near(first.position.x, 1.0, "delay fills immediately")
	scheduler.update(1.5)
	check(updates == [1.0, 20.0, 6.0], "completion samples endpoint then restores captured value")
	check(filled.completion_reason == T.Reason.COMPLETED, "fill completes")
	var curve := Curve.new()
	curve.add_point(Vector2(0.0, 0.0))
	curve.add_point(Vector2(1.0, 1.0))
	var curved := T.value(0.0, 1.0, 1.0)
	curved.curve = curve
	var c := scheduler.add(self, curved)
	var expected := curve.sample(0.5)
	curve.set_point_value(1, 0.0)
	scheduler.update(0.5)
	near(c.value, expected, "curve is duplicated per playback")
	var skew := T.value(0.0, 1.0, 1.0)
	skew.skew = 2.0
	var d := scheduler.add(self, skew)
	scheduler.update(0.5)
	near(d.value, 0.25, "skew precedes easing")
	scheduler.dispose()
	first.free()
	second.free()
	return true

func _interpolation() -> bool:
	var scheduler := T.Scheduler.new()
	var target := RefCounted.new()
	for pair in [[0.0, 10.0, 5.0], [0, 3, 2], [Vector2.ZERO, Vector2(2, 4), Vector2(1, 2)],
			[Vector3.ZERO, Vector3(2, 4, 6), Vector3(1, 2, 3)], [Vector4.ZERO, Vector4(2, 4, 6, 8), Vector4(1, 2, 3, 4)],
			[Color(0, 0, 0, 0), Color.WHITE, Color(0.5, 0.5, 0.5, 0.5)],
			[Rect2(0, 0, 0, 0), Rect2(2, 4, 6, 8), Rect2(1, 2, 3, 4)]]:
		var h := scheduler.add(target, T.value(pair[0], pair[1], 1.0))
		scheduler.update(0.5)
		check(h.value == pair[2], "interpolate type %s: got %s, expected %s (state %s, error %s)" % [type_string(typeof(pair[0])), h.value, pair[2], h.state, h.error])
		h.cancel()
	var rotation := scheduler.add(target, T.value(Quaternion.IDENTITY, -Quaternion(Vector3.UP, PI / 2.0), 1.0))
	scheduler.update(0.5)
	check(rotation.value.is_equal_approx(Quaternion(Vector3.UP, PI / 4.0)), "quaternion uses shortest path")
	var integer := T.value(0, 9223372036854775807, 1.0)
	integer.ease_function = func(_t): return 2.0
	var saturated := scheduler.add(target, integer)
	scheduler.update(1.0)
	check(saturated.value == 9223372036854775807, "integer overshoot saturates without wrapping")
	var node := Node2D.new()
	add_child(node)
	scheduler.add(node, T.property(^"position:x", 10.0, 1.0))
	scheduler.add(node, T.property(^"position:y", 20.0, 1.0))
	scheduler.update(0.5)
	check(node.position == Vector2(5, 10), "component writes compose")
	scheduler.add(node, T.property(^"position:x", 100.0, 0.0))
	scheduler.update(0.0)
	near(node.position.x, 100.0, "insertion order determines last write")
	var material := StandardMaterial3D.new()
	var resource_tween := scheduler.add(material, T.property(^"roughness", 0.0, 1.0), node)
	scheduler.update(0.5)
	near(material.roughness, 0.5, "resource property writes")
	check(resource_tween != null, "resource tween uses separate owner")
	node.free()
	scheduler.dispose()
	return true

func _callbacks() -> bool:
	var scheduler := T.Scheduler.new()
	var events: Array[String] = []
	var definition := T.value(0.0, 1.0, 1.0)
	definition.on_add = func(_h): events.append("add")
	definition.on_start = func(_h): events.append("start")
	definition.on_update = func(_h, _v): events.append("update")
	definition.on_end = func(h):
		check(h.is_terminal, "terminal state visible in on_end")
		events.append("end")
		h.cancel()
	definition.on_finally = func(_h): events.append("finally")
	var handle := scheduler.add(self, definition)
	handle.ended.connect(func(_r): events.append("signal"))
	scheduler.update(2.0)
	handle.cancel()
	check(events == ["add", "start", "update", "end", "finally", "signal"], "terminal ordering/idempotence")
	events.clear()
	var cancel := T.value(0.0, 1.0)
	cancel.on_add = func(h): h.cancel()
	cancel.on_cancel = func(_h): events.append("cancel")
	cancel.on_finally = func(_h): events.append("finally")
	var cancelled := scheduler.add(self, cancel)
	check(cancelled.is_settled and events == ["cancel", "finally"], "cancel during on_add settles")
	var spawned: Array = []
	var parent := T.value(0.0, 1.0, 1.0)
	parent.on_update = func(h, _v):
		spawned.append(scheduler.add(self, T.value(0.0, 10.0, 1.0)))
		h.cancel()
		scheduler.update(99.0)
	scheduler.add(self, parent)
	scheduler.update(0.5)
	check(spawned.size() == 1 and spawned[0].value == 0.0, "callback additions wait for next tick")
	check(scheduler.last_error.contains("Recursive"), "recursive updates rejected")
	scheduler.update(0.5)
	near(spawned[0].value, 5.0, "spawned tween gets next delta")
	var dispose := T.value(0.0, 1.0)
	dispose.on_update = func(_h, _v): scheduler.dispose()
	scheduler.add(self, dispose)
	scheduler.update(0.0)
	check(scheduler.active_count == 0, "dispose during update cleans up all work")
	return true

func _lifetime() -> bool:
	var scheduler := T.Scheduler.new()
	var node := Node2D.new()
	add_child(node)
	var h := scheduler.add(node, T.property(^"position:x", 10.0, 1.0))
	h.pause()
	remove_child(node)
	check(h.completion_reason == T.Reason.OWNER_EXITED and h.is_settled, "paused owner exit settles immediately")
	node.free()
	var queued := Node2D.new()
	add_child(queued)
	var events: Array = []
	var def := T.property(^"position:x", 10.0, 1.0)
	def.suppress_callbacks_when_target_invalid = true
	def.on_cancel = func(_h): events.append("cancel")
	def.on_finally = func(_h): events.append("finally")
	var dying := scheduler.add(queued, def)
	dying.pause()
	queued.queue_free()
	scheduler.update(0.5, 0.5, T.Process.PHYSICS)
	check(dying.completion_reason == T.Reason.TARGET_FREED, "queued target checked across lanes and pause")
	near(queued.position.x, 0.0, "queued target never written")
	check(events.is_empty(), "invalid-target callback suppression")
	var object := Object.new()
	var freed := scheduler.add(object, T.value(0.0, 1.0, 1.0))
	object.free()
	scheduler.update(0.0)
	check(freed.completion_reason == T.Reason.TARGET_FREED, "freed Object detected")
	var owner := Node.new()
	add_child(owner)
	var resource := Gradient.new()
	var bound := scheduler.add(resource, T.value(0.0, 1.0, 1.0), owner)
	owner.free()
	check(bound.completion_reason == T.Reason.OWNER_EXITED, "resource follows owner lifetime")
	scheduler.dispose()
	return true

func _setter_reentrancy() -> bool:
	var scheduler := T.Scheduler.new()
	var target := PropertyProbe.new()
	add_child(target)
	var h := scheduler.add(target, T.property(^"amount", 10.0, 1.0))
	target.when_written = h.cancel
	scheduler.update(0.5)
	check(h.completion_reason == T.Reason.CANCELLED, "setter can cancel its own tween")
	near(h.value, 5.0, "last sample remains inspectable after setter cancellation")
	target.when_written = Callable()
	var exit := T.property(^"amount", 10.0, 1.0)
	exit.on_start = func(instance): instance.target.get_parent().remove_child(instance.target)
	var exited := scheduler.add(target, exit)
	scheduler.update(0.5)
	check(exited.completion_reason == T.Reason.OWNER_EXITED, "on_start exit prevents property write")
	near(target.amount, 5.0, "no write after on_start exit")
	add_child(target)
	target.when_read = func(): remove_child(target)
	check(scheduler.add(target, T.property(^"amount", 10.0, 1.0)).completion_reason == T.Reason.FAILED, "getter invalidation is rejected before lifetime binding")
	target.when_read = Callable()
	target.free()
	var holder: Array = []
	var ease := T.value(0.0, 1.0, 1.0)
	ease.ease_function = func(t):
		holder[0].cancel()
		return t
	holder.append(scheduler.add(self, ease))
	scheduler.update(0.5)
	check(holder[0].is_settled and holder[0].value == 0.0, "easing can cancel without later writes")
	holder.clear()
	var disposal := T.value(0.0, 1.0, 1.0)
	disposal.on_cancel = func(_h): scheduler.cancel_all()
	var first := scheduler.add(self, disposal)
	var second := scheduler.add(self, T.value(0.0, 1.0, 1.0))
	scheduler.dispose()
	check(first.is_settled and second.is_settled, "reentrant cancel_all during disposal settles all handles")
	return true

func _pause_and_lanes() -> bool:
	var scheduler := T.Scheduler.new()
	var node := Node.new()
	add_child(node)
	node.process_mode = Node.PROCESS_MODE_DISABLED
	var bound := scheduler.add(node, T.value(0.0, 1.0, 1.0))
	var tree_def := T.value(0.0, 1.0, 1.0)
	tree_def.pause_mode = T.Pause.SCENE_TREE
	var tree_bound := scheduler.add(node, tree_def)
	var always_def := tree_def.copy()
	always_def.pause_mode = T.Pause.ALWAYS
	always_def.process_mode = T.Process.PHYSICS
	always_def.use_unscaled_time = true
	var always := scheduler.add(node, always_def)
	scheduler.update(0.25, 0.5)
	near(bound.value, 0.0, "bound honors disabled owner")
	near(tree_bound.value, 0.25, "tree policy ignores owner process mode")
	near(always.value, 0.0, "physics tween ignores process lane")
	get_tree().paused = true
	scheduler.update(0.25, 0.5, T.Process.PHYSICS)
	near(always.value, 0.5, "always physics uses unscaled delta while paused")
	always.pause()
	scheduler.update(0.25, 0.5, T.Process.PHYSICS)
	near(always.value, 0.5, "instance pause wins")
	always.resume()
	scheduler.update(0.25)
	near(tree_bound.value, 0.25, "tree pause blocks scene-tree policy")
	get_tree().paused = false
	node.process_mode = Node.PROCESS_MODE_INHERIT
	node.set_process(false)
	scheduler.update(0.25)
	near(bound.value, 0.25, "set_process false does not disable bound tween")
	node.free()
	scheduler.dispose()
	return true

func _detected_faults() -> bool:
	var scheduler := T.Scheduler.new()
	var events: Array = []
	var def := T.value(0.0, 1.0, 1.0)
	def.ease_function = func(_t): return NAN
	def.on_finally = func(_h): events.append("finally")
	var faulty := scheduler.add(self, def)
	var healthy := scheduler.add(self, T.value(0.0, 1.0, 1.0))
	scheduler.update(1.0)
	check(faulty.state == T.State.FAULTED and faulty.completion_reason == T.Reason.FAILED, "invalid ease faults handle")
	check(faulty.is_settled and not faulty.error.is_empty() and events == ["finally"], "fault cleanup and diagnosis")
	check(healthy.completion_reason == T.Reason.COMPLETED, "fault does not stop other tweens")
	var callback_owner := Node.new()
	var stale := T.value(0.0, 1.0, 1.0)
	stale.on_start = callback_owner.set_process.bind(true).unbind(1)
	var h := scheduler.add(self, stale)
	callback_owner.free()
	scheduler.update(1.0)
	check(h.state == T.State.FAULTED and h.is_settled, "invalid Callable is detected")
	scheduler.dispose()
	return true

func _reference_cleanup() -> bool:
	var scheduler := T.Scheduler.new()
	var h := scheduler.add(self, T.value(0.0, 1.0, 1.0))
	var weak_handle: WeakRef = weakref(h)
	h = null
	scheduler.dispose()
	check(weak_handle.get_ref() == null, "scheduler releases handles on dispose")
	var weak_scheduler: WeakRef = weakref(scheduler)
	scheduler = null
	check(weak_scheduler.get_ref() == null, "no scheduler/handle ownership cycle")
	var retained_scheduler := T.Scheduler.new()
	var retained := retained_scheduler.add(self, T.value(0.0, 1.0))
	retained_scheduler.update(0.0)
	var weak_retained_scheduler: WeakRef = weakref(retained_scheduler)
	retained_scheduler = null
	check(weak_retained_scheduler.get_ref() == null and retained.is_settled, "finished handle does not retain scheduler")
	var observer_scheduler := T.Scheduler.new()
	var observed := observer_scheduler.add(self, T.value(0.0, 1.0))
	_capture_in_observer(observed)
	var weak_observed: WeakRef = weakref(observed)
	observer_scheduler.update(0.0)
	observed = null
	check(weak_observed.get_ref() == null, "completion releases observer closures capturing handle")
	observer_scheduler.dispose()
	return true

func _capture_in_observer(handle) -> void:
	handle.ended.connect(func(_reason): handle.cancel())

func _record_wait(handle) -> void:
	_wait_results.append(await handle.wait())

func _sequence(scheduler) -> void:
	var first = scheduler.add(self, T.value(0.0, 1.0, 0.25))
	_sequence_handles.append(first)
	await first.wait()
	_sequence_handles.append(scheduler.add(self, T.value(0.0, 1.0, 1.0)))

func _await_and_carry() -> void:
	var scheduler := T.Scheduler.new()
	var h := scheduler.add(self, T.value(0.0, 1.0, 1.0))
	_record_wait(h)
	_record_wait(h)
	check(_wait_results.is_empty(), "await suspends before completion")
	scheduler.update(1.0)
	check(_wait_results == [T.Reason.COMPLETED, T.Reason.COMPLETED], "multiple waiters resume")
	check(await h.wait() == T.Reason.COMPLETED, "late await returns cached reason")
	_sequence(scheduler)
	scheduler.update(0.5)
	check(_sequence_handles.size() == 2, "await continuation starts next step")
	near(_sequence_handles[1].value, 0.0, "continuation waits for next update")
	scheduler.update(0.0)
	near(_sequence_handles[1].value, 0.25, "continuation carries predecessor overshoot")
	var cancel := scheduler.add(self, T.value(0.0, 1.0, 1.0))
	_record_wait(cancel)
	cancel.cancel()
	check(_wait_results.back() == T.Reason.CANCELLED, "cancellation resumes waiters")
	scheduler.dispose()
	_sequence_handles.clear()

func _automatic_runner() -> void:
	var node := Node2D.new()
	add_child(node)
	var h := T.play(node, T.property(^"position:x", 10.0, 0.0))
	var runner = get_tree().get_meta(T.Runner.META_KEY)
	check(runner == T._runner(get_tree()), "one automatic runner per tree")
	check(not runner.is_inside_tree(), "runner attachment is deferred")
	await get_tree().process_frame
	await get_tree().process_frame
	check(runner.is_inside_tree() and runner.process_priority == 1000, "runner attached with correct priority")
	check(h.is_settled and node.position.x == 10.0, "automatic processing completes tween")
	var child := Node2D.new()
	node.add_child(child)
	var a := T.play(node, T.property(^"position:x", 0.0, 10.0))
	var b := T.play(child, T.property(^"position:x", 10.0, 10.0))
	T.cancel_tweens(node, true)
	check(a.is_settled and b.is_settled, "cancel_tweens includes descendants")
	var disposed := T.play(node, T.property(^"position:x", 1.0, 10.0))
	runner.free()
	check(disposed.completion_reason == T.Reason.RUNNER_DISPOSED, "runner teardown settles work")
	check(not get_tree().has_meta(T.Runner.META_KEY), "runner removes tree metadata")
	# Disposal before deferred attachment must settle work and remove its metadata too.
	var pending := T.play(node, T.property(^"position:x", 1.0, 10.0))
	var pending_runner = get_tree().get_meta(T.Runner.META_KEY)
	pending_runner.free()
	check(pending.completion_reason == T.Reason.RUNNER_DISPOSED, "pending runner teardown settles work")
	check(not get_tree().has_meta(T.Runner.META_KEY), "pending runner removes metadata")
	await get_tree().process_frame
	# A queued runner can be replaced; deleting the old runner must keep the new one.
	var old := T.play(node, T.property(^"position:x", 2.0, 10.0))
	var old_runner = get_tree().get_meta(T.Runner.META_KEY)
	old_runner.queue_free()
	var replacement := T.play(node, T.property(^"position:x", 3.0, 10.0))
	var replacement_runner = get_tree().get_meta(T.Runner.META_KEY)
	check(old_runner != replacement_runner, "queued runner is replaced")
	await get_tree().process_frame
	await get_tree().process_frame
	check(old.is_settled and not replacement.is_settled, "replacement outlives old runner")
	check(get_tree().get_meta(T.Runner.META_KEY) == replacement_runner, "old runner preserves replacement metadata")
	replacement_runner.free()
	node.free()

func _expected_rejection(start: Callable, message: String) -> T.Handle:
	# Keep unrelated errors visible; only consume the expected diagnostic for this call.
	failures.append_array(_collector.take_errors())
	var handle: T.Handle = start.call()
	var errors := _collector.take_errors()
	check(handle != null, message + " returns a handle")
	check(errors.size() == 1, message + " reports exactly one error")
	if handle != null and errors.size() == 1:
		check(errors[0].contains(handle.error), message + " preserves diagnostic details")
	return handle

func _rejected_starts() -> void:
	var definition := T.value(0.0, 1.0, -1.0)
	var callbacks: Array = []
	definition.on_add = func(_h): callbacks.append("add")
	definition.on_finally = func(_h): callbacks.append("finally")
	var scheduler := T.Scheduler.new()
	var diagnostics: Array[String] = []
	scheduler.error_reported.connect(func(message): diagnostics.append(message))
	var rejected := scheduler.add(self, definition)
	check(rejected.is_terminal and rejected.is_settled, "rejected start settles without a tick")
	check(rejected.state == T.State.FAULTED and rejected.completion_reason == T.Reason.FAILED, "rejection is a failure, not successful playback")
	check(diagnostics == [rejected.error] and scheduler.last_error == rejected.error, "manual rejection retains scheduler diagnostics")
	check(callbacks.is_empty() and scheduler.active_count == 0, "rejection runs no definition callbacks and registers no work")
	check(rejected.target == null and rejected.value == null and rejected.progress == 0.0, "dummy inspection is safe and retains no target")
	var after_signal: Array = []
	rejected.ended.connect(func(reason): after_signal.append(reason))
	rejected.pause()
	rejected.resume()
	rejected.cancel()
	check(await rejected.wait() == T.Reason.FAILED, "await rejected handle completes immediately")
	check(await rejected.wait() == T.Reason.FAILED, "repeated waits return cached failure")
	scheduler.update(1.0)
	check(after_signal.is_empty() and callbacks.is_empty(), "settled rejection cannot run callbacks or end a second time")
	scheduler.dispose()
	check(await scheduler.add(self, definition).wait() == T.Reason.FAILED, "disposed scheduler still returns an awaitable handle")
	var getter_scheduler := T.Scheduler.new()
	var owner := Node.new()
	add_child(owner)
	var resource := ResourceProbe.new()
	resource.when_read = func(): owner.free()
	var getter_failure := getter_scheduler.add(resource, T.property(^"amount", 1.0), owner)
	check(await getter_failure.wait() == T.Reason.FAILED, "owner freed by a getter still returns a failed handle")
	getter_scheduler.dispose()
	var detached := Node2D.new()
	var starts: Array[Callable] = [
		func(): return T.play(null, definition),
		func(): return T.play(42, definition),
		func():
			var dead := Node2D.new()
			dead.free()
			return T.play(dead, definition),
		func():
			var dead_owner := Node.new()
			dead_owner.free()
			return T.play(RefCounted.new(), definition, dead_owner),
		func(): return T.play(detached, definition),
		func(): return T.play(RefCounted.new(), definition),
		func(): return T.play(self, definition),
		func(): return T.play(self, null),
		func(): return T.play(self, T.property(^"missing", 1.0)),
		func(): return T.play(self, T.value(0.0, 1.0), get_tree().root),
	]
	for index in range(starts.size()):
		var failed := _expected_rejection(starts[index], "automatic rejection %d" % index)
		check(failed.is_settled and await failed.wait() == T.Reason.FAILED, "automatic rejection is safely awaitable without a tick")
	check(callbacks.is_empty(), "automatic rejections run no definition callbacks")
	detached.free()
	var runner = get_tree().get_meta(T.Runner.META_KEY)
	check(runner.scheduler.active_count == 0, "automatic rejection schedules no work")
	runner.free()
	get_tree().set_meta(T.Runner.CLOSING_KEY, true)
	var closing := _expected_rejection(func(): return T.play(self, definition), "closing tree")
	get_tree().remove_meta(T.Runner.CLOSING_KEY)
	check(await closing.wait() == T.Reason.FAILED, "closing tree needs no future tick to settle rejection")
	check(not get_tree().has_meta(T.Runner.META_KEY), "closing tree rejection creates no runner")
	# The API rejects worker-thread use before touching scene state, but still returns a handle.
	var worker := Thread.new()
	var from_worker := _expected_rejection(func():
		worker.start(func(): return T.play(null, null))
		return worker.wait_to_finish(), "worker thread")
	check(await from_worker.wait() == T.Reason.FAILED, "worker-thread rejection is inspectable on main thread")
