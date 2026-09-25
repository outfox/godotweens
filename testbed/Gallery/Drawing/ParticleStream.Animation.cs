// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in ParticleStream.cs.
public sealed partial class ParticleStream
{
    private IEnumerable<TweenInstance> CreateAnimation()
    {
        yield return particles.TweenSpread(75, Seconds, Cycle);
        yield return particles.TweenGravity(new Vector2(15, -55), Seconds, Cycle);
        yield return particles.TweenColor(Palette.Amber, Seconds, Cycle);
        yield return particles.TweenPositionY(-30, Seconds, Cycle);
    }
}
