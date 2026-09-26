// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using tweens.gd;

namespace testbed.Tests.Core;

public class SequenceTimingTests
{
    private sealed class Box { public float Value { get; set; } }

    private sealed class BoxTween : TweenDefinition<Box, float>
    {
        protected override float Read(Box target) => target.Value;
        protected override void Write(Box target, float value) => target.Value = value;
        protected override float Interpolate(float from, float to, float weight) => from + (to - from) * weight;
    }

    /// <summary>Runs posted continuations when pumped.</summary>
    private sealed class QueueContext : SynchronizationContext
    {
        private readonly Queue<(SendOrPostCallback Callback, object? State)> queue = new();
        public override void Post(SendOrPostCallback d, object? state) => queue.Enqueue((d, state));
        public void Pump()
        {
            while (queue.TryDequeue(out var item)) item.Callback(item.State);
        }
    }

    /// <summary>Drives a sequence with inline continuations, or with posted ones pumped after every update.</summary>
    private sealed class Harness : IDisposable
    {
        private readonly SynchronizationContext? previous = SynchronizationContext.Current;
        private readonly QueueContext? context;
        public readonly TweenScheduler Scheduler = new();

        public Harness(bool posted)
        {
            context = posted ? new QueueContext() : null;
            SynchronizationContext.SetSynchronizationContext(context);
        }

        public void Update(double delta, double? unscaled = null, TweenProcessMode mode = TweenProcessMode.Process)
        {
            // A context differing from the captured one makes continuations post instead of running inline.
            SynchronizationContext.SetSynchronizationContext(null);
            try { Scheduler.Update(delta, unscaled, mode); }
            finally { SynchronizationContext.SetSynchronizationContext(context); }
            context?.Pump();
        }

        public void Dispose()
        {
            Scheduler.Dispose();
            SynchronizationContext.SetSynchronizationContext(previous);
        }
    }

    private static BoxTween Leg(double duration, float to = 10) => new() { From = 0, To = to, Duration = duration };

    // Godot resumes awaits inline; a continuation posted for later starts fresh.
    [Theory, InlineData(false, 5), InlineData(true, 3)]
    public void AwaitedCompletionHandsItsOvershootToTheNextTween(bool posted, float expected)
    {
        using var run = new Harness(posted);
        Box first = new(), second = new();
        var sequence = SequenceAsync(run.Scheduler, first, second);
        run.Update(0.6);
        run.Update(0.6);
        Assert.Equal(10, first.Value);
        run.Update(0.3);
        Assert.Equal(expected, second.Value, 3);
        run.Update(1);
        Assert.True(sequence.IsCompletedSuccessfully);

        static async Task SequenceAsync(TweenScheduler scheduler, Box start, Box next)
        {
            if (await scheduler.Add(start, Leg(1)).End != Reason.Completed) return;
            await scheduler.Add(next, Leg(1)).End;
        }
    }

    [Fact]
    public void GroupCreditsTheMemberThatFinishedLast()
    {
        using var run = new Harness(posted: false);
        Box early = new(), late = new(), next = new();
        var sequence = SequenceAsync(run.Scheduler, early, late, next);
        // The early member overshoots by 0.01 one update before the late one overshoots by 0.05.
        run.Update(0.6);
        run.Update(0.6);
        run.Update(0.3);
        Assert.Equal(3.5f, next.Value, 3);
        Assert.True(sequence.IsCompletedSuccessfully);

        static async Task SequenceAsync(TweenScheduler scheduler, Box earlyTarget, Box lateTarget, Box nextTarget)
        {
            if (await Group.Of(scheduler.Add(lateTarget, Leg(1.15)), scheduler.Add(earlyTarget, Leg(0.59))).End != Reason.Completed) return;
            scheduler.Add(nextTarget, Leg(1));
        }
    }

    [Fact]
    public void CarryExpiresOnceItsLaneUpdatesAgain()
    {
        using var run = new Harness(posted: false);
        Box first = new(), second = new();
        var gate = new TaskCompletionSource();
        var sequence = SequenceAsync(run.Scheduler, first, second, gate.Task);
        run.Update(0.6);
        run.Update(0.6);
        run.Update(0.1);
        gate.SetResult();
        run.Update(0.3);
        Assert.Equal(3, second.Value, 3);
        Assert.True(sequence.IsCompletedSuccessfully);

        static async Task SequenceAsync(TweenScheduler scheduler, Box start, Box next, Task continuationGate)
        {
            if (await scheduler.Add(start, Leg(1)).End != Reason.Completed) return;
            await continuationGate;
            scheduler.Add(next, Leg(1));
        }
    }

    [Fact]
    public void CarryRequiresTheSameLaneAndTimeBase()
    {
        using var run = new Harness(posted: false);
        Box first = new(), physics = new(), unscaled = new();
        var sequence = SequenceAsync(run.Scheduler, first, physics, unscaled);
        run.Update(0.6);
        run.Update(0.6);
        run.Update(0.3, mode: TweenProcessMode.Physics);
        run.Update(0.3, 0.3);
        Assert.Equal(3, physics.Value, 3);
        Assert.Equal(3, unscaled.Value, 3);
        Assert.True(sequence.IsCompletedSuccessfully);

        static async Task SequenceAsync(TweenScheduler scheduler, Box start, Box physicsTarget, Box unscaledTarget)
        {
            if (await scheduler.Add(start, Leg(1)).End != Reason.Completed) return;
            var toPhysics = Leg(1);
            toPhysics.ProcessMode = TweenProcessMode.Physics;
            scheduler.Add(physicsTarget, toPhysics);
            var toUnscaled = Leg(1);
            toUnscaled.UseUnscaledTime = true;
            scheduler.Add(unscaledTarget, toUnscaled);
        }
    }

