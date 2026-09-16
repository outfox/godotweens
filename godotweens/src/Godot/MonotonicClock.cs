// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

namespace GodotWeens;

/// <summary>Samples elapsed real time once per process frame, including frames where tweens pause.</summary>
internal sealed class MonotonicClock(Func<ulong> readMicroseconds)
{
    private ulong previous = readMicroseconds();

    internal double Sample()
    {
        var now = readMicroseconds();
        var delta = now >= previous ? (now - previous) / 1_000_000.0 : 0;
        previous = now;
        return delta;
    }
}
