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
    private Label description = null!;
    private readonly List<Control> frames = [];

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
        var heading = this.Add(new HBoxContainer());
        heading.AddThemeConstantOverride("separation", 16);
        heading.AddChild(GalleryTheme.Label(Heading, 24));
        description = heading.Add(GalleryTheme.Label(Description, 14, Palette.Muted));
        description.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        description.TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis;
        description.TooltipText = Description;
        description.MouseFilter = MouseFilterEnum.Pass;

        sourceNavigation = heading.Add(new HBoxContainer { Visible = false, SizeFlagsHorizontal = SizeFlags.ExpandFill });
        var back = sourceNavigation.Add(new Button { Text = "‹ All examples" });
        back.Pressed += ShowGallery;
        examplePicker = sourceNavigation.Add(new OptionButton { SizeFlagsHorizontal = SizeFlags.ExpandFill });
        examplePicker.ItemSelected += index => ShowSource((int)index);

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
        description.Hide();
        sourceView.Show();
        examplePicker.Select(index);
        grid.Columns = 1;
        grid.SizeFlagsVertical = SizeFlags.ExpandFill;
        for (var i = 0; i < cards.Count; i++)
        {
            cards[i].Visible = i == index;
            sourceButtons[i].Visible = false;
            FitPreview(frames[i]);
        }
        sourceView.ShowEffect(effects[index]);
    }

    public void ShowGallery()
    {
        SelectedEffect = -1;
        sourceNavigation.Hide();
        description.Show();
        sourceView.Hide();
        grid.Columns = 2;
        grid.SizeFlagsVertical = SizeFlags.ExpandFill;
        foreach (var card in cards) card.Show();
        foreach (var button in sourceButtons) button.Show();
        foreach (var frame in frames) FitPreview(frame);
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
        panel.AddThemeStyleboxOverride("panel", Own(GalleryTheme.Box(Palette.Surface, 12, 1, null, 12, 10)));
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
        var frame = column.Add(new Control
        {
            CustomMinimumSize = new Vector2(370, 168), MouseFilter = MouseFilterEnum.Ignore,
            SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsVertical = SizeFlags.ExpandFill,
        });
        frames.Add(frame);
        var stage = frame.Add(new Panel
        {
            Name = "PreviewStage",
            ClipChildren = ClipChildrenMode.AndDraw, MouseFilter = MouseFilterEnum.Ignore,
        });
        frame.Resized += () => FitPreview(frame);
        stage.AddThemeStyleboxOverride("panel", Own(GalleryTheme.Box(Palette.Stage, 8)));

        column.Add(GalleryTheme.Label(caption, 13, Palette.Muted)).AutowrapMode = TextServer.AutowrapMode.WordSmart;
        return stage;
    }

    private T Own<T>(T resource) where T : Resource
    {
        resources.Add(resource);
        return resource;
    }

    private void FitPreview(Control frame)
    {
        // Fill gallery cards; fit source previews at 2:1 without making the layout's
        // minimum size depend on the previous window size.
        var stage = frame.GetChild<Control>(0);
        var size = frame.Size;
        if (SelectedEffect >= 0)
        {
            var width = Math.Min(size.X, size.Y * 2);
            size = new Vector2(width, width / 2);
        }
        stage.Size = size;
        stage.Position = (frame.Size - size) / 2;
    }
}
