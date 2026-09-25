// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in EasingRace.cs.
public sealed partial class EasingRace
{
    private const float StartLine = -120, FinishLine = 120, LaneHeight = 24;
    private static readonly (EaseType Ease, string Name)[] Lanes =
    [
        (EaseType.Linear, "Linear"), (EaseType.SineInOut, "Sine"), (EaseType.CubicInOut, "Cubic"), (EaseType.ExpoInOut, "Expo"),
        (EaseType.BackInOut, "Back"), (EaseType.ElasticOut, "Elastic"), (EaseType.BounceOut, "Bounce"),
    ];

    private IEnumerable<TweenInstance> CreateAnimation()
    {
        for (var lane = 0; lane < racers.Length; lane++)
            for (var position = 0; position < racers[lane].Length; position++)
            {
                var ease = Lanes[lane].Ease;
                var delay = position * 0.05;
                void Race(TweenOptions t)
                {
                    t.Ease = ease;
                    t.UsePingPong = true;
                    t.IsInfinite = true;
                    t.PingPongInterval = 0.3;
                    t.RepeatInterval = 0.3;
                    t.Delay = delay;
                }
                yield return racers[lane][position].TweenPositionX(FinishLine, Seconds, Race);
            }
    }
}
