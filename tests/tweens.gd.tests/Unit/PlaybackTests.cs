// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

namespace tweens.gd.Tests.Unit;

public class PlaybackTests
{
    private static Playback Create(TweenOptions options) => new(options);

    [Fact]
    public void DelayHoldsTheTimelineUntilItElapses()
    {
        var playback = Create(new TweenOptions { Duration = 1, Delay = 0.5 });
        playback.Advance(0.25);
        Assert.False(playback.Started);
        Assert.Equal(TweenState.Delayed, playback.State);
        playback.Advance(0.5);
        Assert.True(playback.Started);
        Assert.Equal(TweenState.Playing, playback.State);
        Assert.Equal(0.25f, playback.Progress);
    }

    [Fact]
    public void CompletionReportsOvershootAndFinalProgress()
    {
        var playback = Create(new TweenOptions { Duration = 1 });
        playback.Advance(1.25);
        Assert.True(playback.Completed);
        Assert.Equal(TweenState.Completed, playback.State);
        Assert.Equal(1, playback.Progress);
        Assert.Equal(0.25, playback.Overshoot, 9);
    }

    [Fact]
    public void PingPongWalksBothLegsAndEndsAtTheStart()
    {
        var playback = Create(new TweenOptions { Duration = 1, UsePingPong = true, PingPongInterval = 0.5 });
        playback.Advance(1);
        Assert.Equal(1, playback.Progress);
        Assert.Equal(TweenState.Playing, playback.State);
        playback.Advance(0.25);
        Assert.Equal(TweenState.Interval, playback.State);
        Assert.Equal(1, playback.Progress);
        playback.Advance(0.75);
        Assert.Equal(TweenState.Playing, playback.State);
        Assert.Equal(0.5f, playback.Progress);
        playback.Advance(0.5);
        Assert.Equal(0, playback.Progress);
        Assert.True(playback.Completed);
    }

    [Fact]
    public void RepeatIntervalHoldsTheEndpointBetweenCycles()
    {
        var playback = Create(new TweenOptions { Duration = 1, RepeatInterval = 1, Repeats = 1 });
        playback.Advance(1.5);
        Assert.Equal(TweenState.Interval, playback.State);
        Assert.Equal(1, playback.Progress);
        playback.Advance(0.75);
        Assert.Equal(TweenState.Playing, playback.State);
        Assert.Equal(0.25f, playback.Progress);
        playback.Advance(0.75);
        Assert.True(playback.Completed);
    }

    [Fact]
    public void PingPongRepeatIntervalHoldsTheStartBetweenCycles()
    {
        var playback = Create(new TweenOptions { Duration = 1, UsePingPong = true, RepeatInterval = 1, Repeats = 1 });
        playback.Advance(2.5);
        Assert.Equal(TweenState.Interval, playback.State);
        Assert.Equal(0, playback.Progress);
        playback.Advance(1);
        Assert.Equal(TweenState.Playing, playback.State);
        Assert.Equal(0.5f, playback.Progress);
    }

    [Fact]
    public void ZeroDurationPingPongJumpsBetweenEndpoints()
    {
        var playback = Create(new TweenOptions { UsePingPong = true, PingPongInterval = 1, RepeatInterval = 1, Repeats = 1 });
        playback.Advance(0.5);
        Assert.Equal(TweenState.Interval, playback.State);
        Assert.Equal(1, playback.Progress);
        playback.Advance(1);
        Assert.Equal(TweenState.Interval, playback.State);
        Assert.Equal(0, playback.Progress);
    }

    [Fact]
    public void CycleBoundaryShowsThePreviousEndpoint()
    {
        var playback = Create(new TweenOptions { Duration = 1, Repeats = 2 });
        playback.Advance(1);
        Assert.Equal(1, playback.Progress);
        Assert.Equal(TweenState.Playing, playback.State);
        playback.Advance(0.5);
        Assert.Equal(0.5f, playback.Progress);
    }

    [Fact]
    public void OffsetSkipsIntoTheFirstCycle()
    {
        var playback = Create(new TweenOptions { Duration = 2, Offset = 1 });
        playback.Advance(0);
        Assert.True(playback.Started);
        Assert.Equal(0.5f, playback.Progress);
    }

    [Fact]
    public void CreditStartsTheTimelineLater()
    {
        var playback = Create(new TweenOptions { Duration = 1, Delay = 0.5 });
        playback.Credit(0.75);
        playback.Advance(0);
        Assert.Equal(0.25f, playback.Progress);
    }

    [Fact]
    public void InfiniteRepeatsNeverComplete()
    {
        var playback = Create(new TweenOptions { Duration = 1, Repeats = TweenOptions.Infinite });
        playback.Advance(1e12);
        Assert.False(playback.Completed);
        playback.Advance(double.MaxValue);
        Assert.False(playback.Completed);
    }

    [Fact]
    public void HugeDeltasSaturateInsteadOfOverflowing()
    {
        var playback = Create(new TweenOptions { Duration = 1, Repeats = int.MaxValue });
        playback.Advance(double.MaxValue);
        playback.Advance(double.MaxValue);
        Assert.True(playback.Completed);
    }

    [Theory]
    [InlineData(-1d)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void EveryDurationLikeOptionRejectsInvalidValues(double invalid)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Create(new TweenOptions { Duration = invalid }));
        Assert.Throws<ArgumentOutOfRangeException>(() => Create(new TweenOptions { Delay = invalid }));
        Assert.Throws<ArgumentOutOfRangeException>(() => Create(new TweenOptions { PingPongInterval = invalid }));
        Assert.Throws<ArgumentOutOfRangeException>(() => Create(new TweenOptions { RepeatInterval = invalid }));
        Assert.Throws<ArgumentOutOfRangeException>(() => Create(new TweenOptions { Duration = 1, Offset = invalid }));
    }

    [Fact]
    public void RejectsInconsistentConfiguration()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Create(new TweenOptions { Duration = 1, Offset = 2 }));
        Assert.Throws<ArgumentOutOfRangeException>(() => Create(new TweenOptions { Repeats = -2 }));
        Assert.Throws<ArgumentException>(() => Create(new TweenOptions { ProcessMode = (TweenProcessMode)7 }));
        Assert.Throws<ArgumentException>(() => Create(new TweenOptions { PauseMode = (TweenPauseMode)7 }));
        Assert.Throws<ArgumentException>(() => Create(new TweenOptions { Fill = (FillMode)4 }));
        Assert.Throws<ArgumentException>(() => Create(new TweenOptions { Repeats = TweenOptions.Infinite }));
    }

    [Fact]
    public void RejectsTimelinesTooLongToRepresent()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Create(new TweenOptions { Duration = double.MaxValue, Delay = double.MaxValue }));
        Assert.Throws<ArgumentOutOfRangeException>(() => Create(new TweenOptions { Duration = double.MaxValue / 2, Repeats = 4 }));
        // Infinite repeats only require a finite cycle.
        Create(new TweenOptions { Duration = double.MaxValue / 4, Repeats = TweenOptions.Infinite });
    }

    [Fact]
    public void NonnegativeReturnsValidValues()
    {
        Assert.Equal(0, Playback.Nonnegative(0, "zero"));
        Assert.Equal(3.5, Playback.Nonnegative(3.5, "value"));
        var error = Assert.Throws<ArgumentOutOfRangeException>(() => Playback.Nonnegative(-0.1, "value"));
        Assert.Equal("value", error.ParamName);
    }
}
