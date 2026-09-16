// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using GodotWeens;

namespace testbed;

/// <summary>Interactive consumer of the library; all motion is driven by godotweens.</summary>
public partial class TweenDemo : Control
{
    private readonly List<TweenInstance> handles = [];
    private readonly Color mint = new("79deb4");
    private readonly Color amber = new("f2bc74");
    private Polygon2D movement = null!, transform = null!, chain = null!;
    private ColorRect color = null!;
    private Control movementLane = null!, chainLane = null!;
    private Label status = null!, durationLabel = null!, chainLabel = null!;
    private Button pause = null!;
    private OptionButton easeChoice = null!;
    private HSlider duration = null!;
    private CheckBox pingPong = null!;
    private int generation;
    private bool paused;
    public bool IsPlaying { get; private set; }
    public int DemoTweenCount => handles.FindAll(t => !t.IsTerminal).Count;
    public Task? ChainTask { get; private set; }

    public override void _Ready()
    {
        BuildControls();
        Resized += () => { if (IsNodeReady() && IsPlaying) RestartDemo(); };
        Callable.From(RestartDemo).CallDeferred();
    }

    private static Label Text(string text, int size, Color? tint = null)
    {
        var label = new Label { Text = text };
        label.AddThemeFontSizeOverride("font_size", size);
        if (tint.HasValue) label.AddThemeColorOverride("font_color", tint.Value);
        return label;
    }

    private void BuildControls()
    {
        var background = new ColorRect { Color = new Color("17222b"), MouseFilter = MouseFilterEnum.Ignore };
        AddChild(background);
        background.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        var margin = new MarginContainer(); AddChild(margin);
        margin.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        foreach (var side in new[] { "left", "right", "top", "bottom" }) margin.AddThemeConstantOverride("margin_" + side, 32);
        var layout = new VBoxContainer(); layout.AddThemeConstantOverride("separation", 12); margin.AddChild(layout);
        var title = new HBoxContainer(); layout.AddChild(title);
        var heading = Text("godotweens", 36); heading.SizeFlagsHorizontal = SizeFlags.ExpandFill; title.AddChild(heading);
        title.AddChild(Text("C# / GODOT", 16, mint));
        layout.AddChild(Text("Change the timing. Watch the same tween system drive every example.", 18, new Color("b6c8d2")));
        layout.AddChild(new HSeparator());

        var settings = new HBoxContainer(); settings.AddThemeConstantOverride("separation", 16); layout.AddChild(settings);
        settings.AddChild(Text("Ease", 18));
        easeChoice = new OptionButton { CustomMinimumSize = new Vector2(160, 40) };
        foreach (var ease in new[] { EaseType.CubicInOut, EaseType.Linear, EaseType.BackOut, EaseType.ElasticOut, EaseType.BounceOut })
            easeChoice.AddItem(ease.ToString(), (int)ease);
        settings.AddChild(easeChoice);
        durationLabel = Text("Duration 1.4 s", 18); settings.AddChild(durationLabel);
        duration = new HSlider { MinValue = 0.2, MaxValue = 3, Step = 0.1, Value = 1.4,
            CustomMinimumSize = new Vector2(140, 40), SizeFlagsHorizontal = SizeFlags.ExpandFill,
            TooltipText = "Seconds per forward or backward leg" };
        settings.AddChild(duration);
        pingPong = new CheckBox { Text = "Ping-pong", ButtonPressed = true }; settings.AddChild(pingPong);

        (movementLane, _) = Row(layout, "Position", "Node2D.Position");
        movement = Marker(movementLane, mint);
        (var transformLane, _) = Row(layout, "Scale & rotation", "Two tweens, one node");
        transform = Marker(transformLane, amber); transform.Position = new Vector2(80, 40);
        (var colorLane, _) = Row(layout, "Color & opacity", "CanvasItem.Modulate");
        color = new ColorRect { Position = new Vector2(20, 12), Size = new Vector2(160, 56), Color = Colors.White };
        colorLane.AddChild(color);
        (chainLane, var chainRow) = Row(layout, "Async sequence", "await Completion");
        chain = Marker(chainLane, mint);
        chainLabel = Text("Move → return → done", 16, new Color("b6c8d2")); chainRow.AddChild(chainLabel);

        var actions = new HBoxContainer(); actions.AddThemeConstantOverride("separation", 12); layout.AddChild(actions);
        pause = AddButton(actions, "Pause", TogglePause);
        AddButton(actions, "Cancel", StopDemo);
        AddButton(actions, "Restart", RestartDemo);
        status = Text("Starting…", 18, mint); status.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        status.HorizontalAlignment = HorizontalAlignment.Right; actions.AddChild(status);
        layout.AddChild(Text("Pause holds every active tween. Cancel keeps its current value. Restart captures fresh values.", 16,
            new Color("b6c8d2")));
        easeChoice.ItemSelected += _ => RestartDemo();
        duration.ValueChanged += value => { durationLabel.Text = $"Duration {value:0.0} s"; RestartDemo(); };
        pingPong.Toggled += _ => RestartDemo();
    }

