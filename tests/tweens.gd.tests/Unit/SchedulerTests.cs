// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using tweens.gd.Tests.Support;

namespace tweens.gd.Tests.Unit;

public class SchedulerTests
{
    [Fact]
    public void AddValidatesItsArguments()
    {
        using var scheduler = new TweenScheduler();
        Assert.Throws<ArgumentNullException>(() => scheduler.Add<Box, float>(null!, new PlainTween()));
        Assert.Throws<ArgumentNullException>(() => scheduler.Add(new Box(), (ITweenDefinition<Box, float>)null!));
        Assert.Throws<ArgumentNullException>(() => scheduler.Add(new Box(), new PlainTween(), null!));
    }

    [Fact]
    public void ActiveCountTracksUnfinishedTweens()
    {
        using var scheduler = new TweenScheduler();
        Assert.Equal(0, scheduler.ActiveCount);
        var short1 = scheduler.Add(new Box(), new PlainTween { Duration = 1 });
        scheduler.Add(new Box(), new PlainTween { Duration = 2 });
        Assert.Equal(2, scheduler.ActiveCount);
        short1.Cancel();
        Assert.Equal(1, scheduler.ActiveCount);
        scheduler.Update(2);
        Assert.Equal(0, scheduler.ActiveCount);
    }

    [Fact]
    public void UpdateValidatesItsArguments()
    {
        using var scheduler = new TweenScheduler();
        Assert.Throws<ArgumentOutOfRangeException>(() => scheduler.Update(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => scheduler.Update(1, double.NaN));
        Assert.Throws<ArgumentOutOfRangeException>(() => scheduler.Update(1, mode: (TweenProcessMode)2));
    }

    [Fact]
    public void RecursiveUpdatesFaultTheCallingTween()
    {
        using var scheduler = new TweenScheduler();
        var tween = scheduler.Add(new Box(), new PlainTween { Duration = 1, OnUpdate = (t, _) => t.Scheduler.Update(0) });
        scheduler.Update(0.5);
        Assert.Equal(TweenState.Faulted, tween.State);
        Assert.IsType<InvalidOperationException>(tween.Error);
    }

    [Fact]
    public void UnscaledTweensUseTheUnscaledDeltaWhenGiven()
    {
        using var scheduler = new TweenScheduler();
        var scaled = new Box(); var unscaled = new Box();
        scheduler.Add(scaled, new PlainTween { To = 1, Duration = 1 });
        scheduler.Add(unscaled, new PlainTween { To = 1, Duration = 1, UseUnscaledTime = true });
        scheduler.Update(0.25);
        Assert.Equal(0.25f, unscaled.Value);
        scheduler.Update(0.25, 0.5);
        Assert.Equal(0.5f, scaled.Value);
        Assert.Equal(0.75f, unscaled.Value);
    }

    [Fact]
    public void TweensAddedDuringAnUpdateWaitForTheNextOne()
    {
        using var scheduler = new TweenScheduler();
        var later = new Box();
        TweenInstance? added = null;
        scheduler.Add(new Box(), new PlainTween
        {
            Duration = 1,
            OnStart = t => added = t.Scheduler.Add(later, new PlainTween { To = 4, Duration = 1 }),
        });
        scheduler.Update(0.5);
        Assert.NotNull(added);
        Assert.Equal(0, later.Value);
        scheduler.Update(0.5);
        Assert.Equal(2, later.Value);
    }

    [Fact]
    public void CancelAllToleratesCallbacksThatAddOrRemoveTweens()
    {
        using var scheduler = new TweenScheduler();
        var survivors = new List<TweenInstance>();
        scheduler.Add(new Box(), new PlainTween
        {
            Duration = 1,
            OnCancel = t => survivors.Add(t.Scheduler.Add(new Box(), new PlainTween { Duration = 1 })),
        });
        scheduler.Add(new Box(), new PlainTween { Duration = 1 });
        scheduler.CancelAll();
        Assert.Single(survivors);
        Assert.False(survivors[0].IsTerminal);
        Assert.Equal(1, scheduler.ActiveCount);
    }

    [Fact]
    public void ObserverExceptionsAreSwallowed()
    {
        using var scheduler = new TweenScheduler();
        scheduler.Report(new FormatException("nobody listens"));
        scheduler.UnhandledException += _ => throw new InvalidOperationException("observer");
        var failing = scheduler.Add(new Box(), new PlainTween { OnStart = _ => throw new FormatException() });
        scheduler.Update(0);
        Assert.Equal(TweenState.Faulted, failing.State);
        Assert.IsType<FormatException>(failing.Error);
    }

    [Fact]
    public void DisposalIsIdempotentAndRejectsFurtherUse()
    {
        var scheduler = new TweenScheduler();
        var tween = scheduler.Add(new Box(), new PlainTween { Duration = 1 });
        scheduler.UnhandledException += _ => { };
        scheduler.Dispose();
        scheduler.Dispose();
        Assert.Equal(Reason.RunnerDisposed, tween.CompletionReason);
        Assert.Throws<ObjectDisposedException>(() => scheduler.Add(new Box(), new PlainTween()));
        Assert.Throws<ObjectDisposedException>(() => scheduler.Update(0));
        Assert.Equal(0, scheduler.ActiveCount);
    }

    [Fact]
    public void DisposalDuringUpdateStopsTheUpdate()
    {
        var scheduler = new TweenScheduler();
        var after = new Box();
        scheduler.Add(new Box(), new PlainTween { Duration = 1, OnUpdate = (t, _) => t.Scheduler.Dispose() });
        var second = scheduler.Add(after, new PlainTween { To = 1, Duration = 1 });
        scheduler.Update(0.5);
        Assert.Equal(0, after.Value);
        Assert.Equal(Reason.RunnerDisposed, second.CompletionReason);
        Assert.Equal(0, scheduler.ActiveCount);
    }

    [Fact]
    public void DisposalDuringPreparationSettlesTheNewTween()
    {
        var scheduler = new TweenScheduler();
        var released = false;
        var tween = scheduler.Add(new Box(), new ProbeTween
        {
            Duration = 1,
            Preparing = _ => scheduler.Dispose(),
            Releasing = () => released = true,
        });
        Assert.Equal(Reason.RunnerDisposed, tween.CompletionReason);
        Assert.True(released);
        Assert.True(tween.End.IsCompletedSuccessfully);
    }

    [Fact]
    public void SchedulersBelongToTheirCreatingThread()
    {
        using var scheduler = new TweenScheduler();
        var tween = scheduler.Add(new Box(), new PlainTween { Duration = 1 });
        Threads.RunElsewhere(() =>
        {
            Assert.Throws<InvalidOperationException>(() => scheduler.Update(0));
            Assert.Throws<InvalidOperationException>(() => scheduler.Add(new Box(), new PlainTween()));
            Assert.Throws<InvalidOperationException>(() => scheduler.ActiveCount);
            Assert.Throws<InvalidOperationException>(() => scheduler.CancelAll());
            Assert.Throws<InvalidOperationException>(() => scheduler.Dispose());
            Assert.Throws<InvalidOperationException>(() => tween.Pause());
            Assert.Throws<InvalidOperationException>(() => tween.Cancel());
            Assert.Throws<InvalidOperationException>(() => { _ = tween.End; });
        });
        Assert.False(tween.IsTerminal);
    }
}
