// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using tweens.gd;

namespace testbed.Tests;

public class PlaybackTests
{
    private sealed class Box { public float Value { get; set; } = 7; }

    // This subclass is intentionally in the consuming assembly.
    private sealed class BoxTween : TweenDefinition<Box, float>
    {
        protected override float Read(Box target) => target.Value;
        protected override void Write(Box target, float value) => target.Value = value;
        protected override float Interpolate(float from, float to, float weight) => Interpolators.Float(from, to, weight);
    }

    [Fact]
    public async Task DelayConsumesOnlyRemainingDeltaAndCallsCallbacksInOrder()
    {
        using var scheduler = new TweenScheduler();
        var box = new Box();
        var events = new List<string>();
        var tween = scheduler.Add(box, new BoxTween
        {
            From = 0, To = 10, Duration = 1, Delay = 0.25,
            OnAdd = _ => events.Add("add"), OnStart = _ => events.Add("start"),
            OnUpdate = (_, _) => events.Add("update"), OnEnd = _ => events.Add("end"),
            OnFinally = t => { Assert.True(t.IsTerminal); events.Add("finally"); },
        });
        scheduler.Update(0.5);
        Assert.Equal(2.5f, box.Value);
        scheduler.Update(0.75);
        Assert.Equal(10, box.Value);
        Assert.Equal(Reason.Completed, await tween.Completion);
        Assert.Equal(new[] { "add", "start", "update", "update", "end", "finally" }, events);
        Assert.Equal(0, scheduler.ActiveCount);
    }

    [Theory]
    [InlineData(FillMode.None, 7, 7)]
    [InlineData(FillMode.ApplyFromDuringDelay, 0, 7)]
    [InlineData(FillMode.RetainFinalValue, 7, 10)]
    [InlineData(FillMode.Both, 0, 10)]
    public void FillModes(FillMode mode, float duringDelay, float final)
    {
        using var scheduler = new TweenScheduler();
        var box = new Box();
        scheduler.Add(box, new BoxTween { From = 0, To = 10, Delay = 1, Duration = 1, Fill = mode });
        Assert.Equal(duringDelay, box.Value);
        scheduler.Update(2);
        Assert.Equal(final, box.Value);
    }

    [Fact]
    public void SnapshotsEndpointsAndConfiguration()
    {
        using var scheduler = new TweenScheduler();
        var a = new Box();
        var b = new Box { Value = 20 };
        var definition = new BoxTween { To = 0, Duration = 1 };
        scheduler.Add(a, definition);
        definition.To = 30;
        scheduler.Add(b, definition);
        definition.Duration = 10;
        definition.To = 100;
        a.Value = -100;
        scheduler.Update(0.5);
        Assert.Equal(3.5f, a.Value);
        Assert.Equal(25, b.Value);
    }

