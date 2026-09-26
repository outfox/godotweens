// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Threading.Tasks;
using tweens.gd;
namespace testbed;

// Scene setup is in TextReveal.cs.
public sealed partial class TextReveal
{
    private async Task AnimateAsync()
    {
        await Group.Of([
            text.TweenVisibleRatio(1, Seconds, Cycle),
            text.TweenSelfModulate(Palette.Amber, Seconds, Cycle),
            underline.TweenScaleX(1, Seconds, Cycle),
            underline.TweenColor(Palette.Amber, Seconds, Cycle),
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
