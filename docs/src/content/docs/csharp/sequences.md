---
title: Sequences
description: Chain, group, stagger, wait, loop, and stop multi-step animations with ordinary async C#.
---

A sequence is ordinary async C#. Await each step before starting the next, play
steps together as a group, and stop as soon as a step doesn't complete.

| Goal | Tool |
| --- | --- |
| Run B after A | `await a.End`, check the reason, then start B |
| Run A and B together | `node.Tween(a, b)` or `Group.Of(a, b)`, then await the group's `End` |
| Offset tweens within a step | `Delay` on different targets or properties |
| Wait between steps | `await node.TweenFloat(1, seconds).End` |
| Stop the sequence | Cancel the running step; the reason check ends the method |

Snippets run in an async Node method with in-tree `sprite` (`Sprite2D`) and
`label` (`Label`) nodes. They assume `using Godot;`, `using tweens.gd;`, and
implicit usings for `System`, `System.Linq`, and `System.Threading.Tasks`.

## One step after another

Start the next tween only after the previous one completes, and check the reason.
A cancelled, freed, or shut-down step also finishes `End`, and the sequence
should usually stop there:

```csharp
if (await sprite.TweenPosition(new Vector2(400, 180), 0.6).End != Reason.Completed)
    return;
if (await sprite.TweenScale(new Vector2(1.2f, 1.2f), 0.2).End != Reason.Completed)
    return;
await sprite.TweenModulateAlpha(0, 0.3).End;
```

Each step reads its omitted `From` when it starts, so it continues from wherever
the previous step left the property.

## Steps that run together

A group plays tweens as one step. Start several definitions on one node with
`node.Tween(first, second, ...)`. It also accepts definitions for the node's base
types:

```csharp
var grow = new Tweens.Scale2D { To = new Vector2(1.2f, 1.2f), Duration = 0.2 };
var dim = new Tweens.ModulateAlpha { To = 0.5f, Duration = 0.2 };
if (await sprite.Tween(grow, dim).End != Reason.Completed)
    return;
```

Group tweens that are already playing, on any targets, with `Group.Of`:

```csharp
var step = Group.Of(sprite.TweenPosition(new Vector2(400, 180), 0.6), label.TweenModulateAlpha(0, 0.6));
if (await step.End != Reason.Completed)
    return;
```

A group completes when every member completes. If one member stops early
(cancelled, freed, or faulted), the group cancels the others and reports that
member's reason, or faults. `Pause()`, `Resume()`, and `Cancel()` act on every
member.

