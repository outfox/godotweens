// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in CurveFollower3D.cs.
public sealed partial class CurveFollower3D
{
    private async Task AnimateAsync()
    {
        var tweens = new List<TweenInstance>();
        var lap = Seconds * 1.5;
        tweens.Add(leader.TweenProgressRatio(1, lap, Cycle));
        for (var e = 0; e < echoes.Length; e++)
        {
            var trailing = CycleAfter((e + 1) * 0.08);
            tweens.Add(echoes[e].TweenProgressRatio(1, lap, trailing));
            tweens.Add(echoes[e].TweenVOffset(0.35f, Seconds, trailing));
        }
        tweens.Add(leader.TweenVOffset(0.35f, Seconds, Cycle));
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

    private Action<TweenOptions> CycleAfter(double delay) => options =>
    {
        Cycle(options);
        options.Delay = delay;
    };
}
