// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in PolygonEchoes.cs.
public sealed partial class PolygonEchoes
{
    private IEnumerable<TweenInstance> CreateAnimation()
    {
        for (var e = 0; e < echoes.Length; e++)
        {
            var lag = echoes.Length - e;
            foreach (var tween in AnimatePolygon(echoes[e], CycleAfter(lag * 0.08))) yield return tween;
        }
        foreach (var tween in AnimatePolygon(polygon, Cycle)) yield return tween;
    }

    private IEnumerable<TweenInstance> AnimatePolygon(Polygon2D target, Action<TweenOptions> timing)
    {
        yield return target.TweenColor(Palette.Amber, Seconds, timing);
        yield return target.TweenOffset(new Vector2(40, 0), Seconds, timing);
        yield return target.TweenRotation(Mathf.Pi, Seconds * 2, timing);
    }
}
