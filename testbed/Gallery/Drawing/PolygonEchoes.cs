// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Linq;
using Godot;
using godotweens;
namespace testbed;

public sealed class PolygonEchoes : GalleryEffect
{
    private static readonly Vector2[] Outline = [new(-45, -40), new(35, -50), new(65, 15), new(0, 50), new(-60, 15)];
    private Polygon2D polygon = null!;

    /// <summary>Faintest first, so each echo draws beneath the ones closer to the polygon.</summary>
    private Polygon2D[] echoes = [];

    public override string Title => "Polygon2D";
    public override string Caption => "Color, offset, and rotation. Echoes replay the tweens with a Delay.";

    protected override void Build()
    {
        var view = View();
        echoes = Enumerable.Range(1, 3).Reverse().Select(e => view.Add(new Polygon2D
        {
            Polygon = Outline, Color = Palette.Mint, Modulate = Colors.White with { A = 0.32f - e * 0.08f }, Antialiased = true,
        })).ToArray();

        polygon = view.Add(new Polygon2D { Polygon = Outline, Color = Palette.Mint, Antialiased = true });
        polygon.Add(new Line2D
        {
            Points = Outline, Closed = true, Width = 2, DefaultColor = Colors.White with { A = 0.5f },
            JointMode = Line2D.LineJointMode.Round, Antialiased = true,
        });
    }

    protected override void Animate()
    {
        for (var e = 0; e < echoes.Length; e++)
        {
            var lag = echoes.Length - e;
            Play(echoes[e], CycleAfter(lag * 0.08));
        }
        Play(polygon, Cycle);
    }

    private void Play(Polygon2D target, Action<TweenOptions> timing)
    {
        Keep(target.TweenColor(Palette.Amber, Seconds, timing));
        Keep(target.TweenOffset(new Vector2(40, 0), Seconds, timing));
        Keep(target.TweenRotation(Mathf.Pi, Seconds * 2, timing));
    }
}
