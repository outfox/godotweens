// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using System.Linq;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in CurveFollower2D.cs.
public sealed partial class CurveFollower2D
{
    private TweenInstance[] FollowPath(double duration, EaseType ease, bool pingPong)
    {
        var tweens = new List<TweenInstance>();
        for (var e = 0; e < echoes.Length; e++)
        {
            var delay = (e + 1) * 0.07;
            tweens.AddRange([
                echoes[e].TweenProgressRatio(1, duration, Trailing),
                echoes[e].TweenVOffset(20, duration, Trailing),
            ]);

            void Trailing(TweenOptions options)
            {
                Loop(options);
                options.Delay = delay;
            }
        }
        tweens.AddRange([
            leader.TweenProgressRatio(1, duration, Loop),
            leader.TweenVOffset(20, duration, Loop),
            ship.TweenScale(new Vector2(1.6f, 1.6f), duration, Loop),
        ]);
        return tweens.ToArray();

        void Loop(TweenOptions options)
        {
            options.Ease = ease;
            options.UsePingPong = pingPong;
            options.IsInfinite = true;
            options.RepeatInterval = 0.25;
            options.PingPongInterval = 0.15;
        }
    }
}
