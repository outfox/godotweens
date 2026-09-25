// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in CameraPan.cs.
public sealed partial class CameraPan
{
    private IEnumerable<TweenInstance> CreateAnimation()
    {
        yield return camera.TweenZoom(new Vector2(1.8f, 1.8f), Seconds, Cycle);
        yield return camera.TweenOffset(new Vector2(90, 25), Seconds, Cycle);

        const double pulse = 1.2;
        yield return beacon.TweenScale(new Vector2(2.4f, 2.4f), pulse, t =>
        {
            t.From = Vector2.One;
            t.Ease = EaseType.QuartOut;
            t.IsInfinite = true;
        });
        yield return beacon.TweenModulateAlpha(0, pulse, t =>
        {
            t.From = 1;
            t.Ease = EaseType.QuadIn;
            t.IsInfinite = true;
        });
    }
}
