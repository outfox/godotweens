// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using godotweens;
namespace testbed;

public sealed class CombinedTransforms : GalleryEffect
{
    private Polygon2D shape = null!, shadow = null!;

    public override string Title => "Combined Transforms";
    public override string Caption => "Skew, rotation, and independent X/Y scale. Outline marks the rest pose.";

    protected override void Build()
    {
        var view = View();
        var rest = Line(view, [new(0, -34), new(34, 0), new(0, 34), new(-34, 0)], new Color("3a4f69"), 1.5f);
        rest.Closed = true;
        shadow = Diamond(view, new Vector2(8, 10), new Color(0, 0, 0, 0.4f), 34);
        shape = Diamond(view, Vector2.Zero, Palette.Blue, 34);
        Diamond(shape, Vector2.Zero, new Color("aec3ff"), 20);
        Line(shape, [Vector2.Zero, new(42, 0)], Palette.Amber, 3);
        Blob(shape, 4, 4, Palette.Amber);
    }

    protected override void Animate()
    {
        foreach (var target in new[] { shape, shadow })
        {
            Keep(target.TweenSkew(0.5f, Seconds, Cycle));
            Keep(target.TweenRotation(Mathf.Pi, Seconds, Cycle));
            Keep(target.TweenScaleX(1.8f, Seconds, Cycle));
            Keep(target.TweenScaleY(0.6f, Seconds, Cycle));
        }
    }
}
