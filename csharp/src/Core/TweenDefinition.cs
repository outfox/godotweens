// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

namespace tweens.gd;

/// <summary>A definition that can start on <typeparamref name="TTarget"/>, whatever its value type.</summary>
public interface ITweenDefinition<in TTarget> where TTarget : class
{
    internal TweenInstance AddTo(TweenScheduler scheduler, TTarget target);
}

/// <summary>A reusable typed definition. Override the three property operations for custom tweens.</summary>
public abstract class TweenDefinition<TTarget, TValue> : TweenOptions, ITweenDefinition<TTarget>
    where TTarget : class where TValue : struct
{
    TweenInstance ITweenDefinition<TTarget>.AddTo(TweenScheduler scheduler, TTarget target) => scheduler.Add(target, this);

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

    /// <summary>Prepare per-playback bindings on the private snapshot, before its initial read.</summary>
    protected virtual void Prepare(TTarget target) { }
    /// <summary>Restore captured state at non-retaining completion. May remove an override instead of writing a value.</summary>
    protected virtual void Restore(TTarget target, TValue initial) => Write(target, initial);
    /// <summary>Release only resources owned by this playback snapshot, including after failed preparation.</summary>
    protected virtual void Release() { }

    internal void PrepareTarget(TTarget target) => Prepare(target);
    internal void RestoreValue(TTarget target, TValue initial) => Restore(target, initial);
    internal void ReleaseSnapshot() => Release();

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
