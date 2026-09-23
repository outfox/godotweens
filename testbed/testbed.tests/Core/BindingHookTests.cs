// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using godotweens;
namespace testbed.Tests;

public class BindingHookTests
{
    private sealed class State { public float Value; public int Prepared, Restored, Released; }
    private sealed class Definition(State state) : TweenDefinition<State, float>
    {
        public bool FailPrepare, FailRestore, FailRelease;
        protected override void Prepare(State target) { state.Prepared++; if (FailPrepare) throw new InvalidOperationException("prepare"); }
        protected override float Read(State target) => target.Value;
        protected override void Write(State target, float value) => target.Value = value;
        protected override float Interpolate(float from, float to, float t) => from + (to - from) * t;
        protected override void Restore(State target, float initial) { state.Restored++; if (FailRestore) throw new InvalidOperationException("restore"); target.Value = initial; }
        protected override void Release() { state.Released++; if (FailRelease) throw new InvalidOperationException("release"); }
    }
    [Fact]
    public void FailedPreparationCleansUpAndPreservesBothErrors()
    {
        var state = new State(); using var scheduler = new TweenScheduler();
        var error = Assert.Throws<AggregateException>(() => scheduler.Add(state, new Definition(state) { FailPrepare = true, FailRelease = true }));
        Assert.Equal(new[] { "prepare", "release" }, error.InnerExceptions.Select(e => e.Message));
        Assert.Equal(1, state.Released); Assert.Equal(0, scheduler.ActiveCount);
    }
    [Theory]
    [InlineData(false)] [InlineData(true)]
    public async Task RestoreAndCleanupFailuresSettleWaiters(bool restore)
    {
        var state = new State(); using var scheduler = new TweenScheduler();
        var handle = scheduler.Add(state, new Definition(state) { To = 1, Duration = 1, Fill = FillMode.None,
            FailRestore = restore, FailRelease = !restore });
        var completion = handle.Completion;
        scheduler.Update(1);
        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () => await completion);
        Assert.Equal(restore ? "restore" : "release", error.Message);
        Assert.Equal(TweenState.Faulted, handle.State); Assert.Equal(1, state.Released); Assert.Equal(1, state.Restored);
        Assert.Equal(0, scheduler.ActiveCount);
    }
    [Fact]
    public void SchedulerDisposedDuringCaptureDoesNotStrandPlayback()
    {
        var state = new State(); using var scheduler = new TweenScheduler();
        var tween = scheduler.Add(state, new PropertyTween<State, float>(s => { scheduler.Dispose(); return s.Value; },
            (s, v) => s.Value = v, (a, b, t) => a));
        Assert.Equal(TweenCompletionReason.RunnerDisposed, tween.CompletionReason);
        Assert.True(tween.Completion.IsCompletedSuccessfully);
    }
}
