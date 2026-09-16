// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

namespace godotweens;

/// <summary>A reusable typed definition. Override the three property operations for custom tweens.</summary>
public abstract class TweenDefinition<TTarget, TValue> : TweenOptions
    where TTarget : class where TValue : struct
{
    public TValue? From { get; set; }
    public TValue? To { get; set; }
    public Action<TweenInstance<TTarget, TValue>>? OnAdd { get; set; }
    public Action<TweenInstance<TTarget, TValue>>? OnStart { get; set; }
    public Action<TweenInstance<TTarget, TValue>, TValue>? OnUpdate { get; set; }
    public Action<TweenInstance<TTarget, TValue>>? OnEnd { get; set; }
    public Action<TweenInstance<TTarget, TValue>>? OnCancel { get; set; }
    public Action<TweenInstance<TTarget, TValue>>? OnFinally { get; set; }

    protected abstract TValue Read(TTarget target);
    protected abstract void Write(TTarget target, TValue value);
    protected abstract TValue Interpolate(TValue from, TValue to, float weight);

    // Clone custom configuration as well, so subsequent property edits do not change bindings.
    internal TweenDefinition<TTarget, TValue> Snapshot() => (TweenDefinition<TTarget, TValue>)MemberwiseClone();
    internal TValue ReadValue(TTarget target) => Read(target);
    internal void WriteValue(TTarget target, TValue value) => Write(target, value);
    internal TValue InterpolateValue(TValue from, TValue to, float weight) => Interpolate(from, to, weight);
}

/// <summary>Animate a custom property without reflection. Captured mutable objects remain shared.</summary>
public class PropertyTween<TTarget, TValue>(Func<TTarget, TValue> getter,
    Action<TTarget, TValue> setter, Func<TValue, TValue, float, TValue> interpolate)
    : TweenDefinition<TTarget, TValue> where TTarget : class where TValue : struct
{
    private readonly Func<TTarget, TValue> getter = getter ?? throw new ArgumentNullException(nameof(getter));
    private readonly Action<TTarget, TValue> setter = setter ?? throw new ArgumentNullException(nameof(setter));
    private readonly Func<TValue, TValue, float, TValue> interpolate = interpolate ?? throw new ArgumentNullException(nameof(interpolate));
    protected override TValue Read(TTarget target) => getter(target);
    protected override void Write(TTarget target, TValue value) => setter(target, value);
    protected override TValue Interpolate(TValue from, TValue to, float weight) => interpolate(from, to, weight);
}
