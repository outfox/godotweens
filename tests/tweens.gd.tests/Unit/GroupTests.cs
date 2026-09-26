// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using tweens.gd.Tests.Support;

namespace tweens.gd.Tests.Unit;

public class GroupTests
{
    private static TweenInstance Start(TweenScheduler scheduler, double duration, Action<PlainTween>? configure = null)
    {
        var definition = new PlainTween { Duration = duration };
        configure?.Invoke(definition);
        return scheduler.Add(new Box(), definition);
    }

    [Fact]
    public void OfValidatesItsMembers()
    {
        using var scheduler = new TweenScheduler();
        Assert.Throws<ArgumentNullException>(() => Group.Of(null!));
        Assert.Throws<ArgumentException>(() => Group.Of());
        Assert.Throws<ArgumentNullException>(() => Group.Of(Start(scheduler, 1), null!));
    }

    [Fact]
    public async Task CompletesWhenEveryMemberCompleted()
    {
        using var scheduler = new TweenScheduler();
        var members = new[] { Start(scheduler, 1), Start(scheduler, 2) };
        var group = Group.Of(members);
        Assert.Equal(members, group.Members);
        var end = group.End;
        Assert.Same(end, group.End);
        scheduler.Update(1);
        Assert.False(group.IsTerminal);
        scheduler.Update(1);
        Assert.True(group.IsTerminal);
        Assert.Equal(Reason.Completed, group.CompletionReason);
        Assert.Null(group.Error);
        Assert.Equal(Reason.Completed, await end);
    }

    [Fact]
    public async Task EndRequestedAfterSettlementIsAlreadyComplete()
    {
        using var scheduler = new TweenScheduler();
        var group = Group.Of(Start(scheduler, 0));
        scheduler.Update(0);
        Assert.True(group.End.IsCompletedSuccessfully);
        Assert.Equal(Reason.Completed, await group.End);
    }

    [Fact]
    public async Task OneStoppedMemberCancelsTheOthers()
    {
        using var scheduler = new TweenScheduler();
        var a = Start(scheduler, 1);
        var b = Start(scheduler, 1);
        var c = Start(scheduler, 1);
        var group = Group.Of(a, b, c);
        b.Cancel();
        Assert.All(new[] { a, b, c }, t => Assert.Equal(Reason.Cancelled, t.CompletionReason));
        Assert.Equal(Reason.Cancelled, await group.End);
    }

    [Fact]
    public void AlreadySettledMembersAreAccountedAtCreation()
    {
        using var scheduler = new TweenScheduler();
        var done = Start(scheduler, 0);
        scheduler.Update(0);
        var running = Start(scheduler, 1);
        var group = Group.Of(done, running);
        Assert.False(group.IsTerminal);
        scheduler.Update(1);
        Assert.Equal(Reason.Completed, group.CompletionReason);

        var cancelled = Start(scheduler, 1);
        cancelled.Cancel();
        var sibling = Start(scheduler, 1);
        var stopped = Group.Of(cancelled, sibling);
        Assert.True(stopped.IsTerminal);
        Assert.Equal(Reason.Cancelled, sibling.CompletionReason);
    }

    [Fact]
    public async Task MemberErrorsFaultTheGroup()
    {
        using var scheduler = new TweenScheduler();
        var single = Group.Of(Start(scheduler, 1, d => d.OnStart = _ => throw new FormatException()), Start(scheduler, 1));
        var several = Group.Of(
            Start(scheduler, 1, d => d.OnStart = _ => throw new FormatException()),
            Start(scheduler, 1, d => d.OnCancel = _ => throw new InvalidCastException()));
        scheduler.Update(0.5);
        Assert.IsType<FormatException>(single.Error);
        Assert.Equal(Reason.Cancelled, single.CompletionReason);
        await Assert.ThrowsAsync<FormatException>(() => single.End);
        var aggregate = Assert.IsType<AggregateException>(several.Error);
        Assert.Equal(2, aggregate.InnerExceptions.Count);
        await Assert.ThrowsAsync<AggregateException>(() => several.End);
    }

    [Fact]
    public void PauseResumeAndCancelApplyToEveryMember()
    {
        using var scheduler = new TweenScheduler();
        var a = Start(scheduler, 1);
        var b = Start(scheduler, 2);
        var group = Group.Of(a, b);
        Assert.False(group.IsPaused);
        a.Pause();
        Assert.False(group.IsPaused);
        group.Pause();
        Assert.True(group.IsPaused);
        group.Resume();
        Assert.False(a.IsPaused || b.IsPaused);
        scheduler.Update(1);
        b.Pause();
        Assert.True(group.IsPaused);
        group.Cancel();
        Assert.Equal(Reason.Completed, a.CompletionReason);
        Assert.Equal(Reason.Cancelled, b.CompletionReason);
        Assert.False(group.IsPaused);
        Assert.Equal(Reason.Cancelled, group.CompletionReason);
    }
}
