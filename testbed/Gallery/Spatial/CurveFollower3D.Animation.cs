// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using System.Threading.Tasks;
using tweens.gd;
namespace testbed;

// Scene setup is in CurveFollower3D.cs.
public sealed partial class CurveFollower3D
{
    private readonly Tweens.PathFollow3DProgressRatio progress = new()
    {
        To = 1,
        Ease = DefaultEase,
        UsePingPong = true,
        Repeats = TweenOptions.Infinite,
        RepeatInterval = 0.25,
        PingPongInterval = 0.15,
    };

    private readonly Tweens.PathFollow3DVOffset offset = new()
    {
        To = 0.35f,
        Ease = DefaultEase,
        UsePingPong = true,
        Repeats = TweenOptions.Infinite,
        RepeatInterval = 0.25,
        PingPongInterval = 0.15,
    };

    private async Task AnimateAsync()
    {
        var progress = this.progress with { Duration = Seconds * 1.5 };
        var offset = this.offset with { Duration = Seconds };

        var tweens = new List<TweenInstance>();
        for (var e = 0; e < echoes.Length; e++)
        {
            var delay = (e + 1) * 0.08;
            tweens.AddRange([
                echoes[e].Tween(progress with { Delay = delay }),
                echoes[e].Tween(offset with { Delay = delay }),
            ]);
        }
        tweens.AddRange([
            leader.Tween(progress),
            leader.Tween(offset),
        ]);
        await Group.Of([.. tweens]).End;
    }
}
