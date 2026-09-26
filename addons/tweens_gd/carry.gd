# SPDX-License-Identifier: MIT
# SPDX-FileCopyrightText: 2026 Moritz Voss
extends RefCounted
## Main-thread continuation scope, shared by handles and groups.
## Stamps reference their scheduler weakly, so completed work cannot retain it.

static var current: Dictionary = {}

static func enter(stamp: Dictionary) -> Dictionary:
	var previous := current
	current = stamp
	return previous

static func leave(previous: Dictionary) -> void:
	current = previous

static func credit(scheduler: RefCounted, mode: int, unscaled: bool, tick: int) -> float:
	if current.is_empty(): return 0.0
	if current.scheduler.get_ref() != scheduler or current.mode != mode or current.unscaled != unscaled or current.tick != tick:
		return 0.0
	return current.seconds
