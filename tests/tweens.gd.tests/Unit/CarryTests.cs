// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using tweens.gd.Tests.Support;

namespace tweens.gd.Tests.Unit;

/// <summary>Tweens started as a timeline ends continue it, inheriting the overshoot of its last update.</summary>
public class CarryTests
{
    [Fact]
    public void TweensStartedFromOnEndInheritTheOvershoot()
    {
        using var scheduler = new TweenScheduler();
        var next = new Box();
        scheduler.Add(new Box(), new PlainTween
        {
            Duration = 1,
            OnEnd = t => t.Scheduler.Add(next, new PlainTween { To = 1, Duration = 1 }),
        });
        scheduler.Update(1.25);
        scheduler.Update(0.25);
        Assert.Equal(0.5f, next.Value);
    }

    [Fact]
    public void SynchronousContinuationsOfEndInheritTheOvershoot()
    {
        using var scheduler = new TweenScheduler();
        var next = new Box();
        var first = scheduler.Add(new Box(), new PlainTween { Duration = 1 });
        first.End.ContinueWith(_ => scheduler.Add(next, new PlainTween { To = 1, Duration = 1 }),
            TaskContinuationOptions.ExecuteSynchronously);
        scheduler.Update(1.5);
        scheduler.Update(0.25);
        Assert.Equal(0.75f, next.Value);
    }

    [Fact]
    public void FaultedAndCancelledTimelinesCarryNothing()
    {
        using var scheduler = new TweenScheduler();
        var afterFault = new Box();
        var afterCancel = new Box();
        var faulted = scheduler.Add(new Box(), new ProbeTween { Duration = 1, Releasing = () => throw new FormatException() });
        var cancelled = scheduler.Add(new Box(), new PlainTween { Duration = 2 });
        faulted.End.ContinueWith(_ => scheduler.Add(afterFault, new PlainTween { To = 1, Duration = 1 }),
            TaskContinuationOptions.ExecuteSynchronously);
        cancelled.End.ContinueWith(_ => scheduler.Add(afterCancel, new PlainTween { To = 1, Duration = 1 }),
            TaskContinuationOptions.ExecuteSynchronously);
        scheduler.Update(1.5);
        cancelled.Cancel();
        scheduler.Update(0.25);
        Assert.Equal(0.25f, afterFault.Value);
        Assert.Equal(0.25f, afterCancel.Value);
    }

    [Theory]
    [InlineData(TweenProcessMode.Physics, false)]
    [InlineData(TweenProcessMode.Process, true)]
    public void OnlyTheSameLaneAndTimeBaseInheritTheOvershoot(TweenProcessMode mode, bool unscaled)
    {
        using var scheduler = new TweenScheduler();
        var next = new Box();
        scheduler.Add(new Box(), new PlainTween
        {
            Duration = 1,
            OnEnd = t => t.Scheduler.Add(next, new PlainTween { To = 1, Duration = 1, ProcessMode = mode, UseUnscaledTime = unscaled }),
        });
        scheduler.Update(1.5);
        scheduler.Update(0.25, mode: mode);
        Assert.Equal(0.25f, next.Value);
    }

    [Fact]
    public void OtherSchedulersAndLaterTicksInheritNothing()
    {
        using var scheduler = new TweenScheduler();
        using var other = new TweenScheduler();
        var elsewhere = new Box();
        Carry? stamp = null;
        scheduler.Add(new Box(), new PlainTween
        {
            Duration = 1,
            OnEnd = t =>
            {
                stamp = t.Stamp;
                other.Add(elsewhere, new PlainTween { To = 1, Duration = 1 });
            },
        });
        scheduler.Update(1.5);
        other.Update(0.25);
        Assert.Equal(0.25f, elsewhere.Value);

        // A stale carry, from before the lane's latest update, is ignored.
        scheduler.Update(0);
        using (TweenCarry.Enter(stamp))
        {
            Assert.False(TweenCarry.TryGet(scheduler, TweenProcessMode.Process, false, out var credit));
            Assert.Equal(0, credit);
        }
    }

