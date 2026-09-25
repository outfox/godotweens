// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in ParentedRotation.cs.
public sealed partial class ParentedRotation
{
    private IEnumerable<TweenInstance> CreateAnimation()
    {
        var orientation = Quaternion.FromEuler(new Vector3(0.5f, 2.5f, 0.8f));
        yield return cube.TweenGlobalQuaternion(orientation, Seconds, Cycle);
        yield return cube.TweenScale(new Vector3(1.4f, 0.7f, 1.1f), Seconds, Cycle);
    }
}
