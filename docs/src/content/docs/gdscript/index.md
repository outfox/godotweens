---
title: GDScript addon
description: Install the experimental pure GDScript core and start reusable property and value tweens.
---

:::note[Experimental API]
The core addon is implemented in pure GDScript. It is available from source;
there is no published addon package yet. Full C# parity and web exports remain
work in progress.
:::

tweens.gd provides reusable definitions, independent playback handles, timing,
easing and node ownership for GDScript. It requires no .NET runtime, autoload,
editor plugin activation or GDExtension.

## Install and start

Copy the repository's `addons/tweens_gd/` directory, including its licenses,
into your project's `addons/` directory. Start from `_ready()` or later on an
in-tree node:

```gdscript
extends Node2D

const Tweens = preload("res://addons/tweens_gd/tweens.gd")

func _ready() -> void:
	var move := Tweens.property(^"position", Vector2(400, 180), 0.6)
	move.ease = Tweens.Ease.CUBIC_OUT
	var handle := Tweens.play(self, move)
	if await handle.wait() == Tweens.Reason.COMPLETED:
		print("Arrived")
```

Each start copies its definition and captures the current property value.
Changing the definition affects future starts. `definition.copy()` creates a
separate configuration; Curves are duplicated while Callables remain shared.
The editor also registers the global `TweensGd` class. Explicit preloads work
without an editor-generated class cache.

## Core API

| API | Behavior |
| --- | --- |
| `Tweens.property(path, to, seconds)` | Create a property definition; component paths such as `position:x` and `modulate:a` compose independently |
| `Tweens.value(from, to, seconds)` | Create a callback-only definition, delivering samples to `on_update(handle, value)` |
| `Tweens.play(target, definition, owner = null)` | Always return a handle; node targets bind to themselves, other Objects require an owner |
| `handle.pause()` / `resume()` / `cancel()` | Control independent playback |
| `await handle.wait()` | Get a completion reason, including when already finished |
| `Tweens.group(handles)` / `Tweens.Group.of(handles)` | Group existing handles for shared controls, sibling cancellation and `await group.wait()` |
| `Tweens.cancel_tweens(owner, include_children = false)` | Cancel that owner's automatic playback |
| `Tweens.Scheduler.new()` | Create a manual scheduler with `add()`, `update()`, `cancel_all()` and `dispose()` |

Supported values are numbers, Vector2/3/4, Color, Quaternion and Rect2.
Properties are validated at runtime. Paths select properties and value components
on the target; they cannot traverse nodes or cross into another Object. Animate
a resource by passing it as the target and supplying an owner. Shared resources
are modified directly.

Definitions expose `from_value`, `to_value`, `duration`, `delay`, `offset`,
`repeats`, `use_ping_pong`, `ping_pong_interval`, `repeat_interval`, `fill`,
`ease`, `skew`, `ease_function`, `curve`, `process_mode`, `pause_mode` and
`use_unscaled_time`. Null endpoints use the captured initial value. Defaults
include linear easing, zero duration, no repeats and retained final values.
Use `Tweens.INFINITE` for infinite repeats and `Tweens.Fill`, `Tweens.Ease`,
`Tweens.Process` and `Tweens.Pause` for option constants.

The timing rules follow [timing](/concepts/timing/): preserve excess delta, delay
before offset, forward/return legs in each ping-pong cycle, and no final repeat
interval. Zero duration completes on the first eligible update. There are 33
built-in eases; custom easing is a synchronous Callable or a duplicated Curve.

## Completion and lifetime

Reasons are `COMPLETED`, `CANCELLED`, `TARGET_FREED`, `OWNER_EXITED`,
`RUNNER_DISPOSED` and `FAILED`. Check `handle.error` on detected failures.
`play()` and manual `scheduler.add()` return an already-settled `FAILED` handle
when a start is rejected. No null check or future tick is needed: `await
handle.wait()` returns immediately, including on repeated waits. Rejected starts
schedule no work, retain no target/configuration, and run no definition callbacks.
Their `target` and `value` are null and `progress` is zero; pause, resume and cancel
remain safe. Automatic playback still reports errors through Godot's error log;
manual schedulers retain `last_error` and emit `error_reported(message)`.

