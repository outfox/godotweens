// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Threading.Tasks;
using tweens.gd;
namespace testbed;

// Scene setup is in LightSweep.cs.
public sealed partial class LightSweep
{
    private async Task AnimateAsync()
    {
        await Group.Of([
            light.TweenTextureScale(2.5f, Seconds, Cycle),
            light.TweenEnergy(2, Seconds, Cycle),
            light.TweenPositionX(100, Seconds, Cycle),
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
