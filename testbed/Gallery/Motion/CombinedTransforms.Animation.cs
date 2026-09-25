// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in CombinedTransforms.cs.
public sealed partial class CombinedTransforms
{
    private IEnumerable<TweenInstance> CreateAnimation()
    {
        foreach (var target in new[] { shape, shadow })
        {
            yield return target.TweenSkew(0.5f, Seconds, Cycle);
            yield return target.TweenRotation(Mathf.Pi, Seconds, Cycle);
            yield return target.TweenScaleX(1.8f, Seconds, Cycle);
            yield return target.TweenScaleY(0.6f, Seconds, Cycle);
        }
    }
}
