# SPDX-License-Identifier: MIT
# SPDX-FileCopyrightText: 2026 Moritz Voss
# SPDX-FileCopyrightText: 2020 Jeffrey Lanters
# Adapted from unity-tweens; see THIRD-PARTY-NOTICES.md.
class_name TweensGdEasing
extends RefCounted

const Types = preload("types.gd")
const A := 1.70158
const B := A * 1.525
const C := A + 1.0
const D := TAU / 3.0
const E := TAU / 4.5

static func evaluate(ease: int, progress: float) -> float:
	var t := clampf(progress, 0.0, 1.0)
	match ease:
		Types.Ease.LINEAR: return t
		Types.Ease.SMOOTH_STEP: return t * t * (3.0 - 2.0 * t)
		Types.Ease.SMOOTHER_STEP: return t * t * t * (t * (6.0 * t - 15.0) + 10.0)
		Types.Ease.SINE_IN: return 1.0 - cos(t * PI / 2.0)
		Types.Ease.SINE_OUT: return sin(t * PI / 2.0)
		Types.Ease.SINE_IN_OUT: return -(cos(PI * t) - 1.0) / 2.0
		Types.Ease.QUAD_IN: return t * t
		Types.Ease.QUAD_OUT: return 1.0 - (1.0 - t) * (1.0 - t)
		Types.Ease.QUAD_IN_OUT: return 2.0 * t * t if t < 0.5 else 1.0 - pow(-2.0 * t + 2.0, 2.0) / 2.0
		Types.Ease.CUBIC_IN: return t * t * t
		Types.Ease.CUBIC_OUT: return 1.0 - pow(1.0 - t, 3.0)
		Types.Ease.CUBIC_IN_OUT: return 4.0 * t * t * t if t < 0.5 else 1.0 - pow(-2.0 * t + 2.0, 3.0) / 2.0
		Types.Ease.QUART_IN: return pow(t, 4.0)
		Types.Ease.QUART_OUT: return 1.0 - pow(1.0 - t, 4.0)
		Types.Ease.QUART_IN_OUT: return 8.0 * pow(t, 4.0) if t < 0.5 else 1.0 - pow(-2.0 * t + 2.0, 4.0) / 2.0
		Types.Ease.QUINT_IN: return pow(t, 5.0)
		Types.Ease.QUINT_OUT: return 1.0 - pow(1.0 - t, 5.0)
		Types.Ease.QUINT_IN_OUT: return 16.0 * pow(t, 5.0) if t < 0.5 else 1.0 - pow(-2.0 * t + 2.0, 5.0) / 2.0
		Types.Ease.EXPO_IN: return 0.0 if t == 0.0 else pow(2.0, 10.0 * t - 10.0)
		Types.Ease.EXPO_OUT: return 1.0 if t == 1.0 else 1.0 - pow(2.0, -10.0 * t)
		Types.Ease.EXPO_IN_OUT:
			if t == 0.0 or t == 1.0: return t
			return pow(2.0, 20.0 * t - 10.0) / 2.0 if t < 0.5 else (2.0 - pow(2.0, -20.0 * t + 10.0)) / 2.0
		Types.Ease.CIRC_IN: return 1.0 - sqrt(1.0 - t * t)
		Types.Ease.CIRC_OUT: return sqrt(1.0 - pow(t - 1.0, 2.0))
		Types.Ease.CIRC_IN_OUT: return (1.0 - sqrt(1.0 - pow(2.0 * t, 2.0))) / 2.0 if t < 0.5 else (sqrt(1.0 - pow(-2.0 * t + 2.0, 2.0)) + 1.0) / 2.0
		Types.Ease.BACK_IN: return C * t * t * t - A * t * t
		Types.Ease.BACK_OUT: return 1.0 + C * pow(t - 1.0, 3.0) + A * pow(t - 1.0, 2.0)
		Types.Ease.BACK_IN_OUT: return pow(2.0 * t, 2.0) * ((B + 1.0) * 2.0 * t - B) / 2.0 if t < 0.5 else (pow(2.0 * t - 2.0, 2.0) * ((B + 1.0) * (t * 2.0 - 2.0) + B) + 2.0) / 2.0
		Types.Ease.ELASTIC_IN:
			return t if t == 0.0 or t == 1.0 else -pow(2.0, 10.0 * t - 10.0) * sin((t * 10.0 - 10.75) * D)
		Types.Ease.ELASTIC_OUT:
			return t if t == 0.0 or t == 1.0 else pow(2.0, -10.0 * t) * sin((t * 10.0 - 0.75) * D) + 1.0
		Types.Ease.ELASTIC_IN_OUT:
			if t == 0.0 or t == 1.0: return t
			return -pow(2.0, 20.0 * t - 10.0) * sin((20.0 * t - 11.125) * E) / 2.0 if t < 0.5 else pow(2.0, -20.0 * t + 10.0) * sin((20.0 * t - 11.125) * E) / 2.0 + 1.0
		Types.Ease.BOUNCE_IN: return 1.0 - _bounce_out(1.0 - t)
		Types.Ease.BOUNCE_OUT: return _bounce_out(t)
		Types.Ease.BOUNCE_IN_OUT: return (1.0 - _bounce_out(1.0 - 2.0 * t)) / 2.0 if t < 0.5 else (1.0 + _bounce_out(2.0 * t - 1.0)) / 2.0
	return NAN

static func _bounce_out(t: float) -> float:
	if t < 1.0 / 2.75: return 7.5625 * t * t
	if t < 2.0 / 2.75:
		t -= 1.5 / 2.75
		return 7.5625 * t * t + 0.75
	if t < 2.5 / 2.75:
		t -= 2.25 / 2.75
		return 7.5625 * t * t + 0.9375
	t -= 2.625 / 2.75
	return 7.5625 * t * t + 0.984375
