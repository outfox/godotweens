// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using godotweens;
namespace testbed;

public sealed class SlimeHop : GalleryEffect
{
    private const float Ground = 52, Apex = Ground - 78, Stride = 90, Bounds = 136;
    private static readonly Vector2 Crouched = new(1.38f, 0.6f), Launched = new(0.68f, 1.45f), Falling = new(0.82f, 1.25f),
        Squashed = new(1.6f, 0.5f);

    private Node2D slime = null!, body = null!, eyes = null!, pupils = null!;
    private Polygon2D[] drops = [];
    private int direction = 1, hops;

    public override string Title => "Slime Hop";
    public override string Caption => "Anticipation, launch stretch, flips, elastic landing, blinking eyes.";

    protected override void Build()
    {
        var view = View();
        Line(view, [new(-210, Ground), new(210, Ground)], Palette.Outline, 2);
        drops = Enumerable.Range(0, 5).Select(_ => Blob(view, 4, 4, Palette.Mint)).ToArray();
        foreach (var drop in drops) drop.Modulate = Colors.Transparent;

        slime = view.Add(new Node2D { Position = new Vector2(-135, Ground) });
        body = slime.Add(new Node2D { Position = new Vector2(0, -20) });
        body.Add(new Polygon2D { Polygon = Dome(), Color = Palette.Mint, Antialiased = true });
        Blob(body, 8, 4, new Color(1, 1, 1, 0.5f), new Vector2(-17, -24)).Rotation = -0.5f;

        eyes = body.Add(new Node2D { Position = new Vector2(0, -8) });
        foreach (var x in new[] { -11f, 11f }) Blob(eyes, 6, 7.5f, Colors.White, new Vector2(x, 0));
        pupils = eyes.Add(new Node2D());
        foreach (var x in new[] { -11f, 11f }) Blob(pupils, 3.2f, 4, Palette.Background, new Vector2(x + 1, 1));
    }

    /// <summary>Round on top, nearly flat underneath, centered on the body's pivot.</summary>
    private static Vector2[] Dome() => Enumerable.Range(0, 40)
        .Select(i => i * MathF.Tau / 40)
        .Select(a => new Vector2(MathF.Cos(a) * 36, MathF.Sin(a) * (MathF.Sin(a) > 0 ? 20 : 38)))
        .ToArray();

    protected override void Animate()
    {
        Keep(eyes.TweenScaleY(0.1f, 0.07, t => { t.UsePingPong = true; t.IsInfinite = true; t.RepeatInterval = 2.2; t.Delay = 0.9; }));
        Sequence = Repeat(Hop);
    }

    private async Task<bool> Hop(int run)
    {
        var air = 0.32 * Tempo;
        if (Math.Abs(slime.Position.X + direction * Stride) > Bounds) direction = -direction;
        var target = slime.Position.X + direction * Stride;

        Keep(pupils.TweenPositionX(direction * 3, 0.2 * Tempo, t => t.Ease = EaseType.BackOut));
        if (!await Finished(run, Keep(slime.TweenScale(Crouched, 0.32 * Tempo, t => t.Ease = EaseType.SineOut)))) return false;
        if (!await Finished(run, Keep(slime.TweenScale(Launched, 0.08 * Tempo, t => t.Ease = EaseType.QuadOut)))) return false;

        Keep(slime.TweenPositionX(target, air * 2, t => t.Ease = EaseType.SineInOut));
        if (++hops % 2 == 0)
            Keep(body.TweenRotation(direction * MathF.Tau, air * 2, t => { t.From = 0; t.Ease = EaseType.CubicInOut; }));

        void Rising(TweenOptions t) => t.Ease = EaseType.QuadOut;
        var rise = Finished(run,
            Keep(slime.TweenPositionY(Apex, air, Rising)),
            Keep(slime.TweenScale(Vector2.One, air, Rising)));
        if (!await rise) return false;

        void Dropping(TweenOptions t) => t.Ease = EaseType.QuadIn;
        var fall = Finished(run,
            Keep(slime.TweenPositionY(Ground, air, Dropping)),
            Keep(slime.TweenScale(Falling, air, Dropping)));
        if (!await fall) return false;

        Splash(target);
        if (!await Finished(run, Keep(slime.TweenScale(Squashed, 0.06 * Tempo, t => t.Ease = EaseType.QuadOut)))) return false;
        return await Finished(run, Keep(slime.TweenScale(Vector2.One, 0.75 * Tempo, t => t.Ease = EaseType.ElasticOut)));
    }

    /// <summary>Flings the droplets outward along the upper half of an ellipse.</summary>
    private void Splash(float x)
    {
        var duration = 0.4 * Tempo;
        var origin = new Vector2(x, Ground - 6);
        for (var i = 0; i < drops.Length; i++)
        {
            var angle = MathF.PI + (i + 0.5f) / drops.Length * MathF.PI;
            var landing = origin + new Vector2(MathF.Cos(angle) * 58, MathF.Sin(angle) * 34);
            drops[i].Position = origin;
            drops[i].Scale = Vector2.One * (i % 2 == 0 ? 1 : 0.7f);
            Keep(drops[i].TweenPosition(landing, duration, t => t.Ease = EaseType.QuartOut));
            Keep(drops[i].TweenModulateAlpha(0, duration, t => { t.From = 1; t.Ease = EaseType.CubicIn; }));
        }
    }
}
