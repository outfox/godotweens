---
title: Easing
description: Built-in easing families, overshoot, custom functions, and Godot curves.
---

Easing maps normalized leg progress to an interpolation weight. It changes how
motion accelerates and decelerates while keeping the same timing. The C# library
provides 31 easing functions; GDScript equivalents are planned.

## Built-in functions

`EaseType.Linear` is the default. Each other family has `In`, `Out`, and `InOut`
variants, for example `EaseType.CubicOut`.

| Family | Character |
| --- | --- |
| `Sine` | Gentle acceleration/deceleration |
| `Quad`, `Cubic`, `Quart`, `Quint` | Increasingly pronounced polynomial curves |
| `Expo` | Strong exponential acceleration/deceleration |
| `Circ` | Circular curve |
| `Back` | Overshoots an endpoint |
| `Elastic` | Oscillates around an endpoint |
| `Bounce` | Repeated bounces |

Back and elastic weights remain unclamped. Overshoot is useful for motion, but
native property limits still apply to constrained values such as alpha or ranges.
Try the easing selector in the [gallery](/csharp/gallery/).

## Custom functions

Supply a function from normalized progress to weight. Here `sprite` is an in-tree
`Sprite2D`; import `Godot` and `tweens.gd` and run on Godot's main thread:

```csharp
var eased = sprite.TweenPosition(new Vector2(300, 120), 0.5,
    options => options.EaseFunction = t => t * t);
```

`EaseFunction` overrides `Ease`. A Godot `Curve` can also override `Ease`; its
samples use normalized time from 0 to 1 and the curve is duplicated for each
instance. Specify only one of `EaseFunction` and `Curve`; supplying both is
rejected. Exceptions in easing fault playback and its completion task.
