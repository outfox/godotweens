// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in RangeMeter.cs.
public sealed partial class RangeMeter
{
    private IEnumerable<TweenInstance> CreateAnimation()
    {
        yield return progress.TweenValue(100, Seconds, Cycle);
        yield return swatch.TweenColor(Palette.Blue, Seconds, Cycle);
        yield return swatch.TweenSelfModulateAlpha(0.25f, Seconds, Cycle);
    }
}
