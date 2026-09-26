// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

namespace tweens.gd.Tests.Support;

/// <summary>A plain managed target, so scheduler behavior can be tested without the engine.</summary>
public sealed class Box
{
    public float Value { get; set; }
}

/// <summary>A float tween whose property operations and hooks can be replaced to inject behavior or faults.</summary>
public sealed class ProbeTween : TweenDefinition<Box, float>
{
    public Func<Box, float>? Reader { get; set; }
    public Action<Box, float>? Writer { get; set; }
    public Func<float, float, float, float>? Interpolator { get; set; }
    public Action<Box>? Preparing { get; set; }
    public Action<Box, float>? Restoring { get; set; }
    public Action? Releasing { get; set; }

    protected override float Read(Box target) => Reader is null ? target.Value : Reader(target);

    protected override void Write(Box target, float value)
    {
        if (Writer is null) target.Value = value;
        else Writer(target, value);
    }

    protected override float Interpolate(float from, float to, float weight)
        => Interpolator is null ? Interpolators.Float(from, to, weight) : Interpolator(from, to, weight);

    protected override void Prepare(Box target) => Preparing?.Invoke(target);

    protected override void Restore(Box target, float initial)
    {
        if (Restoring is null) base.Restore(target, initial);
        else Restoring(target, initial);
    }

    protected override void Release() => Releasing?.Invoke();
}

/// <summary>A float tween relying on every default hook of <see cref="TweenDefinition{TTarget,TValue}"/>.</summary>
public sealed class PlainTween : TweenDefinition<Box, float>
{
    protected override float Read(Box target) => target.Value;
    protected override void Write(Box target, float value) => target.Value = value;
    protected override float Interpolate(float from, float to, float weight) => Interpolators.Float(from, to, weight);
}
