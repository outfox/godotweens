---
title: C# sequences
description: Chain, overlap, stagger, pause, and stop multi-step animations with async composition.
---

A sequence is ordinary async C#: each tween's `End` task says when a step
has ended, a `Group` plays several tweens as one step, and the `Delay` option
offsets tweens within a step. Awaited steps continue each other's timelines
exactly, without frame drift. The rest of this page covers the patterns and
their timing rules.

| Goal | Tool |
| --- | --- |
| Run B after A | `await a.End`, then start B |
| Run A and B together | `sprite.Tween(a, b)` or `Group.Of(a, b)`, then `await group.End` |
| Offset tweens within a step | `Delay` on different targets or properties |
| Wait between steps | A throwaway `TweenFloat` wait |
| Stop the sequence | Cancel the running tweens; check each step's reason |

The snippets below run in an async Node method with in-tree `sprite` (`Sprite2D`)
and `label` (`Label`) nodes. Import `Godot`, `tweens.gd`, and `System`, plus
`System.Linq` and `System.Threading.Tasks` if implicit usings are disabled.

## One step after another

Start the next tween only after the previous one completes. Check the reason:
a cancelled, freed, or shut-down step also settles `End`, and the
sequence should usually end there rather than continue.

```csharp
if (await sprite.TweenPosition(new Vector2(400, 180), 0.6).End != Reason.Completed)
    return;
if (await sprite.TweenScale(new Vector2(1.2f, 1.2f), 0.2).End != Reason.Completed)
    return;
await sprite.TweenModulateAlpha(0, 0.3).End;
```

A tween added after an earlier step reads its omitted `From` at that point, so
each step continues from wherever the previous step left the property.

## Steps that run together

A group plays tweens as one step. Start definitions on one node together with
`node.Tween(first, second, ...)`, which accepts definitions for the node's base
types too:

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
member. `Task.WhenAll` also works, but it lets the other tweens keep running and
can hand the next step a slightly wrong start time; see
[timing between steps](#timing-between-steps).

## Stagger with Delay

`Delay` offsets tweens inside a single step, which is exact to the frame:

```csharp
Label[] letters = [label];
var reveals = letters.Select((letter, i) =>
    letter.TweenModulateAlpha(1, 0.3, options => { options.From = 0; options.Delay = i * 0.05; }));
await Group.Of([.. reveals]).End;
```

Use it for **different** targets or properties only. Every tween reads its
omitted `From` when it is added, not when its delay ends. A delayed tween on the
same property therefore starts from the value captured when it was added, and
at its start it snaps the property back to that value:

```csharp
// Wrong: the second tween captured From = the start position, not (400, 180).
sprite.TweenPosition(new Vector2(400, 180), 0.6);
sprite.TweenPosition(new Vector2(400, 0), 0.4, options => options.Delay = 0.6);
```

Await the first tween instead, or give the delayed tween an explicit `From`.

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

Avoid `Task.Delay` for this. It ignores tree pause, `Engine.TimeScale`, and node
lifetime, so the sequence can resume against a paused or freed scene.

## Stop a sequence

A sequence stops when its current step settles with a reason other than
`Completed` and your code returns. Anything that settles the running tweens
stops it:

- Call `Cancel()` on the current tween or group.
- Call `owner.CancelTweens(includeChildren: true)` on a common ancestor.
- Free the node, or remove it from the tree; its tweens settle as `TargetFreed`
  or `OwnerExited`.

To stop only your *wait* when an external token fires, and leave playback to
you, use `AwaitDecommissionAsync`:

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

`Pause()` on a tween or group pauses only that step. If the sequence starts its
next step while you are paused, the new tweens play. To pause every current and
future step, pause the node the tweens are bound to instead. With the default
`TweenPauseMode.Bound`, tweens follow the node's `CanProcess()`:

```csharp
sprite.ProcessMode = ProcessModeEnum.Disabled; // Pauses every tween bound to sprite.
sprite.ProcessMode = ProcessModeEnum.Inherit;
```

Pausing the scene tree also pauses bound tweens unless their node processes
while paused.

## Timing between steps

When a step completes, the code awaiting it resumes immediately, inside the same
scheduler update. Tweens it starts inherit the time by which the finished step
overshot its end. They appear from the next frame, at exactly the point a gapless
timeline would put them, so long sequences do not drift. For a group, the time is
taken from the member that finished last.

The handover applies when all of these hold:

- You await the tween's or the group's `End` directly. With
  `Task.WhenAll`, the time comes from whichever member the scheduler settled last,
  which is not necessarily the last to finish.
- The next tweens start before the sequence awaits anything else.
- They use the same `ProcessMode` and time base (`UseUnscaledTime`) as the step
  they follow.
- The await resumes inline, which is the default on Godot's main thread. A
  continuation posted for later, for example from another synchronization
  context, starts its tweens without the handover.

Tweens started from an `OnEnd` callback continue the finishing tween's timeline in
the same way. Callbacks cannot be checked for a reason as easily as async code, so
prefer async code for anything longer than a single follow-up.

## Loops

Repeat a whole sequence with an ordinary loop that ends when a step does not
complete. A single tween repeats with `Repeats` instead; see
[timing](/concepts/timing/).

```csharp
while (await sprite.TweenPositionY(120, 0.4).End == Reason.Completed
       && await sprite.TweenPositionY(180, 0.4).End == Reason.Completed)
{
}
```

## Errors

A faulted tween makes its `End` throw when awaited, and a group containing
it faults too. Wrap a sequence in `try`/`catch` when it starts from an `async void`
Godot callback such as `_Ready`, or the exception is lost to the synchronization
context. See [playback and async](/csharp/playback/#callbacks-and-errors).

## Examples in the gallery

The testbed gallery shows complete sequences next to their source:

- **Async Sequence** (Motion & paths): a one-shot outward leg, then a return and a turn
  together, with each step's reason checked.
- **Card Deal** (Choreography): staggered steps, nested per-card sequences
  inside `Task.WhenAll`, a hold, and a looping replay.
