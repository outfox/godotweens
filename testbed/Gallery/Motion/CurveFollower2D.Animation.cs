// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in CurveFollower2D.cs.
public sealed partial class CurveFollower2D
{
    private readonly Tweens.PathFollow2DProgressRatio progress = new()
    {
        To = 1,
        Ease = DefaultEase,
        UsePingPong = true,
        Repeats = TweenOptions.Infinite,
        RepeatInterval = 0.25,
        PingPongInterval = 0.15,
    };

    private readonly Tweens.PathFollow2DVOffset offset = new()
    {
        To = 20,
        Ease = DefaultEase,
        UsePingPong = true,
        Repeats = TweenOptions.Infinite,
        RepeatInterval = 0.25,
        PingPongInterval = 0.15,
    };

    private readonly Tweens.Scale2D scale = new()
    {
        To = new Vector2(1.6f, 1.6f),
        Ease = DefaultEase,
        UsePingPong = true,
        Repeats = TweenOptions.Infinite,
        RepeatInterval = 0.25,
        PingPongInterval = 0.15,
    };

    private async Task FollowPath()
    {
        var progress = this.progress with { Duration = Seconds };
        var offset = this.offset with { Duration = Seconds };
        var scale = this.scale with { Duration = Seconds };

        var tweens = new List<TweenInstance>();
        for (var e = 0; e < echoes.Length; e++)
        {
            var delay = (e + 1) * 0.07;
            tweens.AddRange([
                echoes[e].Tween(progress with { Delay = delay }),
                echoes[e].Tween(offset with { Delay = delay }),
            ]);
        }
        tweens.AddRange([
            leader.Tween(progress),
            leader.Tween(offset),
            ship.Tween(scale),
        ]);
        await Group.Of([.. tweens]).End;
    }
}
