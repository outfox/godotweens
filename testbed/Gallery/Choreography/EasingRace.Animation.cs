// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using System.Threading.Tasks;
using tweens.gd;
namespace testbed;

// Scene setup is in EasingRace.cs.
public sealed partial class EasingRace
{
    private const float StartLine = -120, FinishLine = 120, LaneHeight = 24;
    private static readonly (EaseType Ease, string Name)[] Lanes =
    [
        (EaseType.Linear, "Linear"), (EaseType.SineInOut, "Sine"), (EaseType.CubicInOut, "Cubic"), (EaseType.ExpoInOut, "Expo"),
        (EaseType.BackInOut, "Back"), (EaseType.ElasticOut, "Elastic"), (EaseType.BounceOut, "Bounce"),
    ];

    private async Task AnimateAsync()
    {
        var tweens = new List<TweenInstance>();
        for (var lane = 0; lane < racers.Length; lane++)
            for (var position = 0; position < racers[lane].Length; position++)
            {
                var ease = Lanes[lane].Ease;
                var delay = position * 0.05;
                tweens.Add(racers[lane][position].TweenPositionX(FinishLine, Seconds, options =>
                {
                    options.Ease = ease;
                    options.UsePingPong = true;
                    options.Repeats = TweenOptions.Infinite;
                    options.PingPongInterval = 0.3;
                    options.RepeatInterval = 0.3;
                    options.Delay = delay;
                }));
            }
        await Group.Of([.. tweens]).End;
    }
}