    [Theory, InlineData(false), InlineData(true)]
    public void OnEndContinuesTheTimelineButUnrelatedTweensDoNot(bool posted)
    {
        using var run = new Harness(posted);
        Box first = new(), chained = new(), unrelated = new();
        var leg = Leg(1);
        leg.OnEnd = tween => tween.Scheduler.Add(chained, Leg(1));
        run.Scheduler.Add(first, leg);
        var sequence = SequenceAsync(run.Scheduler);
        run.Update(0.6);
        run.Update(0.6);
        run.Scheduler.Add(unrelated, Leg(1));
        run.Update(0.3);
        Assert.Equal(5, chained.Value, 3);
        Assert.Equal(3, unrelated.Value, 3);
        Assert.True(sequence.IsCompletedSuccessfully);

        // A carry handed to an awaiting method ends when the continuation yields.
        static async Task SequenceAsync(TweenScheduler scheduler) => await scheduler.Add(new Box(), Leg(1)).End;
    }

    [Fact]
    public void CreditConsumesTheNextTweensDelayFirst()
    {
        using var run = new Harness(posted: false);
        Box first = new(), second = new();
        var sequence = SequenceAsync(run.Scheduler, first, second);
        run.Update(0.6);
        run.Update(0.6);
        run.Update(0.3);
        Assert.Equal(4, second.Value, 3);
        Assert.True(sequence.IsCompletedSuccessfully);

        static async Task SequenceAsync(TweenScheduler scheduler, Box start, Box next)
        {
            if (await scheduler.Add(start, Leg(1)).End != Reason.Completed) return;
            var delayed = Leg(1);
            delayed.Delay = 0.1;
            scheduler.Add(next, delayed);
        }
    }

    [Fact]
    public async Task GroupCompletesWhenEveryMemberCompletes()
    {
        using var scheduler = new TweenScheduler();
        var group = Group.Of(scheduler.Add(new Box(), Leg(1)), scheduler.Add(new Box(), Leg(2)));
        scheduler.Update(1);
        Assert.False(group.IsTerminal);
        scheduler.Update(1);
        Assert.True(group.IsTerminal);
        Assert.Equal(Reason.Completed, await group.End);
        Assert.Equal(Reason.Completed, group.CompletionReason);
    }

    [Fact]
    public async Task StoppedMemberCancelsItsSiblingsAndReportsItsReason()
    {
        using var scheduler = new TweenScheduler();
        var stopped = scheduler.Add(new Box(), Leg(1));
        var sibling = scheduler.Add(new Box(), Leg(1));
        var group = Group.Of(stopped, sibling);
        // Disposing early is the behavior under test; using also cleans up if setup fails.
        // ReSharper disable once DisposeOnUsingVariable
        scheduler.Dispose();
        Assert.Equal(Reason.RunnerDisposed, await group.End);
        Assert.True(sibling.IsTerminal);

        using var other = new TweenScheduler();
        var cancelled = other.Add(new Box(), Leg(1));
        var running = other.Add(new Box(), Leg(1));
        var second = Group.Of(cancelled, running);
        cancelled.Cancel();
        Assert.Equal(TweenState.Cancelled, running.State);
        Assert.Equal(Reason.Cancelled, await second.End);
    }

    [Fact]
    public async Task FaultedMemberFaultsTheGroupAndCancelsItsSiblings()
    {
        using var scheduler = new TweenScheduler();
        var failing = Leg(1);
        failing.OnUpdate = (_, _) => throw new InvalidOperationException("boom");
        var sibling = scheduler.Add(new Box(), Leg(1));
        var group = Group.Of(scheduler.Add(new Box(), failing), sibling);
        scheduler.Update(0.1);
        Assert.Equal(TweenState.Cancelled, sibling.State);
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => group.End);
        Assert.Equal("boom", error.Message);
        Assert.Same(error, group.Error);
    }

    [Fact]
    public async Task GroupPausesResumesAndCancelsItsMembers()
    {
        using var scheduler = new TweenScheduler();
        var box = new Box();
        var group = Group.Of(scheduler.Add(box, Leg(1)), scheduler.Add(new Box(), Leg(1)));
        group.Pause();
        Assert.True(group.IsPaused);
        scheduler.Update(0.5);
        Assert.Equal(0, box.Value);
        group.Resume();
        scheduler.Update(0.5);
        Assert.Equal(5, box.Value, 3);
        group.Cancel();
        Assert.Equal(Reason.Cancelled, await group.End);
        Assert.All(group.Members, member => Assert.Equal(TweenState.Cancelled, member.State));
    }

    [Fact]
    public void GroupRejectsEmptyAndNullMembers()
    {
        Assert.Throws<ArgumentException>(() => Group.Of());
        Assert.Throws<ArgumentNullException>(() => Group.Of(null!));
        using var scheduler = new TweenScheduler();
        Assert.Throws<ArgumentNullException>(() => Group.Of(scheduler.Add(new Box(), Leg(1)), null!));
    }
}
