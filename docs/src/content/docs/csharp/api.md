---
title: Core API
description: Entry points, reusable definitions, playback handles, scheduler methods, and completion states.
---

This page lists the core C# API: the methods that start playback and the types
they return, what a definition holds, the members of a playback handle, and the
manual scheduler. Explanations and examples are in the linked guides.

`using tweens.gd;` brings in extension methods, timing options, and playback
handles. Definitions live in the root `Tweens` namespace, such as
`Tweens.PathFollow2DVOffset`; no alias or `using Tweens;` is needed.

## Start playback

| Entry point | Returns | Purpose |
| --- | --- | --- |
| `node.Tween(definition)` | `TweenInstance<TTarget, TValue>` | Animate an in-tree node or deliver callback values |
| `node.Tween(first, second, ...)` | `Group` | Start several definitions together |
| `Group.Of(tweens)` | `Group` | Treat running tweens as one step with a shared `End` |
| `node.TweenProperty(to, duration, configure)` | `TweenInstance<TTarget, TValue>` | Convenience form; replace `Property` with a method from the catalog |
| `node.TweenProperty(to, duration, options)` | `TweenInstance<TTarget, TValue>` | Convenience form that copies a shared `TweenOptions`; `duration` takes precedence |
| `resource.Tween(definition, tree)` | `TweenInstance<TResource, TValue>` | Scope resource playback to a `SceneTree` |
| `resource.Tween(definition, owner)` | `TweenInstance<TResource, TValue>` | Bind resource playback to an owner node |
| `owner.Tween(resource, definition)` | `TweenInstance<TResource, TValue>` | Owner-first form of resource playback |
| `node.CancelTweens(includeChildren: false)` | `void` | Cancel automatic tweens owned by this node |

`TTarget` is the class the definition targets, which can be a base class of the
node: `sprite.TweenPosition(...)` returns `TweenInstance<Node2D, Vector2>`.

Resource convenience methods also take the tree or owner context. Tree-based
resource overloads accept an optional owner. See [materials](/csharp/materials/)
for exact examples, [node/value definitions](/csharp/nodes/), and
[shader definitions](/csharp/shaders/).

## Definitions

The built-in `Tweens.*` definitions are `readonly record struct` values with
nullable `From`/`To` endpoints, timing, and typed callbacks. Store them in readonly
fields and vary a copy when starting playback (see
[reusable definitions](/concepts/definitions/)):

```csharp title="Trail.cs"
public partial class Trail : PathFollow2D
{
    static readonly Tweens.PathFollow2DVOffset Offset = new() { To = 20 };

    public void Drift(double seconds, double delay) =>
        this.Tween(Offset with { Duration = seconds, Delay = delay });
}
```

`TweenOptions` is also a readonly record struct. It holds shared timing and can
be passed to a convenience method or assigned to a definition's `Options`.
Flat properties such as `Duration` and `Delay` update that same options value.
Assign `Options` before individual overrides in an initializer: a later
`Options` assignment replaces all timing settings. Both `new` and `default`
retain the final value unless `Fill` is explicitly changed.

`ITweenDefinition<TTarget, TValue>` connects definitions to typed playback;
`ITweenDefinition<TTarget>` supports groups with different value types.
`TTarget` is a class and `TValue` is a struct.

