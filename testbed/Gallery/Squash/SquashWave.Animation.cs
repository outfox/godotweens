// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in SquashWave.cs.
public sealed partial class SquashWave
{
    private async Task AnimateAsync()
    {
        var tweens = new List<TweenInstance>();
        var beat = Seconds * 0.5;
        for (var i = 0; i < pills.Length; i++)
        {
            var pill = pills[i];
            var delay = i * 0.07 * Tempo;
            tweens.Add(pill.TweenScaleY(2.6f, beat, options =>
            {
                Cycle(options);
                options.Delay = delay;
            }));
            tweens.Add(pill.TweenScaleX(0.62f, beat, options =>
            {
                Cycle(options);
                options.Delay = delay;
            }));
            tweens.Add(pill.TweenColor(pill.Color.Lightened(0.45f), beat, options =>
            {
                Cycle(options);
                options.Delay = delay;
            }));
        }
        await Group.Of([.. tweens]).End;
    }

    private void Cycle(TweenOptionsBuilder options)
    {
        options.Ease = DefaultEase;
        options.UsePingPong = true;
        options.Repeats = TweenOptions.Infinite;
        options.RepeatInterval = 0.1;
        options.PingPongInterval = 0.05;
    }
}
