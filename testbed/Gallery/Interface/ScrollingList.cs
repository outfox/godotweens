// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd;
namespace testbed;

public sealed partial class ScrollingList : GalleryEffect
{
    private static readonly Color[] Accents = [Palette.Mint, Palette.Blue, Palette.Amber];
    private static readonly string[] States = ["ready", "queued", "paused"];
    private ScrollContainer scroll = null!;

    public override string Title => "Scroll Position";
    public override string Caption => "ScrollContainer.ScrollVertical uses integer interpolation.";

    protected override void Build()
    {
        scroll = Fill(new ScrollContainer
        {
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled, MouseFilter = Control.MouseFilterEnum.Ignore,
        }, 8);
        var list = scroll.Add(new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill });
        list.AddThemeConstantOverride("separation", 10);
        for (var i = 0; i < 8; i++) list.AddChild(Entry(i));
    }

    private PanelContainer Entry(int index)
    {
        var accent = Accents[index % Accents.Length];
        var entry = new PanelContainer { CustomMinimumSize = new Vector2(0, 48) };
        entry.AddThemeStyleboxOverride("panel", Own(GalleryTheme.Box(new Color("293c50"), 7, 0, null, 12, 8)));

        var line = entry.Add(new HBoxContainer());
        line.AddThemeConstantOverride("separation", 12);
        line.Add(new ColorRect
        {
            Color = accent, CustomMinimumSize = new Vector2(4, 26), SizeFlagsVertical = Control.SizeFlags.ShrinkCenter,
        });
        line.AddChild(GalleryTheme.Label($"0{index + 1}", 16, Palette.Muted));
        var name = GalleryTheme.Label($"Item {(char)('A' + index)}", 16);
        name.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        line.AddChild(name);
        line.AddChild(GalleryTheme.Label(States[index % States.Length], 13, accent));
        return entry;
    }

    protected override void Animate() => TrackTweens(CreateAnimation());
}
