// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using godotweens;
namespace testbed.Tests;
public class IntegerInterpolationTests
{
    [Theory]
    [InlineData(0, 3, 0.5f, 2)]
    [InlineData(0, -3, 0.5f, -2)]
    [InlineData(6, 2, 0.5f, 4)]
    [InlineData(int.MinValue, int.MaxValue, 0.5f, -1)]
    [InlineData(int.MinValue, int.MaxValue, 1, int.MaxValue)]
    [InlineData(int.MinValue, int.MaxValue, 2, int.MaxValue)]
    [InlineData(int.MinValue, int.MaxValue, -1, int.MinValue)]
    public void RoundsAndSaturatesWithoutOverflow(int from, int to, float weight, int expected)
        => Assert.Equal(expected, Interpolators.Int(from, to, weight));

    [Theory]
    [InlineData(float.NaN)]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity)]
    public void RejectsNonFiniteWeights(float weight)
        => Assert.Throws<ArgumentOutOfRangeException>(() => Interpolators.Int(0, 10, weight));
}
