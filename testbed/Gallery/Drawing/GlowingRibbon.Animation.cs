// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in GlowingRibbon.cs.
public sealed partial class GlowingRibbon
{
    private IEnumerable<TweenInstance> CreateAnimation()
    {
        yield return ribbon.TweenWidth(16, Seconds, Cycle);
        yield return ribbon.TweenDefaultColor(Palette.Blue, Seconds, Cycle);
        yield return glow.TweenWidth(40, Seconds, Cycle);
        yield return glow.TweenDefaultColor(Palette.Blue with { A = 0.28f }, Seconds, Cycle);
    }
}
