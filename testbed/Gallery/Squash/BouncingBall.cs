// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Linq;
using Godot;
using tweens.gd;
namespace testbed;

public sealed partial class BouncingBall : GalleryEffect
{

    private Node2D ball = null!, spin = null!;
    private Polygon2D shadow = null!;
    private Line2D ring = null!;
    private Polygon2D[] dust = [];
    private int direction = 1;

    public override string Title => "Bouncing Ball";
    public override string Caption => "Stretch in flight, squash on impact, shadow and dust per landing.";

    protected override void Build()
    {
        var view = View();
        var start = new Vector2(-150, Ground);
        Line(view, [new(-210, Ground), new(210, Ground)], Palette.Outline, 2);
        shadow = Blob(view, 22, 5, new Color(0, 0, 0, 0.45f), start + new Vector2(0, 2));

        ring = view.Add(new Line2D
        {
            Points = Ellipse(24, 6, 40), Closed = true, DefaultColor = Palette.Amber, Width = 3, Antialiased = true,
            Position = start, Modulate = Colors.Transparent,
        });

        dust = Enumerable.Range(0, 6).Select(_ => Blob(view, 3.5f, 3.5f, Palette.Soft)).ToArray();
        foreach (var mote in dust) mote.Modulate = Colors.Transparent;

        ball = view.Add(new Node2D { Position = start });
        spin = ball.Add(new Node2D { Position = new Vector2(0, -20) });
        Blob(spin, 20, 20, Palette.Amber);
        Vector2[] stripe = [new(-19.5f, -4), new(19.5f, -4), new(19.5f, 4), new(-19.5f, 4)];
        spin.Add(new Polygon2D { Polygon = stripe, Color = new Color("d8893a") });
        Blob(ball, 5, 3.5f, new Color(1, 1, 1, 0.55f), new Vector2(-8, -30)).Rotation = -0.6f;
    }

    protected override void Animate() => Sequence = Repeat(Bounce);
}
