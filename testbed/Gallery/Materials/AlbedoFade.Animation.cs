// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in AlbedoFade.cs.
public sealed partial class AlbedoFade
{
    private IEnumerable<TweenInstance> CreateAnimation() =>
    [
        material.TweenAlbedoAlpha(0.08f, Seconds, Stage, Cycle),
    ];
}
