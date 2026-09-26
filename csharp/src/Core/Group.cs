// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

namespace tweens.gd;

/// <summary>Tweens that play as one step. If one stops without completing, the group cancels the others.</summary>
public sealed class Group
{
    private readonly TweenInstance[] members;
    private TaskCompletionSource<Reason>? completion;
    private int unsettled;
    private bool stopping;
    private Reason? firstStop;
    private List<Exception>? errors;

    public IReadOnlyList<TweenInstance> Members => members;
    public bool IsTerminal { get; private set; }
    public Reason? CompletionReason { get; private set; }
    public Exception? Error { get; private set; }
    public bool IsPaused
    {
        get
        {
            var active = members.Where(member => !member.IsTerminal).ToArray();
            return active.Length > 0 && active.All(member => member.IsPaused);
        }
    }

    /// <summary>Completed when every member completed; otherwise the first reason a member stopped for.</summary>
    public Task<Reason> End
    {
        get
        {
            if (completion is null)
            {
                completion = new();
                if (IsTerminal) SetCompletion();
            }
            return completion.Task;
        }
    }

    private Group(TweenInstance[] members)
    {
        this.members = members;
        unsettled = members.Length;
        // Subscribe first: settling one member can cancel, and so settle, the others.
        foreach (var member in members)
            if (!member.IsSettled) member.Settled += OnSettled;
        foreach (var member in members)
            if (member.IsSettled) OnSettled(member);
    }

    /// <summary>Groups tweens that are already playing.</summary>
    public static Group Of(params TweenInstance[] tweens)
    {
        ArgumentNullException.ThrowIfNull(tweens);
        if (tweens.Length == 0) throw new ArgumentException("A group needs at least one tween.", nameof(tweens));
        if (Array.IndexOf(tweens, null) >= 0) throw new ArgumentNullException(nameof(tweens), "A group cannot contain null.");
        return new Group([.. tweens]);
    }

    public void Pause()
    {
        foreach (var member in members) member.Pause();
    }

    public void Resume()
    {
        foreach (var member in members) member.Resume();
    }

    public void Cancel()
    {
        foreach (var member in members) member.Cancel();
    }

    private void OnSettled(TweenInstance member)
    {
        unsettled--;
        if (member.Error is not null) (errors ??= []).Add(member.Error);
        if (member.Error is not null || member.CompletionReason != Reason.Completed)
        {
            firstStop ??= member.CompletionReason;
            CancelSiblings();
        }
        if (unsettled == 0) Settle();
    }

    private void CancelSiblings()
    {
        if (stopping) return;
        stopping = true;
        foreach (var sibling in members)
            if (!sibling.IsTerminal) sibling.Cancel();
    }

    private void Settle()
    {
        IsTerminal = true;
        CompletionReason = firstStop ?? Reason.Completed;
        Error = errors switch
        {
            null => null,
            [var single] => single,
            _ => new AggregateException(errors),
        };
        // Awaiting code resumes inline here; tweens it starts continue from the member that finished last.
        using var scope = TweenCarry.Enter(Error is null && CompletionReason == Reason.Completed ? LastToFinish() : null);
        SetCompletion();
    }

    // The latest update decides; within it, the smallest overshoot finished last. Mixed lanes carry nothing.
    private Carry? LastToFinish()
    {
        Carry? last = null;
        foreach (var member in members)
        {
            if (member.Stamp is not { } stamp) return null;
            if (last is null)
            {
                last = stamp;
                continue;
            }
            if (stamp.Scheduler != last.Scheduler || stamp.Mode != last.Mode || stamp.Unscaled != last.Unscaled) return null;
            if (stamp.Tick > last.Tick || (stamp.Tick == last.Tick && stamp.Overshoot < last.Overshoot)) last = stamp;
        }
        return last;
    }

    private void SetCompletion()
    {
        if (Error is not null) completion?.TrySetException(Error);
        else completion?.TrySetResult(CompletionReason!.Value);
    }
}
