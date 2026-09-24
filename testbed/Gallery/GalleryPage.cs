// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using godotweens;
namespace testbed;

/// <summary>One disposable playground. Only the selected page is instantiated.</summary>
public abstract partial class GalleryPage : VBoxContainer
{
    public static readonly Color Mint = new("79deb4"), Amber = new("f2bc74"), Blue = new("8caaff"), Muted = new("a9bccd"), Soft = GalleryTheme.Soft;
    private readonly List<TweenInstance> handles = [];
    private readonly List<Resource> resources = [];
    protected GridContainer Grid = null!;
    protected double Seconds;
    protected EaseType Ease;
    protected bool PingPong;
    protected int Generation;
    public Task? SequenceTask { get; protected set; }
    public int ActiveCount => handles.Count(t => !t.IsTerminal);
    public bool AllPaused => handles.Where(t => !t.IsTerminal).All(t => t.IsPaused);
    public string? Error => handles.FirstOrDefault(t => t.Error is not null)?.Error?.Message;
    public abstract string Heading { get; }
    public abstract string Description { get; }

    public override void _Ready()
    {
        AddThemeConstantOverride("separation", 10);
        AddChild(Text(Heading, 28));
        AddChild(Text(Description, 15, Muted));
        Grid = new GridContainer { Columns = 2, SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsVertical = SizeFlags.ExpandFill };
        Grid.AddThemeConstantOverride("h_separation", 14); Grid.AddThemeConstantOverride("v_separation", 14);
        AddChild(Grid);
        Build();
    }
    protected abstract void Build();
    protected abstract void Animate();
    public void Start(double seconds, EaseType ease, bool pingPong)
    {
        Seconds = seconds; Ease = ease; PingPong = pingPong;
        Animate();
    }
    protected void Cycle(TweenOptions d)
    {
        d.Ease = Ease; d.UsePingPong = PingPong; d.IsInfinite = true;
        d.RepeatInterval = 0.25; d.PingPongInterval = 0.15;
    }
    protected T Keep<T>(T handle) where T : TweenInstance
    {
        // Looping sequences create handles indefinitely; faulted ones stay so Error can report them.
        if (handles.Count > 256) handles.RemoveAll(h => h.State is TweenState.Completed or TweenState.Cancelled);
        handles.Add(handle); return handle;
    }
    /// <summary>Timing scale relative to the default leg duration, for choreographed sequences.</summary>
    protected double Tempo => Seconds / 1.8;
    /// <summary>True when every tween completed and the page has not been stopped or restarted since <paramref name="run"/>.</summary>
    protected async Task<bool> All(int run, params TweenInstance[] tweens)
    {
        var results = await Task.WhenAll(tweens.Select(t => t.Completion));
        return run == Generation && results.All(r => r == TweenCompletionReason.Completed);
    }
    protected Task<bool> Wait(int run, double seconds) => All(run, Keep(this.TweenFloat(1, seconds, d => d.From = 0)));
    /// <summary>Repeats an async step until it reports false, e.g. after Stop.</summary>
    protected async Task Repeat(Func<int, Task<bool>> step)
    {
        var run = Generation;
        try { while (await step(run)) { } }
        catch (Exception error) { GD.PushError(error.ToString()); }
    }
    /// <summary>Decaying oscillation that ends at the start value; use as EaseFunction for shakes.</summary>
    protected static float Shake(float t) => MathF.Sin(t * 42) * (1 - t) * (1 - t);
    protected T Own<T>(T resource) where T : Resource { resources.Add(resource); return resource; }
    public virtual void Pause(bool pause) { foreach (var h in handles) h.IsPaused = pause; }
    public virtual void Stop()
    {
        Generation++;
        foreach (var h in handles.ToArray()) h.Cancel();
        handles.Clear();
    }
    public override void _ExitTree() => Stop();
    // Called after Free(), so children have relinquished their native resource references.
    public void ReleaseResources() { foreach (var r in resources) r.Dispose(); resources.Clear(); }

