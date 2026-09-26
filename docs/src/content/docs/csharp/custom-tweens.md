---
title: Custom tweens
description: Animate callback values and custom properties, or drive a scheduler yourself.
---

When there's no native property adapter for what you want to animate, use a
callback value tween. If you can supply a typed getter, setter, and interpolator,
use `Tweens.Property<TTarget, TValue>` instead. Neither approach needs reflection
or property paths.

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

`owner` is an in-tree `Node` that controls the tween's lifetime, and each value
arrives through `OnUpdate`. The eight value definitions are `Tweens.Float`,
`Tweens.Double`, `Tweens.Vector2`, `Tweens.Vector3`, `Tweens.Vector4`,
`Tweens.Color`, `Tweens.Quaternion`, and `Tweens.Rect2`.

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

A `TweenScheduler` belongs to the thread that created it. Drive it with
`Update(delta, unscaledDelta, mode)`, and dispose it when you're finished to
settle any remaining work. A manual scheduler with no owner has no tree pause
policy. Native targets still require Godot's main thread, and nodes must have an
in-tree owner.

For a custom property on a `Node`, pass its definition to `node.Tween(definition)`
to use automatic scheduling instead. `CancelTweens` only cancels automatically
scheduled tweens and doesn't reach separate manual schedulers.

## Custom definitions and bindings

Derive from `TweenDefinition<TTarget, TValue>` and implement the protected `Read`,
`Write`, and `Interpolate` methods. `TTarget` is a reference type and `TValue` is a
value type. The `Interpolators` helpers cover the built-in numeric and vector
types.

For per-playback bindings, override `Prepare`, `Restore`, and `Release`.
`Prepare` runs on the playback's private definition snapshot, before its initial
read. `Restore` may write back a property value or remove an override instead.
`Release` also runs after a failed preparation, and it must release only resources
owned by that snapshot. The snapshot is shallow, so reference-valued configuration
on a custom definition stays shared. Don't mutate that shared configuration while
independent playbacks use it.
