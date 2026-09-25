// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in UvScroll.cs.
public sealed partial class UvScroll
{
    private IEnumerable<TweenInstance> CreateAnimation()
    {
        var slow = Seconds * 2;
        yield return material.TweenUv1OffsetX(1, slow, Stage, Cycle);
        yield return material.TweenUv1Scale(new Vector3(2.5f, 2.5f, 1), slow, Stage, Cycle);
    }
}
