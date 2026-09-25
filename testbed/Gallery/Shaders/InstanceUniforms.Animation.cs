// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in InstanceUniforms.cs.
public sealed partial class InstanceUniforms
{
    private async Task AnimateAsync()
    {
        await Group.Of([
            first.TweenInstanceShaderParameter("amount", 0.85f, Seconds, Cycle),
            second.TweenInstanceShaderParameter("amount", 0.15f, Seconds, Cycle),
        ]).End;
    }

    private void Cycle(TweenOptions options)
    {
        options.Ease = Ease;
        options.UsePingPong = PingPong;
        options.Repeats = TweenOptions.Infinite;
        options.RepeatInterval = 0.25;
        options.PingPongInterval = 0.15;
    }
}
