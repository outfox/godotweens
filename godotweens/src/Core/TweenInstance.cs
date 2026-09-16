// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;

namespace GodotWeens;

/// <summary>A single playback. Mutating operations belong to its scheduler's thread.</summary>
public abstract class TweenInstance
{
    private TaskCompletionSource<TweenCompletionReason>? completion;
    private bool settled;
    private int operationDepth;
    private bool paused;
    private Action? exitHandler;
    internal readonly TweenScheduler Scheduler;
    internal readonly Node? Owner;
    internal readonly TweenProcessMode Mode;
    private readonly TweenPauseMode pauseMode;
    internal readonly bool Unscaled;
    private protected readonly Playback Clock;

    public TweenState State { get; protected set; } = TweenState.Delayed;
    public bool IsTerminal => State is TweenState.Completed or TweenState.Cancelled or TweenState.Faulted;
    public TweenCompletionReason? CompletionReason { get; private set; }
    public Exception? Error { get; private set; }
    public float Progress => Clock.Progress;
    public bool IsPaused
    {
        get => paused;
        set { Scheduler.EnsureThread(); paused = value; }
    }

    /// <summary>Shared completion. Cancellation is a result; callback errors fault the task.</summary>
    public Task<TweenCompletionReason> Completion
    {
        get
        {
            Scheduler.EnsureThread();
            if (completion is null)
            {
                completion = new();
                if (settled) SetCompletion();
            }
            return completion.Task;
        }
    }

    internal TweenInstance(TweenScheduler scheduler, TweenOptions options, Node? owner)
    {
        Scheduler = scheduler;
        Owner = owner;
        Mode = options.ProcessMode;
        pauseMode = options.PauseMode;
        Unscaled = options.UseUnscaledTime;
        Clock = new Playback(options);
    }

    public void Pause() => IsPaused = true;
    public void Resume() => IsPaused = false;
    public void Cancel()
    {
        Scheduler.EnsureThread();
        Finish(TweenCompletionReason.Cancelled);
    }

    /// <summary>The token cancels only this wait, not the tween. Use Cancel to stop playback.</summary>
    public Task<TweenCompletionReason> AwaitDecommissionAsync(CancellationToken cancellationToken = default)
        => cancellationToken.CanBeCanceled ? Completion.WaitAsync(cancellationToken) : Completion;

    internal void BindLifetime()
    {
        if (Owner is null) return;
        exitHandler = () => Finish(Owner.IsQueuedForDeletion()
            ? TweenCompletionReason.TargetFreed : TweenCompletionReason.OwnerExited);
        Owner.TreeExiting += exitHandler;
    }

    internal bool CheckTarget()
    {
        if (IsTerminal) return false;
        if (Owner is null) return true;
        if (!GodotObject.IsInstanceValid(Owner) || Owner.IsQueuedForDeletion())
            Finish(TweenCompletionReason.TargetFreed);
        else if (!Owner.IsInsideTree())
            Finish(TweenCompletionReason.OwnerExited);
        return !IsTerminal;
    }

    internal bool CanAdvance()
    {
        if (!CheckTarget() || paused) return false;
        return Owner is null || pauseMode switch
        {
            TweenPauseMode.Always => true,
            TweenPauseMode.SceneTree => !Owner.GetTree().Paused,
            _ => Owner.CanProcess(),
        };
    }

    internal abstract void Initialize();
    internal abstract void Advance(double delta);
    protected abstract void InvokeTerminal(TweenCompletionReason reason, bool faulted);
    protected abstract void Release();

    private protected void BeginOperation() => operationDepth++;
    private protected void EndOperation()
    {
        operationDepth--;
        if (IsTerminal && !settled && operationDepth == 0) Settle();
    }

    internal void Finish(TweenCompletionReason reason, Exception? error = null)
    {
        if (IsTerminal)
        {
            // A callback may cancel itself and then throw. Its awaiters still need the error.
            if (error is not null && !settled)
            {
                Error = Error is null ? error : new AggregateException(Error, error);
                State = TweenState.Faulted;
            }
            else if (error is not null) Scheduler.Report(error);
            return;
        }
        CompletionReason = reason;
        Error = error;
        State = error is not null ? TweenState.Faulted : reason == TweenCompletionReason.Completed
            ? TweenState.Completed : TweenState.Cancelled;
        try { InvokeTerminal(reason, error is not null); }
        catch (Exception callbackError)
        {
            Error = Error is null ? callbackError : new AggregateException(Error, callbackError);
            State = TweenState.Faulted;
        }
        finally
        {
            if (exitHandler is not null && GodotObject.IsInstanceValid(Owner))
                Owner!.TreeExiting -= exitHandler;
            exitHandler = null;
            Release();
            if (operationDepth == 0) Settle();
        }
    }

