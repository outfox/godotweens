// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in GlowingRibbon.cs.
public sealed partial class GlowingRibbon
{
    private async Task AnimateAsync()
    {
        await Group.Of([
            ribbon.TweenWidth(16, Seconds, Cycle),
            ribbon.TweenDefaultColor(Palette.Blue, Seconds, Cycle),
            glow.TweenWidth(40, Seconds, Cycle),
            glow.TweenDefaultColor(Palette.Blue with { A = 0.28f }, Seconds, Cycle),
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
