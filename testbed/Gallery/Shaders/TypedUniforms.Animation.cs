// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in TypedUniforms.cs.
public sealed partial class TypedUniforms
{
    private IEnumerable<TweenInstance> CreateAnimation()
    {
        yield return material.TweenShaderParameter("tint", Palette.Amber, Seconds, Stage, Cycle);
        yield return material.TweenShaderParameter("offset", new Vector2(0.25f, 0.33f), Seconds, Stage, Cycle);
    }
}
