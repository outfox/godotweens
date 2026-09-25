// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in TextReveal.cs.
public sealed partial class TextReveal
{
    private IEnumerable<TweenInstance> CreateAnimation()
    {
        yield return text.TweenVisibleRatio(1, Seconds, Cycle);
        yield return text.TweenSelfModulate(Palette.Amber, Seconds, Cycle);
        yield return underline.TweenScaleX(1, Seconds, Cycle);
        yield return underline.TweenColor(Palette.Amber, Seconds, Cycle);
    }
}
