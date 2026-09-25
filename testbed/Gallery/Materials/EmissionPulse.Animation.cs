// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in EmissionPulse.cs.
public sealed partial class EmissionPulse
{
    private async Task AnimateAsync()
    {
        await Group.Of([
            material.TweenEmission(new Color(0.12f, 0.08f, 0.3f), Seconds, Stage, Cycle),
            material.TweenEmissionEnergyMultiplier(2, Seconds, Stage, Cycle),
        ]).End;
    }

    private void Cycle(TweenOptionsBuilder options)
    {
        options.Ease = DefaultEase;
        options.UsePingPong = true;
        options.Repeats = TweenOptions.Infinite;
        options.RepeatInterval = 0.25;
        options.PingPongInterval = 0.15;
    }
}
