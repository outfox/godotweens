---
title: C# core API
description: Entry points, reusable definitions, playback handles, scheduler methods, and completion states.
---

All library types and extension methods live in `tweens.gd`.

## Start playback

| Entry point | Purpose |
| --- | --- |
| `node.Tween(definition)` | Animate an in-tree node or deliver callback values |
| `node.TweenProperty(to, duration, configure)` | Convenience form; replace `Property` with a method from the catalog |
| `resource.Tween(definition, tree)` | Scope resource playback to a `SceneTree` |
| `resource.Tween(definition, owner)` | Bind resource playback to an owner node |
| `owner.Tween(resource, definition)` | Owner-first form of resource playback |
| `node.CancelTweens(includeChildren: false)` | Cancel automatic tweens owned by this node |

Resource convenience methods also take the tree or owner context. Tree-based
resource overloads accept an optional owner. See [materials](/csharp/materials/)
for exact examples, [node/value definitions](/csharp/nodes/), and
[shader definitions](/csharp/shaders/).

## Definitions

`TweenDefinition<TTarget, TValue>` inherits `TweenOptions` and adds nullable
`From`/`To` endpoints and typed callbacks. `TTarget` must be a class and `TValue`
a struct. Definitions are reusable; playback snapshots configuration on addition.

| Configuration | Reference |
| --- | --- |
| `Duration`, `Delay`, `Offset`, `LoopCount`, `IsInfinite`, `UsePingPong`, `PingPongInterval`, `RepeatInterval`, `Fill` | [Timing and loops](/concepts/timing/) |
| `Ease`, `EaseFunction`, `Curve` | [Easing](/concepts/easing/) |
| `ProcessMode`, `UseUnscaledTime` | [Process and physics](/concepts/timing/#process-and-physics) |
| `PauseMode`, `SuppressCallbacksWhenTargetInvalid` | [Lifetime and ownership](/concepts/lifetime/) |
| `OnAdd`, `OnStart`, `OnUpdate`, `OnEnd`, `OnCancel`, `OnFinally` | [Callbacks and errors](/csharp/playback/#callbacks-and-errors) |

Callbacks receive the typed instance; `OnUpdate` also receives the sampled value.
See [custom tweens](/csharp/custom-tweens/) to implement property operations or
per-playback bindings.

## Playback handles

| Member | Meaning |
| --- | --- |
| `Pause()`, `Resume()`, `IsPaused` | Control explicit pause |
| `Cancel()` | End playback and retain the latest sample |
| `State` | `Delayed`, `Playing`, `Interval`, `Completed`, `Cancelled`, or `Faulted` |
| `IsTerminal` | True when completed, cancelled, or faulted |
| `Progress` | Normalized current-leg progress, before easing |
| `CompletionReason` | Nullable reason; faults are described by `Error` |
| `Error` | Failure retained after faulted playback |
| `Completion` | Shared `Task<TweenCompletionReason>` |
| `AwaitDecommissionAsync(token)` | Wait with cancellation that affects only the wait |
| `Target` | Original target, on the generic handle |

Pause is separate from `State`: there is no `Paused` state. Reading state does not
replace awaiting completion when coordinating async work.

## Manual scheduler

| Member | Purpose |
| --- | --- |
| `Add(target, definition)` | Add playback; a node target becomes its owner |
| `Add(target, definition, owner)` | Bind a separate target to an in-tree owner |
| `Update(delta, unscaledDelta = null, mode = TweenProcessMode.Process)` | Advance the selected process mode |
| `ActiveCount` | Number of nonterminal instances |
| `UnhandledException` | Receive errors after failing tweens are cleaned up |
| `Dispose()` | Stop and release remaining playback |

Create, update, and dispose on the same thread. Recursive scheduler updates are
rejected. Use a manual scheduler for deterministic tests or managed targets;
ordinary Godot node/resource tweens use the automatic runner.
