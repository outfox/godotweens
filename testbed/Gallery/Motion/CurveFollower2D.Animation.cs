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
    private async Task FollowPath()
    {
        var tweens = new List<TweenInstance>();
        for (var e = 0; e < echoes.Length; e++)
        {
            var delay = (e + 1) * 0.07;
            tweens.AddRange([
                echoes[e].TweenProgressRatio(1, Seconds, options =>
                {
                    Cycle(options);
                    options.Delay = delay;
                }),
                echoes[e].TweenVOffset(20, Seconds, options =>
                {
                    Cycle(options);
                    options.Delay = delay;
                }),
            ]);
        }
        tweens.AddRange([
            leader.TweenProgressRatio(1, Seconds, Cycle),
            leader.TweenVOffset(20, Seconds, Cycle),
            ship.TweenScale(new Vector2(1.6f, 1.6f), Seconds, Cycle),
        ]);
        await Group.Of([.. tweens]).End;
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
