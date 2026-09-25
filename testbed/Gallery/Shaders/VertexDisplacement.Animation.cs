// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in VertexDisplacement.cs.
public sealed partial class VertexDisplacement
{
    private IEnumerable<TweenInstance> CreateAnimation() =>
    [
        deformed.TweenInstanceShaderParameter("amplitude", 0.22f, Seconds, Cycle),
    ];
}
