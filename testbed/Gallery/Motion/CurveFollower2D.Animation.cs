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
        var progressDefinition = progress with { Duration = Seconds };
        var offsetDefinition = offset with { Duration = Seconds };
        var scaleDefinition = scale with { Duration = Seconds };

        var tweens = new List<TweenInstance>();
        for (var e = 0; e < echoes.Length; e++)
        {
            var delay = (e + 1) * 0.07;
            tweens.AddRange([
                echoes[e].Tween(progressDefinition with { Delay = delay }),
                echoes[e].Tween(offsetDefinition with { Delay = delay }),
            ]);
        }
        tweens.AddRange([
            leader.Tween(progressDefinition),
            leader.Tween(offsetDefinition),
            ship.Tween(scaleDefinition),
        ]);
        await Group.Of([.. tweens]).End;
    }
}
