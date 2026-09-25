// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

public sealed partial class JellyButton : GalleryEffect
{
    private static readonly Color[] Confetti = [Palette.Mint, Palette.Amber, Palette.Blue, new("ff8fa3"), Colors.White];

    private readonly Random random = new();
    private Control shaker = null!;
    private Button button = null!;
    private Line2D burst = null!;
    private Polygon2D[] shards = [];
    private Label bonus = null!, score = null!;
    private TweenInstance? squish;
    private float total, shown;

    public override string Title => "Jelly Button";
    public override string Caption => "Click it! Elastic pop, arcing confetti, rolling score, and shake.";

    protected override void Build()
    {
        var center = Stage.Add(new Control { MouseFilter = Control.MouseFilterEnum.Ignore });
        center.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.Center);
        shaker = center.Add(new Control { MouseFilter = Control.MouseFilterEnum.Ignore });

        var burstLayer = shaker.Add(new Node2D { Position = new Vector2(0, 8) });
        burst = burstLayer.Add(new Line2D
        {
            Points = Ellipse(60, 30, 48), Closed = true, DefaultColor = Palette.Mint, Width = 4, Antialiased = true,
            Modulate = Colors.Transparent,
        });
        shards = Enumerable.Range(0, 18).Select(i => burstLayer.Add(new Polygon2D
        {
            Polygon = [new(-5, -2.5f), new(5, -2.5f), new(5, 2.5f), new(-5, 2.5f)],
            Color = Confetti[i % Confetti.Length], Modulate = Colors.Transparent,
        })).ToArray();

        button = shaker.Add(BuildButton());
        button.Pressed += () => Pop(Generation);

        bonus = GalleryTheme.Label($"+{Points}", 22, Palette.Amber);
        bonus.Modulate = Colors.Transparent;
        bonus.Position = new Vector2(-18, -60);
        shaker.AddChild(bonus);

        BuildScoreboard();
    }

    private Button BuildButton()
    {
        var tap = new Button
        {
            Text = "TAP", FocusMode = Control.FocusModeEnum.None, Size = new Vector2(150, 58),
            Position = new Vector2(-75, -21), PivotOffset = new Vector2(75, 58),
        };
        tap.AddThemeFontSizeOverride("font_size", 24);
        foreach (var state in new[] { "normal", "hover", "pressed", "hover_pressed" })
        {
            var box = Own(GalleryTheme.Box(state.Contains("hover") ? new Color("97ecc8") : Palette.Mint, 18, 0));
            box.ShadowColor = new Color(0, 0, 0, 0.35f);
            box.ShadowSize = 8;
            box.ShadowOffset = new Vector2(0, 4);
            tap.AddThemeStyleboxOverride(state, box);
        }
        foreach (var color in new[] { "font_color", "font_hover_color", "font_pressed_color", "font_hover_pressed_color" })
            tap.AddThemeColorOverride(color, Palette.Background);
        return tap;
    }

    private void BuildScoreboard()
    {
        var caption = GalleryTheme.Label("SCORE", 12, Palette.Muted);
        Stage.AddChild(caption);
        caption.SetAnchorsPreset(Control.LayoutPreset.TopRight);
        (caption.OffsetLeft, caption.OffsetRight, caption.OffsetTop) = (-110, -64, 19);

        score = GalleryTheme.Label("000", 26);
        score.HorizontalAlignment = HorizontalAlignment.Right;
        Stage.AddChild(score);
        score.SetAnchorsPreset(Control.LayoutPreset.TopRight);
        (score.OffsetLeft, score.OffsetRight, score.OffsetTop, score.OffsetBottom) = (-64, -14, 6, 42);
        score.PivotOffset = new Vector2(36, 18);
    }

    protected override void Animate() => Sequence = Repeat(AutoTap);

    private async Task<bool> AutoTap(int run)
    {
        if (!await Wait(run, 1.5 * Tempo)) return false;
        Pop(run);
        return true;
    }

    private void Pop(int run)
    {
        Squish(run);
        TrackTweens(Flash());
        foreach (var shard in shards) TrackTweens(Throw(shard));
        TrackTweens(FloatBonus());
        TrackTweens(AddToScore());
    }

    /// <summary>Flattens the button, then springs it back unless the effect was stopped in between.</summary>
    private void Squish(int run)
    {
        squish?.Cancel();
        squish = Keep(Flatten(() =>
        {
            if (run == Generation) squish = Keep(SpringBack());
        }));
    }
}
