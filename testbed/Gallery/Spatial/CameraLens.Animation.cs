// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in CameraLens.cs.
public sealed partial class CameraLens
{
    private IEnumerable<TweenInstance> CreateAnimation()
    {
        yield return camera.TweenFov(65, Seconds, Cycle);
        yield return camera.TweenHOffset(0.7f, Seconds, Cycle);
    }
}
