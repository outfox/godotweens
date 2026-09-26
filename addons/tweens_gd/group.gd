# SPDX-License-Identifier: MIT
# SPDX-FileCopyrightText: 2026 Moritz Voss
class_name TweensGdGroup
extends RefCounted
## Existing tweens that finish as one step. Await wait(), including after settlement.

signal ended(reason: int)

const Types = preload("types.gd")
const Handle = preload("handle.gd")
const Carry = preload("carry.gd")

var members: Array[Handle]:
	get: return _members.duplicate()
var is_terminal: bool:
	get: return _watcher.settled
var is_settled: bool:
	get: return _watcher.settled
var completion_reason: int:
	get: return _watcher.reason
var error: String:
	get: return "\n".join(_watcher.errors)
var errors: Array[String]:
	get: return _watcher.errors.duplicate()
var is_paused: bool:
	get:
		var active := false
		for member in _members:
			if member.is_terminal: continue
			active = true
			if not member.is_paused: return false
		return active
	set(paused):
		if not _main_thread(): return
		for member in _members:
			if not member.is_terminal: member.is_paused = paused

var _members: Array[Handle] = []
var _watcher: Watcher

## Invalid input returns an already-settled FAILED group without touching members.
## Repeated handles are included once, preserving their first occurrence's order.
static func of(tweens: Variant) -> TweensGdGroup:
	if OS.get_thread_caller_id() != OS.get_main_thread_id():
		return _reject("Use tweens.gd on Godot's main thread.")
	if not tweens is Array or tweens.is_empty():
		return _reject("A group needs a nonempty Array of tween handles.")
	var unique: Array[Handle] = []
	var seen: Dictionary = {}
	for member in tweens:
		if not is_instance_valid(member) or not member is Handle:
			return _reject("Every group member must be a tween handle.")
		var id: int = member.get_instance_id()
		if seen.has(id): continue
		seen[id] = true
		unique.append(member)
	return TweensGdGroup.new(unique)

static func _reject(message: String) -> TweensGdGroup:
	push_error("tweens.gd: " + message)
	return TweensGdGroup.new([], message)

func _init(tweens: Array[Handle] = [], rejection: String = "") -> void:
	_members = tweens.duplicate()
	_watcher = Watcher.new(_members, rejection)
	if not _watcher.settled: _watcher.ended.connect(_on_ended)

func pause() -> void:
	is_paused = true

func resume() -> void:
	is_paused = false

func cancel() -> void:
	if not _main_thread(): return
	for member in _members: member.cancel()

func wait() -> int:
	if not _main_thread(): return Types.Reason.FAILED
	if is_settled: return completion_reason
	return await ended

func _main_thread() -> bool:
	if OS.get_thread_caller_id() == OS.get_main_thread_id(): return true
	push_error("Use tweens.gd on Godot's main thread.")
	return false

func _on_ended(reason: int) -> void:
	ended.emit(reason)
	for connection in ended.get_connections(): ended.disconnect(connection.callable)

# Handles retain this coordinator through their signal bindings, but it only
# weakly references them. Thus fail-fast cancellation survives dropping the group,
# without forming a group/handle cycle or keeping a discarded scheduler alive.
class Watcher extends RefCounted:
	signal ended(reason: int)
	var settled := false
	var reason := -1
	var errors: Array[String] = []
	var _members: Array[WeakRef] = []
	var _pending: Array[bool] = []
	var _remaining: int
	var _stopping := false
	var _stamp: Dictionary = {}
	var _same_clock := true

	func _init(tweens: Array[Handle], rejection: String) -> void:
		if not rejection.is_empty() or tweens.is_empty():
			settled = true
			reason = Types.Reason.FAILED
			errors.append(rejection if not rejection.is_empty() else "A group needs at least one tween.")
			return
		_remaining = tweens.size()
		_pending.resize(_remaining)
		_pending.fill(true)
		# Subscribe to every pending member before processing any settled failures.
		for index in range(tweens.size()):
			var member := tweens[index]
			_members.append(weakref(member))
			if not member.is_settled: member.ended.connect(_deliver.bind(self, index))
		for index in range(tweens.size()):
			if tweens[index].is_settled: _accept(index)

	func _deliver(_reason: int, _keep_alive: RefCounted, index: int) -> void:
		_accept(index)

	func _accept(index: int) -> void:
		# Cancellation can recursively settle members that construction also visits.
		if not _pending[index]: return
		_pending[index] = false
		_remaining -= 1
		var member: Handle = _members[index].get_ref()
		if not member.error.is_empty(): errors.append(member.error)
		if member.completion_reason != Types.Reason.COMPLETED:
			if reason == -1: reason = member.completion_reason
			if not _stopping:
				_stopping = true
				for reference in _members:
					var sibling: Handle = reference.get_ref()
					if sibling != null and not sibling.is_terminal: sibling.cancel()
		else:
			_include_stamp(member._stamp)
		if _remaining == 0 and not settled:
			settled = true
			if reason == -1: reason = Types.Reason.COMPLETED
			var previous := Carry.enter(_stamp if _same_clock and reason == Types.Reason.COMPLETED else {})
			ended.emit(reason)
			for connection in ended.get_connections(): ended.disconnect(connection.callable)
			Carry.leave(previous)

	func _include_stamp(stamp: Dictionary) -> void:
		if not _same_clock: return
		if stamp.is_empty():
			_same_clock = false
			return
		if _stamp.is_empty():
			_stamp = stamp
			return
		if stamp.scheduler.get_ref() != _stamp.scheduler.get_ref() or stamp.mode != _stamp.mode or stamp.unscaled != _stamp.unscaled:
			_same_clock = false
			return
		# Latest tick wins; on the same tick, the smallest overshoot finished last.
		if stamp.tick > _stamp.tick or (stamp.tick == _stamp.tick and stamp.seconds < _stamp.seconds):
			_stamp = stamp
