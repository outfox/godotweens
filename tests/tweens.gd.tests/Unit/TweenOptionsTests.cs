// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using tweens.gd.Tests.Support;

namespace tweens.gd.Tests.Unit;

public class TweenOptionsTests
{
    private static readonly Func<float, float> Ease = static x => x * x;

    private static TweenOptions Everything() => new()
    {
        Duration = 1.5, Delay = 0.25, PingPongInterval = 0.5, RepeatInterval = 0.75, Offset = 0.125, Repeats = 3,
        UsePingPong = true, UseUnscaledTime = true, Fill = FillMode.Both, Ease = EaseType.BounceOut,
        EaseFunction = Ease, ProcessMode = TweenProcessMode.Physics, PauseMode = TweenPauseMode.Always,
        SuppressCallbacksWhenTargetInvalid = true,
    };

    [Fact]
    public void DefaultOptionsRetainTheFinalValue()
    {
        Assert.Equal(FillMode.RetainFinalValue, default(TweenOptions).Fill);
        Assert.Equal(FillMode.RetainFinalValue, new TweenOptions().Fill);
        Assert.Equal(default, new TweenOptions());
        Assert.Equal(FillMode.RetainFinalValue, new PlainTween().Fill);
        Assert.Equal(TweenOptions.Infinite, TweenOptionsBuilder.Infinite);
    }

    [Theory]
    [InlineData(FillMode.None)]
    [InlineData(FillMode.ApplyFromDuringDelay)]
    [InlineData(FillMode.RetainFinalValue)]
    [InlineData(FillMode.Both)]
    public void FillRoundTripsEveryValue(FillMode fill)
    {
        Assert.Equal(fill, new TweenOptions { Fill = fill }.Fill);
        Assert.Equal(fill, (new TweenOptions() with { Fill = fill }).Fill);
    }

    [Fact]
    public void CopyToAndToOptionsTransferEveryOption()
    {
        var options = Everything();
        var builder = new PlainTween();
        options.CopyTo(builder);
        Assert.Equal(1.5, builder.Duration);
        Assert.Equal(0.25, builder.Delay);
        Assert.Equal(0.5, builder.PingPongInterval);
        Assert.Equal(0.75, builder.RepeatInterval);
        Assert.Equal(0.125, builder.Offset);
        Assert.Equal(3, builder.Repeats);
        Assert.True(builder.UsePingPong);
        Assert.True(builder.UseUnscaledTime);
        Assert.Equal(FillMode.Both, builder.Fill);
        Assert.Equal(EaseType.BounceOut, builder.Ease);
        Assert.Same(Ease, builder.EaseFunction);
        Assert.Null(builder.Curve);
        Assert.Equal(TweenProcessMode.Physics, builder.ProcessMode);
        Assert.Equal(TweenPauseMode.Always, builder.PauseMode);
        Assert.True(builder.SuppressCallbacksWhenTargetInvalid);
        Assert.Equal(options, builder.ToOptions());
    }

    [Fact]
    public void OptionsAreValueTypesWithStructuralEquality()
    {
        var a = Everything();
        var b = a with { Delay = 1 };
        Assert.NotEqual(a, b);
        Assert.Equal(a, b with { Delay = 0.25 });
        Assert.Equal(a.GetHashCode(), (b with { Delay = 0.25 }).GetHashCode());
        Assert.Contains("Delay", a.ToString());
    }
}
