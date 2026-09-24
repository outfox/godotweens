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
    private ColorRect swatch = null!;
    private Control tile = null!;
    private ScrollContainer scroll = null!;
    protected override void Build()
    {
        var words = Card("01 / Visible Ratio", "Label.VisibleRatio and SelfModulate.");
        reveal = Text("VisibleRatio\nreveals text\nover time.", 30, Colors.White);
        reveal.SelfModulate = Mint;
        reveal.Position = new Vector2(18, 8); reveal.VisibleRatio = 0; words.AddChild(reveal);

        var meters = Card("02 / Range Value", "ProgressBar.Value with color and alpha tweens.");
        var caption = Text("VALUE", 13, Muted); caption.Position = new Vector2(10, 8); meters.AddChild(caption);
        progress = new ProgressBar { Position = new Vector2(8, 55), Size = new Vector2(340, 30), Step = 0, ShowPercentage = false };
        progress.AddThemeStyleboxOverride("background", Own(Box(new Color("293c50"), 6)));
        progress.AddThemeStyleboxOverride("fill", Own(Box(Mint, 6))); meters.AddChild(progress);
        swatch = new ColorRect { Color = Mint, Position = new Vector2(8, 110), Size = new Vector2(64, 28) }; meters.AddChild(swatch);
        var readout = Text("0%", 26); readout.Position = new Vector2(280, 105); meters.AddChild(readout);
        progress.ValueChanged += v => readout.Text = $"{v:0}%";

        var layout = Card("03 / Offset Transforms", "Position, rotation, and scale within an HBoxContainer.");
        var row = new HBoxContainer { Position = new Vector2(15, 35) }; row.AddThemeConstantOverride("separation", 18); layout.AddChild(row);
        for (var i = 0; i < 3; i++)
        {
            var block = new PanelContainer { CustomMinimumSize = new Vector2(88, 82), OffsetTransformEnabled = true };
            block.AddThemeStyleboxOverride("panel", Own(Box(i == 1 ? new Color("355a66") : new Color("293c50"), 10)));
            var label = Text($"0{i + 1}", 24, i == 1 ? Mint : Muted); label.HorizontalAlignment = HorizontalAlignment.Center;
            block.AddChild(label); row.AddChild(block);
            if (i == 1) { tile = block; tile.OffsetTransformPivot = new Vector2(44, 41); }
        }
        var browse = Card("04 / Scroll Position", "ScrollContainer.ScrollVertical uses integer interpolation.");
        scroll = new ScrollContainer { Position = new Vector2(8, 8), Size = new Vector2(345, 152),
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled, MouseFilter = MouseFilterEnum.Ignore };
        browse.AddChild(scroll);
        var list = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill }; list.AddThemeConstantOverride("separation", 10); scroll.AddChild(list);
        foreach (var name in new[] { "01   Item A", "02   Item B", "03   Item C", "04   Item D", "05   Item E", "06   Item F" })
        {
            var entry = new PanelContainer { CustomMinimumSize = new Vector2(0, 48) };
            entry.AddThemeStyleboxOverride("panel", Own(Box(new Color("293c50"), 7)));
            entry.AddChild(Text(name, 16)); list.AddChild(entry);
        }
    }
    protected override void Animate()
    {
        Keep(reveal.TweenVisibleRatio(1, Seconds, Cycle));
        Keep(reveal.TweenSelfModulate(Amber, Seconds, Cycle));
        Keep(progress.TweenValue(100, Seconds, Cycle));
        Keep(swatch.TweenColor(Blue, Seconds, Cycle));
        Keep(swatch.TweenSelfModulateAlpha(0.25f, Seconds, Cycle));
        Keep(tile.TweenOffsetTransformPosition(new Vector2(0, -22), Seconds, Cycle));
        Keep(tile.TweenOffsetTransformRotation(0.18f, Seconds, Cycle));
        Keep(tile.TweenOffsetTransformScale(new Vector2(1.13f, 1.13f), Seconds, Cycle));
        Keep(scroll.TweenScrollVertical(200, Seconds * 2, Cycle));
    }
}
