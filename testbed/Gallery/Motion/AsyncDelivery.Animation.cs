// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in AsyncDelivery.cs.
public sealed partial class AsyncDelivery
{
    private const float Left = -150, Right = 150, Rail = 15;

    private TweenInstance Outward() => courier.TweenPositionX(Right, Seconds, t => t.Ease = Ease);

    // Both tweens start together after the outward leg completes (see Deliver in AsyncDelivery.cs).
    private TweenInstance[] ReturnAndTurn() =>
    [
        courier.TweenPositionX(Left, Seconds, t => t.Ease = Ease),
        courier.TweenRotation(Mathf.Tau, Seconds, t => t.Ease = Ease),
    ];

    /// <summary>Shows the step's label and lights its progress dot, and every dot before it.</summary>
    private IEnumerable<TweenInstance> Report(int step, string text)
    {
        status.Text = text;
        for (var i = 0; i < steps.Length; i++)
            yield return steps[i].TweenColor(i <= step ? Palette.Mint : Palette.Outline, 0.2);
        yield return steps[step].TweenScale(Vector2.One, 0.5, t => { t.From = new Vector2(2, 2); t.Ease = EaseType.ElasticOut; });
    }
}
