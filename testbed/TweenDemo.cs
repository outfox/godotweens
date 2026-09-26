// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
namespace testbed;

/// <summary>Gallery shell. Pages own their examples; navigation destroys the previous playground.</summary>
public partial class TweenDemo : Control
{
    private static readonly (string Name, Func<GalleryPage> Create)[] Pages =
    [
        ("Squash & stretch", () => new SquashPage()),
        ("Choreography", () => new ChoreographyPage()),
        ("Motion & paths", () => new MotionPage()),
        ("Interface", () => new InterfacePage()),
        ("Drawing & particles", () => new DrawingPage()),
        ("3D", () => new SpatialPage()),
        ("Materials", () => new MaterialsPage()),
        ("Shaders", () => new ShadersPage()),
    ];

    public static readonly string[] PageNames = Pages.Select(p => p.Name).ToArray();

    private readonly List<Button> navigation = [];
    private readonly List<Resource> themeResources = [];
    private VBoxContainer? content;
    private Label durationLabel = null!;
    private HSlider duration = null!;
    private GalleryPage? page;
    private int revision;

    public int SelectedPage { get; private set; }
    public GalleryPage? CurrentPage => page;

    public override void _Ready()
    {
        if (GalleryEffect.Supports2DMsaa) GetTree().Root.Msaa2D = Viewport.Msaa.Msaa4X;
        BuildControls();
        SelectPage(0);
    }

    private void BuildControls()
    {
        Theme = GalleryTheme.Build(themeResources);
        var background = new ColorRect { Color = Palette.Background, MouseFilter = MouseFilterEnum.Ignore };
        AddChild(background);
        background.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

        var margin = new MarginContainer();
        AddChild(margin);
        margin.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        foreach (var side in new[] { "left", "right", "top", "bottom" }) margin.AddThemeConstantOverride("margin_" + side, 16);

        var layout = new VBoxContainer();
        layout.AddThemeConstantOverride("separation", 10);
        margin.AddChild(layout);
        layout.AddChild(BuildHeader());
        layout.AddChild(new HSeparator());

        var body = new HBoxContainer { SizeFlagsVertical = SizeFlags.ExpandFill };
        body.AddThemeConstantOverride("separation", 16);
        layout.AddChild(body);
        body.AddChild(BuildSidebar());
        var scroll = new ScrollContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsVertical = SizeFlags.ExpandFill,
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
        };
        body.AddChild(scroll);
        content = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsVertical = SizeFlags.ExpandFill };
        scroll.AddChild(content);

        layout.AddChild(BuildActions());
    }

    private HBoxContainer BuildHeader()
    {
        var header = new HBoxContainer();
        header.AddThemeConstantOverride("separation", 24);
        var title = GalleryTheme.Label("tweens.gd / testbed", 22);
        header.AddChild(title);
        var settings = BuildSettings();
        settings.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        header.AddChild(settings);
        return header;
    }

    private HBoxContainer BuildSettings()
    {
        var settings = new HBoxContainer();
        settings.AddThemeConstantOverride("separation", 10);

        settings.AddChild(GalleryTheme.Label("Leg duration", 16, Palette.Muted));
        durationLabel = GalleryTheme.Label("1.8 s");
        durationLabel.CustomMinimumSize = new Vector2(44, 0);
        settings.AddChild(durationLabel);
        duration = new HSlider
        {
            MinValue = 0.4, MaxValue = 4, Step = 0.1, Value = 1.8,
            CustomMinimumSize = new Vector2(120, 34), SizeFlagsHorizontal = SizeFlags.ExpandFill,
        };
        duration.ValueChanged += seconds =>
        {
            durationLabel.Text = $"{seconds:0.0} s";
            RestartDemo();
        };
        settings.AddChild(duration);

        return settings;
    }

    private VBoxContainer BuildSidebar()
    {
        var sidebar = new VBoxContainer { CustomMinimumSize = new Vector2(194, 0) };
        sidebar.AddThemeConstantOverride("separation", 6);
        sidebar.AddChild(GalleryTheme.Label("EXAMPLES", 12, Palette.Muted));

        var idle = GalleryTheme.Box(Colors.Transparent, 8, 0, null, 14, 8);
        var hover = GalleryTheme.Box(Palette.Raised, 8, 0, null, 14, 8);
        var selected = GalleryTheme.Box(Palette.Selected, 8, 0, Palette.Mint, 14, 8);
        selected.BorderWidthLeft = 3;
        themeResources.AddRange([idle, hover, selected]);

        for (var i = 0; i < PageNames.Length; i++)
        {
            var index = i;
            var button = new Button
            {
                Text = $"{i + 1:00}  {PageNames[i]}", ToggleMode = true, FocusMode = FocusModeEnum.None,
                Alignment = HorizontalAlignment.Left, CustomMinimumSize = new Vector2(194, 38),
            };
            button.AddThemeStyleboxOverride("normal", idle);
            button.AddThemeStyleboxOverride("hover", hover);
            button.AddThemeStyleboxOverride("pressed", selected);
            button.AddThemeStyleboxOverride("hover_pressed", selected);
            button.AddThemeFontSizeOverride("font_size", 14);
            button.Pressed += () => SelectPage(index);
            sidebar.AddChild(button);
            navigation.Add(button);
        }

        sidebar.AddChild(new Control { CustomMinimumSize = new Vector2(0, 6) });
        sidebar.AddChild(GalleryTheme.Label("Settings apply to\nthe current page.", 14, Palette.Muted));
        return sidebar;
    }

    private HBoxContainer BuildActions()
    {
        var actions = new HBoxContainer();
        actions.AddThemeConstantOverride("separation", 10);
        ActionButton(actions, "Restart page", RestartDemo);
        return actions;
    }

    private static void ActionButton(HBoxContainer row, string text, Action action)
    {
        var button = new Button { Text = text, CustomMinimumSize = new Vector2(100, 34) };
        button.Pressed += action;
        row.AddChild(button);
    }

    public void SelectPage(int index)
    {
        if (index < 0 || index >= Pages.Length) throw new ArgumentOutOfRangeException(nameof(index));
        if (!IsInsideTree() || content is null) return;
        DestroyPage();
        SelectedPage = index;
        for (var i = 0; i < navigation.Count; i++) navigation[i].SetPressedNoSignal(i == index);

        page = Pages[index].Create();
        page.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        page.SizeFlagsVertical = SizeFlags.ExpandFill;
        content.AddChild(page);

        // Start once the page's layout has settled. Bound to this node, so Godot drops the call if the demo is freed first.
        CallDeferred(MethodName.StartSelectedPage, ++revision);
    }

    /// <summary>Starts the current page, unless a newer selection superseded <paramref name="selection"/>.</summary>
    private void StartSelectedPage(int selection)
    {
        if (!IsInsideTree() || selection != revision || page is null) return;
        try
        {
            page.Start(duration.Value);
        }
        catch (Exception error)
        {
            DestroyPage();
            GD.PushError(error.ToString());
        }
    }

    public void RestartDemo()
    {
        if (content is null) return;
        var selectedEffect = page?.SelectedEffect ?? -1;
        SelectPage(SelectedPage);
        if (selectedEffect >= 0) page?.ShowSource(selectedEffect);
    }

    private void DestroyPage()
    {
        if (page is null) return;
        page.Free();
        page.ReleaseResources();
        page = null;
    }

    public override void _ExitTree()
    {
        // Children are still valid during tree exit; release page-owned resources after their deletion.
        DestroyPage();
        foreach (var resource in themeResources) resource.Dispose();
        themeResources.Clear();
    }
}
