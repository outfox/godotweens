# tweens.gd for GDScript

An initial, pure GDScript implementation of reusable tween definitions and
independent playback handles. Copy this entire `tweens_gd` directory into your
project's `addons/` directory. No plugin activation, autoload, .NET runtime or
GDExtension is required.

The API is experimental. It has been tested headlessly on Windows with the
repository's pinned 2dog/Godot 4.7.2 engine. Standard Godot exports, older engine
versions and Web/WASM have not yet been validated.

## Start and await

```gdscript
extends Node2D

const Tweens = preload("res://addons/tweens_gd/tweens.gd")

func _ready() -> void:
	var move := Tweens.property(^"position", Vector2(400, 180), 0.6)
	move.ease = Tweens.Ease.CUBIC_OUT
	var movement := Tweens.play(self, move)
	if await movement.wait() == Tweens.Reason.COMPLETED:
		print("Arrived")
```

`TweensGd` is also registered as a global class in the editor. Explicit preloads
work before the editor has generated a global class cache.

`property(path, to, seconds)` and `value(from, to, seconds)` create mutable
`TweensGdDefinition` objects. `play(target, definition, owner = null)` snapshots
the configuration and always returns a `TweensGdHandle`. Change a definition for future
starts without changing existing playback; `definition.copy()` creates a separate
configuration. Curves are duplicated; Callables and their captured objects are
shared references. Subclass-specific fields are not copied automatically.

Node targets must be inside the tree and use their own lifetime. Resource/Object
targets require an explicit in-tree owner for automatic playback:

```gdscript
var fade := Tweens.property(^"modulate:a", 0.0, 0.3)
var first := Tweens.play(sprite, fade)
var second := Tweens.play(label, fade)
first.pause()
first.resume()
second.cancel()
Tweens.cancel_tweens(sprite, true) # Include descendant owners.

var roughness := Tweens.property(^"roughness", 0.2, 0.5)
var material_handle := Tweens.play(material, roughness, mesh_instance)
```

## Properties and values

Supported values: `float`, `int`, `Vector2`, `Vector3`, `Vector4`, `Color`,
`Quaternion` and `Rect2`. Property paths select a property on the target and
optional value components, such as `position:x`, `modulate:a`, or `region_rect:size:x`.
Paths cannot cross into another Object/resource or traverse nodes. Pass that
object as the target with an owner instead.

The initial property is captured at start, before `on_add`. Null `from_value`
or `to_value` endpoints use the captured value. An empty property path creates
a callback-only tween; its omitted endpoints use `initial_value` (default `0.0`).

```gdscript
var score := Tweens.value(0.0, 100.0, 1.0)
score.on_update = func(_handle, value): label.text = str(roundi(value))
var handle := Tweens.play(self, score)
```

Quaternion interpolation normalizes endpoints and uses shortest-path slerp.
Integers round ties away from zero and saturate at GDScript's signed 64-bit limits;
intermediate interpolation uses floating point, so values above 2^53 can lose
precision. Exact integer endpoints are preserved. C#'s integer adapters use 32-bit
limits. Colors/vectors preserve easing overshoot. Engine setters can clamp values.

Component writes read the other components at application time. Concurrent
tweens write in insertion order. Resource properties modify the supplied resource,
including when it is shared. Generic property paths provide access to Node2D,
Node3D, Control and ordinary material properties; dedicated typed adapters and
shader-uniform restoration are not implemented yet.

## Timing configuration

| Field | Default and behavior |
| --- | --- |
| `duration` | `0.0`; seconds per leg; zero completes on the first eligible update |
| `delay` | `0.0`; initial wait, preserving unused delta |
| `offset` | `0.0`; seconds into the first leg, between zero and duration; delay comes first |
| `repeats` | `0`; cycles after the first; `Tweens.INFINITE` (`-1`) repeats until cancelled |
| `use_ping_pong` | `false`; forward and return legs form one cycle |
| `ping_pong_interval` | `0.0`; wait at the far endpoint before returning |
| `repeat_interval` | `0.0`; wait between cycles, never after the last |
| `ease` | `Tweens.Ease.LINEAR`; 33 functions matching the C# catalog |
| `skew` | `1.0`; positive exponent applied to normalized time before easing |
| `ease_function` / `curve` | Optional synchronous Callable or duplicated Curve; choose one |
| `fill` | `Tweens.Fill.RETAIN_FINAL_VALUE`; also `NONE`, `APPLY_FROM_DURING_DELAY`, `BOTH` |
| `process_mode` | `Tweens.Process.PROCESS`; `PHYSICS` uses physics updates |
| `pause_mode` | `Tweens.Pause.BOUND`; alternatives `SCENE_TREE` and `ALWAYS` |
| `use_unscaled_time` | `false`; automatic process uses monotonic ticks; physics uses `1 / Engine.physics_ticks_per_second` |
| `suppress_callbacks_when_target_invalid` | `false`; optionally suppress terminal callbacks for invalid targets/owners |