Multiple callers can await `wait()`. The raw `ended(reason)` signal fires once;
use `wait()` when the handle might have already ended or been rejected.

Callbacks are synchronous: `on_add(handle)`, `on_start(handle)`,
`on_update(handle, value)`, `on_end(handle)`, `on_cancel(handle)` and
`on_finally(handle)`. Terminal callbacks see terminal state; waiters resume after
cleanup. Tweens started in an end callback or an inline `wait()` continuation
inherit overshoot when using the same scheduler, process lane and time scale.
Late waits and continuations after another awaited signal do not inherit it.

Use the API on the main thread. Owner exit, including removal/reparenting,
cancels playback immediately, even while paused. Queued/freed targets are not
written. Instance pause always wins; `BOUND`, `SCENE_TREE` and `ALWAYS` select
the owner/tree pause policy. The automatic runner attaches deferred and processes
at priority 1000. Dispose manually created schedulers to settle their work.

## Groups

```gdscript
var motion := Tweens.group([
	Tweens.play(self, Tweens.property(^"position", Vector2(400, 180), 0.6)),
	Tweens.play(self, Tweens.property(^"modulate:a", 0.0, 0.3)),
])
if await motion.wait() == Tweens.Reason.COMPLETED:
	print("Moved and faded")
```

A group awaits every member's settlement, including callbacks and cleanup. The
first member that stops without completing cancels active siblings; the group
preserves that stop reason. Already-rejected handles participate too, so a failed
start cancels the rest of its group. Completed members retain their own results.

Use `pause()`, `resume()`, `cancel()` or assign `is_paused` for shared controls.
`is_paused` is true when every active member is paused, and false when none remain.
The result exposes `is_terminal`, `is_settled`, `completion_reason`, an `error`
string and an `errors` array of detected member diagnostics. Multiple and late
`wait()` calls work; prefer them to awaiting the one-shot `ended(reason)` signal.

`members` and `errors` return copies. Input handles are snapshotted and duplicates
are removed in first-occurrence order. An empty array or invalid member reports an
error and returns an already-settled `FAILED` group, without changing the supplied
handles. `group()` never returns null. Pass playback handles; start definitions
with `play()` or `scheduler.add()` first. Release groups when you no longer need
their member handles. Dropping a group leaves playback and sibling cancellation active.

Successful inline continuations use the last-finishing member's overshoot: the
latest update, then the smallest overshoot in that update. All members and the
new playback must share a scheduler, process lane and time scale. Mixed-clock and
interrupted groups carry no overshoot. The next tween first samples on the next update.

## Parity and current limits

| Area | Status |
| --- | --- |
| Definitions, handles, timing, easing, fills | Implemented; timing/easing fixtures also run against C# |
| Await, callbacks, cancellation, owner lifetime | Implemented with GDScript completion reasons |
| Groups, shared controls, sibling cancellation and continuation overshoot | Implemented; shared completion/carry fixtures also run against C# |
| Process/physics, pause and unscaled clocks | Implemented |
| Node2D/3D, Control, ordinary resource properties | Available through generic property paths |
| Named typed adapters, custom adapters, shader uniforms | Follow-up work |
| Per-wait cancellation tokens and C# exception tasks | Not provided |
| Platform validation | Windows headless tests on pinned 2dog/Godot 4.7.2 |
| Standard Godot exports, older engines, Web/WASM | Not yet validated |

GDScript cannot catch arbitrary script errors as C# exceptions. The addon detects
invalid configuration, stale Callables, invalid easing results and nonfinite
interpolation. Errors inside user callbacks and property setters remain Godot
script errors; conversion to `FAILED` is not guaranteed. Do not await inside
callbacks; write a separate coroutine that awaits playback handles.

The desktop benchmark shows that high tween counts need further optimization.
No supported count or frame-time budget is promised. The backend remains pure
GDScript; there is no native extension requirement.
