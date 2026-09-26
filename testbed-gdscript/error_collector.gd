# SPDX-License-Identifier: MIT
# SPDX-FileCopyrightText: 2026 Moritz Voss
extends Logger
## Test-only: script errors must fail the suite, even if Godot resumes callers.
var messages: Array[String] = []
var mutex := Mutex.new()

func _log_error(_function: String, file: String, line: int, code: String,
		rationale: String, _editor_notify: bool, error_type: int,
		_script_backtraces: Array[ScriptBacktrace]) -> void:
	if error_type == ERROR_TYPE_WARNING: return
	mutex.lock()
	messages.append("%s:%d: %s %s" % [file, line, code, rationale])
	mutex.unlock()

func take_errors() -> Array[String]:
	mutex.lock()
	var result := messages.duplicate()
	messages.clear()
	mutex.unlock()
	return result
