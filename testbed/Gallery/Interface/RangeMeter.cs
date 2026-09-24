// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using godotweens;
namespace testbed;

public sealed class RangeMeter : GalleryEffect
{
    private static readonly Color Track = new("293c50");
    private ProgressBar progress = null!;
    private ColorRect swatch = null!;

    public override string Title => "Range Value";
    public override string Caption => "ProgressBar.Value with color and alpha tweens.";

    protected override void Build()
    {
        var gauge = Fill(new VBoxContainer { Alignment = BoxContainer.AlignmentMode.Center }, 16);
        gauge.AddThemeConstantOverride("separation", 8);

        var header = new HBoxContainer();
        gauge.AddChild(header);
        var caption = GalleryTheme.Label("VALUE", 13, Palette.Muted);
        caption.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        caption.SizeFlagsVertical = Control.SizeFlags.ShrinkEnd;
        header.AddChild(caption);
        var readout = GalleryTheme.Label("0%", 28);
        header.AddChild(readout);

        progress = new ProgressBar { CustomMinimumSize = new Vector2(0, 30), Step = 0, ShowPercentage = false };
        progress.AddThemeStyleboxOverride("background", Own(GalleryTheme.Box(Track, 6)));
        var fill = Own(GalleryTheme.Box(Palette.Mint, 6));
        fill.ShadowColor = Palette.Mint with { A = 0.35f };
        fill.ShadowSize = 10;
        progress.AddThemeStyleboxOverride("fill", fill);
        progress.ValueChanged += value => readout.Text = $"{value:0}%";
        gauge.AddChild(progress);

        var ticks = new HBoxContainer();
        gauge.AddChild(ticks);
        (string Mark, HorizontalAlignment Alignment)[] scale =
            [("0", HorizontalAlignment.Left), ("50", HorizontalAlignment.Center), ("100", HorizontalAlignment.Right)];
        foreach (var (mark, alignment) in scale)
        {
            var tick = GalleryTheme.Label(mark, 11, Palette.Muted);
            tick.HorizontalAlignment = alignment;
            tick.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            ticks.AddChild(tick);
        }

        gauge.AddChild(new Control { CustomMinimumSize = new Vector2(0, 4) });
        var sample = new HBoxContainer();
        sample.AddThemeConstantOverride("separation", 12);
        gauge.AddChild(sample);
        var label = GalleryTheme.Label("COLOR", 13, Palette.Muted);
        label.CustomMinimumSize = new Vector2(52, 0);
        sample.AddChild(label);
        swatch = new ColorRect
        {
            Color = Palette.Mint, CustomMinimumSize = new Vector2(0, 24), SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
        };
        sample.AddChild(swatch);
    }

    protected override void Animate()
    {
        Keep(progress.TweenValue(100, Seconds, Cycle));
        Keep(swatch.TweenColor(Palette.Blue, Seconds, Cycle));
        Keep(swatch.TweenSelfModulateAlpha(0.25f, Seconds, Cycle));
    }
}
