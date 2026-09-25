// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in SharedUniform.cs.
public sealed partial class SharedUniform
{
    private IEnumerable<TweenInstance> CreateAnimation() =>
    [
        material.TweenShaderParameter("amount", 0.85f, Seconds, Stage, Cycle),
    ];
}
