// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Linq;
using Godot;
using tweens.gd;
namespace testbed;

public sealed partial class SquashWave : GalleryEffect
{
    private const int Count = 15;
    private const float Ground = 58, Spacing = 26;
    private Polygon2D[] pills = [];

    public override string Title => "Squash Wave";
    public override string Caption => "Per-item Delay turns one looping tween into a wave. Uses Easing.";

    protected override void Build()
    {
        var view = View();
        var outline = Pill();
        pills = Enumerable.Range(0, Count).Select(i => view.Add(new Polygon2D
        {
            Polygon = outline, Color = Gradient(i / (Count - 1f)), Antialiased = true,
            Position = new Vector2((i - Count / 2) * Spacing, Ground),
        })).ToArray();
        Line(view, [new(-210, Ground + 1), new(210, Ground + 1)], Palette.Outline, 2);
    }

    /// <summary>Mint through blue to amber.</summary>
    private static Color Gradient(float t)
        => t < 0.5f ? Palette.Mint.Lerp(Palette.Blue, t * 2) : Palette.Blue.Lerp(Palette.Amber, t * 2 - 1);

    /// <summary>Rounded pill anchored at its base, so ScaleY grows upward from the floor.</summary>
    private static Vector2[] Pill()
    {
        var bottom = Ellipse(7, 7, 12, new Vector2(0, -7)).Where(p => p.Y >= -7);
        var top = Ellipse(7, 7, 12, new Vector2(0, -29)).Where(p => p.Y <= -29);
        return bottom.Concat(top).OrderBy(p => MathF.Atan2(p.Y + 18, p.X)).ToArray();
    }

    protected override void Animate() => TrackTweens(CreateAnimation());
}