    [Fact]
    public void PingPongIntervalsAndRepeats()
    {
        using var scheduler = new TweenScheduler();
        var box = new Box();
        var tween = scheduler.Add(box, new BoxTween
        { From = 0, To = 10, Duration = 1, UsePingPong = true, PingPongInterval = 0.5, RepeatInterval = 0.25, Repeats = 1 });
        scheduler.Update(1);
        Assert.Equal(10, box.Value);
        scheduler.Update(0.25);
        Assert.Equal(TweenState.Interval, tween.State);
        Assert.Equal(10, box.Value);
        scheduler.Update(0.75);
        Assert.Equal(5, box.Value);
        scheduler.Update(0.5);
        Assert.Equal(0, box.Value);
        Assert.False(tween.IsTerminal);
        scheduler.Update(2.75);
        Assert.Equal(0, box.Value);
        Assert.Equal(TweenState.Completed, tween.State);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void LargeAndSubdividedDeltasReachSameState(bool pingPong)
    {
        using var whole = new TweenScheduler();
        using var pieces = new TweenScheduler();
        var definition = new BoxTween { From = -1, To = 2, Duration = 1, Delay = 0.25,
            RepeatInterval = 0.25, PingPongInterval = 0.25, UsePingPong = pingPong, Repeats = TweenOptions.Infinite, Offset = 0.5 };
        var a = new Box(); var b = new Box();
        var first = whole.Add(a, definition); var second = pieces.Add(b, definition);
        whole.Update(123.5);
        for (var i = 0; i < 494; i++) pieces.Update(0.25);
        Assert.Equal(a.Value, b.Value);
        Assert.Equal(first.State, second.State);
        Assert.Equal(first.Progress, second.Progress);
    }

    [Fact]
    public void ZeroDurationAndHugeFiniteRepeatsFinishWithoutIteration()
    {
        using var scheduler = new TweenScheduler();
        var box = new Box();
        var tween = scheduler.Add(box, new BoxTween { To = 12, Repeats = int.MaxValue });
        Assert.False(tween.IsTerminal);
        scheduler.Update(0);
        Assert.True(tween.IsTerminal);
        Assert.Equal(12, box.Value);
        Assert.Throws<ArgumentException>(() => scheduler.Add(box, new BoxTween { Repeats = TweenOptions.Infinite }));
    }

    [Fact]
    public void DefaultRepeatsPlaysOnceAndRejectsValuesBelowInfinite()
    {
        using var scheduler = new TweenScheduler();
        var box = new Box();
        var tween = scheduler.Add(box, new BoxTween { From = 0, To = 10, Duration = 1 });
        scheduler.Update(1);
        Assert.True(tween.IsTerminal);
        Assert.Equal(10, box.Value);
        Assert.Throws<ArgumentOutOfRangeException>(() => scheduler.Add(box, new BoxTween { Duration = 1, Repeats = -2 }));
    }

    [Fact]
    public void OffsetIsOnlyAppliedToFirstCycle()
    {
        using var scheduler = new TweenScheduler();
        var box = new Box();
        var tween = scheduler.Add(box, new BoxTween { From = 0, To = 10, Duration = 1, Offset = 0.5, Repeats = 1 });
        scheduler.Update(0.75);
        Assert.Equal(2.5f, box.Value);
        scheduler.Update(0.75);
        Assert.Equal(TweenState.Completed, tween.State);
    }

    [Fact]
    public async Task PauseAndCancellationAreIdempotent()
    {
        using var scheduler = new TweenScheduler();
        var box = new Box();
        var callbacks = 0;
        var tween = scheduler.Add(box, new BoxTween { To = 0, Duration = 1, Fill = FillMode.None,
            OnCancel = t => { callbacks++; t.Cancel(); }, OnFinally = _ => callbacks++ });
        tween.Pause();
        scheduler.Update(20);
        Assert.Equal(7, box.Value);
        tween.Resume();
        scheduler.Update(0.5);
        tween.Cancel();
        tween.Cancel();
        Assert.Equal(3.5f, box.Value);
        Assert.Equal(2, callbacks);
        Assert.Equal(Reason.Cancelled, await tween.Completion);
    }

    [Fact]
    public void CancelFromStartNeverWritesAndCallbackAdditionWaitsForNextTick()
    {
        using var scheduler = new TweenScheduler();
        var first = new Box(); var second = new Box();
        scheduler.Add(first, new BoxTween { To = 0, OnStart = t =>
        {
            t.Cancel();
            scheduler.Add(second, new BoxTween { To = 0 });
        } });
        scheduler.Update(1);
        Assert.Equal(7, first.Value);
        Assert.Equal(7, second.Value);
        scheduler.Update(0);
        Assert.Equal(0, second.Value);
    }

    [Fact]
    public void CancelFromUpdatePreventsRestorationAndEndCallback()
    {
        using var scheduler = new TweenScheduler();
        var box = new Box();
        var ended = false;
        scheduler.Add(box, new BoxTween { To = 10, Fill = FillMode.None,
            OnUpdate = (t, _) => t.Cancel(), OnEnd = _ => ended = true });
        scheduler.Update(1);
        Assert.Equal(10, box.Value);
        Assert.False(ended);
    }

    [Fact]
    public async Task ExceptionsFaultOneTweenAndFinallyStillRuns()
    {
        using var scheduler = new TweenScheduler();
        var reports = new List<Exception>();
        scheduler.UnhandledException += reports.Add;
        var final = 0;
        var failed = scheduler.Add(new Box(), new BoxTween { OnUpdate = (_, _) => throw new InvalidOperationException("update"),
            OnFinally = _ => { final++; throw new ArgumentException("finally"); } });
        var healthy = scheduler.Add(new Box(), new BoxTween { To = 12 });
        scheduler.Update(1);
        var error = await Assert.ThrowsAsync<AggregateException>(async () => await failed.Completion);
        Assert.Equal(2, error.InnerExceptions.Count);
        Assert.Single(reports);
        Assert.Equal(1, final);
        Assert.Equal(Reason.Completed, await healthy.Completion);
    }

    [Fact]
    public async Task MultipleWaitersAndWaitCancellationDoNotCancelPlayback()
    {
        using var scheduler = new TweenScheduler();
        using var token = new CancellationTokenSource();
        var tween = scheduler.Add(new Box(), new BoxTween { Duration = 1 });
        var a = tween.Completion; var b = tween.Completion;
        Assert.Same(a, b);
        var waiting = tween.AwaitDecommissionAsync(token.Token);
        token.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await waiting);
        Assert.False(tween.IsTerminal);
        scheduler.Update(1);
        Assert.Equal(Reason.Completed, await a);
        Assert.Equal(Reason.Completed, await b);
    }

    [Fact]
    public async Task DisposalSettlesPausedWaitersEvenWhenCalledFromCallback()
    {
        var scheduler = new TweenScheduler();
        var first = scheduler.Add(new Box(), new BoxTween { OnStart = _ => scheduler.Dispose() });
        var second = scheduler.Add(new Box(), new BoxTween { Duration = 5 });
        second.Pause();
        scheduler.Update(1);
        Assert.Equal(Reason.RunnerDisposed, await first.Completion);
        Assert.Equal(Reason.RunnerDisposed, await second.Completion);
        Assert.Equal(0, scheduler.ActiveCount);
    }

