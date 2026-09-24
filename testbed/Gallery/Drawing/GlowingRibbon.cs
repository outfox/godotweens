// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using godotweens;
namespace testbed;

public sealed class GlowingRibbon : GalleryEffect
{
    private Line2D ribbon = null!, glow = null!;

    public override string Title => "Line2D";
    public override string Caption => "Width and DefaultColor on a fixed path, with a matching glow pass.";

    protected override void Build()
    {
        var view = View();
        Vector2[] zigzag = [new(-170, 35), new(-100, -40), new(-25, 25), new(55, -45), new(160, 30)];
        glow = WithRoundCaps(Line(view, zigzag, Palette.Mint with { A = 0.28f }, 12));
        ribbon = WithRoundCaps(Line(view, zigzag, Palette.Mint, 3));
        foreach (var point in zigzag)
        {
            Blob(view, 4, 4, Palette.Background, point);
            Blob(view, 2.5f, 2.5f, Colors.White, point);
        }
    }

    private static Line2D WithRoundCaps(Line2D line)
    {
        line.BeginCapMode = Line2D.LineCapMode.Round;
        line.EndCapMode = Line2D.LineCapMode.Round;
        line.JointMode = Line2D.LineJointMode.Round;
        return line;
    }

    protected override void Animate()
    {
        Keep(ribbon.TweenWidth(16, Seconds, Cycle));
        Keep(ribbon.TweenDefaultColor(Palette.Blue, Seconds, Cycle));
        Keep(glow.TweenWidth(40, Seconds, Cycle));
        Keep(glow.TweenDefaultColor(Palette.Blue with { A = 0.28f }, Seconds, Cycle));
    }
}
