// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd;

namespace testbed.Tests.Core;

public class EasingTests
{
    [Fact]
    public void All31EasesHaveCorrectEndpointsAndFiniteSamples()
    {
        var types = Enum.GetValues<EaseType>();
        Assert.Equal(31, types.Length);
        foreach (var ease in types)
        {
            Assert.InRange(Math.Abs(Easing.Evaluate(ease, 0)), 0, 0.00001f);
            Assert.InRange(Math.Abs(Easing.Evaluate(ease, 1) - 1), 0, 0.00001f);
            for (var i = 0; i <= 100; i++) Assert.True(float.IsFinite(Easing.Evaluate(ease, i / 100f)));
        }
        Assert.Equal(0.25f, Easing.Evaluate(EaseType.QuadIn, 0.5f));
        Assert.Equal(0.875f, Easing.Evaluate(EaseType.CubicOut, 0.5f));
        Assert.True(Easing.Evaluate(EaseType.BackOut, 0.7f) > 1);
        Assert.True(Interpolators.Float(0, 10, Easing.Evaluate(EaseType.BackOut, 0.7f)) > 10);
    }

    [Fact]
    public void QuaternionUsesShortestNormalizedPathAndRectInterpolatesPositionAndSize()
    {
        var start = Quaternion.Identity;
        var end = new Quaternion(Vector3.Up, Mathf.Pi / 2);
        var result = Interpolators.Quaternion(start, end, 0.5f);
        Assert.True(result.IsNormalized());
        Assert.True(result.IsEqualApprox(new Quaternion(Vector3.Up, Mathf.Pi / 4)));
        Assert.True(Interpolators.Quaternion(start, -end, 0.5f).IsEqualApprox(result));
        Assert.Equal(new Rect2(5, 10, 15, 20), Interpolators.Rect2(new Rect2(), new Rect2(10, 20, 30, 40), 0.5f));
    }
}
