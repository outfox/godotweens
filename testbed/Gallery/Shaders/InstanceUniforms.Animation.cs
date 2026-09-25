// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in InstanceUniforms.cs.
public sealed partial class InstanceUniforms
{
    private IEnumerable<TweenInstance> CreateAnimation()
    {
        yield return first.TweenInstanceShaderParameter("amount", 0.85f, Seconds, Cycle);
        yield return second.TweenInstanceShaderParameter("amount", 0.15f, Seconds, Cycle);
    }
}
