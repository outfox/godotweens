# SPDX-License-Identifier: MIT
# SPDX-FileCopyrightText: 2026 Moritz Voss
extends Node

const Scheduler = preload("scheduler.gd")
const Types = preload("types.gd")
const META_KEY := &"_tweens_gd_runner"
const CLOSING_KEY := &"_tweens_gd_closing"

var scheduler := Scheduler.new()
var _last_ticks: int
var _tree: SceneTree

func _init() -> void:
	name = "TweensGd"
	process_mode = Node.PROCESS_MODE_ALWAYS
	process_priority = 1000
	process_physics_priority = 1000
	_last_ticks = Time.get_ticks_usec()
	scheduler.error_reported.connect(_report_error)

func bind_tree(tree: SceneTree) -> void:
	_tree = tree
	tree.root.tree_exiting.connect(_tree_exiting)
	attach.call_deferred(tree)

func attach(tree: SceneTree) -> void:
	if not is_instance_valid(tree) or not tree.has_meta(META_KEY) or tree.get_meta(META_KEY) != self or scheduler.is_disposed:
		queue_free()
		return
	_tree = tree
	tree.root.add_child(self)

func _process(delta: float) -> void:
	var now := Time.get_ticks_usec()
	var unscaled := float(now - _last_ticks) / 1000000.0
	_last_ticks = now
	scheduler.update(delta, unscaled)

func _physics_process(delta: float) -> void:
	scheduler.update(delta, 1.0 / float(Engine.physics_ticks_per_second), Types.Process.PHYSICS)

func _exit_tree() -> void:
	_shutdown()

func _notification(what: int) -> void:
	if what == NOTIFICATION_PREDELETE: _shutdown()

func _shutdown() -> void:
	scheduler.dispose()
	if is_instance_valid(_tree) and is_instance_valid(_tree.root) and _tree.root.tree_exiting.is_connected(_tree_exiting):
		_tree.root.tree_exiting.disconnect(_tree_exiting)
	if is_instance_valid(_tree) and _tree.has_meta(META_KEY) and _tree.get_meta(META_KEY) == self:
		_tree.remove_meta(META_KEY)

func _tree_exiting() -> void:
	_tree.set_meta(CLOSING_KEY, true)
	_shutdown()
	# Also free a runner whose deferred attachment never happened.
	if not is_inside_tree(): queue_free()

func _report_error(message: String) -> void:
	push_error("tweens.gd: " + message)
