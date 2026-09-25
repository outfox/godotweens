// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in CurveFollower3D.cs.
public sealed partial class CurveFollower3D
{
    private IEnumerable<TweenInstance> CreateAnimation()
    {
        var lap = Seconds * 1.5;
        yield return leader.TweenProgressRatio(1, lap, Cycle);
        for (var e = 0; e < echoes.Length; e++)
        {
            var trailing = CycleAfter((e + 1) * 0.08);
            yield return echoes[e].TweenProgressRatio(1, lap, trailing);
            yield return echoes[e].TweenVOffset(0.35f, Seconds, trailing);
        }
        yield return leader.TweenVOffset(0.35f, Seconds, Cycle);
    }
}