    public static Label Text(string text, int size = 16, Color? color = null)
    {
        var label = new Label { Text = text, MouseFilter = MouseFilterEnum.Ignore };
        label.AddThemeFontSizeOverride("font_size", size);
        label.AddThemeColorOverride("font_color", color ?? GalleryTheme.Text); return label;
    }
    public static StyleBoxFlat Box(Color color, int radius = 12) => GalleryTheme.Box(color, radius);
    protected Control Card(string title, string caption)
    {
        var panel = new PanelContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsVertical = SizeFlags.ExpandFill };
        panel.AddThemeStyleboxOverride("panel", Own(GalleryTheme.Box(GalleryTheme.Surface, 12, 1))); Grid.AddChild(panel);
        var column = new VBoxContainer(); column.AddThemeConstantOverride("separation", 8); panel.AddChild(column);
        column.AddChild(Text(title, 19));
        // Drawn clipping keeps viewports inside the rounded inset.
        var stage = new Panel { CustomMinimumSize = new Vector2(370, 168), SizeFlagsVertical = SizeFlags.ExpandFill,
            SizeFlagsHorizontal = SizeFlags.ExpandFill, ClipChildren = ClipChildrenMode.AndDraw, MouseFilter = MouseFilterEnum.Ignore };
        stage.AddThemeStyleboxOverride("panel", Own(GalleryTheme.Box(GalleryTheme.Stage, 8)));
        column.AddChild(stage);
        var note = Text(caption, 13, Muted); note.AutowrapMode = TextServer.AutowrapMode.WordSmart; column.AddChild(note);
        return stage;
    }
    protected SubViewport View(Control stage, bool spatial = false)
    {
        var container = new SubViewportContainer { Stretch = true, MouseFilter = MouseFilterEnum.Ignore };
        stage.AddChild(container); container.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        var viewport = new SubViewport { Size = new Vector2I(440, 180), OwnWorld3D = spatial,
            TransparentBg = !spatial, RenderTargetUpdateMode = SubViewport.UpdateMode.Always,
            HandleInputLocally = false };
        container.AddChild(viewport);
        if (!spatial) viewport.AddChild(new Camera2D { Position = Vector2.Zero });
        return viewport;
    }
    protected (SubViewport View, Camera3D Camera) World(Control stage)
    {
        var view = View(stage, true);
        var environment = Own(new Godot.Environment { BackgroundMode = Godot.Environment.BGMode.Color,
            BackgroundColor = GalleryTheme.Stage, AmbientLightSource = Godot.Environment.AmbientSource.Color,
            AmbientLightColor = new Color("9eb6e3"), AmbientLightEnergy = 0.3f,
            TonemapMode = Godot.Environment.ToneMapper.Filmic, GlowEnabled = true, GlowIntensity = 0.9f,
            GlowBloom = 0.02f, GlowHdrThreshold = 0.9f,
            FogEnabled = true, FogLightColor = GalleryTheme.Stage, FogDensity = 0.09f });
        view.AddChild(new WorldEnvironment { Environment = environment });
        var camera = new Camera3D { Position = new Vector3(0, 0.8f, 3.6f), Fov = 43, Current = true };
        view.AddChild(camera); camera.LookAt(Vector3.Zero);
        view.AddChild(new DirectionalLight3D { RotationDegrees = new Vector3(-50, -30, 0), LightEnergy = 0.9f, ShadowEnabled = true });
        return (view, camera);
    }
    /// <summary>A matte ground plane close to the stage color, so objects sit on a surface and receive shadows.</summary>
    protected MeshInstance3D Floor(Node parent, float y)
        => Mesh(parent, new PlaneMesh { Size = new Vector2(16, 10) }, Own(new StandardMaterial3D { AlbedoColor = new Color("1b283a"),
            Roughness = 0.85f }), new Vector3(0, y, 0));
    protected StandardMaterial3D Surface(Color color) => Own(new StandardMaterial3D { AlbedoColor = color, Roughness = 0.35f });
    protected MeshInstance3D Mesh(Node parent, Godot.Mesh mesh, Material material, Vector3 position = default)
    {
        var node = new MeshInstance3D { Mesh = Own(mesh), MaterialOverride = material, Position = position };
        parent.AddChild(node); return node;
    }
    protected static Polygon2D Diamond(Node parent, Vector2 position, Color color, float radius = 16)
    {
        var node = new Polygon2D { Polygon = [new(0, -radius), new(radius, 0), new(0, radius), new(-radius, 0)],
            Position = position, Color = color }; parent.AddChild(node); return node;
    }
    protected static Vector2[] Ellipse(float rx, float ry, int segments = 32, Vector2 center = default)
        => Enumerable.Range(0, segments).Select(i => center + new Vector2(MathF.Cos(i * MathF.Tau / segments) * rx,
            MathF.Sin(i * MathF.Tau / segments) * ry)).ToArray();
    /// <summary>Rounded rectangle outline with half extents <paramref name="w"/> by <paramref name="h"/>.</summary>
    protected static Vector2[] Rounded(float w, float h, float r)
    {
        Vector2[] corners = [new(w - r, h - r), new(-w + r, h - r), new(-w + r, -h + r), new(w - r, -h + r)];
        return corners.SelectMany((c, k) => Enumerable.Range(0, 7).Select(i =>
        {
            var a = (k + i / 6f) * MathF.PI / 2;
            return c + new Vector2(MathF.Cos(a), MathF.Sin(a)) * r;
        })).ToArray();
    }
    protected static Polygon2D Blob(Node parent, float rx, float ry, Color color, Vector2 position = default)
    {
        var node = new Polygon2D { Polygon = Ellipse(rx, ry), Color = color, Position = position, Antialiased = true };
        parent.AddChild(node); return node;
    }
    protected static Line2D Line(Node parent, Vector2[] points, Color color, float width = 2)
    {
        var line = new Line2D { Points = points, DefaultColor = color, Width = width, Antialiased = true };
        parent.AddChild(line); return line;
    }
}
