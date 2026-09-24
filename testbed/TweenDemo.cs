// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using godotweens;
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
    private static readonly EaseType[] EaseChoices =
        [EaseType.CubicInOut, EaseType.Linear, EaseType.SineInOut, EaseType.BackOut, EaseType.ElasticOut, EaseType.BounceOut];

    public static readonly string[] PageNames = Pages.Select(p => p.Name).ToArray();

    private readonly List<Button> navigation = [];
    private readonly List<Resource> themeResources = [];
    private VBoxContainer content = null!;
    private Label status = null!, durationLabel = null!;
    private Button pause = null!;
    private OptionButton ease = null!;
    private HSlider duration = null!;
    private CheckBox pingPong = null!;
    private GalleryPage? page;
    private bool paused;
    private int revision;

    public int SelectedPage { get; private set; }
    public bool IsPlaying { get; private set; }
    public int DemoTweenCount => page?.ActiveCount ?? 0;
    public Task? ChainTask => page?.SequenceTask;
    public GalleryPage? CurrentPage => page;

    public override void _Ready()
    {
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
        foreach (var side in new[] { "left", "right", "top", "bottom" }) margin.AddThemeConstantOverride("margin_" + side, 24);

        var layout = new VBoxContainer();
        layout.AddThemeConstantOverride("separation", 14);
        margin.AddChild(layout);
        layout.AddChild(BuildHeader());
        layout.AddChild(BuildSettings());
        layout.AddChild(new HSeparator());

        var body = new HBoxContainer { SizeFlagsVertical = SizeFlags.ExpandFill };
        body.AddThemeConstantOverride("separation", 20);
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

    private static HBoxContainer BuildHeader()
    {
        var header = new HBoxContainer();
        var title = GalleryTheme.Label("godotweens / testbed", 30);
        title.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        header.AddChild(title);
        header.AddChild(GalleryTheme.Label("C# / GODOT", 14, Palette.Mint));
        return header;
    }

    private HBoxContainer BuildSettings()
    {
        var settings = new HBoxContainer();
        settings.AddThemeConstantOverride("separation", 14);

        settings.AddChild(GalleryTheme.Label("Easing", 16, Palette.Muted));
        ease = new OptionButton { CustomMinimumSize = new Vector2(180, 38) };
        foreach (var choice in EaseChoices) ease.AddItem(choice.ToString(), (int)choice);
        ease.ItemSelected += _ => RestartDemo();
        settings.AddChild(ease);
        settings.AddChild(new Control { CustomMinimumSize = new Vector2(8, 0) });

        settings.AddChild(GalleryTheme.Label("Leg duration", 16, Palette.Muted));
        durationLabel = GalleryTheme.Label("1.8 s", 16);
        durationLabel.CustomMinimumSize = new Vector2(44, 0);
        settings.AddChild(durationLabel);
        duration = new HSlider
        {
            MinValue = 0.4, MaxValue = 4, Step = 0.1, Value = 1.8,
            CustomMinimumSize = new Vector2(150, 38), SizeFlagsHorizontal = SizeFlags.ExpandFill,
        };
        duration.ValueChanged += seconds =>
        {
            durationLabel.Text = $"{seconds:0.0} s";
            RestartDemo();
        };
        settings.AddChild(duration);

        pingPong = new CheckBox { Text = "Ping-pong", ButtonPressed = true };
        pingPong.Toggled += _ => RestartDemo();
        settings.AddChild(pingPong);
        return settings;
    }

    private VBoxContainer BuildSidebar()
    {
        var sidebar = new VBoxContainer { CustomMinimumSize = new Vector2(214, 0) };
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
                Alignment = HorizontalAlignment.Left, CustomMinimumSize = new Vector2(214, 44),
            };
            button.AddThemeStyleboxOverride("normal", idle);
            button.AddThemeStyleboxOverride("hover", hover);
            button.AddThemeStyleboxOverride("pressed", selected);
            button.AddThemeStyleboxOverride("hover_pressed", selected);
            button.AddThemeFontSizeOverride("font_size", 15);
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
        pause = ActionButton(actions, "Pause", TogglePause);
        ActionButton(actions, "Cancel", StopDemo);
        ActionButton(actions, "Restart page", RestartDemo);
        status = GalleryTheme.Label("Starting…", 15, Palette.Mint);
        status.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        status.HorizontalAlignment = HorizontalAlignment.Right;
        actions.AddChild(status);
        return actions;
    }

    private static Button ActionButton(HBoxContainer row, string text, Action action)
    {
        var button = new Button { Text = text, CustomMinimumSize = new Vector2(110, 40) };
        button.Pressed += action;
        row.AddChild(button);
        return button;
    }

    public void SelectPage(int index)
    {
        if (index < 0 || index >= Pages.Length) throw new ArgumentOutOfRangeException(nameof(index));
        if (!IsInsideTree()) return;
        StopDemo();
        DestroyPage();
        SelectedPage = index;
        for (var i = 0; i < navigation.Count; i++) navigation[i].SetPressedNoSignal(i == index);

        page = Pages[index].Create();
        page.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        page.SizeFlagsVertical = SizeFlags.ExpandFill;
        content.AddChild(page);

        // Start once the page's layout has settled; a newer selection in the same frame supersedes this one.
        var selected = page;
        var current = ++revision;
        Callable.From(() =>
        {
            if (!IsInsideTree() || current != revision || selected != page) return;
            try
            {
                selected.Start(duration.Value, (EaseType)ease.GetSelectedId(), pingPong.ButtonPressed);
                IsPlaying = true;
                paused = false;
                pause.Text = "Pause";
                pause.Disabled = false;
            }
            catch (Exception error)
            {
                selected.Stop();
                status.Text = "Could not start this page: " + error.Message;
                GD.PushError(error.ToString());
            }
        }).CallDeferred();
    }

    public void RestartDemo()
    {
        if (content is not null) SelectPage(SelectedPage);
    }

    public void TogglePause()
    {
        if (!IsPlaying) return;
        paused = !paused;
        page?.Pause(paused);
        pause.Text = paused ? "Resume" : "Pause";
    }

    public void StopDemo()
    {
        revision++;
        IsPlaying = false;
        page?.Stop();
        if (pause is not null)
        {
            pause.Text = "Pause";
            pause.Disabled = true;
        }
        if (status is not null) status.Text = "Cancelled · values retained";
    }

    public override void _Process(double delta)
    {
        if (page?.Error is { } error) status.Text = "Tween error: " + error;
        else if (IsPlaying) status.Text = $"{(paused ? "Paused" : "Playing")}  ·  {DemoTweenCount} active tweens";
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
        StopDemo();
        // Children are still valid during tree exit; release page-owned resources after their deletion.
        DestroyPage();
        foreach (var resource in themeResources) resource.Dispose();
        themeResources.Clear();
    }
}
