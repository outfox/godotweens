// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in SharedMaterial.cs.
public sealed partial class SharedMaterial
{
    private async Task AnimateAsync()
    {
        await Group.Of([
            material.TweenAlbedoColor(Palette.Amber, Seconds, Stage, Cycle),
            material.TweenRoughness(0.95f, Seconds, Stage, Cycle),
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
