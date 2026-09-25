// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in Spirograph.cs.
public sealed partial class Spirograph
{
    private const int TrailLength = 420;

    private async Task AnimateAsync()
    {
        var pulse = 0.6 * Tempo;
        var revolution = 7 * Tempo;
        await Group.Of([
            sun.TweenScale(new Vector2(1.35f, 1.35f), pulse, options =>
            {
                options.Ease = EaseType.SineInOut;
                options.UsePingPong = true;
                options.Repeats = TweenOptions.Infinite;
            }),
            Stage.TweenFloat(1, revolution, options =>
            {
                options.From = 0;
                options.Repeats = TweenOptions.Infinite;
                options.OnUpdate = (_, progress) => Draw(progress);
            }),
        ]).End;
    }

    private void Draw(float progress)
    {
        var a = progress * MathF.Tau;
        // Integer frequency ratios keep both curves closed, so the loop wraps seamlessly.
        var innerJoint = Polar(a * 2, 58);
        var innerTip = innerJoint + Polar(-a * 8, 26);
        var outerJoint = Polar(-a, 76);
        var outerTip = outerJoint + Polar(a * 11, 12);

        innerArm.Points = [Vector2.Zero, innerJoint, innerTip];
        outerArm.Points = [Vector2.Zero, outerJoint, outerTip];
        innerPen.Position = innerTip;
        outerPen.Position = outerTip;
        Extend(innerTrail, innerTip);
        Extend(outerTrail, outerTip);
    }

    private static Vector2 Polar(float angle, float radius) => new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * radius;

    private static void Extend(Line2D trail, Vector2 tip)
    {
        trail.AddPoint(tip);
        while (trail.GetPointCount() > TrailLength) trail.RemovePoint(0);
    }
}
