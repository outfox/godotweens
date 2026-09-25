// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in PolygonEchoes.cs.
public sealed partial class PolygonEchoes
{
    private async Task AnimateAsync()
    {
        var trails = echoes.Select((echo, i) => AnimatePolygon(echo, CycleAfter((echoes.Length - i) * 0.08)));
        await Task.WhenAll([.. trails, AnimatePolygon(polygon, Cycle)]);
    }

    private async Task AnimatePolygon(Polygon2D target, Action<TweenOptionsBuilder> timing)
    {
        await Group.Of(
            target.TweenColor(Palette.Amber, Seconds, timing),
            target.TweenOffset(new Vector2(40, 0), Seconds, timing),
            target.TweenRotation(Mathf.Pi, Seconds * 2, timing)
        ).End;
    }

    private void Cycle(TweenOptionsBuilder options)
    {
        options.Ease = DefaultEase;
        options.UsePingPong = true;
        options.Repeats = TweenOptions.Infinite;
        options.RepeatInterval = 0.25;
        options.PingPongInterval = 0.15;
    }

    private Action<TweenOptionsBuilder> CycleAfter(double delay) => options =>
    {
        Cycle(options);
        options.Delay = delay;
    };
}
