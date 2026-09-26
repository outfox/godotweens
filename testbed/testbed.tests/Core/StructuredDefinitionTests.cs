// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Reflection;
using System.Runtime.CompilerServices;
using tweens.gd;

namespace testbed.Tests.Core;

public class StructuredDefinitionTests
{
    private sealed class Meter { public float Value; }

    private readonly Tweens.Property<Meter, float> movement = new(
        static target => target.Value,
        static (target, value) => target.Value = value,
        Interpolators.Float)
    {
        To = 10,
        Duration = 1,
    };

    [Fact]
    public async Task ReadonlyFieldCanBeReusedWithIndependentEndpointsDelaysAndCallbacks()
    {
        using var scheduler = new TweenScheduler();
        var first = new Meter { Value = 2 };
        var second = new Meter { Value = 4 };
        var samples = new List<float>();
        var a = scheduler.Add(first, movement);
        var b = scheduler.Add(second, movement with
        {
            To = 20,
            Delay = 0.5,
            OnUpdate = (instance, value) =>
            {
                Assert.Same(second, instance.Target);
                samples.Add(value);
            },
        });

        scheduler.Update(0.5);
        Assert.Equal(6, first.Value);
        Assert.Equal(4, second.Value);
        a.Cancel();
        scheduler.Update(0.5);
        Assert.Equal(12, second.Value);
        scheduler.Update(0.5);
        Assert.Equal(20, second.Value);
        Assert.Equal(Reason.Cancelled, await a.End);
        Assert.Equal(Reason.Completed, await b.End);
        Assert.Equal(new float[] { 4, 12, 20 }, samples);
        Assert.Equal(10, movement.To);
        Assert.Equal(0, movement.Delay);
        Assert.Null(movement.OnUpdate);
        Assert.Equal(0, scheduler.ActiveCount);
    }

    [Theory]
    [InlineData(FillMode.None, 3)]
    [InlineData(FillMode.RetainFinalValue, 10)]
    public void DefaultAndExplicitFillModesRemainDistinct(FillMode fill, float expected)
    {
        Assert.Equal(new TweenOptions(), default(TweenOptions));
        Assert.Equal(FillMode.RetainFinalValue, default(TweenOptions).Fill);
        Assert.Equal(FillMode.RetainFinalValue, default(Tweens.Position2D).Fill);
        Assert.Equal(new Tweens.Position2D(), default(Tweens.Position2D));
        Assert.Equal(default, new TweenOptions { Fill = FillMode.RetainFinalValue });
        using var scheduler = new TweenScheduler();
        var meter = new Meter { Value = 3 };
        scheduler.Add(meter, movement with { Fill = fill });
        scheduler.Update(1);
        Assert.Equal(expected, meter.Value);
    }

    [Fact]
    public void SharedOptionsAndFlatOverridesAreValueCopies()
    {
        var timing = new TweenOptions { Duration = 2, Delay = 0.2, Repeats = 3 };
        var progress = new Tweens.PathFollow2DProgressRatio { Options = timing, To = 1 };
        var trailing = progress with { Duration = 4, Delay = 0.5 };
        Assert.Equal(timing, progress.Options);
        Assert.Equal(timing with { Duration = 4, Delay = 0.5 }, trailing.Options);
        Assert.Equal(1, trailing.To);
        Assert.Equal(trailing, progress with { Options = trailing.Options });
    }

    [Fact]
    public void TimingValidationStillHappensBeforeCallbacksOrWrites()
    {
        using var scheduler = new TweenScheduler();
        var meter = new Meter { Value = 3 };
        var called = false;
        Assert.Throws<ArgumentOutOfRangeException>(() => scheduler.Add(meter,
            movement with { Delay = double.NaN, OnAdd = _ => called = true }));
        Assert.False(called);
        Assert.Equal(3, meter.Value);
        Assert.Equal(0, scheduler.ActiveCount);
    }

    [Fact]
    public void EveryBuiltInAdapterHasAnImmutableTypedDefinition()
    {
        var assembly = typeof(TweenOptions).Assembly;
        var adapters = assembly.GetTypes().Where(type => type.Namespace == "tweens.gd"
            && type.IsSealed && type.IsClass && type.Name.Contains("Tween")
            && type.BaseType is { IsGenericType: true } parent
            && (parent.GetGenericTypeDefinition() == typeof(PropertyTween<,>)
                || parent.GetGenericTypeDefinition() == typeof(TweenDefinition<,>)
                || parent.GetGenericTypeDefinition() == typeof(InstanceShaderParameterTween<,>))).ToArray();
        Assert.NotEmpty(adapters);
        foreach (var adapter in adapters)
        {
            var name = adapter.Name.Replace("Tween", "");
            var definition = assembly.GetType("Tweens." + name, throwOnError: true)!;
            Assert.True(definition.IsValueType);
            Assert.NotNull(definition.GetCustomAttribute<IsReadOnlyAttribute>());
            Assert.Contains(definition.GetInterfaces(), type => type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(ITweenDefinition<,>));
            foreach (var property in definition.GetProperties())
                Assert.Contains(typeof(IsExternalInit), property.SetMethod!.ReturnParameter.GetRequiredCustomModifiers());
        }
    }
}
