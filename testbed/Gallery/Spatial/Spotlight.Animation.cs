// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Threading.Tasks;
using tweens.gd;
namespace testbed;

// Scene setup is in Spotlight.cs.
public sealed partial class Spotlight
{
    private async Task AnimateAsync()
    {
        await Group.Of([
            spot.TweenSpotAngle(52, Seconds, Cycle),
            spot.TweenLightColor(Palette.Blue, Seconds, Cycle),
            spot.TweenLightEnergy(3, Seconds, Cycle),
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
