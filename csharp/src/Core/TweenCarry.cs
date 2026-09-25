// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

namespace tweens.gd;

/// <summary>Where and when a timeline ended, and by how much its last update overshot the end.</summary>
internal sealed record Carry(TweenScheduler Scheduler, TweenProcessMode Mode, bool Unscaled, long Tick, double Overshoot);

/// <summary>Hands a finished timeline's overshoot to the tweens that continue it.</summary>
/// <remarks>
/// The scope is open while OnEnd runs and while End resolves. Awaiting code on Godot's main thread resumes
/// inline there, so tweens it starts continue the timeline; a continuation posted for later gets no credit.
/// </remarks>
internal static class TweenCarry
{
    [ThreadStatic] private static Carry? current;

    internal static Scope Enter(Carry? carry)
    {
        var previous = current;
        current = carry;
        return new Scope(previous);
    }

    /// <summary>The credit for a tween added now, if a carry for its scheduler, lane and time base is current.</summary>
    internal static bool TryGet(TweenScheduler scheduler, TweenProcessMode mode, bool unscaled, out double credit)
    {
        var carry = current;
        var valid = carry is not null && carry.Scheduler == scheduler && carry.Mode == mode &&
            carry.Unscaled == unscaled && carry.Tick == scheduler.Tick(mode);
        credit = valid ? carry!.Overshoot : 0;
        return valid;
    }

    internal readonly struct Scope(Carry? previous) : IDisposable
    {
        public void Dispose() => current = previous;
    }
}
