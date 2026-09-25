// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

namespace tweens.gd;

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
public enum Reason { Completed, Cancelled, TargetFreed, OwnerExited, RunnerDisposed }

/// <summary>Reusable timing configuration. Values are snapshotted on addition.</summary>
public class TweenOptions
{
    /// <summary>A <see cref="Repeats"/> value that repeats until cancelled.</summary>
    public const int Infinite = -1;

    public double Duration { get; set; }
    public double Delay { get; set; }
    public double PingPongInterval { get; set; }
    public double RepeatInterval { get; set; }
    public double Offset { get; set; }
    /// <summary>Cycles after the first, or <see cref="Infinite"/>. A ping-pong cycle includes both legs.</summary>
    public int Repeats { get; set; }
    public bool UsePingPong { get; set; }
    public bool UseUnscaledTime { get; set; }
    public FillMode Fill { get; set; } = FillMode.RetainFinalValue;
    public EaseType Ease { get; set; }
    public Func<float, float>? EaseFunction { get; set; }
    public Godot.Curve? Curve { get; set; }
    public TweenProcessMode ProcessMode { get; set; }
    public TweenPauseMode PauseMode { get; set; }
    public bool SuppressCallbacksWhenTargetInvalid { get; set; }

    internal void CopyTo(TweenOptions target)
    {
        target.Duration = Duration;
        target.Delay = Delay;
        target.PingPongInterval = PingPongInterval;
        target.RepeatInterval = RepeatInterval;
        target.Offset = Offset;
        target.Repeats = Repeats;
        target.UsePingPong = UsePingPong;
        target.UseUnscaledTime = UseUnscaledTime;
        target.Fill = Fill;
        target.Ease = Ease;
        target.EaseFunction = EaseFunction;
        target.Curve = Curve;
        target.ProcessMode = ProcessMode;
        target.PauseMode = PauseMode;
        target.SuppressCallbacksWhenTargetInvalid = SuppressCallbacksWhenTargetInvalid;
    }
}

internal sealed class Playback
{
    private readonly double duration, delay, turn, repeat, offset, span, total;
    private readonly bool pingPong;
    private double elapsed;
    internal float Progress { get; private set; }
    internal bool Started { get; private set; }
    internal bool Completed { get; private set; }
    /// <summary>Time past the end of the timeline in the completing update.</summary>
    internal double Overshoot { get; private set; }
    internal TweenState State { get; private set; } = TweenState.Delayed;

    internal Playback(TweenOptions options)
    {
        duration = Nonnegative(options.Duration, nameof(options.Duration));
        delay = Nonnegative(options.Delay, nameof(options.Delay));
        turn = Nonnegative(options.PingPongInterval, nameof(options.PingPongInterval));
        repeat = Nonnegative(options.RepeatInterval, nameof(options.RepeatInterval));
        offset = Nonnegative(options.Offset, nameof(options.Offset));
        if (offset > duration) throw new ArgumentOutOfRangeException(nameof(options.Offset));
        if (options.Repeats < TweenOptions.Infinite) throw new ArgumentOutOfRangeException(nameof(options.Repeats));
        if (!Enum.IsDefined(options.ProcessMode) || !Enum.IsDefined(options.PauseMode) ||
            (options.Fill & ~FillMode.Both) != 0)
            throw new ArgumentException("Invalid tween mode.", nameof(options));
        pingPong = options.UsePingPong;
        var infinite = options.Repeats == TweenOptions.Infinite;
        span = duration + (pingPong ? turn + duration : 0) + repeat;
        // Double arithmetic keeps int.MaxValue repeats from overflowing.
        total = infinite ? double.PositiveInfinity : span * ((double)options.Repeats + 1) - repeat;
        if (!double.IsFinite(span + delay) || !infinite && !double.IsFinite(total + delay))
            throw new ArgumentOutOfRangeException(nameof(options.Duration), "Timeline is too long.");
        if (infinite && span == 0)
            throw new ArgumentException("An infinite tween must have a nonzero cycle duration.", nameof(options));
    }

    /// <summary>Starts the timeline this many seconds in, e.g. where a predecessor's timeline ended.</summary>
    internal void Credit(double seconds) => elapsed = seconds;

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
        if (time >= total)
        {
            Progress = pingPong ? 0 : 1;
            Overshoot = time - total;
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
