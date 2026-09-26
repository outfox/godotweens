# SPDX-License-Identifier: MIT
# SPDX-FileCopyrightText: 2026 Moritz Voss
extends RefCounted
## Desktop microbenchmark, not a frame budget or a browser performance claim.
const T = preload("res://addons/tweens_gd/tweens.gd")
const WARMUP := 30
const SAMPLES := 120

static func run(owner: Node) -> String:
	var results: Array = []
	for workload in ["value", "position", "color", "resource"]:
		for count in [100, 1000, 10000]:
			for backend in ["gdscript", "godot_tween"]:
				results.append(_measure(owner, workload, count, backend))
	return JSON.stringify({
		"engine": Engine.get_version_info(), "os": OS.get_name(), "cpu": OS.get_processor_name(),
		"debug_build": OS.is_debug_build(), "warmup": WARMUP, "samples": SAMPLES,
		"delta_seconds": 1.0 / 60.0, "duration_seconds": 1000.0,
		"notes": "Manual linear updates, no rendering. Godot uses one parallel Tween. Creation excludes target creation; cleanup includes disposal. Not C# comparison, full-frame or web certification.",
		"results": results,
	}, "\t")

static func _measure(owner: Node, workload: String, count: int, backend: String) -> Dictionary:
	var targets: Array[Object] = []
	var definition: T.Definition
	match workload:
		"value": definition = T.value(0.0, 1.0, 1000.0)
		"position": definition = T.property(^"position", Vector2(100, 200), 1000.0)
		"color": definition = T.property(^"modulate", Color(0, 0.5, 1, 0), 1000.0)
		"resource": definition = T.property(^"roughness", 0.0, 1000.0)
	if workload == "value": definition.on_update = _consume_value
	for index in range(count):
		if workload in ["position", "color"]:
			var node := Node2D.new()
			owner.add_child(node)
			targets.append(node)
		elif workload == "resource": targets.append(StandardMaterial3D.new())
		else: targets.append(RefCounted.new())
	var scheduler := T.Scheduler.new()
	var builtin: Tween
	var start := Time.get_ticks_usec()
	if backend == "gdscript":
		for target in targets: scheduler.add(target, definition)
	else:
		builtin = owner.create_tween().set_parallel(true)
		builtin.pause()
		for target in targets:
			if workload == "value": builtin.tween_method(_consume, 0.0, 1.0, 1000.0)
			else: builtin.tween_property(target, definition.property, definition.to_value, 1000.0)
	var create_us := Time.get_ticks_usec() - start
	var samples: Array[int] = []
	for frame in range(WARMUP + SAMPLES):
		start = Time.get_ticks_usec()
		if backend == "gdscript": scheduler.update(1.0 / 60.0)
		else: builtin.custom_step(1.0 / 60.0)
		var elapsed := Time.get_ticks_usec() - start
		if frame >= WARMUP: samples.append(elapsed)
	samples.sort()
	start = Time.get_ticks_usec()
	scheduler.dispose()
	if builtin != null: builtin.kill()
	var dispose_us := Time.get_ticks_usec() - start
	for target in targets:
		if target is Node: target.free()
	return {"workload": workload, "count": count, "backend": backend,
		"create_us": create_us, "dispose_us": dispose_us,
		"median_update_us": samples[SAMPLES / 2], "p95_update_us": samples[int(SAMPLES * 0.95) - 1]}

static func _consume_value(_handle, _value) -> void:
	pass

static func _consume(_value) -> void:
	pass
