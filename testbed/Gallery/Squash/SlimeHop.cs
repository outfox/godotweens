// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Linq;
using Godot;
using tweens.gd;
namespace testbed;

public sealed partial class SlimeHop : GalleryEffect
{

    private Node2D slime = null!, body = null!, eyes = null!, pupils = null!;
    private Polygon2D[] drops = [];
    private int direction = 1, hops;

    public override string Title => "Slime Hop";
    public override string Caption => "Anticipation, launch stretch, flips, elastic landing, blinking eyes.";

    protected override void Build()
    {
        var view = View();
        Line(view, [new(-210, Ground), new(210, Ground)], Palette.Outline, 2);
        drops = Enumerable.Range(0, 5).Select(_ => Blob(view, 4, 4, Palette.Mint)).ToArray();
        foreach (var drop in drops) drop.Modulate = Colors.Transparent;

        slime = view.Add(new Node2D { Position = new Vector2(-135, Ground) });
        body = slime.Add(new Node2D { Position = new Vector2(0, -20) });
        body.Add(new Polygon2D { Polygon = Dome(), Color = Palette.Mint, Antialiased = true });
        Blob(body, 8, 4, new Color(1, 1, 1, 0.5f), new Vector2(-17, -24)).Rotation = -0.5f;

        eyes = body.Add(new Node2D { Position = new Vector2(0, -8) });
        foreach (var x in new[] { -11f, 11f }) Blob(eyes, 6, 7.5f, Colors.White, new Vector2(x, 0));
        pupils = eyes.Add(new Node2D());
        foreach (var x in new[] { -11f, 11f }) Blob(pupils, 3.2f, 4, Palette.Background, new Vector2(x + 1, 1));
    }

    /// <summary>Round on top, nearly flat underneath, centered on the body's pivot.</summary>
    private static Vector2[] Dome() => Enumerable.Range(0, 40)
        .Select(i => i * MathF.Tau / 40)
        .Select(a => new Vector2(MathF.Cos(a) * 36, MathF.Sin(a) * (MathF.Sin(a) > 0 ? 20 : 38)))
        .ToArray();

    protected override void Animate() => Sequence = Run(AnimateAsync());
}
