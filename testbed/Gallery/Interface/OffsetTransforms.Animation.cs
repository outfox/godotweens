// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in OffsetTransforms.cs.
public sealed partial class OffsetTransforms
{
    private IEnumerable<TweenInstance> CreateAnimation()
    {
        yield return featured.TweenOffsetTransformPosition(new Vector2(0, -22), Seconds, Cycle);
        yield return featured.TweenOffsetTransformRotation(0.18f, Seconds, Cycle);
        yield return featured.TweenOffsetTransformScale(new Vector2(1.13f, 1.13f), Seconds, Cycle);
    }
}
