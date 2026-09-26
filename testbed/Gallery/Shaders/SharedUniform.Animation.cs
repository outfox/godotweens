// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Threading.Tasks;
using tweens.gd;
namespace testbed;

// Scene setup is in SharedUniform.cs.
public sealed partial class SharedUniform
{
    private async Task AnimateAsync()
    {
        await Group.Of([
            material.TweenShaderParameter("amount", 0.85f, Seconds, Stage, Cycle),
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
