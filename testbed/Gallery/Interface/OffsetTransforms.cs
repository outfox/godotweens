// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using godotweens;
namespace testbed;

public sealed class OffsetTransforms : GalleryEffect
{
    private static readonly Vector2 TileSize = new(88, 82);
    private Control featured = null!;

    public override string Title => "Offset Transforms";
    public override string Caption => "Position, rotation, and scale within an HBoxContainer.";

    protected override void Build()
    {
        var row = Fill(new CenterContainer(), 0).Add(new HBoxContainer());
        row.AddThemeConstantOverride("separation", 18);

        row.AddChild(Tile(1, highlighted: false));
        featured = row.Add(Tile(2, highlighted: true));
        featured.OffsetTransformPivot = TileSize / 2;
        row.AddChild(Tile(3, highlighted: false));
    }

    private PanelContainer Tile(int number, bool highlighted)
    {
        var tile = new PanelContainer { CustomMinimumSize = TileSize, OffsetTransformEnabled = true };
        var style = Own(GalleryTheme.Box(highlighted ? new Color("2f5560") : new Color("293c50"), 10, highlighted ? 2 : 0, Palette.Mint));
        style.ShadowColor = highlighted ? Palette.Mint with { A = 0.25f } : new Color(0, 0, 0, 0.35f);
        style.ShadowSize = highlighted ? 14 : 6;
        style.ShadowOffset = highlighted ? Vector2.Zero : new Vector2(0, 4);
        tile.AddThemeStyleboxOverride("panel", style);

        var label = tile.Add(GalleryTheme.Label($"0{number}", 24, highlighted ? Palette.Mint : Palette.Muted));
        label.HorizontalAlignment = HorizontalAlignment.Center;
        return tile;
    }

    protected override void Animate()
    {
        Keep(featured.TweenOffsetTransformPosition(new Vector2(0, -22), Seconds, Cycle));
        Keep(featured.TweenOffsetTransformRotation(0.18f, Seconds, Cycle));
        Keep(featured.TweenOffsetTransformScale(new Vector2(1.13f, 1.13f), Seconds, Cycle));
    }
}
