// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in RangeMeter.cs.
public sealed partial class RangeMeter
{
    private async Task AnimateAsync()
    {
        await Group.Of([
            progress.TweenValue(100, Seconds, Cycle),
            swatch.TweenColor(Palette.Blue, Seconds, Cycle),
            swatch.TweenSelfModulateAlpha(0.25f, Seconds, Cycle),
        ]).End;
    }

    private void Cycle(TweenOptionsBuilder options)
    {
        options.Ease = DefaultEase;
        options.UsePingPong = true;
        options.Repeats = TweenOptions.Infinite;
        options.RepeatInterval = 0.25;
        options.PingPongInterval = 0.15;
    }
}
