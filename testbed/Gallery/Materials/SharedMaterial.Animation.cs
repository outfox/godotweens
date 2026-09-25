// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in SharedMaterial.cs.
public sealed partial class SharedMaterial
{
    private IEnumerable<TweenInstance> CreateAnimation()
    {
        yield return material.TweenAlbedoColor(Palette.Amber, Seconds, Stage, Cycle);
        yield return material.TweenRoughness(0.95f, Seconds, Stage, Cycle);
    }
}
