// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using Godot;
using tweens.gd;
namespace testbed;

public sealed class Spirograph : GalleryEffect
{
    private const int TrailLength = 420;
    private static readonly Color Arm = new("56718f"), Guide = new("1f2d3f");

    private Line2D innerTrail = null!, outerTrail = null!, innerArm = null!, outerArm = null!;
    private Polygon2D innerPen = null!, outerPen = null!, sun = null!;

    public override string Title => "Spirograph";
    public override string Caption => "One looping TweenFloat drives both epicycles through OnUpdate.";

    protected override void Build()
    {
        var view = View();
        Ring(view, Vector2.Zero, 40, Guide, 2, 64);
        Ring(view, Vector2.Zero, 86, Guide, 2, 64);

        innerTrail = Trail(view, Palette.Mint);
        outerTrail = Trail(view, Palette.Amber);
        innerArm = Line(view, [], Arm, 2.5f);
        outerArm = Line(view, [], Arm, 2.5f);
        sun = Blob(view, 9, 9, Palette.Blue);
        innerPen = Blob(view, 5, 5, Palette.Mint);
        outerPen = Blob(view, 5, 5, Palette.Amber);
    }

    /// <summary>A line that fades out towards its oldest point.</summary>
    private Line2D Trail(Node parent, Color color) => parent.Add(new Line2D
    {
        Width = 5, JointMode = Line2D.LineJointMode.Round, Antialiased = true,
        Gradient = Own(new Gradient { Colors = [color with { A = 0 }, color], Offsets = [0, 1] }),
    });

    protected override void Animate()
    {
        var pulse = 0.6 * Tempo;
        Keep(sun.TweenScale(new Vector2(1.35f, 1.35f), pulse, t =>
        {
            t.Ease = EaseType.SineInOut;
            t.UsePingPong = true;
            t.IsInfinite = true;
        }));

        var revolution = 7 * Tempo;
        Keep(Stage.TweenFloat(1, revolution, t =>
        {
            t.From = 0;
            t.IsInfinite = true;
            t.OnUpdate = (_, progress) => Draw(progress);
        }));
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
