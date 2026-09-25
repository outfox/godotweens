// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd;
namespace testbed;

public sealed partial class TextReveal : GalleryEffect
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

        underline = Stage.Add(new ColorRect
        {
            Color = Palette.Mint, Position = new Vector2(20, 142), Size = new Vector2(230, 3), Scale = new Vector2(0, 1),
        });
    }

    protected override void Animate() => Sequence = Run(AnimateAsync());
}
