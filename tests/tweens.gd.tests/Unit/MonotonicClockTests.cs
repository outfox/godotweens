// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

namespace tweens.gd.Tests.Unit;

public class MonotonicClockTests
{
    [Fact]
    public void SamplesElapsedSecondsAndIgnoresBackwardJumps()
    {
        var readings = new Queue<ulong>([1_000_000, 1_500_000, 1_200_000, 3_200_000]);
        var clock = new MonotonicClock(readings.Dequeue);
        Assert.Equal(0.5, clock.Sample());
        Assert.Equal(0, clock.Sample());
        Assert.Equal(2, clock.Sample());
    }
}