    private void Settle()
    {
        settled = true;
        if (Error is not null) Scheduler.Report(Error);
        SetCompletion();
    }

    private void SetCompletion()
    {
        if (Error is not null) completion?.TrySetException(Error);
        else completion?.TrySetResult(CompletionReason!.Value);
    }
}

public sealed class TweenInstance<TTarget, TValue> : TweenInstance
    where TTarget : class where TValue : struct
{
    private TweenDefinition<TTarget, TValue>? definition;
    private Func<float, float>? ease;
    private Curve? curve;
    private readonly TValue initial, from, to;
    private bool started;
    public TTarget Target { get; }
    public TValue Value { get; private set; }

    internal TweenInstance(TweenScheduler scheduler, TTarget target,
        TweenDefinition<TTarget, TValue> source, Node? owner) : base(scheduler, source, owner)
    {
        Target = target;
        definition = source.Snapshot();
        if (definition.Curve is not null && definition.EaseFunction is not null)
            throw new ArgumentException("Specify either Curve or EaseFunction, not both.", nameof(source));
        initial = definition.ReadValue(target);
        Value = initial;
        from = definition.From ?? initial;
        to = definition.To ?? initial;
        ease = definition.EaseFunction ?? Easing.GetFunction(definition.Ease);
        if (definition.Curve is not null)
        {
            curve = (Curve)definition.Curve.Duplicate();
            ease = curve.Sample;
        }
    }

    internal override void Initialize()
    {
        BeginOperation();
        try
        {
            definition!.OnAdd?.Invoke(this);
            if (!CheckTarget()) return;
            if (definition.Delay > 0 && definition.Fill.HasFlag(FillMode.ApplyFromDuringDelay)) Apply(from);
        }
        catch (Exception error) { Finish(TweenCompletionReason.Cancelled, error); }
        finally { EndOperation(); }
    }

    internal override void Advance(double delta)
    {
        BeginOperation();
        try
        {
            Clock.Advance(delta);
            State = Clock.State == TweenState.Completed ? TweenState.Playing : Clock.State;
            if (!Clock.Started) return;
            if (!started)
            {
                started = true;
                definition!.OnStart?.Invoke(this);
                if (!CheckTarget()) return;
            }
            var weight = ease!(Math.Clamp(Progress, 0, 1));
            if (!float.IsFinite(weight)) throw new InvalidOperationException("Easing returned a non-finite value.");
            if (!CheckTarget()) return;
            var value = definition!.InterpolateValue(from, to, weight);
            if (!CheckTarget()) return;
            Apply(value);
            if (!CheckTarget() || !Clock.Completed) return;
            if (!definition!.Fill.HasFlag(FillMode.RetainFinalValue)) Apply(initial);
            if (CheckTarget()) Finish(TweenCompletionReason.Completed);
        }
        catch (Exception error) { Finish(TweenCompletionReason.Cancelled, error); }
        finally { EndOperation(); }
    }

    private void Apply(TValue value)
    {
        if (!CheckTarget()) return;
        definition!.WriteValue(Target, value);
        Value = value;
        if (CheckTarget()) definition!.OnUpdate?.Invoke(this, value);
    }

    protected override void InvokeTerminal(TweenCompletionReason reason, bool faulted)
    {
        var snapshot = definition!;
        if (snapshot.SuppressCallbacksWhenTargetInvalid && Owner is not null &&
            (!GodotObject.IsInstanceValid(Owner) || Owner.IsQueuedForDeletion() || !Owner.IsInsideTree())) return;
        Exception? failure = null;
        try
        {
            if (!faulted)
            {
                if (reason == TweenCompletionReason.Completed) snapshot.OnEnd?.Invoke(this);
                else snapshot.OnCancel?.Invoke(this);
            }
        }
        catch (Exception error) { failure = error; }
        try { snapshot.OnFinally?.Invoke(this); }
        catch (Exception error) { failure = failure is null ? error : new AggregateException(failure, error); }
        if (failure is not null) throw failure;
    }

    protected override void Release()
    {
        definition = null;
        ease = null;
        curve?.Dispose();
        curve = null;
    }
}
