# SPDX-License-Identifier: MIT
# SPDX-FileCopyrightText: 2026 Moritz Voss
class_name TweensGdDefinition
extends RefCounted
## Reusable configuration. Each start copies it; null endpoints use the captured value.

const Types = preload("types.gd")

var property: NodePath
var from_value: Variant = null
var to_value: Variant = null
## Initial value for callback-only tweens (an empty property path).
var initial_value: Variant = 0.0
var duration: float = 0.0
var delay: float = 0.0
var offset: float = 0.0
var repeats: int = 0
var use_ping_pong: bool = false
var ping_pong_interval: float = 0.0
var repeat_interval: float = 0.0
var fill: int = Types.Fill.RETAIN_FINAL_VALUE
var ease: int = Types.Ease.LINEAR
var skew: float = 1.0
var ease_function: Callable
var curve: Curve
var process_mode: int = Types.Process.PROCESS
var pause_mode: int = Types.Pause.BOUND
var use_unscaled_time: bool = false
var suppress_callbacks_when_target_invalid: bool = false
## Callbacks take the handle, except on_update(handle, value).
var on_add: Callable
var on_start: Callable
var on_update: Callable
var on_end: Callable
var on_cancel: Callable
var on_finally: Callable

func copy() -> TweensGdDefinition:
	var result := get_script().new() as TweensGdDefinition
	result.property = property
	result.from_value = from_value
	result.to_value = to_value
	result.initial_value = initial_value
	result.duration = duration
	result.delay = delay
	result.offset = offset
	result.repeats = repeats
	result.use_ping_pong = use_ping_pong
	result.ping_pong_interval = ping_pong_interval
	result.repeat_interval = repeat_interval
	result.fill = fill
	result.ease = ease
	result.skew = skew
	result.ease_function = ease_function
	result.process_mode = process_mode
	result.pause_mode = pause_mode
	result.use_unscaled_time = use_unscaled_time
	result.suppress_callbacks_when_target_invalid = suppress_callbacks_when_target_invalid
	result.on_add = on_add
	result.on_start = on_start
	result.on_update = on_update
	result.on_end = on_end
	result.on_cancel = on_cancel
	result.on_finally = on_finally
	if curve != null:
		result.curve = curve.duplicate() as Curve
	return result

## Returns an empty string on success. No partial playback is created on failure.
func validate() -> String:
	for seconds in [duration, delay, offset, ping_pong_interval, repeat_interval]:
		if not is_finite(seconds) or seconds < 0.0:
			return "Timing must be finite and nonnegative."
	if offset > duration: return "Offset must not exceed duration."
	if repeats < Types.INFINITE: return "Repeats must be -1 or nonnegative."
	if not is_finite(skew) or skew <= 0.0: return "Skew must be finite and positive."
	if not Types.Ease.values().has(ease): return "Unknown easing function."
	if not Types.Process.values().has(process_mode) or not Types.Pause.values().has(pause_mode):
		return "Unknown process or pause mode."
	if fill < 0 or fill > Types.Fill.BOTH: return "Unknown fill flags."
	if curve != null and not ease_function.is_null(): return "Choose either curve or ease_function."
	for callback in [ease_function, on_add, on_start, on_update, on_end, on_cancel, on_finally]:
		if not callback.is_null() and not callback.is_valid(): return "A configured Callable is invalid."
	var span := duration + (duration + ping_pong_interval if use_ping_pong else 0.0) + repeat_interval
	if not is_finite(span + delay): return "Timeline is too long."
	if repeats == Types.INFINITE:
		if span == 0.0: return "An infinite tween needs a nonzero cycle duration."
	elif not is_finite(span * (float(repeats) + 1.0) - repeat_interval + delay):
		return "Timeline is too long."
	return ""
