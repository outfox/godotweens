// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Linq;
using Godot;
using tweens.gd;
namespace testbed;

public sealed partial class LightSweep : GalleryEffect
{
    private const float Cell = 32, Tile = 27;
    private static readonly Color Warm = new("fff0cd");
    private PointLight2D light = null!;

    public override string Title => "PointLight2D";
    public override string Caption => "TextureScale, energy, and position.";

    protected override void Build()
    {
        var view = View();
        view.Add(new CanvasModulate { Color = new Color("3a4a64") });

        var tile = Rounded(Tile / 2, Tile / 2, 5).Select(p => p + new Vector2(Tile / 2, Tile / 2)).ToArray();
        for (var y = -3; y < 3; y++)
            for (var x = -7; x < 7; x++)
                view.Add(new Polygon2D
                {
                    Polygon = tile, Antialiased = true, Position = new Vector2(x * Cell, y * Cell),
                    Color = (x + y) % 2 == 0 ? Palette.Mint : Palette.Blue,
                });

        light = view.Add(new PointLight2D
        {
            Texture = RadialFalloff(128), Energy = 0.7f, TextureScale = 1.2f, Position = new Vector2(-100, 0), Color = Warm,
        });
        Blob(light, 6, 6, Warm);
        Blob(light, 3, 3, Colors.White);
    }

    protected override void Animate() => TrackTweens(CreateAnimation());
}