    [Fact]
    public void ScopesRestoreThePreviousCarry()
    {
        using var scheduler = new TweenScheduler();
        var outer = new Carry(scheduler, TweenProcessMode.Process, false, 0, 0.5);
        using (TweenCarry.Enter(outer))
        {
            using (TweenCarry.Enter(null))
                Assert.False(TweenCarry.TryGet(scheduler, TweenProcessMode.Process, false, out _));
            Assert.True(TweenCarry.TryGet(scheduler, TweenProcessMode.Process, false, out var credit));
            Assert.Equal(0.5, credit);
        }
        Assert.False(TweenCarry.TryGet(scheduler, TweenProcessMode.Process, false, out _));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GroupsFinishingInOneUpdateContinueFromTheSmallestOvershoot(bool reversed)
    {
        using var scheduler = new TweenScheduler();
        var next = new Box();
        var shorter = scheduler.Add(new Box(), new PlainTween { Duration = 0.5 });
        var longer = scheduler.Add(new Box(), new PlainTween { Duration = 1 });
        var group = reversed ? Group.Of(longer, shorter) : Group.Of(shorter, longer);
        group.End.ContinueWith(_ => scheduler.Add(next, new PlainTween { To = 1, Duration = 1 }),
            TaskContinuationOptions.ExecuteSynchronously);
        scheduler.Update(1.25);
        scheduler.Update(0.25);
        Assert.Equal(0.5f, next.Value);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GroupsFinishingOverSeveralUpdatesContinueFromTheLatest(bool reversed)
    {
        using var scheduler = new TweenScheduler();
        var next = new Box();
        var early = scheduler.Add(new Box(), new PlainTween { Duration = 0.5 });
        var late = scheduler.Add(new Box(), new PlainTween { Duration = 1 });
        var group = reversed ? Group.Of(late, early) : Group.Of(early, late);
        group.End.ContinueWith(_ => scheduler.Add(next, new PlainTween { To = 1, Duration = 1 }),
            TaskContinuationOptions.ExecuteSynchronously);
        scheduler.Update(0.75);
        scheduler.Update(0.5);
        scheduler.Update(0.25);
        Assert.Equal(0.5f, next.Value);
    }

    [Fact]
    public void GroupsSpanningLanesOrSchedulersCarryNothing()
    {
        using var scheduler = new TweenScheduler();
        using var other = new TweenScheduler();
        var afterLanes = new Box();
        var afterTimeBases = new Box();
        var afterSchedulers = new Box();
        var lanes = Group.Of(
            scheduler.Add(new Box(), new PlainTween { Duration = 1 }),
            scheduler.Add(new Box(), new PlainTween { Duration = 1, ProcessMode = TweenProcessMode.Physics }));
        var timeBases = Group.Of(
            scheduler.Add(new Box(), new PlainTween { Duration = 1 }),
            scheduler.Add(new Box(), new PlainTween { Duration = 1, UseUnscaledTime = true }));
        var schedulers = Group.Of(
            scheduler.Add(new Box(), new PlainTween { Duration = 1 }),
            other.Add(new Box(), new PlainTween()));
        Continue(lanes, afterLanes);
        Continue(timeBases, afterTimeBases);
        Continue(schedulers, afterSchedulers);
        other.Update(0);
        scheduler.Update(1.5);
        scheduler.Update(1.5, mode: TweenProcessMode.Physics);
        scheduler.Update(0.25);
        Assert.Equal(0.25f, afterLanes.Value);
        Assert.Equal(0.25f, afterTimeBases.Value);
        Assert.Equal(0.25f, afterSchedulers.Value);

        void Continue(Group group, Box box) => group.End.ContinueWith(
            _ => scheduler.Add(box, new PlainTween { To = 1, Duration = 1 }), TaskContinuationOptions.ExecuteSynchronously);
    }
}