Timing must be finite and nonnegative. Infinite zero-time cycles are rejected.
Large deltas skip cycles arithmetically. `progress` is the current leg position,
reversing on return, rather than overall completion. A sample at an exact cycle
boundary displays the previous endpoint. Skipped cycles do not synthesize callbacks.

Without retention, natural completion restores the captured initial value;
cancellation retains the last applied value. A ping-pong tween ends at its from
endpoint. Instance pause always wins over tree/owner pause settings. A node's
`set_process(false)` alone does not disable bound tweens.

## Completion and callbacks

Use `await handle.wait()`. It returns the cached reason immediately if the handle
has already settled, and supports multiple waiters. `ended(reason)` is a one-shot
signal; awaiting that raw signal after emission would wait forever.

Reasons: `COMPLETED`, `CANCELLED`, `TARGET_FREED`, `OWNER_EXITED`,
`RUNNER_DISPOSED`, and `FAILED`. Inspect `handle.error` for a detected failure.
Invalid starts return an already-settled `FAILED` handle instead of null, so
`await Tweens.play(target, definition).wait()` is safe without a null check or a
future tick. Rejected starts schedule no work, retain no target/configuration, and
run no definition callbacks. Their `value` and `target` are null and `progress` is
zero. Pause, resume and cancel remain safe; repeated waits return the same reason.
Rejection diagnostics are still reported to Godot's error log. Use `wait()` rather
than the raw signal: an already-rejected handle will not emit `ended` later.

Removal/reparenting ends node-owned playback immediately, including while paused.
Queued targets are never written. Direct `free()` can report `OWNER_EXITED` because
Godot emits tree exit before invalidating the node.

Callbacks are synchronous Callables: `on_add(handle)`, `on_start(handle)`,
`on_update(handle, value)`, `on_end(handle)`, `on_cancel(handle)`, and
`on_finally(handle)`. Order is add, optional delay fill, start once, updates,
end/cancel, finally, then the completion signal. Detected failures run finally.
Terminal state is visible inside terminal callbacks. Calling cancel repeatedly is
safe. New tweens created by callbacks first sample on the next eligible update.

Tweens started synchronously inside `on_end` or a resuming `wait()` inherit the
finished tween's overshoot on the same scheduler, process lane and time scale.
Unrelated starts, late waits on already-finished handles and continuations after
another awaited signal do not inherit it. Check the reason before starting a next
step when cancellation should stop a sequence.

Use the API on Godot's main thread. Do not `await` inside callbacks; put sequences
in a separate coroutine awaiting `wait()`. GDScript cannot catch arbitrary script
errors as C# exceptions. Invalid configuration, stale Callables, nonnumeric/nonfinite
easing and nonfinite interpolation are detected; arbitrary errors inside callbacks
or custom property setters remain Godot script errors, with no promised conversion
to `FAILED`. There is no per-wait cancellation token, group API or C# task parity.

## Manual scheduling and diagnostics

```gdscript
var scheduler := Tweens.Scheduler.new()
var handle := scheduler.add(target, definition) # Optional third argument: owner.
scheduler.update(0.25) # Explicit delta; optional unscaled delta and process lane.
if handle.completion_reason == Tweens.Reason.FAILED:
	print(handle.error)
scheduler.dispose()
```

A manual scheduler can animate non-node Objects without an owner. Node targets
still require an in-tree node. `add()` also always returns a handle, already settled
with `FAILED` on invalid input. `error_reported(message)`, `last_error` and
`handle.error` provide diagnostics. `update()` rejects
recursive calls and invalid deltas. `cancel_all()`, `cancel_owner()` and
`active_count` inspect/control its work. Always dispose manual schedulers to settle
their active handles. Automatic `cancel_tweens()` affects the per-tree runner only.

The automatic runner attaches deferred under the root and uses process/physics
priority 1000. It releases playback on teardown. Handles retain their target for
inspection; release handles you no longer need.

## Validation and remaining work

The repository's `testbed-gdscript/` contains a standalone Godot test project,
a minimal 2dog launcher, shared C#/GDScript timing/easing fixtures and a reproducible
desktop microbenchmark. See its README for commands. Performance at high tween
counts needs further work; this implementation is not advertised as equivalent
to the built-in native Tween's throughput.

Next slices: groups, typed property conveniences, custom adapters, shader uniforms,
broader conformance and rendering coverage, constrained-device/web benchmarks,
export validation and distributable packages. The addon remains pure GDScript.