    [Fact]
    public void RecursiveCancellationAndDisposalAreSafe()
    {
        using var scheduler = new TweenScheduler();
        scheduler.Add(new Box(), new BoxTween { OnCancel = _ => scheduler.CancelAll() });
        scheduler.Add(new Box(), new BoxTween { OnCancel = _ => scheduler.Dispose() });
        scheduler.Add(new Box(), new BoxTween());
        scheduler.CancelAll();
        Assert.Equal(0, scheduler.ActiveCount);
    }

    [Fact]
    public void LanesAndUnscaledDeltaAreIndependent()
    {
        using var scheduler = new TweenScheduler();
        var a = new Box(); var b = new Box(); var c = new Box();
        scheduler.Add(a, new BoxTween { From = 0, To = 10, Duration = 1 });
        scheduler.Add(b, new BoxTween { From = 0, To = 10, Duration = 1, UseUnscaledTime = true });
        scheduler.Add(c, new BoxTween { From = 0, To = 10, Duration = 1, ProcessMode = TweenProcessMode.Physics });
        scheduler.Update(0.25, 0.5);
        Assert.Equal(2.5f, a.Value); Assert.Equal(5, b.Value); Assert.Equal(7, c.Value);
        scheduler.Update(0.5, mode: TweenProcessMode.Physics);
        Assert.Equal(5, c.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void RejectsInvalidTiming(double duration)
    {
        using var scheduler = new TweenScheduler();
        Assert.Throws<ArgumentOutOfRangeException>(() => scheduler.Add(new Box(), new BoxTween { Duration = duration }));
        Assert.Throws<ArgumentOutOfRangeException>(() => scheduler.Update(duration));
    }

    [Fact]
    public void SteadyStateManualUpdatesDoNotAllocate()
    {
        using var scheduler = new TweenScheduler();
        for (var i = 0; i < 100; i++) scheduler.Add(new Box(), new BoxTween { Duration = 1, Repeats = TweenOptions.Infinite });
        for (var i = 0; i < 100; i++) scheduler.Update(0.01);
        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 100; i++) scheduler.Update(0.01);
        Assert.Equal(0, GC.GetAllocatedBytesForCurrentThread() - before);
    }

    [Fact]
    public async Task CancellingThenThrowingStillFaultsCompletion()
    {
        using var scheduler = new TweenScheduler();
        var final = 0;
        var tween = scheduler.Add(new Box(), new BoxTween
        {
            OnUpdate = (t, _) => { t.Cancel(); throw new InvalidOperationException("after cancellation"); },
            OnFinally = _ => final++,
        });
        var waiting = tween.Completion;
        scheduler.Update(0);
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await waiting);
        Assert.Equal(TweenState.Faulted, tween.State);
        Assert.Equal(1, final);
    }

    [Fact]
    public void OnAddCanCancelAndConcurrentWritesUseInsertionOrder()
    {
        using var scheduler = new TweenScheduler();
        var box = new Box();
        var cancelled = scheduler.Add(box, new BoxTween { OnAdd = t => t.Cancel(), From = 0, Delay = 1, Fill = FillMode.Both });
        Assert.Equal(7, box.Value);
        Assert.True(cancelled.IsTerminal);
        scheduler.Add(box, new BoxTween { To = 10 });
        scheduler.Add(box, new BoxTween { To = 20 });
        scheduler.Update(0);
        Assert.Equal(20, box.Value);
    }

    [Fact]
    public void ZeroDurationPingPongAndRepeatIntervalsStillConsumeTime()
    {
        using var scheduler = new TweenScheduler();
        var box = new Box();
        var tween = scheduler.Add(box, new BoxTween { From = 0, To = 10, UsePingPong = true, PingPongInterval = 1, RepeatInterval = 1, Repeats = 1 });
        scheduler.Update(0.5);
        Assert.Equal(10, box.Value);
        scheduler.Update(1);
        Assert.Equal(0, box.Value);
        scheduler.Update(1);
        Assert.Equal(10, box.Value);
        scheduler.Update(0.5);
        Assert.Equal(0, box.Value);
        Assert.True(tween.IsTerminal);
    }

    [Fact]
    public void InjectedClockDoesNotAccumulatePausedFrames()
    {
        ulong ticks = 100;
        var clock = new MonotonicClock(() => ticks);
        using var scheduler = new TweenScheduler();
        var box = new Box();
        var tween = scheduler.Add(box, new BoxTween { From = 0, To = 10, Duration = 1, UseUnscaledTime = true });
        ticks += 250_000; scheduler.Update(0, clock.Sample());
        Assert.Equal(2.5f, box.Value);
        tween.Pause();
        ticks += 10_000_000; scheduler.Update(0, clock.Sample());
        tween.Resume();
        ticks += 250_000; scheduler.Update(0, clock.Sample());
        Assert.Equal(5, box.Value);
    }
}
