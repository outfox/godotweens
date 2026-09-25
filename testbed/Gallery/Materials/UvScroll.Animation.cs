// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in UvScroll.cs.
public sealed partial class UvScroll
{
    private async Task AnimateAsync()
    {
        var slow = Seconds * 2;
        await Group.Of([
            material.TweenUv1OffsetX(1, slow, Stage, Cycle),
            material.TweenUv1Scale(new Vector3(2.5f, 2.5f, 1), slow, Stage, Cycle),
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
