// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in OffsetTransforms.cs.
public sealed partial class OffsetTransforms
{
    private async Task AnimateAsync()
    {
        await Group.Of([
            featured.TweenOffsetTransformPosition(new Vector2(0, -22), Seconds, Cycle),
            featured.TweenOffsetTransformRotation(0.18f, Seconds, Cycle),
            featured.TweenOffsetTransformScale(new Vector2(1.13f, 1.13f), Seconds, Cycle),
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
