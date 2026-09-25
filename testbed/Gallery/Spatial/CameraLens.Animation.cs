// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in CameraLens.cs.
public sealed partial class CameraLens
{
    private async Task AnimateAsync()
    {
        await Group.Of([
            camera.TweenFov(65, Seconds, Cycle),
            camera.TweenHOffset(0.7f, Seconds, Cycle),
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
