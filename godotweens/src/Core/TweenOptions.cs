// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

namespace GodotWeens;

[Flags]
public enum FillMode
{
    None = 0,
    ApplyFromDuringDelay = 1,
    RetainFinalValue = 2,
    Both = ApplyFromDuringDelay | RetainFinalValue,
}

public enum TweenProcessMode { Process, Physics }
public enum TweenPauseMode { Bound, SceneTree, Always }
public enum TweenState { Delayed, Playing, Interval, Completed, Cancelled, Faulted }
public enum TweenCompletionReason { Completed, Cancelled, TargetFreed, OwnerExited, RunnerDisposed }

/// <summary>Reusable timing configuration. Values are snapshotted on addition.</summary>
public abstract class TweenOptions
{
    public double Duration { get; set; }
    public double Delay { get; set; }
    public double PingPongInterval { get; set; }
    public double RepeatInterval { get; set; }
    public double Offset { get; set; }
    /// <summary>Total cycles, including the first. A ping-pong cycle includes both legs.</summary>
    public int LoopCount { get; set; } = 1;
    public bool IsInfinite { get; set; }
    public bool UsePingPong { get; set; }
    public bool UseUnscaledTime { get; set; }
    public FillMode Fill { get; set; } = FillMode.RetainFinalValue;
    public EaseType Ease { get; set; }
    public Func<float, float>? EaseFunction { get; set; }
    public Godot.Curve? Curve { get; set; }
    public TweenProcessMode ProcessMode { get; set; }
    public TweenPauseMode PauseMode { get; set; }
    public bool SuppressCallbacksWhenTargetInvalid { get; set; }
}

internal sealed class Playback
{
    private readonly double duration, delay, turn, repeat, offset, span, total;
    private readonly bool pingPong, infinite;
    private double elapsed;
    internal float Progress { get; private set; }
    internal bool Started { get; private set; }
    internal bool Completed { get; private set; }
    internal TweenState State { get; private set; } = TweenState.Delayed;

    internal Playback(TweenOptions options)
    {
        duration = Nonnegative(options.Duration, nameof(options.Duration));
        delay = Nonnegative(options.Delay, nameof(options.Delay));
        turn = Nonnegative(options.PingPongInterval, nameof(options.PingPongInterval));
        repeat = Nonnegative(options.RepeatInterval, nameof(options.RepeatInterval));
        offset = Nonnegative(options.Offset, nameof(options.Offset));
        if (offset > duration) throw new ArgumentOutOfRangeException(nameof(options.Offset));
        if (options.LoopCount < 1) throw new ArgumentOutOfRangeException(nameof(options.LoopCount));
        if (!Enum.IsDefined(options.ProcessMode) || !Enum.IsDefined(options.PauseMode) ||
            (options.Fill & ~FillMode.Both) != 0)
            throw new ArgumentException("Invalid tween mode.", nameof(options));
        pingPong = options.UsePingPong;
        infinite = options.IsInfinite;
        span = duration + (pingPong ? turn + duration : 0) + repeat;
        total = span * options.LoopCount - repeat;
        if (!double.IsFinite(span) || !double.IsFinite(total + delay))
            throw new ArgumentOutOfRangeException(nameof(options.Duration), "Timeline is too long.");
        if (infinite && span == 0)
            throw new ArgumentException("An infinite tween must have a nonzero cycle duration.", nameof(options));
    }

    internal static double Nonnegative(double value, string name)
    {
        if (!double.IsFinite(value) || value < 0) throw new ArgumentOutOfRangeException(name);
        return value;
    }

    internal void Advance(double delta)
    {
        // Saturation also makes extremely large finite deltas well-defined.
        elapsed = Math.Min(double.MaxValue, elapsed + delta);
        if (elapsed < delay) return;
        Started = true;
        var time = Math.Min(double.MaxValue, elapsed - delay + offset);
        if (!infinite && time >= total)
        {
            Progress = pingPong ? 0 : 1;
            Completed = true;
            State = TweenState.Completed;
            return;
        }
        var local = time % span;
        // At a cycle boundary, display the previous endpoint before restarting.
        if (local == 0 && time > 0) local = span;
        State = TweenState.Playing;
        if (duration > 0 && local <= duration)
            Progress = (float)(local / duration);
        else if (!pingPong)
        {
            Progress = 1;
            State = TweenState.Interval;
        }
        else if (local < duration + turn)
        {
            Progress = 1;
            State = TweenState.Interval;
        }
        else if (duration > 0 && local <= duration * 2 + turn)
            Progress = (float)(1 - (local - duration - turn) / duration);
        else
        {
            Progress = 0;
            State = TweenState.Interval;
        }
    }
}