:::tip[Prefer a group to `Task.WhenAll`]
`Task.WhenAll` also waits for several tweens, but it lets the others keep running
when one stops, and it can hand the next step a slightly wrong start time. See
[timing between steps](#timing-between-steps).
:::

## Stagger with Delay

`Delay` offsets tweens inside one step, exact to the frame. Combine it with a
definition and `with` to fade in a whole menu, one item after another:

```csharp title="Menu.cs"
public partial class Menu : VBoxContainer
{
    static readonly Tweens.ModulateAlpha FadeIn = new()
    {
        From = 0,
        Duration = 0.3,
        Fill = FillMode.Both,
    };

    public async Task<bool> Reveal()
    {
        var items = GetChildren().OfType<Control>();
        var reveals = items.Select((item, i) => item.Tween(FadeIn with { Delay = i * 0.05 }));
        return await Group.Of([.. reveals]).End == Reason.Completed;
    }
}
```

`Fill = FillMode.Both` applies `From` during the delay, so items that are still
waiting stay hidden instead of showing at full opacity first.

:::caution[Stagger different targets, not one property]
A tween reads its omitted `From` when it's added, not when its delay ends. A
delayed tween on the same property starts from the value captured at the start,
and snaps the property back to it:

```csharp
// Wrong: the second tween captured From = the start position, not (400, 180).
sprite.TweenPosition(new Vector2(400, 180), 0.6);
sprite.TweenPosition(new Vector2(400, 0), 0.4, options => options.Delay = 0.6);
```

Await the first tween instead, or give the delayed tween an explicit `From`.
:::

## Wait between steps

A callback value tween on any in-tree node makes a wait that follows the same
pause, time scale, and lifetime rules as the animation around it:

```csharp
if (await sprite.TweenPosition(new Vector2(400, 180), 0.6).End != Reason.Completed)
    return;
if (await sprite.TweenFloat(1, 0.5).End != Reason.Completed)
    return; // A 0.5-second hold.
await sprite.TweenPosition(new Vector2(40, 180), 0.6).End;
```

:::caution[Avoid `Task.Delay`]
`Task.Delay` ignores tree pause, `Engine.TimeScale`, and node lifetime. The
sequence can resume against a paused or freed scene.
:::

## Loops

Repeat a whole sequence with an ordinary loop that ends when a step doesn't
complete. To repeat a single tween, set `Repeats` instead, as described in
[timing](/concepts/timing/).

```csharp
while (await sprite.TweenPositionY(120, 0.4).End == Reason.Completed
       && await sprite.TweenPositionY(180, 0.4).End == Reason.Completed)
{
}
```

## Stop a sequence

A sequence stops when its current step ends with a reason other than `Completed`
and your code returns, so anything that stops the running tweens stops the
sequence:

- Call `Cancel()` on the current tween or group.
- Call `owner.CancelTweens(includeChildren: true)` on a common ancestor.
- Free the node, or remove it from the tree. Its tweens end with `TargetFreed`
  or `OwnerExited`.

To stop only your *wait* when an external token fires, and decide about playback
yourself, use `AwaitDecommissionAsync`:

```csharp
var movement = sprite.TweenPosition(new Vector2(400, 180), 0.6);
try
{
    await movement.AwaitDecommissionAsync(cancellationToken);
}
catch (OperationCanceledException)
{
    movement.Cancel();
    throw;
}
```

## Pause a sequence

`Pause()` on a tween or group pauses only that step. If your code starts the next
step while that one is paused, the new tweens play normally. To pause every
current and future step, pause the node the tweens are bound to. With the default
`TweenPauseMode.Bound`, tweens follow the node's `CanProcess()`:

```csharp
sprite.ProcessMode = ProcessModeEnum.Disabled; // Pauses every tween bound to sprite.
sprite.ProcessMode = ProcessModeEnum.Inherit;
```

Pausing the scene tree also pauses bound tweens, unless their node processes
while paused.

## Errors

A faulted tween makes its `End` throw when awaited, and a group containing it
faults too.

:::caution[Catch errors in `async void` callbacks]
Wrap a sequence in `try`/`catch` when it starts from an `async void` Godot
callback such as `_Ready`. Otherwise the exception is lost to the
synchronization context. See [errors](/csharp/playback/#errors).
:::

## Timing between steps

When a step completes, the code awaiting it resumes immediately, inside the same
scheduler update. Tweens it starts inherit the time by which the finished step
overshot its end. They appear from the next frame at exactly the point a gapless
timeline would put them, so long sequences don't drift. For a group, the time
comes from the member that finished last.

The handover applies when all of these hold:

- You await the tween's or the group's `End` directly. With `Task.WhenAll`, the
  time comes from whichever member the scheduler settled last, which isn't
  necessarily the last to finish.
- The next tweens start before the sequence awaits anything else.
- They use the same `ProcessMode` and time base (`UseUnscaledTime`) as the step
  they follow.
- The await resumes inline, which is the default on Godot's main thread. A
  continuation posted for later, for example from another synchronization
  context, starts its tweens without the handover.

Tweens started from an `OnEnd` callback continue the finishing tween's timeline in
the same way. Callbacks can't check a reason as easily as async code, so prefer
async code for anything longer than a single follow-up.
