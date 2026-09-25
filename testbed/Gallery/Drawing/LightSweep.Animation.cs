// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in LightSweep.cs.
public sealed partial class LightSweep
{
    private IEnumerable<TweenInstance> CreateAnimation()
    {
        yield return light.TweenTextureScale(2.5f, Seconds, Cycle);
        yield return light.TweenEnergy(2, Seconds, Cycle);
        yield return light.TweenPositionX(100, Seconds, Cycle);
    }
}
