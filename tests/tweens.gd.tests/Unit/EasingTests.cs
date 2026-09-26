// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

namespace tweens.gd.Tests.Unit;

public class EasingTests
{
    public static TheoryData<EaseType> All => new(Enum.GetValues<EaseType>());

    [Theory]
    [MemberData(nameof(All))]
    public void EveryEaseStartsAtZeroAndEndsAtOne(EaseType ease)
    {
        Assert.Equal(0, Easing.Evaluate(ease, 0), 5);
        Assert.Equal(1, Easing.Evaluate(ease, 1), 5);
    }

    [Theory]
    [MemberData(nameof(All))]
    public void EveryEaseIsFiniteAndClampsItsInput(EaseType ease)
    {
        for (var i = 0; i <= 100; i++)
            Assert.True(float.IsFinite(Easing.Evaluate(ease, i / 100f)), $"{ease} at {i}%");
        Assert.Equal(Easing.Evaluate(ease, 0), Easing.Evaluate(ease, -3));
        Assert.Equal(Easing.Evaluate(ease, 1), Easing.Evaluate(ease, 7));
    }

    [Theory]
    [MemberData(nameof(All))]
    public void InOutEasesAreSymmetricAroundTheMidpoint(EaseType ease)
    {
        if (!ease.ToString().EndsWith("InOut") && ease is not (EaseType.SmoothStep or EaseType.SmootherStep)) return;
        Assert.Equal(0.5f, Easing.Evaluate(ease, 0.5f), 4);
        foreach (var t in new[] { 0.1f, 0.25f, 0.4f })
            Assert.Equal(1 - Easing.Evaluate(ease, t), Easing.Evaluate(ease, 1 - t), 4);
    }

    [Theory]
    [InlineData(EaseType.SineIn, EaseType.SineOut)]
    [InlineData(EaseType.QuadIn, EaseType.QuadOut)]
    [InlineData(EaseType.CubicIn, EaseType.CubicOut)]
    [InlineData(EaseType.QuartIn, EaseType.QuartOut)]
    [InlineData(EaseType.QuintIn, EaseType.QuintOut)]
    [InlineData(EaseType.ExpoIn, EaseType.ExpoOut)]
    [InlineData(EaseType.CircIn, EaseType.CircOut)]
    [InlineData(EaseType.BackIn, EaseType.BackOut)]
    [InlineData(EaseType.ElasticIn, EaseType.ElasticOut)]
    [InlineData(EaseType.BounceIn, EaseType.BounceOut)]
    public void OutEasesMirrorInEases(EaseType @in, EaseType @out)
    {
        foreach (var t in new[] { 0.1f, 0.3f, 0.5f, 0.7f, 0.9f })
            Assert.Equal(1 - Easing.Evaluate(@in, 1 - t), Easing.Evaluate(@out, t), 4);
    }

    [Theory]
    [InlineData(EaseType.Linear, 0.25f, 0.25f)]
    [InlineData(EaseType.QuadIn, 0.5f, 0.25f)]
    [InlineData(EaseType.QuadInOut, 0.25f, 0.125f)]
    [InlineData(EaseType.CubicInOut, 0.25f, 0.0625f)]
    [InlineData(EaseType.QuartInOut, 0.25f, 0.03125f)]
    [InlineData(EaseType.QuintInOut, 0.25f, 0.015625f)]
    [InlineData(EaseType.ExpoIn, 0.5f, 0.03125f)]
    [InlineData(EaseType.SineInOut, 0.5f, 0.5f)]
    [InlineData(EaseType.SmoothStep, 0.25f, 0.15625f)]
    [InlineData(EaseType.SmoothStep, 0.75f, 0.84375f)]
    [InlineData(EaseType.SmootherStep, 0.25f, 0.103515625f)]
    [InlineData(EaseType.SmootherStep, 0.75f, 0.896484375f)]
    public void KnownSamples(EaseType ease, float progress, float expected)
        => Assert.Equal(expected, Easing.Evaluate(ease, progress), 5);

    [Theory]
    [InlineData(EaseType.SmoothStep)]
    [InlineData(EaseType.SmootherStep)]
    public void SmoothStepsIncreaseWithoutOvershooting(EaseType ease)
    {
        var previous = 0f;
        for (var i = 0; i <= 100; i++)
        {
            var value = Easing.Evaluate(ease, i / 100f);
            Assert.InRange(value, previous, 1f);
            previous = value;
        }
    }

    [Theory]
    [InlineData(EaseType.SmoothStep)]
    [InlineData(EaseType.SmootherStep)]
    public void SmoothStepsHaveZeroEndpointVelocity(EaseType ease)
    {
        const float h = 0.001f;
        Assert.InRange(Math.Abs(Easing.Evaluate(ease, h) / h), 0, 0.004f);
        Assert.InRange(Math.Abs((1 - Easing.Evaluate(ease, 1 - h)) / h), 0, 0.004f);
    }

    [Fact]
    public void SmootherStepHasZeroEndpointAcceleration()
    {
        const float h = 0.01f;
        var start = (Easing.Evaluate(EaseType.SmootherStep, 2 * h) - 2 * Easing.Evaluate(EaseType.SmootherStep, h)) / (h * h);
        var end = (1 - 2 * Easing.Evaluate(EaseType.SmootherStep, 1 - h) + Easing.Evaluate(EaseType.SmootherStep, 1 - 2 * h)) / (h * h);
        Assert.InRange(Math.Abs(start), 0, 0.6f);
        Assert.InRange(Math.Abs(end), 0, 0.6f);
    }

    [Fact]
    public void OvershootingEasesLeaveTheUnitRange()
    {
        Assert.True(Easing.Evaluate(EaseType.BackIn, 0.3f) < 0);
        Assert.True(Easing.Evaluate(EaseType.BackOut, 0.7f) > 1);
        Assert.True(Easing.Evaluate(EaseType.ElasticOut, 0.2f) > 1);
    }

    [Theory]
    [InlineData(1 / 2.75f)]
    [InlineData(2 / 2.75f)]
    [InlineData(2.5f / 2.75f)]
    public void BounceOutIsContinuousAcrossSegments(float t)
    {
        const float epsilon = 1e-4f;
        Assert.Equal(Easing.Evaluate(EaseType.BounceOut, t - epsilon), Easing.Evaluate(EaseType.BounceOut, t + epsilon), 2);
    }

    [Fact]
    public void UnknownEaseIsRejected()
    {
        Assert.Throws<NotImplementedException>(() => Easing.Evaluate((EaseType)1234, 0.5f));
        Assert.Throws<NotImplementedException>(() => Easing.GetFunction((EaseType)(-1)));
    }
}
