// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using godotweens;
namespace testbed;

/// <summary>Gallery shell. Pages own their examples; navigation destroys the previous playground.</summary>
public partial class TweenDemo : Control
{
    public static readonly string[] PageNames = ["Squash & stretch", "Choreography", "Motion & paths", "Interface", "Drawing & particles", "3D", "Materials", "Shaders"];
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
        BuildControls(); SelectPage(0);
    }
    private void BuildControls()
    {
        Theme = GalleryTheme.Build(themeResources);
        var background = new ColorRect { Color = GalleryTheme.Background, MouseFilter = MouseFilterEnum.Ignore };
        AddChild(background); background.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        var margin = new MarginContainer(); AddChild(margin); margin.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        foreach (var side in new[] { "left", "right", "top", "bottom" }) margin.AddThemeConstantOverride("margin_" + side, 24);
        var layout = new VBoxContainer(); layout.AddThemeConstantOverride("separation", 14); margin.AddChild(layout);
        var header = new HBoxContainer(); layout.AddChild(header);
        var title = GalleryPage.Text("godotweens / testbed", 30); title.SizeFlagsHorizontal = SizeFlags.ExpandFill; header.AddChild(title);
        header.AddChild(GalleryPage.Text("C# / GODOT", 14, GalleryPage.Mint));
        var settings = new HBoxContainer(); settings.AddThemeConstantOverride("separation", 14); layout.AddChild(settings);
        settings.AddChild(GalleryPage.Text("Easing", 16, GalleryPage.Muted));
        ease = new OptionButton { CustomMinimumSize = new Vector2(180, 38) };
        foreach (var choice in new[] { EaseType.CubicInOut, EaseType.Linear, EaseType.SineInOut, EaseType.BackOut, EaseType.ElasticOut, EaseType.BounceOut })
            ease.AddItem(choice.ToString(), (int)choice);
        settings.AddChild(ease);
        settings.AddChild(new Control { CustomMinimumSize = new Vector2(8, 0) });
        settings.AddChild(GalleryPage.Text("Leg duration", 16, GalleryPage.Muted));
        durationLabel = GalleryPage.Text("1.8 s", 16); durationLabel.CustomMinimumSize = new Vector2(44, 0); settings.AddChild(durationLabel);
        duration = new HSlider { MinValue = 0.4, MaxValue = 4, Step = 0.1, Value = 1.8,
            CustomMinimumSize = new Vector2(150, 38), SizeFlagsHorizontal = SizeFlags.ExpandFill };
        settings.AddChild(duration);
        pingPong = new CheckBox { Text = "Ping-pong", ButtonPressed = true }; settings.AddChild(pingPong);
        layout.AddChild(new HSeparator());
        var body = new HBoxContainer { SizeFlagsVertical = SizeFlags.ExpandFill }; body.AddThemeConstantOverride("separation", 20); layout.AddChild(body);
        var sidebar = new VBoxContainer { CustomMinimumSize = new Vector2(214, 0) }; sidebar.AddThemeConstantOverride("separation", 6); body.AddChild(sidebar);
        sidebar.AddChild(GalleryPage.Text("EXAMPLES", 12, GalleryPage.Muted));
        var idle = GalleryTheme.Box(Colors.Transparent, 8, 0, null, 14, 8);
        var hover = GalleryTheme.Box(GalleryTheme.Raised, 8, 0, null, 14, 8);
        var selected = GalleryTheme.Box(GalleryTheme.Selected, 8, 0, GalleryPage.Mint, 14, 8); selected.BorderWidthLeft = 3;
        themeResources.AddRange([idle, hover, selected]);
        for (var i = 0; i < PageNames.Length; i++)
        {
            var index = i;
            var button = new Button { Text = $"{i + 1:00}  {PageNames[i]}", ToggleMode = true, FocusMode = FocusModeEnum.None,
                Alignment = HorizontalAlignment.Left, CustomMinimumSize = new Vector2(214, 44) };
            button.AddThemeStyleboxOverride("normal", idle); button.AddThemeStyleboxOverride("hover", hover);
            button.AddThemeStyleboxOverride("pressed", selected); button.AddThemeStyleboxOverride("hover_pressed", selected);
            button.AddThemeFontSizeOverride("font_size", 15);
            button.Pressed += () => SelectPage(index); sidebar.AddChild(button); navigation.Add(button);
        }
        sidebar.AddChild(new Control { CustomMinimumSize = new Vector2(0, 6) });
        sidebar.AddChild(GalleryPage.Text("Settings apply to\nthe current page.", 14, GalleryPage.Muted));
        var scroll = new ScrollContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsVertical = SizeFlags.ExpandFill,
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled };
        body.AddChild(scroll);
        content = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsVertical = SizeFlags.ExpandFill }; scroll.AddChild(content);
        var actions = new HBoxContainer(); actions.AddThemeConstantOverride("separation", 10); layout.AddChild(actions);
        pause = Button(actions, "Pause", TogglePause);
        Button(actions, "Cancel", StopDemo); Button(actions, "Restart page", RestartDemo);
        status = GalleryPage.Text("Starting…", 15, GalleryPage.Mint); status.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        status.HorizontalAlignment = HorizontalAlignment.Right; actions.AddChild(status);
        ease.ItemSelected += _ => RestartDemo();
        duration.ValueChanged += v => { durationLabel.Text = $"{v:0.0} s"; RestartDemo(); };
        pingPong.Toggled += _ => RestartDemo();
    }
    private static Button Button(HBoxContainer row, string text, Action action)
    {
        var button = new Button { Text = text, CustomMinimumSize = new Vector2(110, 40) };
        button.Pressed += action; row.AddChild(button); return button;
    }
    public void SelectPage(int index)
    {
        if (index < 0 || index >= PageNames.Length) throw new ArgumentOutOfRangeException(nameof(index));
        if (!IsInsideTree()) return;
        StopDemo(); DestroyPage(); SelectedPage = index;
        for (var i = 0; i < navigation.Count; i++) navigation[i].SetPressedNoSignal(i == index);
        page = index switch { 0 => new SquashPage(), 1 => new ChoreographyPage(), 2 => new MotionPage(), 3 => new InterfacePage(),
            4 => new EffectsPage(), 5 => new SpatialPage(), 6 => new MaterialsPage(), _ => new ShadersPage() };
        page.SizeFlagsHorizontal = SizeFlags.ExpandFill; page.SizeFlagsVertical = SizeFlags.ExpandFill;
        content.AddChild(page);
        var selected = page; var current = ++revision;
        Callable.From(() =>
        {
            if (!IsInsideTree() || current != revision || selected != page) return;
            try
            {
                selected.Start(duration.Value, (EaseType)ease.GetSelectedId(), pingPong.ButtonPressed);
                IsPlaying = true; paused = false; pause.Text = "Pause"; pause.Disabled = false;
            }
            catch (Exception error) { selected.Stop(); status.Text = "Could not start this page: " + error.Message; GD.PushError(error.ToString()); }
        }).CallDeferred();
    }
    public void RestartDemo() { if (content is not null) SelectPage(SelectedPage); }
    public void TogglePause()
    {
        if (!IsPlaying) return;
        paused = !paused; page?.Pause(paused); pause.Text = paused ? "Resume" : "Pause";
    }
    public void StopDemo()
    {
        revision++; IsPlaying = false; page?.Stop();
        if (pause is not null) { pause.Text = "Pause"; pause.Disabled = true; }
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
        page.Free(); page.ReleaseResources(); page = null;
    }
    public override void _ExitTree()
    {
        StopDemo();
        // Children are still valid during tree exit; release page-owned resources after their deletion.
        DestroyPage();
        foreach (var resource in themeResources) resource.Dispose(); themeResources.Clear();
    }
}
