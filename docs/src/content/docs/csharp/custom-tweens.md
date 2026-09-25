---
title: Custom C# tweens
description: Animate callback values and custom properties, or drive a scheduler yourself.
---

Use a callback value tween when there is no native property adapter. Use
`Tweens.Property<TTarget, TValue>` when you can supply a typed getter, setter, and
interpolator. Neither approach needs reflection or property paths.

## Callback values

Import `Godot` and `tweens.gd`, and run this in a Node method on Godot's main thread:

```csharp
var value = owner.Tween(new Tweens.Float
{
    From = 10,
    To = 100,
    Duration = 1,
    OnUpdate = (_, sample) => GD.Print(sample),
});
```

The owner is an in-tree `Node`. It controls lifetime; values are delivered through
`OnUpdate`. The eight value definitions are `Tweens.Float`, `Tweens.Double`,
`Tweens.Vector2`, `Tweens.Vector3`, `Tweens.Vector4`, `Tweens.Color`, `Tweens.Quaternion`,
and `Tweens.Rect2`.

## Custom managed properties

This complete example animates a managed object with a manually driven scheduler:

```csharp title="MeterExample.cs"
using tweens.gd;

public sealed class Meter
{
    public float Value { get; set; }
}

public static class MeterExample
{
    public static float SampleMidpoint()
    {
        var meter = new Meter();
        using var scheduler = new TweenScheduler();
        var definition = new Tweens.Property<Meter, float>(
            target => target.Value,
            (target, value) => target.Value = value,
            Interpolators.Float)
        {
            From = 0,
            To = 100,
            Duration = 1,
        };

        scheduler.Add(meter, definition);
        scheduler.Update(0.5);
        return meter.Value; // 50 with the default linear easing.
    }
}
```

`TweenScheduler` belongs to its creating thread. Drive it with `Update(delta,
unscaledDelta, mode)` and dispose it when finished; disposal settles remaining
work. A manual scheduler with no owner has no tree pause policy. Native targets
still require Godot's main thread, and nodes must have an in-tree owner.

For a custom property on a `Node`, pass its definition to `node.Tween(definition)`
to use automatic scheduling instead. Automatic `CancelTweens` does not reach
separate manual schedulers.

## Custom definitions and bindings

Derive from `TweenDefinition<TTarget, TValue>` and implement protected `Read`,
`Write`, and `Interpolate` methods. `TTarget` is a reference type; `TValue` is a
value type. The `Interpolators` helpers cover the built-in numeric/vector types.

For per-playback bindings, override `Prepare`, `Restore`, and `Release`.
Preparation runs on the private definition snapshot before its initial read.
Restoration may restore a property value or remove an override. Cleanup also
runs after failed preparation and must release only resources owned by that
snapshot. Custom reference-valued configuration remains shared after the shallow
snapshot; do not mutate shared configuration during independent playbacks.
