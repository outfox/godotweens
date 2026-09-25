// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in JellyButton.cs.
public sealed partial class JellyButton
{
    private const int Points = 10;

    private IEnumerable<TweenInstance> Flash()
    {
        yield return shaker.TweenPosition(new Vector2(9, 5), 0.4 * Tempo, t => { t.From = Vector2.Zero; t.EaseFunction = Shake; });
        var flash = 0.6 * Tempo;
        yield return burst.TweenScale(new Vector2(2.1f, 2.1f), flash, t => { t.From = new Vector2(0.7f, 0.7f); t.Ease = EaseType.QuartOut; });
        yield return burst.TweenModulateAlpha(0, flash, t => { t.From = 1; t.Ease = EaseType.QuadIn; });
    }

    /// <summary>Invokes the next phase when the flattening tween ends.</summary>
    private TweenInstance Flatten(Action onEnd) =>
        button.TweenScale(new Vector2(1.3f, 0.7f), 0.07 * Tempo, t =>
        {
            t.Ease = EaseType.QuadOut;
            t.OnEnd = _ => onEnd();
        });

    private TweenInstance SpringBack() =>
        button.TweenScale(Vector2.One, 0.8 * Tempo, t => t.Ease = EaseType.ElasticOut);

    private IEnumerable<TweenInstance> Throw(Polygon2D shard)
    {
        var flight = 0.9 * Tempo;
        var angle = (float)(random.NextDouble() * MathF.PI * 1.2 + MathF.PI * 0.9);
        var reach = 90 + (float)random.NextDouble() * 110;
        var landing = new Vector2(MathF.Cos(angle) * reach, 70 + (float)random.NextDouble() * 20);
        var spin = angle + (float)(random.NextDouble() * 12 - 6);
        var lift = 1 + (float)random.NextDouble();
        // (2 + lift)w² − (1 + lift)w dips below zero before landing at 1: a launch arc from a single scalar tween.
        float Arc(float w) => (2 + lift) * w * w - (1 + lift) * w;

        shard.Position = Vector2.Zero;
        shard.Rotation = angle;
        shard.Modulate = Colors.White;
        yield return shard.TweenPositionX(landing.X, flight, t => t.Ease = EaseType.QuartOut);
        yield return shard.TweenPositionY(landing.Y, flight, t => t.EaseFunction = Arc);
        yield return shard.TweenRotation(spin, flight, t => t.Ease = EaseType.QuadOut);
        yield return shard.TweenModulateAlpha(0, 0.3 * Tempo, t => t.Delay = 0.6 * Tempo);
    }

    private IEnumerable<TweenInstance> FloatBonus()
    {
        bonus.Modulate = Colors.White;
        yield return bonus.TweenPositionY(-95, 0.7 * Tempo, t => { t.From = -50; t.Ease = EaseType.QuartOut; });
        yield return bonus.TweenModulateAlpha(0, 0.3 * Tempo, t => t.Delay = 0.4 * Tempo);
    }

    /// <summary>Rolls the displayed score up from whatever it currently shows.</summary>
    private IEnumerable<TweenInstance> AddToScore()
    {
        total += Points;
        yield return Stage.TweenFloat(total, 0.5 * Tempo, t =>
        {
            t.From = shown;
            t.Ease = EaseType.CubicOut;
            t.OnUpdate = (_, value) =>
            {
                shown = value;
                score.Text = $"{value:000}";
            };
        });
        yield return score.TweenScale(Vector2.One, 0.6 * Tempo, t => { t.From = new Vector2(1.45f, 1.45f); t.Ease = EaseType.ElasticOut; });
    }
}
