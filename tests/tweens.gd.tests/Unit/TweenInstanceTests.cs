// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using tweens.gd.Tests.Support;

namespace tweens.gd.Tests.Unit;

public class TweenInstanceTests
{
    [Theory]
    [InlineData(1, 0.25f)]
    [InlineData(2, 0.0625f)]
    [InlineData(0.5, 0.5f)]
    public void SkewWarpsTimeBeforeEasingAndRetracesItDuringPingPong(double skew, float quarterTime)
    {
        using var scheduler = new TweenScheduler();
        var linear = new Box();
        var eased = new Box();
        var custom = new Box();
        var definition = new PlainTween { To = 1, Duration = 1, Skew = skew, UsePingPong = true };
        var tween = scheduler.Add(linear, definition);
        definition.Ease = EaseType.QuadOut;
        scheduler.Add(eased, definition);
        definition.EaseFunction = static t => t + 1;
        scheduler.Add(custom, definition);
        definition.Skew = 3; // Active playbacks keep their captured exponent.

        scheduler.Update(0);
        Assert.Equal(0, linear.Value);
        Assert.Equal(0, eased.Value);
        Assert.Equal(1, custom.Value);
        scheduler.Update(0.25);
        Assert.Equal(0.25f, tween.Progress);
        Assert.Equal(quarterTime, linear.Value);
        Assert.Equal(1 - (1 - quarterTime) * (1 - quarterTime), eased.Value);
        Assert.Equal(1 + quarterTime, custom.Value); // Easing output remains unclamped.
        scheduler.Update(0.75);
        Assert.Equal(1, linear.Value);
        Assert.Equal(1, eased.Value);
        Assert.Equal(2, custom.Value);
        scheduler.Update(0.75);
        Assert.Equal(0.25f, tween.Progress);
        Assert.Equal(quarterTime, linear.Value);
        Assert.Equal(1 - (1 - quarterTime) * (1 - quarterTime), eased.Value);
        Assert.Equal(1 + quarterTime, custom.Value);
        scheduler.Update(0.25);
        Assert.Equal(0, linear.Value);
        Assert.Equal(0, eased.Value);
        Assert.Equal(1, custom.Value);
        Assert.Equal(Reason.Completed, tween.CompletionReason);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void InvalidSkewIsRejectedBeforeReadingOrWritingTheTarget(double skew)
    {
        using var scheduler = new TweenScheduler();
        var box = new Box { Value = 3 };
        var error = Assert.Throws<ArgumentOutOfRangeException>(() => scheduler.Add(box,
            new ProbeTween { Skew = skew, Reader = _ => throw new Exception("Must not read") }));
        Assert.Equal("Skew", error.ParamName);
        Assert.Equal(3, box.Value);
    }

    [Theory]
    [InlineData(false, 1f)]
    [InlineData(true, 0f)]
    public void SkewPreservesZeroDurationEndpoints(bool pingPong, float expected)
    {
        using var scheduler = new TweenScheduler();
        var box = new Box();
        var tween = scheduler.Add(box, new PlainTween { To = 1, Skew = 0.5, UsePingPong = pingPong });
        scheduler.Update(0);
        Assert.Equal(expected, box.Value);
        Assert.Equal(Reason.Completed, tween.CompletionReason);
    }

    [Fact]
    public async Task CallbacksRunInOrderWithTheirValues()
    {
        using var scheduler = new TweenScheduler();
        var box = new Box { Value = 2 };
        var events = new List<string>();
        var tween = scheduler.Add(box, new PlainTween
        {
            To = 6, Duration = 1,
            OnAdd = t => events.Add($"add {t.State}"),
            OnStart = t => events.Add($"start {t.State}"),
            OnUpdate = (t, value) => events.Add($"update {value} {t.Value}"),
            OnEnd = t => events.Add($"end {t.State} {t.CompletionReason}"),
            OnFinally = t => events.Add($"finally {t.IsTerminal}"),
        });
        Assert.Same(box, tween.Target);
        Assert.Equal(2, tween.Value);
        scheduler.Update(0.5);
        Assert.Equal(0.5f, tween.Progress);
        scheduler.Update(0.5);
        Assert.Equal(Reason.Completed, await tween.End);
        Assert.Equal(["add Delayed", "start Playing", "update 4 4", "update 6 6", "end Completed Completed", "finally True"], events);
        Assert.Null(tween.Error);
    }

    [Fact]
    public void OmittedEndpointsUseTheValueCapturedAtAddition()
    {
        using var scheduler = new TweenScheduler();
        var toward = new Box { Value = 4 };
        var back = new Box { Value = 4 };
        scheduler.Add(toward, new PlainTween { From = 0, Duration = 1 });
        scheduler.Add(back, new PlainTween { To = 0, Duration = 1 });
        scheduler.Update(0.25);
        Assert.Equal(1, toward.Value);
        Assert.Equal(3, back.Value);
    }

    [Fact]
    public void DefinitionsAreSnapshotsAtAddition()
    {
        using var scheduler = new TweenScheduler();
        var box = new Box();
        var definition = new PlainTween { To = 10, Duration = 1 };
        scheduler.Add(box, definition);
        definition.To = -10;
        definition.Duration = 100;
        scheduler.Update(0.5);
        Assert.Equal(5, box.Value);
    }

    [Fact]
    public void EaseTypeAndEaseFunctionShapeTheWeight()
    {
        using var scheduler = new TweenScheduler();
        var eased = new Box(); var custom = new Box();
        scheduler.Add(eased, new PlainTween { To = 1, Duration = 1, Ease = EaseType.QuadIn });
        scheduler.Add(custom, new PlainTween { To = 1, Duration = 1, EaseFunction = static x => 1 - x });
        scheduler.Update(0.5);
        Assert.Equal(0.25f, eased.Value);
        Assert.Equal(0.5f, custom.Value);
        scheduler.Update(0.25);
        Assert.Equal(0.25f, custom.Value);
    }

    [Theory]
    [InlineData(EaseType.SmoothStep, 0.15625f)]
    [InlineData(EaseType.SmootherStep, 0.103515625f)]
    public void SmoothStepsShapeForwardAndPingPongPlayback(EaseType ease, float quarterWeight)
    {
        using var scheduler = new TweenScheduler();
        var box = new Box();
        var tween = scheduler.Add(box, new PlainTween { To = 1, Duration = 1, Ease = ease, UsePingPong = true });
        scheduler.Update(0.25);
        Assert.Equal(quarterWeight, box.Value);
        scheduler.Update(0.75);
        Assert.Equal(1, box.Value);
        scheduler.Update(0.25);
        Assert.Equal(1 - quarterWeight, box.Value);
        scheduler.Update(0.75);
        Assert.Equal(0, box.Value);
        Assert.Equal(Reason.Completed, tween.CompletionReason);
    }

    [Fact]
    public void NonFiniteEasingFaultsTheTween()
    {
        using var scheduler = new TweenScheduler();
        var box = new Box { Value = 3 };
        var tween = scheduler.Add(box, new PlainTween { To = 1, Duration = 1, EaseFunction = static _ => float.NaN });
        scheduler.Update(0.5);
        Assert.Equal(TweenState.Faulted, tween.State);
        Assert.Equal(Reason.Cancelled, tween.CompletionReason);
        Assert.IsType<InvalidOperationException>(tween.Error);
        Assert.Equal(3, box.Value);
    }

    [Fact]
    public void DelayedApplyFromWritesAtAdditionOnlyWhenRequested()
    {
        using var scheduler = new TweenScheduler();
        var applied = new Box { Value = 5 };
        var retained = new Box { Value = 5 };
        var immediate = new Box { Value = 5 };
        scheduler.Add(applied, new PlainTween { From = 1, To = 2, Delay = 1, Duration = 1, Fill = FillMode.ApplyFromDuringDelay });
        scheduler.Add(retained, new PlainTween { From = 1, To = 2, Delay = 1, Duration = 1 });
        scheduler.Add(immediate, new PlainTween { From = 1, To = 2, Duration = 1, Fill = FillMode.Both });
        Assert.Equal(1, applied.Value);
        Assert.Equal(5, retained.Value);
        Assert.Equal(5, immediate.Value);
        scheduler.Update(0.5);
        Assert.Equal(TweenState.Delayed, scheduler.Add(new Box(), new PlainTween { Delay = 1 }).State);
    }

    [Fact]
    public void NonRetainingCompletionRestoresTheCapturedValue()
    {
        using var scheduler = new TweenScheduler();
        var box = new Box { Value = 3 };
        var updates = new List<float>();
        scheduler.Add(box, new PlainTween { To = 9, Duration = 1, Fill = FillMode.None, OnUpdate = (_, v) => updates.Add(v) });
        scheduler.Update(1);
        Assert.Equal(3, box.Value);
        Assert.Equal([9f, 3f], updates);
    }

    [Fact]
    public void CustomRestoreReplacesTheDefaultWrite()
    {
        using var scheduler = new TweenScheduler();
        var box = new Box { Value = 3 };
        var restored = float.NaN;
        scheduler.Add(box, new ProbeTween { To = 9, Duration = 1, Fill = FillMode.None, Restoring = (_, initial) => restored = initial });
        scheduler.Update(1);
        Assert.Equal(3, restored);
        Assert.Equal(9, box.Value);
    }

    [Fact]
    public void CancellingFromEachStageStopsBeforeTheNextStage()
    {
        using var scheduler = new TweenScheduler();
        TweenInstance? current = null;
        var box = new Box { Value = 1 };
        var updates = 0;

        // From the easing function: nothing is written.
        current = scheduler.Add(box, new PlainTween { To = 5, Duration = 1, EaseFunction = x => { current!.Cancel(); return x; } });
        scheduler.Update(0.5);
        Assert.Equal(1, box.Value);

        // From interpolation: nothing is written.
        current = scheduler.Add(box, new ProbeTween { To = 5, Duration = 1, Interpolator = (a, _, _) => { current!.Cancel(); return a; } });
        scheduler.Update(0.5);
        Assert.Equal(1, box.Value);

        // From the write: the value is written, but not reported.
        current = scheduler.Add(box, new ProbeTween
        {
            To = 5, Duration = 1, OnUpdate = (_, _) => updates++,
            Writer = (b, v) => { b.Value = v; current!.Cancel(); },
        });
        scheduler.Update(0.5);
        Assert.Equal(3, box.Value);
        Assert.Equal(0, updates);
    }

    [Fact]
    public void CancellingAroundRestorationPreventsCompletion()
    {
        using var scheduler = new TweenScheduler();
        var reasons = new List<Reason?>();
        TweenInstance? current = null;

        // The final update cancels before restoration.
        current = scheduler.Add(new Box(), new PlainTween
        {
            To = 5, Duration = 1, Fill = FillMode.None,
            OnUpdate = (t, v) => { if (v == 5) t.Cancel(); },
        });
        scheduler.Update(1);
        reasons.Add(current.CompletionReason);

        // Restoration itself cancels, so the restored value is not reported.
        var restoredReports = 0;
        current = scheduler.Add(new Box(), new ProbeTween
        {
            To = 5, Duration = 1, Fill = FillMode.None,
            Restoring = (b, v) => { b.Value = v; current!.Cancel(); },
            OnUpdate = (_, v) => { if (v == 0) restoredReports++; },
        });
        scheduler.Update(1);
        reasons.Add(current.CompletionReason);
        Assert.Equal(0, restoredReports);

        // The report of the restored value cancels.
        current = scheduler.Add(new Box(), new PlainTween
        {
            To = 5, Duration = 1, Fill = FillMode.None,
            OnUpdate = (t, v) => { if (v == 0) t.Cancel(); },
        });
        scheduler.Update(1);
        reasons.Add(current.CompletionReason);
        Assert.All(reasons, reason => Assert.Equal(Reason.Cancelled, reason));
    }

    [Fact]
    public void OnStartCancellationSkipsTheFirstWrite()
    {
        using var scheduler = new TweenScheduler();
        var box = new Box { Value = 1 };
        scheduler.Add(box, new PlainTween { To = 5, Duration = 1, OnStart = t => t.Cancel() });
        scheduler.Update(0.5);
        Assert.Equal(1, box.Value);
    }

    [Fact]
    public void OnAddFailuresAndCancellationsSettleImmediately()
    {
        using var scheduler = new TweenScheduler();
        var finals = 0;
        var box = new Box { Value = 1 };
        var failed = scheduler.Add(box, new PlainTween { OnAdd = _ => throw new FormatException(), OnFinally = _ => finals++ });
        Assert.Equal(TweenState.Faulted, failed.State);
        Assert.IsType<FormatException>(failed.Error);
        var cancelled = scheduler.Add(box, new PlainTween
        {
            From = 7, Delay = 1, Fill = FillMode.Both, OnAdd = t => t.Cancel(), OnFinally = _ => finals++,
        });
        Assert.Equal(TweenState.Cancelled, cancelled.State);
        Assert.Equal(1, box.Value);
        Assert.Equal(2, finals);
    }

    [Fact]
    public void PreparationFailuresReleaseTheSnapshot()
    {
        using var scheduler = new TweenScheduler();
        var released = 0;
        Assert.Throws<FormatException>(() => scheduler.Add(new Box(), new ProbeTween
        {
            Preparing = _ => throw new FormatException(), Releasing = () => released++,
        }));
        Assert.Equal(1, released);
        var both = Assert.Throws<AggregateException>(() => scheduler.Add(new Box(), new ProbeTween
        {
            Preparing = _ => throw new FormatException(), Releasing = () => throw new InvalidCastException(),
        }));
        Assert.IsType<FormatException>(both.InnerExceptions[0]);
        Assert.IsType<InvalidCastException>(both.InnerExceptions[1]);
        Assert.Throws<FormatException>(() => scheduler.Add(new Box(), new ProbeTween { Reader = _ => throw new FormatException() }));
        Assert.Equal(0, scheduler.ActiveCount);
    }

    [Fact]
    public async Task TerminalCallbacksFollowTheOutcome()
    {
        using var scheduler = new TweenScheduler();
        var events = new List<string>();
        PlainTween Recording(string name) => new()
        {
            Duration = 1,
            OnEnd = _ => events.Add(name + " end"),
            OnCancel = _ => events.Add(name + " cancel"),
            OnFinally = _ => events.Add(name + " finally"),
        };
        var completed = scheduler.Add(new Box(), Recording("a"));
        var cancelled = scheduler.Add(new Box(), Recording("b"));
        var faultingDefinition = Recording("c");
        faultingDefinition.OnStart = _ => throw new FormatException();
        var faulted = scheduler.Add(new Box(), faultingDefinition);
        cancelled.Cancel();
        scheduler.Update(1);
        Assert.Equal(["b cancel", "b finally", "a end", "a finally", "c finally"], events);
        Assert.Equal(Reason.Completed, await completed.End);
        Assert.Equal(Reason.Cancelled, await cancelled.End);
        await Assert.ThrowsAsync<FormatException>(() => faulted.End);
    }

    [Fact]
    public async Task ThrowingTerminalCallbacksFaultTheTween()
    {
        using var scheduler = new TweenScheduler();
        var end = scheduler.Add(new Box(), new PlainTween { OnEnd = _ => throw new FormatException() });
        var cancel = scheduler.Add(new Box(), new PlainTween { Duration = 1, OnCancel = _ => throw new FormatException() });
        var final = scheduler.Add(new Box(), new PlainTween { OnFinally = _ => throw new FormatException() });
        var both = scheduler.Add(new Box(), new PlainTween
        {
            OnEnd = _ => throw new FormatException(), OnFinally = _ => throw new InvalidCastException(),
        });
        cancel.Cancel();
        scheduler.Update(0);
        foreach (var tween in new TweenInstance[] { end, cancel, final })
        {
            Assert.Equal(TweenState.Faulted, tween.State);
            await Assert.ThrowsAsync<FormatException>(() => tween.End);
        }
        var error = Assert.IsType<AggregateException>(both.Error);
        Assert.Collection(error.InnerExceptions, e => Assert.IsType<FormatException>(e), e => Assert.IsType<InvalidCastException>(e));
    }

    [Fact]
    public void ReleaseFailuresFaultTheTweenAndCombineWithEarlierErrors()
    {
        using var scheduler = new TweenScheduler();
        var reports = new List<Exception>();
        scheduler.UnhandledException += reports.Add;
        var release = scheduler.Add(new Box(), new ProbeTween { Releasing = () => throw new FormatException() });
        var both = scheduler.Add(new Box(), new ProbeTween
        {
            OnEnd = _ => throw new InvalidCastException(), Releasing = () => throw new FormatException(),
        });
        scheduler.Update(0);
        Assert.Equal(TweenState.Faulted, release.State);
        Assert.Equal(Reason.Completed, release.CompletionReason);
        Assert.IsType<FormatException>(release.Error);
        Assert.IsType<AggregateException>(both.Error);
        Assert.Equal(2, reports.Count);
    }

    [Fact]
    public async Task ThrowingAfterSelfCancellationStillFaults()
    {
        using var scheduler = new TweenScheduler();
        var finals = 0;
        var tween = scheduler.Add(new Box(), new PlainTween
        {
            Duration = 1,
            OnUpdate = (t, _) => { t.Cancel(); throw new FormatException(); },
            OnFinally = _ => finals++,
        });
        var end = tween.End;
        scheduler.Update(0.5);
        await Assert.ThrowsAsync<FormatException>(() => end);
        Assert.Equal(TweenState.Faulted, tween.State);
        Assert.Equal(Reason.Cancelled, tween.CompletionReason);
        Assert.Equal(1, finals);
    }

    [Fact]
    public void FaultsCombineWithThrowingFinallyCallbacks()
    {
        using var scheduler = new TweenScheduler();
        var tween = scheduler.Add(new Box(), new PlainTween
        {
            Duration = 1,
            OnStart = _ => throw new FormatException(),
            OnFinally = _ => throw new InvalidCastException(),
        });
        scheduler.Update(0.5);
        var error = Assert.IsType<AggregateException>(tween.Error);
        Assert.Collection(error.InnerExceptions, e => Assert.IsType<FormatException>(e), e => Assert.IsType<InvalidCastException>(e));
    }

    [Fact]
    public void FinishedTweensCannotAdvance()
    {
        using var scheduler = new TweenScheduler();
        var tween = scheduler.Add(new Box(), new PlainTween { Duration = 1 });
        Assert.True(tween.CanAdvance());
        tween.Cancel();
        Assert.False(tween.CanAdvance());
        Assert.False(tween.CheckTarget());
    }

    [Fact]
    public void ErrorsAfterSelfCancellationCombine()
    {
        using var scheduler = new TweenScheduler();
        var tween = scheduler.Add(new Box(), new PlainTween
        {
            Duration = 1,
            OnCancel = _ => throw new FormatException("cancel"),
            OnUpdate = (t, _) => { t.Cancel(); throw new InvalidCastException("update"); },
        });
        scheduler.Update(0.5);
        var error = Assert.IsType<AggregateException>(tween.Error);
        Assert.Collection(error.InnerExceptions, e => Assert.IsType<FormatException>(e), e => Assert.IsType<InvalidCastException>(e));
        Assert.Equal(TweenState.Faulted, tween.State);
    }

    [Fact]
    public void SuppressedCallbacksStillRunWhileTheTargetIsValid()
    {
        using var scheduler = new TweenScheduler();
        var cancelled = false;
        var tween = scheduler.Add(new Box(), new PlainTween
        {
            Duration = 1, SuppressCallbacksWhenTargetInvalid = true, OnCancel = _ => cancelled = true,
        });
        tween.Cancel();
        Assert.True(cancelled);
    }

    [Fact]
    public async Task EndIsSharedAndAvailableAfterSettlement()
    {
        using var scheduler = new TweenScheduler();
        var late = scheduler.Add(new Box(), new PlainTween());
        scheduler.Update(0);
        Assert.True(late.End.IsCompletedSuccessfully);
        Assert.Same(late.End, late.End);
        Assert.Equal(Reason.Completed, await late.AwaitDecommissionAsync(CancellationToken.None));

        var early = scheduler.Add(new Box(), new PlainTween { Duration = 1 });
        var waiting = early.AwaitDecommissionAsync(CancellationToken.None);
        Assert.Same(early.End, waiting);
        var cancellable = early.AwaitDecommissionAsync(TestContext.Current.CancellationToken);
        Assert.NotSame(early.End, cancellable);
        scheduler.Update(1);
        Assert.True(cancellable.IsCompletedSuccessfully);
        Assert.Equal(Reason.Completed, await cancellable);
    }

    [Fact]
    public void SettledObserversAreIsolatedFromEachOther()
    {
        using var scheduler = new TweenScheduler();
        var reports = new List<Exception>();
        scheduler.UnhandledException += reports.Add;
        var tween = scheduler.Add(new Box(), new PlainTween { Duration = 1 });
        tween.Settled += _ => throw new FormatException();
        Assert.False(tween.IsSettled);
        tween.Cancel();
        Assert.True(tween.IsSettled);
        Assert.IsType<FormatException>(Assert.Single(reports));
        Assert.True(tween.End.IsCompletedSuccessfully);
    }

    [Fact]
    public void PausedTweensHoldTheirTimeline()
    {
        using var scheduler = new TweenScheduler();
        var box = new Box();
        var tween = scheduler.Add(box, new PlainTween { To = 1, Duration = 1, PauseMode = TweenPauseMode.Always });
        tween.IsPaused = true;
        Assert.True(tween.IsPaused);
        scheduler.Update(0.5);
        Assert.Equal(0, box.Value);
        tween.Resume();
        Assert.False(tween.IsPaused);
        scheduler.Update(0.5);
        Assert.Equal(0.5f, box.Value);
    }

    [Fact]
    public void StatesFollowTheTimeline()
    {
        using var scheduler = new TweenScheduler();
        var tween = scheduler.Add(new Box(), new PlainTween { Delay = 1, Duration = 1, RepeatInterval = 1, Repeats = 1 });
        Assert.Equal(TweenState.Delayed, tween.State);
        scheduler.Update(1.5);
        Assert.Equal(TweenState.Playing, tween.State);
        scheduler.Update(1);
        Assert.Equal(TweenState.Interval, tween.State);
        scheduler.Update(2);
        Assert.Equal(TweenState.Completed, tween.State);
    }

    [Fact]
    public void PropertyTweenRequiresEveryOperation()
    {
        Func<Box, float> get = static b => b.Value;
        Action<Box, float> set = static (b, v) => b.Value = v;
        Func<float, float, float, float> lerp = Interpolators.Float;
        Assert.Throws<ArgumentNullException>(() => new PropertyTween<Box, float>(null!, set, lerp));
        Assert.Throws<ArgumentNullException>(() => new PropertyTween<Box, float>(get, null!, lerp));
        Assert.Throws<ArgumentNullException>(() => new PropertyTween<Box, float>(get, set, null!));

        using var scheduler = new TweenScheduler();
        var box = new Box { Value = 2 };
        scheduler.Add(box, new PropertyTween<Box, float>(get, set, lerp) { To = 4, Duration = 1 });
        scheduler.Update(0.5);
        Assert.Equal(3, box.Value);
    }
}
