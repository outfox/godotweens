// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in SquashWave.cs.
public sealed partial class SquashWave
{
    private IEnumerable<TweenInstance> CreateAnimation()
    {
        var beat = Seconds * 0.5;
        for (var i = 0; i < pills.Length; i++)
        {
            var pill = pills[i];
            var delay = i * 0.07 * Tempo;
            void Wave(TweenOptions t)
            {
                Cycle(t);
                t.Delay = delay;
                t.RepeatInterval = 0.1;
                t.PingPongInterval = 0.05;
            }
            yield return pill.TweenScaleY(2.6f, beat, Wave);
            yield return pill.TweenScaleX(0.62f, beat, Wave);
            yield return pill.TweenColor(pill.Color.Lightened(0.45f), beat, Wave);
        }
    }
}
