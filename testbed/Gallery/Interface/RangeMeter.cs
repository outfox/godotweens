// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd;
namespace testbed;

public sealed partial class RangeMeter : GalleryEffect
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

        var header = gauge.Add(new HBoxContainer());
        var caption = GalleryTheme.Label("VALUE", 13, Palette.Muted);
        caption.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        caption.SizeFlagsVertical = Control.SizeFlags.ShrinkEnd;
        header.AddChild(caption);
        var readout = header.Add(GalleryTheme.Label("0%", 28));

        progress = gauge.Add(new ProgressBar { CustomMinimumSize = new Vector2(0, 30), Step = 0, ShowPercentage = false });
        progress.AddThemeStyleboxOverride("background", Own(GalleryTheme.Box(Track, 6)));
        var fill = Own(GalleryTheme.Box(Palette.Mint, 6));
        fill.ShadowColor = Palette.Mint with { A = 0.35f };
        fill.ShadowSize = 10;
        progress.AddThemeStyleboxOverride("fill", fill);
        progress.ValueChanged += value => readout.Text = $"{value:0}%";

        var ticks = gauge.Add(new HBoxContainer());
        (string Mark, HorizontalAlignment Alignment)[] scale =
            [("0", HorizontalAlignment.Left), ("50", HorizontalAlignment.Center), ("100", HorizontalAlignment.Right)];
        foreach (var (mark, alignment) in scale)
        {
            var tick = GalleryTheme.Label(mark, 11, Palette.Muted);
            tick.HorizontalAlignment = alignment;
            tick.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            ticks.AddChild(tick);
        }

        gauge.Add(new Control { CustomMinimumSize = new Vector2(0, 4) });
        var sample = gauge.Add(new HBoxContainer());
        sample.AddThemeConstantOverride("separation", 12);
        var label = GalleryTheme.Label("COLOR", 13, Palette.Muted);
        label.CustomMinimumSize = new Vector2(52, 0);
        sample.AddChild(label);
        swatch = sample.Add(new ColorRect
        {
            Color = Palette.Mint, CustomMinimumSize = new Vector2(0, 24), SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
        });
    }

    protected override void Animate() => Sequence = Run(AnimateAsync());
}
