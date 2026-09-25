// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using Godot;
using tweens.gd;
namespace testbed;

public sealed partial class Spirograph : GalleryEffect
{
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

    protected override void Animate() => TrackTweens(CreateAnimation());
}
