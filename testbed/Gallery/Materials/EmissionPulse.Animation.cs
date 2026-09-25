// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in EmissionPulse.cs.
public sealed partial class EmissionPulse
{
    private IEnumerable<TweenInstance> CreateAnimation()
    {
        yield return material.TweenEmission(new Color(0.12f, 0.08f, 0.3f), Seconds, Stage, Cycle);
        yield return material.TweenEmissionEnergyMultiplier(2, Seconds, Stage, Cycle);
    }
}
