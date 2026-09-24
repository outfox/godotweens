// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using godotweens;
namespace testbed;

/// <summary>One disposable playground: a grid of effect cards. Only the selected page is instantiated.</summary>
public abstract partial class GalleryPage : VBoxContainer
{
    private readonly List<Resource> resources = [];
    private GalleryEffect[] effects = [];

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

        var grid = new GridContainer { Columns = 2, SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsVertical = SizeFlags.ExpandFill };
        grid.AddThemeConstantOverride("h_separation", 14);
        grid.AddThemeConstantOverride("v_separation", 14);
        AddChild(grid);

        effects = CreateEffects();
        for (var i = 0; i < effects.Length; i++)
            effects[i].Attach(AddCard(grid, $"{i + 1:00} / {effects[i].Title}", effects[i].Caption));
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
        foreach (var effect in effects) effect.ReleaseResources();
        foreach (var resource in resources) resource.Dispose();
        resources.Clear();
    }

    /// <summary>Adds a titled card to the grid and returns its stage, the area an effect draws into.</summary>
    private Control AddCard(GridContainer grid, string title, string caption)
    {
        var panel = new PanelContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsVertical = SizeFlags.ExpandFill };
        panel.AddThemeStyleboxOverride("panel", Own(GalleryTheme.Box(Palette.Surface, 12, 1)));
        grid.AddChild(panel);

        var column = new VBoxContainer();
        column.AddThemeConstantOverride("separation", 8);
        panel.AddChild(column);
        column.AddChild(GalleryTheme.Label(title, 19));

        // Drawn clipping keeps viewports inside the rounded inset.
        var stage = new Panel
        {
            CustomMinimumSize = new Vector2(370, 168), SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill, ClipChildren = ClipChildrenMode.AndDraw, MouseFilter = MouseFilterEnum.Ignore,
        };
        stage.AddThemeStyleboxOverride("panel", Own(GalleryTheme.Box(Palette.Stage, 8)));
        column.AddChild(stage);

        var note = GalleryTheme.Label(caption, 13, Palette.Muted);
        note.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        column.AddChild(note);
        return stage;
    }

    private T Own<T>(T resource) where T : Resource
    {
        resources.Add(resource);
        return resource;
    }
}