| Configuration | Reference |
| --- | --- |
| `Duration`, `Delay`, `Offset`, `Repeats`, `UsePingPong`, `PingPongInterval`, `RepeatInterval`, `Fill` | [Timing and loops](/concepts/timing/) |
| `Ease`, `EaseFunction`, `Curve` | [Easing](/concepts/easing/) |
| `ProcessMode`, `UseUnscaledTime` | [Process and physics](/concepts/timing/#process-and-physics) |
| `PauseMode`, `SuppressCallbacksWhenTargetInvalid` | [Lifetime and ownership](/concepts/lifetime/) |
| `OnAdd`, `OnStart`, `OnUpdate`, `OnEnd`, `OnCancel`, `OnFinally` | [Callbacks](/csharp/playback/#callbacks) |

Callbacks receive the `TweenInstance<TTarget, TValue>` handle; `OnUpdate` also
receives the sampled `TValue`. See [custom tweens](/csharp/custom-tweens/) to
implement property operations or per-playback bindings.

## Playback handles

Starting one definition returns a `TweenInstance<TTarget, TValue>`. Its non-generic
base class, `TweenInstance`, has every member below except `Target` and `Value`, so
handles with different type arguments fit in one collection.

| Member | Type | Meaning |
| --- | --- | --- |
| `Pause()`, `Resume()` | `void` | Set or clear explicit pause |
| `IsPaused` | `bool` | True while explicitly paused; settable |
| `Cancel()` | `void` | End playback and retain the latest sample |
| `State` | `TweenState` | `Delayed`, `Playing`, `Interval`, `Completed`, `Cancelled`, or `Faulted` |
| `IsTerminal` | `bool` | True when completed, cancelled, or faulted |
| `Progress` | `float` | Normalized current-leg progress, before easing |
| `CompletionReason` | `Reason?` | `null` until playback ends; faults are described by `Error` |
| `Error` | `Exception?` | Failure retained after faulted playback |
| `End` | `Task<Reason>` | Shared completion that any number of callers can await |
| `AwaitDecommissionAsync(token)` | `Task<Reason>` | Wait with cancellation that affects only the wait |
| `Target` | `TTarget` | Original target |
| `Value` | `TValue` | Value read at start, then the latest value written |

Pause is separate from `State`: there is no `Paused` state. Reading state does not
replace awaiting completion when coordinating async work.

`Reason` has five members: `Completed`, `Cancelled`, `TargetFreed`, `OwnerExited`,
and `RunnerDisposed`. [Why it ended](/csharp/playback/#why-it-ended) describes each.

`Group` has `Pause()`, `Resume()`, `Cancel()`, `IsPaused`, `IsTerminal`,
`CompletionReason`, `Error`, and `End`, plus `Members`, an
`IReadOnlyList<TweenInstance>`. It has no `State` or `Progress`. Its `End` reports
`Completed` when every member completes; otherwise it reports the first reason a
member stopped for, or faults.

## Manual scheduler

The automatic Godot runner is built on `TweenScheduler`. Create one with
`new TweenScheduler()` to drive playback yourself.

| Member | Type | Purpose |
| --- | --- | --- |
| `Add(target, definition)` | `TweenInstance<TTarget, TValue>` | Add playback; a node target becomes its owner |
| `Add(target, definition, owner)` | `TweenInstance<TTarget, TValue>` | Bind a separate target to an in-tree owner |
| `Update(delta, unscaledDelta = null, mode = TweenProcessMode.Process)` | `void` | Advance the selected process mode |
| `ActiveCount` | `int` | Number of nonterminal instances |
| `CancelAll()` | `void` | Cancel every tween in this scheduler |
| `UnhandledException` | `event Action<Exception>` | Receive errors after failing tweens are cleaned up |
| `Dispose()` | `void` | Stop and release remaining playback |

Create, update, and dispose on the same thread. Recursive scheduler updates are
rejected. Use a manual scheduler for deterministic tests or managed targets;
ordinary Godot node/resource tweens use the automatic runner.

## Class-based definitions

The older `*Tween` classes, such as `Position2DTween`, and custom subclasses of
`TweenDefinition<TTarget, TValue>` remain supported. They inherit
`TweenOptionsBuilder`, the mutable form of `TweenOptions`, and are snapshotted on
start. The built-in classes are hidden from IntelliSense so that new code sees the
structured definitions in `Tweens`.

The configure callback of a convenience method still receives a mutable builder:
`TweenPosition` on a `Node2D` passes a `Position2DTween`. Shared configurator
methods should accept `TweenOptionsBuilder` rather than `TweenOptions`.
