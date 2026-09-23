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
    public static readonly string[] PageNames = ["Motion & paths", "Interface", "Drawing & particles", "3D stage", "Materials", "Shaders"];
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
        var background = new ColorRect { Color = new Color("101925"), MouseFilter = MouseFilterEnum.Ignore };
        AddChild(background); background.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        var margin = new MarginContainer(); AddChild(margin); margin.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        foreach (var side in new[] { "left", "right", "top", "bottom" }) margin.AddThemeConstantOverride("margin_" + side, 24);
        var layout = new VBoxContainer(); layout.AddThemeConstantOverride("separation", 14); margin.AddChild(layout);
        var header = new HBoxContainer(); layout.AddChild(header);
        var title = GalleryPage.Text("godotweens / playground", 30); title.SizeFlagsHorizontal = SizeFlags.ExpandFill; header.AddChild(title);
        header.AddChild(GalleryPage.Text("06 PAGES  ·  LIVE C#", 14, GalleryPage.Mint));
        var settings = new HBoxContainer(); settings.AddThemeConstantOverride("separation", 14); layout.AddChild(settings);
        settings.AddChild(GalleryPage.Text("Easing", 16, GalleryPage.Muted));
        ease = new OptionButton { CustomMinimumSize = new Vector2(180, 38) };
        foreach (var choice in new[] { EaseType.CubicInOut, EaseType.Linear, EaseType.SineInOut, EaseType.BackOut, EaseType.ElasticOut, EaseType.BounceOut })
            ease.AddItem(choice.ToString(), (int)choice);
        settings.AddChild(ease);
        durationLabel = GalleryPage.Text("Leg duration  1.8 s", 16); settings.AddChild(durationLabel);
        duration = new HSlider { MinValue = 0.4, MaxValue = 4, Step = 0.1, Value = 1.8,
            CustomMinimumSize = new Vector2(150, 38), SizeFlagsHorizontal = SizeFlags.ExpandFill };
        settings.AddChild(duration);
        pingPong = new CheckBox { Text = "Ping-pong", ButtonPressed = true }; settings.AddChild(pingPong);
        layout.AddChild(new HSeparator());
        var body = new HBoxContainer { SizeFlagsVertical = SizeFlags.ExpandFill }; body.AddThemeConstantOverride("separation", 20); layout.AddChild(body);
        var sidebar = new VBoxContainer { CustomMinimumSize = new Vector2(214, 0) }; sidebar.AddThemeConstantOverride("separation", 8); body.AddChild(sidebar);
        sidebar.AddChild(GalleryPage.Text("EXPLORE", 12, GalleryPage.Muted));
        for (var i = 0; i < PageNames.Length; i++)
        {
            var index = i;
            var button = new Button { Text = $"{i + 1:00}  {PageNames[i]}", ToggleMode = true,
                Alignment = HorizontalAlignment.Left, CustomMinimumSize = new Vector2(214, 46) };
            var selected = GalleryPage.Box(new Color("294b48"), 8); themeResources.Add(selected);
            button.AddThemeStyleboxOverride("pressed", selected);
            button.AddThemeFontSizeOverride("font_size", 15);
            button.Pressed += () => SelectPage(index); sidebar.AddChild(button); navigation.Add(button);
        }
        var hint = GalleryPage.Text("Pick a page.\nMix the timing.\nWatch it move.", 14, GalleryPage.Muted); sidebar.AddChild(hint);
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
        duration.ValueChanged += v => { durationLabel.Text = $"Leg duration  {v:0.0} s"; RestartDemo(); };
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
        page = index switch { 0 => new MotionPage(), 1 => new InterfacePage(), 2 => new EffectsPage(),
            3 => new SpatialPage(), 4 => new MaterialsPage(), _ => new ShadersPage() };
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
        if (status is not null) status.Text = "Cancelled · values held. Restart to play.";
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
