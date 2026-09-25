// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in Spotlight.cs.
public sealed partial class Spotlight
{
    private IEnumerable<TweenInstance> CreateAnimation()
    {
        yield return spot.TweenSpotAngle(52, Seconds, Cycle);
        yield return spot.TweenLightColor(Palette.Blue, Seconds, Cycle);
        yield return spot.TweenLightEnergy(3, Seconds, Cycle);
    }
}
