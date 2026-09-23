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
    public static readonly Color Mint = new("79deb4"), Amber = new("f2bc74"), Blue = new("8caaff"), Muted = new("9aaebf");
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
    protected T Keep<T>(T handle) where T : TweenInstance { handles.Add(handle); return handle; }
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
        label.AddThemeColorOverride("font_color", color ?? new Color("eef4fa")); return label;
    }
    public static StyleBoxFlat Box(Color color, int radius = 12)
        => new() { BgColor = color, CornerRadiusTopLeft = radius, CornerRadiusTopRight = radius,
            CornerRadiusBottomLeft = radius, CornerRadiusBottomRight = radius,
            ContentMarginLeft = 16, ContentMarginRight = 16, ContentMarginTop = 14, ContentMarginBottom = 14 };
    protected Control Card(string title, string caption)
    {
        var panel = new PanelContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsVertical = SizeFlags.ExpandFill };
        panel.AddThemeStyleboxOverride("panel", Own(Box(new Color("1d2b3b")))); Grid.AddChild(panel);
        var column = new VBoxContainer(); column.AddThemeConstantOverride("separation", 8); panel.AddChild(column);
        column.AddChild(Text(title, 19));
        var stage = new Control { CustomMinimumSize = new Vector2(370, 168), SizeFlagsVertical = SizeFlags.ExpandFill,
            SizeFlagsHorizontal = SizeFlags.ExpandFill, ClipContents = true };
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
            BackgroundColor = new Color("142030"), AmbientLightSource = Godot.Environment.AmbientSource.Color,
            AmbientLightColor = new Color("9eb6e3"), AmbientLightEnergy = 0.3f,
            TonemapMode = Godot.Environment.ToneMapper.Filmic });
        view.AddChild(new WorldEnvironment { Environment = environment });
        var camera = new Camera3D { Position = new Vector3(0, 0.8f, 3.6f), Fov = 43, Current = true };
        view.AddChild(camera); camera.LookAt(Vector3.Zero);
        view.AddChild(new DirectionalLight3D { RotationDegrees = new Vector3(-35, -30, 0), LightEnergy = 0.9f });
        return (view, camera);
    }
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
    protected static Line2D Line(Node parent, Vector2[] points, Color color, float width = 2)
    {
        var line = new Line2D { Points = points, DefaultColor = color, Width = width, Antialiased = true };
        parent.AddChild(line); return line;
    }
}
