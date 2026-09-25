// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in UvScroll.cs.
public sealed partial class UvScroll
{
    private readonly Tweens.MaterialUv1OffsetX offset = new()
    {
        To = 1,
        Ease = DefaultEase,
        UsePingPong = true,
        Repeats = TweenOptions.Infinite,
        RepeatInterval = 0.25,
        PingPongInterval = 0.15,
    };

    private readonly Tweens.MaterialUv1Scale scale = new()
    {
        To = new Vector3(2.5f, 2.5f, 1),
        Ease = DefaultEase,
        UsePingPong = true,
        Repeats = TweenOptions.Infinite,
        RepeatInterval = 0.25,
        PingPongInterval = 0.15,
    };

    private async Task AnimateAsync()
    {
        var duration = Seconds * 2;
        await Group.Of([
            material.Tween(offset with { Duration = duration }, Stage),
            material.Tween(scale with { Duration = duration }, Stage),
        ]).End;
    }
}
