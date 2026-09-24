// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using godotweens;
namespace testbed;

public sealed class TextReveal : GalleryEffect
{
    private Label text = null!;
    private ColorRect underline = null!;

    public override string Title => "Visible Ratio";
    public override string Caption => "Label.VisibleRatio and SelfModulate.";

    protected override void Build()
    {
        text = GalleryTheme.Label("VisibleRatio\nreveals text\nover time.", 30, Colors.White);
        text.SelfModulate = Palette.Mint;
        text.Position = new Vector2(18, 8);
        text.VisibleRatio = 0;
        Stage.AddChild(text);

        underline = new ColorRect
        {
            Color = Palette.Mint, Position = new Vector2(20, 142), Size = new Vector2(230, 3), Scale = new Vector2(0, 1),
        };
        Stage.AddChild(underline);
    }

    protected override void Animate()
    {
        Keep(text.TweenVisibleRatio(1, Seconds, Cycle));
        Keep(text.TweenSelfModulate(Palette.Amber, Seconds, Cycle));
        Keep(underline.TweenScaleX(1, Seconds, Cycle));
        Keep(underline.TweenColor(Palette.Amber, Seconds, Cycle));
    }
}
