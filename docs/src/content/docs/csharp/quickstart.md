---
title: C# quickstart
description: Move and fade a Sprite2D with a complete, cancellation-aware example.
---

[Install the library](/csharp/installation/), create a scene with a `Sprite2D`,
give it a texture, and attach this script as `MoveAndFade.cs`. Run the scene in
your matching Godot host.

```csharp title="MoveAndFade.cs"
using System;
using Godot;
using tweens.gd;

public partial class MoveAndFade : Sprite2D
{
    public override async void _Ready()
    {
        try
        {
            var movement = this.Tween(new Position2DTween
            {
                To = Position + new Vector2(240, 0),
                Duration = 0.6,
                Ease = EaseType.CubicOut,
            });

            var reason = await movement.Completion;
            if (reason != TweenCompletionReason.Completed)
                return;

            await this.TweenModulateAlpha(0, 0.3).Completion;
        }
        catch (Exception error)
        {
            GD.PushError(error.ToString());
        }
    }
}
```

The movement captures the current position as `From`. When it finishes normally,
the sprite fades out. Leaving the tree cancels its playback; checking the
completion reason prevents the fade from starting after cancellation.

The first tween installs one internal runner under the scene-tree root through
deferred attachment. No autoload, scene script file, or custom host-loop code is
required. Start from `_Ready` or later on Godot's main thread.

## Keep a handle to control it

```csharp
var movement = sprite.TweenPosition(new Vector2(400, 180), 0.6);
movement.Pause();
movement.Resume();
movement.Cancel();
```

Cancellation keeps the latest value. You can also call
`sprite.CancelTweens(includeChildren: true)` to cancel automatic tweens owned by
that node and its descendants.

## Choose the next guide

- [Definitions and playback](/concepts/definitions/) explains snapshots and reuse.
- [Playback and async](/csharp/playback/) covers sequences, parallel work, and errors.
- [Node and value catalog](/csharp/nodes/) lists typed adapters and units.
- [Gallery](/csharp/gallery/) shows combinations you can run and inspect.