    private static (Control Lane, VBoxContainer Caption) Row(VBoxContainer layout, string title, string subtitle)
    {
        var row = new HBoxContainer { SizeFlagsVertical = SizeFlags.ExpandFill, CustomMinimumSize = new Vector2(0, 84) };
        layout.AddChild(row);
        var caption = new VBoxContainer { CustomMinimumSize = new Vector2(240, 0) };
        caption.AddChild(Text(title, 22)); caption.AddChild(Text(subtitle, 16, new Color("b6c8d2")));
        row.AddChild(caption);
        var lane = new Control { SizeFlagsHorizontal = SizeFlags.ExpandFill, ClipContents = true };
        row.AddChild(lane);
        var track = new ColorRect { Color = new Color("33434e"), Position = new Vector2(0, 39), Size = new Vector2(10000, 2), MouseFilter = MouseFilterEnum.Ignore };
        lane.AddChild(track);
        return (lane, caption);
    }

    private static Polygon2D Marker(Control lane, Color tint)
    {
        var marker = new Polygon2D { Polygon = [new(-16, -16), new(16, -16), new(16, 16), new(-16, 16)], Color = tint };
        lane.AddChild(marker); return marker;
    }

    private static Button AddButton(HBoxContainer row, string label, Action action)
    {
        var button = new Button { Text = label, CustomMinimumSize = new Vector2(108, 42) };
        button.Pressed += action; row.AddChild(button); return button;
    }

    public void RestartDemo()
    {
        if (!IsInsideTree() || movement is null) return;
        StopDemo();
        IsPlaying = true;
        paused = false;
        pause.Text = "Pause";
        pause.Disabled = false;
        var seconds = duration.Value;
        var ease = (EaseType)easeChoice.GetSelectedId();
        movement.Position = new Vector2(24, 40);
        transform.Scale = Vector2.One; transform.Rotation = 0;
        color.Modulate = mint;
        chain.Position = new Vector2(24, 40);
        handles.Add(movement.Tween(new Position2DTween { To = new Vector2(Math.Max(60, movementLane.Size.X - 36), 40),
            Duration = seconds, Ease = ease, UsePingPong = pingPong.ButtonPressed, IsInfinite = true, RepeatInterval = 0.3, PingPongInterval = 0.2 }));
        handles.Add(transform.Tween(new Scale2DTween { To = new Vector2(1.6f, 1.6f), Duration = seconds, Ease = ease, UsePingPong = true, IsInfinite = true }));
        handles.Add(transform.Tween(new Rotation2DTween { To = Mathf.Pi, Duration = seconds, Ease = ease, IsInfinite = true }));
        handles.Add(color.Tween(new ModulateTween { To = new Color(amber.R, amber.G, amber.B, 0.2f),
            Duration = seconds, Ease = ease, UsePingPong = true, IsInfinite = true }));
        ChainTask = RunChain(generation, seconds, ease);
        status.Text = "Playing";
    }

    private async Task RunChain(int run, double seconds, EaseType ease)
    {
        try
        {
            chainLabel.Text = "Moving out…";
            var outward = chain.Tween(new Position2DTween { To = new Vector2(Math.Max(60, chainLane.Size.X - 36), 40), Duration = seconds, Ease = ease });
            handles.Add(outward);
            if (await outward.Completion != TweenCompletionReason.Completed || run != generation) return;
            chainLabel.Text = "Returning…";
            var inward = chain.Tween(new Position2DTween { To = new Vector2(24, 40), Duration = seconds, Ease = ease });
            handles.Add(inward);
            if (await inward.Completion != TweenCompletionReason.Completed || run != generation) return;
            chainLabel.Text = "Sequence complete";
        }
        catch (Exception error)
        {
            if (run == generation && IsInsideTree()) status.Text = "Sequence failed — restart to retry";
            GD.PushError(error.ToString());
        }
    }

    public void TogglePause()
    {
        if (!IsPlaying) return;
        paused = !paused;
        foreach (var tween in handles) tween.IsPaused = paused;
        pause.Text = paused ? "Resume" : "Pause";
        status.Text = paused ? "Paused" : "Playing";
    }

    public void StopDemo()
    {
        generation++;
        IsPlaying = false;
        foreach (var tween in handles) tween.Cancel();
        handles.Clear();
        if (status is not null) status.Text = "Cancelled — restart to play";
        if (chainLabel is not null) chainLabel.Text = "Sequence cancelled";
        if (pause is not null) { pause.Text = "Pause"; pause.Disabled = true; }
    }

    public override void _ExitTree() => StopDemo();
}
