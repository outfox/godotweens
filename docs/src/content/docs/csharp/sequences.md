---
title: C# sequences
description: Chain, overlap, stagger, pause, and stop multi-step animations with async composition.
---

tweens.gd has no sequence or timeline object. A sequence is ordinary async C#:
each tween's `Completion` task says when a step has ended, and the `Delay` option
offsets tweens within a step. The rest of this page covers the patterns and
their timing rules.

| Goal | Tool |
| --- | --- |
| Run B after A | `await a.Completion`, then start B |
| Run A and B together | Start both, then `await Task.WhenAll(...)` |
| Offset tweens within a step | `Delay` on different targets or properties |
| Wait between steps | A throwaway `TweenFloat` wait |
| Stop the sequence | Cancel the running tweens; check each step's reason |

The snippets below run in an async Node method with in-tree `sprite` (`Sprite2D`)
and `label` (`Label`) nodes. Import `Godot`, `tweens.gd`, and `System`, plus
`System.Linq` and `System.Threading.Tasks` if implicit usings are disabled.

## One step after another

Start the next tween only after the previous one completes. Check the reason:
a cancelled, freed, or shut-down step also settles `Completion`, and the
sequence should usually end there rather than continue.

```csharp
if (await sprite.TweenPosition(new Vector2(400, 180), 0.6).Completion != Reason.Completed)
    return;
if (await sprite.TweenScale(new Vector2(1.2f, 1.2f), 0.2).Completion != Reason.Completed)
    return;
await sprite.TweenModulateAlpha(0, 0.3).Completion;
```

A tween added after an earlier step reads its omitted `From` at that point, so
each step continues from wherever the previous step left the property.

## Steps that run together

Start every tween of a step before awaiting any of them:

```csharp
var move = sprite.TweenPosition(new Vector2(400, 180), 0.6);
var fade = label.TweenModulateAlpha(0, 0.6);
var reasons = await Task.WhenAll(move.Completion, fade.Completion);
if (reasons.Any(reason => reason != Reason.Completed))
    return;
```

`Task.WhenAll` waits for every tween, even when one is cancelled early; cancel
the siblings yourself if one failure should stop the whole step. A small helper
keeps longer sequences readable:

```csharp
static async Task<bool> Completed(params TweenInstance[] tweens)
{
    var reasons = await Task.WhenAll(tweens.Select(tween => tween.Completion));
    return reasons.All(reason => reason == Reason.Completed);
}

var _ = await Completed(sprite.TweenPosition(new Vector2(400, 180), 0.6), label.TweenModulateAlpha(0, 0.6))
    && await Completed(sprite.TweenModulateAlpha(0, 0.3));
```

## Stagger with Delay

`Delay` offsets tweens inside a single step, which is exact to the frame:

```csharp
Label[] letters = [label];
var reveals = letters.Select((letter, i) =>
    letter.TweenModulateAlpha(1, 0.3, options => { options.From = 0; options.Delay = i * 0.05; }));
await Task.WhenAll(reveals.Select(tween => tween.Completion));
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
if (await sprite.TweenPosition(new Vector2(400, 180), 0.6).Completion != Reason.Completed)
    return;
if (await sprite.TweenFloat(1, 0.5).Completion != Reason.Completed)
    return; // A 0.5-second hold.
await sprite.TweenPosition(new Vector2(40, 180), 0.6).Completion;
```

Avoid `Task.Delay` for this. It ignores tree pause, `Engine.TimeScale`, and node
lifetime, so the sequence can resume against a paused or freed scene.

## Stop a sequence

A sequence stops when its current step settles with a reason other than
`Completed` and your code returns. Anything that settles the running tweens
stops it:

- Call `Cancel()` on the current handles.
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

`Pause()` on a handle pauses only that tween. If the sequence starts its next
step while you are paused, the new tween plays. To pause every current and
future step, pause the node the tweens are bound to instead. With the default
`TweenPauseMode.Bound`, tweens follow the node's `CanProcess()`:

```csharp
sprite.ProcessMode = ProcessModeEnum.Disabled; // Pauses every tween bound to sprite.
sprite.ProcessMode = ProcessModeEnum.Inherit;
```

Pausing the scene tree also pauses bound tweens unless their node processes
while paused.

## Timing between steps

When a step completes, the awaiting code resumes in the same frame and the next
step begins advancing on the following frame. The final value of the finished
step stays on screen in between, so there is no visual gap. However, the rest of
that frame's time is not carried into the next step, so each awaited boundary
can drift by up to one frame.

Where exact timing matters, such as with audio or a long chain, schedule the
steps in one go with `Delay` offsets on different targets or properties, or use
explicit `From` values as described above.

`OnEnd` callbacks can also start a follow-up tween. They have the same timing
rules, but they cannot be awaited or checked for a reason as easily, so prefer
async code for anything longer than a single follow-up.

## Loops

Repeat a whole sequence with an ordinary loop that ends when a step does not
complete. A single tween repeats with `Repeats` instead; see
[timing](/concepts/timing/).

```csharp
while (await sprite.TweenPositionY(120, 0.4).Completion == Reason.Completed
       && await sprite.TweenPositionY(180, 0.4).Completion == Reason.Completed)
{
}
```

## Errors

A faulted tween makes its `Completion` throw when awaited, and `Task.WhenAll`
rethrows it. Wrap a sequence in `try`/`catch` when it starts from an `async void`
Godot callback such as `_Ready`, or the exception is lost to the synchronization
context. See [playback and async](/csharp/playback/#callbacks-and-errors).

## Examples in the gallery

The testbed gallery shows complete sequences next to their source:

- **Async Sequence** (Motion & paths): a one-shot outward leg, then a return and a turn
  together, with each step's reason checked.
- **Card Deal** (Choreography): staggered steps, nested per-card sequences
  inside `Task.WhenAll`, a hold, and a looping replay.
