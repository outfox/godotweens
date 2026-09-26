# SPDX-License-Identifier: MIT
# SPDX-FileCopyrightText: 2026 Moritz Voss
extends RefCounted
## Internal constant-time timeline, shared behavioral contract with the C# Playback.

const Types = preload("types.gd")
const Definition = preload("definition.gd")
const MAX_TIME := 1.7976931348623157e308

var progress: float = 0.0
var started: bool = false
var completed: bool = false
var overshoot: float = 0.0
var state: int = Types.State.DELAYED
var elapsed: float = 0.0
var _duration: float
var _delay: float
var _turn: float
var _offset: float
var _span: float
var _total: float
var _ping_pong: bool

func _init(options: Definition) -> void:
	_duration = options.duration
	_delay = options.delay
	_turn = options.ping_pong_interval
	_offset = options.offset
	_ping_pong = options.use_ping_pong
	_span = _duration + (_turn + _duration if _ping_pong else 0.0) + options.repeat_interval
	_total = INF if options.repeats == Types.INFINITE else _span * (float(options.repeats) + 1.0) - options.repeat_interval

func advance(delta: float) -> void:
	elapsed = minf(MAX_TIME, elapsed + delta)
	if elapsed < _delay: return
	started = true
	var time := minf(MAX_TIME, elapsed - _delay + _offset)
	if time >= _total:
		progress = 0.0 if _ping_pong else 1.0
		overshoot = time - _total
		completed = true
		state = Types.State.COMPLETED
		return
	var local := fmod(time, _span)
	if local == 0.0 and time > 0.0: local = _span
	state = Types.State.PLAYING
	if _duration > 0.0 and local <= _duration:
		progress = local / _duration
	elif not _ping_pong:
		progress = 1.0
		state = Types.State.INTERVAL
	elif local < _duration + _turn:
		progress = 1.0
		state = Types.State.INTERVAL
	elif _duration > 0.0 and local <= _duration * 2.0 + _turn:
		progress = 1.0 - (local - _duration - _turn) / _duration
	else:
		progress = 0.0
		state = Types.State.INTERVAL
