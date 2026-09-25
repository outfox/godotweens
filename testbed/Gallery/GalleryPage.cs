// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

/// <summary>One disposable playground: a grid of effect cards. Only the selected page is instantiated.</summary>
public abstract partial class GalleryPage : VBoxContainer
{
    private readonly List<Resource> resources = [];
    private GalleryEffect[] effects = [];
    private readonly List<PanelContainer> cards = [];
    private readonly List<Button> sourceButtons = [];
    private GridContainer grid = null!;
    private HBoxContainer sourceNavigation = null!;
    private OptionButton examplePicker = null!;
    private GallerySourceView sourceView = null!;

    public int SelectedEffect { get; private set; } = -1;
    public GallerySourceView SourceView => sourceView;

    public abstract string Heading { get; }
    public abstract string Description { get; }
    public Task? SequenceTask { get; private set; }
    public int ActiveCount => effects.Sum(e => e.ActiveCount);
    public bool AllPaused => effects.All(e => e.AllPaused);
    public string? Error => effects.Select(e => e.Error).FirstOrDefault(error => error is not null);

    protected abstract GalleryEffect[] CreateEffects();

    public override void _Ready()
    {
        AddThemeConstantOverride("separation", 10);
        AddChild(GalleryTheme.Label(Heading, 28));
        AddChild(GalleryTheme.Label(Description, 15, Palette.Muted));

        sourceNavigation = this.Add(new HBoxContainer { Visible = false });
        var back = sourceNavigation.Add(new Button { Text = "‹ All examples" });
        back.Pressed += ShowGallery;
        examplePicker = sourceNavigation.Add(new OptionButton { SizeFlagsHorizontal = SizeFlags.ExpandFill });
        examplePicker.ItemSelected += index => ShowSource((int)index);
        sourceNavigation.AddChild(GalleryTheme.Label("C# / LIVE PREVIEW", 12, Palette.Mint));

        var split = this.Add(new HSplitContainer { SizeFlagsVertical = SizeFlags.ExpandFill });
        sourceView = split.Add(new GallerySourceView { Visible = false });
        grid = new GridContainer { Columns = 2, SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsVertical = SizeFlags.ExpandFill };
        grid.AddThemeConstantOverride("h_separation", 14);
        grid.AddThemeConstantOverride("v_separation", 14);
        split.AddChild(grid);

        effects = CreateEffects();
        for (var i = 0; i < effects.Length; i++)
        {
            examplePicker.AddItem(effects[i].Title);
            effects[i].Attach(AddCard(grid, $"{i + 1:00} / {effects[i].Title}", effects[i].Caption, i));
        }
    }

    public void ShowSource(int index)
    {
        if (index < 0 || index >= effects.Length) throw new ArgumentOutOfRangeException(nameof(index));
        SelectedEffect = index;
        sourceNavigation.Show();
        sourceView.Show();
        examplePicker.Select(index);
        grid.Columns = 1;
        // Keep the preview's original landscape proportions, especially for 3D cameras.
        grid.SizeFlagsVertical = SizeFlags.ShrinkCenter;
        for (var i = 0; i < cards.Count; i++)
        {
            cards[i].Visible = i == index;
            sourceButtons[i].Visible = false;
        }
        sourceView.ShowEffect(effects[index]);
    }

    public void ShowGallery()
    {
        SelectedEffect = -1;
        sourceNavigation.Hide();
        sourceView.Hide();
        grid.Columns = 2;
        grid.SizeFlagsVertical = SizeFlags.ExpandFill;
        foreach (var card in cards) card.Show();
        foreach (var button in sourceButtons) button.Show();
    }

    public void Start(double seconds, EaseType ease, bool pingPong)
    {
        foreach (var effect in effects) effect.Start(seconds, ease, pingPong);
        var sequences = effects.Select(e => e.Sequence).OfType<Task>().ToArray();
        SequenceTask = sequences.Length > 0 ? Task.WhenAll(sequences) : null;
    }

    public void Pause(bool paused)
    {
        foreach (var effect in effects) effect.Pause(paused);
    }

    public void Stop()
    {
        foreach (var effect in effects) effect.Stop();
    }

    public override void _ExitTree() => Stop();

    /// <summary>Called after Free(), so children have relinquished their native resource references.</summary>
    public void ReleaseResources()
    {
        sourceView.ReleaseResources();
        foreach (var effect in effects) effect.ReleaseResources();
        foreach (var resource in resources) resource.Dispose();
        resources.Clear();
    }

    /// <summary>Adds a titled card to the grid and returns its stage, the area an effect draws into.</summary>
    private Control AddCard(GridContainer grid, string title, string caption, int index)
    {
        var panel = grid.Add(new PanelContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsVertical = SizeFlags.ExpandFill,
        });
        panel.AddThemeStyleboxOverride("panel", Own(GalleryTheme.Box(Palette.Surface, 12, 1)));
        cards.Add(panel);

        var column = panel.Add(new VBoxContainer());
        column.AddThemeConstantOverride("separation", 8);
        var header = column.Add(new HBoxContainer());
        var label = header.Add(GalleryTheme.Label(title, 19));
        label.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        label.TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis;
        var source = header.Add(new Button { Text = "View C#", TooltipText = "Read the actual source beside this animation" });
        source.AddThemeFontSizeOverride("font_size", 13);
        source.Pressed += () => ShowSource(index);
        sourceButtons.Add(source);

        // Drawn clipping keeps viewports inside the rounded inset.
        var stage = column.Add(new Panel
        {
            CustomMinimumSize = new Vector2(370, 168), SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill, ClipChildren = ClipChildrenMode.AndDraw, MouseFilter = MouseFilterEnum.Ignore,
        });
        stage.AddThemeStyleboxOverride("panel", Own(GalleryTheme.Box(Palette.Stage, 8)));

        column.Add(GalleryTheme.Label(caption, 13, Palette.Muted)).AutowrapMode = TextServer.AutowrapMode.WordSmart;
        return stage;
    }

    private T Own<T>(T resource) where T : Resource
    {
        resources.Add(resource);
        return resource;
    }
}
