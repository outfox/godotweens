---
title: Definitions and playback
description: Reusable configurations, independent handles, snapshots, and competing property writes.
---

A **definition** describes endpoints, timing, easing, and callbacks. A **playback
instance** is one running use of that definition on a target. The behavior below
is implemented in C# and is the intended model for the planned GDScript addon.

## Reuse a definition

In this C# example, `sprite` is an in-tree `Sprite2D` and `label` an in-tree
`Label`. Import `Godot` and `tweens.gd` and run on Godot's main thread.

```csharp
var fade = new ModulateAlphaTween { To = 0, Duration = 0.3 };
var first = sprite.Tween(fade);
var second = label.Tween(fade);
first.Pause();
first.Resume();
second.Cancel();
sprite.CancelTweens(includeChildren: true);
```

Each addition snapshots options, endpoints, callbacks, and custom definition fields. Omitted `From`/`To` use the property's value captured once at addition. Editing the definition afterward does not alter a running tween. Custom reference-valued fields and objects captured by delegates are shared, just as ordinary C# closures are; keep them immutable if independent playback is required. Godot Curves are duplicated for each instance.

Multiple tweens on the same property are allowed: the last application in insertion order wins. Axis and alpha adapters read the other components at application time so independent component tweens compose correctly.
## Choose a C# entry point

Use `target.Tween(new Definition { ... })` for reusable definitions, or a typed
convenience method such as `sprite.TweenPosition(destination, 0.5)`. Both return
independent handles. The configure callback in a convenience method runs before
snapshotting and may override any option.

Callback value tweens have no property to read; omitted endpoints use the type's
default (zero, transparent black, or identity as applicable). Supply explicit
endpoints for a particular range. See [custom tweens](/csharp/custom-tweens/).
