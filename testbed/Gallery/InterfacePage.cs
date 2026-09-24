// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using godotweens;
namespace testbed;

public partial class InterfacePage : GalleryPage
{
    public override string Heading => "Interface";
    public override string Description => "Text, range values, layout transforms, and scrolling.";
    private Label reveal = null!;
    private ProgressBar progress = null!;
    private ColorRect swatch = null!, underline = null!;
    private Control tile = null!;
    private ScrollContainer scroll = null!;
    protected override void Build()
    {
        var words = Card("01 / Visible Ratio", "Label.VisibleRatio and SelfModulate.");
        reveal = Text("VisibleRatio\nreveals text\nover time.", 30, Colors.White);
        reveal.SelfModulate = Mint;
        reveal.Position = new Vector2(18, 8); reveal.VisibleRatio = 0; words.AddChild(reveal);
        underline = new ColorRect { Color = Mint, Position = new Vector2(20, 142), Size = new Vector2(230, 3), Scale = new Vector2(0, 1) };
        words.AddChild(underline);

        var meters = Card("02 / Range Value", "ProgressBar.Value with color and alpha tweens.");
        var gauge = Fill(meters, new VBoxContainer { Alignment = BoxContainer.AlignmentMode.Center }, 16);
        gauge.AddThemeConstantOverride("separation", 8);
        var header = new HBoxContainer(); gauge.AddChild(header);
        var caption = Text("VALUE", 13, Muted); caption.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        caption.SizeFlagsVertical = SizeFlags.ShrinkEnd; header.AddChild(caption);
        var readout = Text("0%", 28); header.AddChild(readout);
        progress = new ProgressBar { CustomMinimumSize = new Vector2(0, 30), Step = 0, ShowPercentage = false };
        progress.AddThemeStyleboxOverride("background", Own(Box(new Color("293c50"), 6)));
        var fill = Own(Box(Mint, 6)); fill.ShadowColor = Mint with { A = 0.35f }; fill.ShadowSize = 10;
        progress.AddThemeStyleboxOverride("fill", fill); gauge.AddChild(progress);
        var ticks = new HBoxContainer(); gauge.AddChild(ticks);
        foreach (var (mark, align) in new[] { ("0", HorizontalAlignment.Left), ("50", HorizontalAlignment.Center), ("100", HorizontalAlignment.Right) })
        {
            var tick = Text(mark, 11, Muted); tick.HorizontalAlignment = align; tick.SizeFlagsHorizontal = SizeFlags.ExpandFill; ticks.AddChild(tick);
        }
        gauge.AddChild(new Control { CustomMinimumSize = new Vector2(0, 4) });
        var sample = new HBoxContainer(); sample.AddThemeConstantOverride("separation", 12); gauge.AddChild(sample);
        var label = Text("COLOR", 13, Muted); label.CustomMinimumSize = new Vector2(52, 0); sample.AddChild(label);
        swatch = new ColorRect { Color = Mint, CustomMinimumSize = new Vector2(0, 24), SizeFlagsHorizontal = SizeFlags.ExpandFill };
        sample.AddChild(swatch);
        progress.ValueChanged += v => readout.Text = $"{v:0}%";

        var layout = Card("03 / Offset Transforms", "Position, rotation, and scale within an HBoxContainer.");
        var row = new HBoxContainer(); row.AddThemeConstantOverride("separation", 18);
        Fill(layout, new CenterContainer(), 0).AddChild(row);
        for (var i = 0; i < 3; i++)
        {
            var block = new PanelContainer { CustomMinimumSize = new Vector2(88, 82), OffsetTransformEnabled = true };
            var style = Own(GalleryTheme.Box(i == 1 ? new Color("2f5560") : new Color("293c50"), 10, i == 1 ? 2 : 0, Mint));
            style.ShadowColor = i == 1 ? Mint with { A = 0.25f } : new Color(0, 0, 0, 0.35f);
            style.ShadowSize = i == 1 ? 14 : 6; style.ShadowOffset = i == 1 ? Vector2.Zero : new Vector2(0, 4);
            block.AddThemeStyleboxOverride("panel", style);
            var number = Text($"0{i + 1}", 24, i == 1 ? Mint : Muted); number.HorizontalAlignment = HorizontalAlignment.Center;
            block.AddChild(number); row.AddChild(block);
            if (i == 1) { tile = block; tile.OffsetTransformPivot = new Vector2(44, 41); }
        }
        var browse = Card("04 / Scroll Position", "ScrollContainer.ScrollVertical uses integer interpolation.");
        scroll = Fill(browse, new ScrollContainer { HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
            MouseFilter = MouseFilterEnum.Ignore }, 8);
        var list = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill }; list.AddThemeConstantOverride("separation", 10); scroll.AddChild(list);
        Color[] accents = [Mint, Blue, Amber];
        string[] states = ["ready", "queued", "paused"];
        for (var i = 0; i < 8; i++)
        {
            var entry = new PanelContainer { CustomMinimumSize = new Vector2(0, 48) };
            entry.AddThemeStyleboxOverride("panel", Own(GalleryTheme.Box(new Color("293c50"), 7, 0, null, 12, 8)));
            var line = new HBoxContainer(); line.AddThemeConstantOverride("separation", 12); entry.AddChild(line);
            line.AddChild(new ColorRect { Color = accents[i % 3], CustomMinimumSize = new Vector2(4, 26), SizeFlagsVertical = SizeFlags.ShrinkCenter });
            line.AddChild(Text($"0{i + 1}", 16, Muted));
            var name = Text($"Item {(char)('A' + i)}", 16); name.SizeFlagsHorizontal = SizeFlags.ExpandFill; line.AddChild(name);
            line.AddChild(Text(states[i % 3], 13, accents[i % 3]));
            list.AddChild(entry);
        }
    }
    /// <summary>Adds <paramref name="child"/> stretched over <paramref name="stage"/>, inset by <paramref name="inset"/>.</summary>
    private static T Fill<T>(Control stage, T child, int inset) where T : Control
    {
        stage.AddChild(child); child.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect, LayoutPresetMode.KeepSize, inset);
        return child;
    }
    protected override void Animate()
    {
        Keep(reveal.TweenVisibleRatio(1, Seconds, Cycle));
        Keep(reveal.TweenSelfModulate(Amber, Seconds, Cycle));
        Keep(underline.TweenScaleX(1, Seconds, Cycle));
        Keep(underline.TweenColor(Amber, Seconds, Cycle));
        Keep(progress.TweenValue(100, Seconds, Cycle));
        Keep(swatch.TweenColor(Blue, Seconds, Cycle));
        Keep(swatch.TweenSelfModulateAlpha(0.25f, Seconds, Cycle));
        Keep(tile.TweenOffsetTransformPosition(new Vector2(0, -22), Seconds, Cycle));
        Keep(tile.TweenOffsetTransformRotation(0.18f, Seconds, Cycle));
        Keep(tile.TweenOffsetTransformScale(new Vector2(1.13f, 1.13f), Seconds, Cycle));
        Keep(scroll.TweenScrollVertical(200, Seconds * 2, Cycle));
    }
}
