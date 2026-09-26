// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
namespace testbed;

public sealed partial class GlowingRibbon : GalleryEffect
{
    private Line2D ribbon = null!, glow = null!;

    public override string Title => "Line2D";
    public override string Caption => "Width and DefaultColor on a fixed path, with a matching glow pass.";

    protected override void Build()
    {
        var view = View();
        Vector2[] zigzag = [new(-170, 35), new(-100, -40), new(-25, 25), new(55, -45), new(160, 30)];
        glow = view.Add(RoundLine(zigzag, Palette.Mint with { A = 0.28f }, 12));
        ribbon = view.Add(RoundLine(zigzag, Palette.Mint, 3));
        foreach (var point in zigzag)
        {
            Blob(view, 4, 4, Palette.Background, point);
            Blob(view, 2.5f, 2.5f, Colors.White, point);
        }
    }

    protected override void Animate() => Sequence = Run(AnimateAsync());

    private static Line2D RoundLine(Vector2[] points, Color color, float width) => new()
    {
        Points = points, DefaultColor = color, Width = width, Antialiased = true,
        BeginCapMode = Line2D.LineCapMode.Round, EndCapMode = Line2D.LineCapMode.Round,
        JointMode = Line2D.LineJointMode.Round,
    };
}
