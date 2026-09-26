# SPDX-License-Identifier: MIT
# SPDX-FileCopyrightText: 2026 Moritz Voss
extends RefCounted

static func supported(value: Variant) -> bool:
	return typeof(value) in [TYPE_INT, TYPE_FLOAT, TYPE_VECTOR2, TYPE_VECTOR3,
		TYPE_VECTOR4, TYPE_COLOR, TYPE_QUATERNION, TYPE_RECT2]

static func compatible(initial: Variant, endpoint: Variant) -> bool:
	if typeof(initial) in [TYPE_INT, TYPE_FLOAT]:
		return typeof(endpoint) in [TYPE_INT, TYPE_FLOAT]
	return typeof(initial) == typeof(endpoint)

static func finite(value: Variant) -> bool:
	match typeof(value):
		TYPE_INT: return true
		TYPE_FLOAT: return is_finite(value)
		TYPE_VECTOR2, TYPE_VECTOR3, TYPE_VECTOR4, TYPE_QUATERNION:
			return value.is_finite()
		TYPE_COLOR: return is_finite(value.r) and is_finite(value.g) and is_finite(value.b) and is_finite(value.a)
		TYPE_RECT2: return value.position.is_finite() and value.size.is_finite()
	return false

static func interpolate(a: Variant, b: Variant, weight: float, value_type: int) -> Variant:
	match value_type:
		TYPE_INT:
			# Preserve exact integer endpoints and saturate overshoot at Variant int64 limits.
			if weight == 0.0 and typeof(a) == TYPE_INT: return a
			if weight == 1.0 and typeof(b) == TYPE_INT: return b
			var number := roundf(lerpf(float(a), float(b), weight))
			if number >= 9223372036854775807.0: return 9223372036854775807
			if number <= -9223372036854775808.0: return -9223372036854775807 - 1
			return int(number)
		TYPE_FLOAT: return lerpf(float(a), float(b), weight)
		TYPE_QUATERNION: return a.normalized().slerp(b.normalized(), weight).normalized()
		TYPE_RECT2: return Rect2(a.position.lerp(b.position, weight), a.size.lerp(b.size, weight))
		_: return a.lerp(b, weight)

## Restrict paths to properties on the target and value components. Crossing into
## another Object would need that object's own lifetime and restoration policy.
static func read_property(target: Object, path: NodePath) -> Variant:
	var parts := String(path).split(":")
	if parts.is_empty() or parts[0].is_empty(): return null
	var found := false
	for entry in target.get_property_list():
		if entry.name == StringName(parts[0]):
			found = true
			break
	if not found: return null
	var value: Variant = target.get(parts[0])
	for index in range(1, parts.size()):
		var components: Array = []
		match typeof(value):
			TYPE_VECTOR2: components = ["x", "y"]
			TYPE_VECTOR3: components = ["x", "y", "z"]
			TYPE_VECTOR4, TYPE_QUATERNION: components = ["x", "y", "z", "w"]
			TYPE_COLOR: components = ["r", "g", "b", "a"]
			TYPE_RECT2: components = ["position", "size", "end"]
		if not components.has(parts[index]): return null
		value = value[parts[index]]
	return value
