// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in JellyButton.cs.
public sealed partial class JellyButton
{
    private const int Points = 10;

    private async Task<bool> AutoTap()
    {
        if (!await Wait(1.5 * Tempo)) return false;
        _ = Run(Pop());
        return true;
    }

    private async Task Pop()
    {
        await Task.WhenAll([Squish(), Flash(), .. shards.Select(Throw), FloatBonus(), AddToScore()]);
    }

    /// <summary>Flattens the button, then springs it back only after successful completion.</summary>
    private async Task Squish()
    {
        squish?.Cancel();
        squish = Flatten();
        if (await Group.Of(squish).End != Reason.Completed) return;
        squish = SpringBack();
        await Group.Of(squish).End;
    }

    private async Task Flash()
    {
        var flash = 0.6 * Tempo;
        await Group.Of([
            shaker.TweenPosition(new Vector2(9, 5), 0.4 * Tempo, options =>
            {
                options.From = Vector2.Zero;
                options.EaseFunction = Shake;
            }),
            burst.TweenScale(new Vector2(2.1f, 2.1f), flash, options =>
            {
                options.From = new Vector2(0.7f, 0.7f);
                options.Ease = EaseType.QuartOut;
            }),
            burst.TweenModulateAlpha(0, flash, options =>
            {
                options.From = 1;
                options.Ease = EaseType.QuadIn;
            }),
        ]).End;
    }

    private TweenInstance Flatten() =>
        button.TweenScale(new Vector2(1.3f, 0.7f), 0.07 * Tempo, options => options.Ease = EaseType.QuadOut);

    private TweenInstance SpringBack() =>
        button.TweenScale(Vector2.One, 0.8 * Tempo, options => options.Ease = EaseType.ElasticOut);

    private async Task Throw(Polygon2D shard)
    {
        var flight = 0.9 * Tempo;
        var angle = (float)(random.NextDouble() * MathF.PI * 1.2 + MathF.PI * 0.9);
        var reach = 90 + (float)random.NextDouble() * 110;
        var landing = new Vector2(MathF.Cos(angle) * reach, 70 + (float)random.NextDouble() * 20);
        var spin = angle + (float)(random.NextDouble() * 12 - 6);
        var lift = 1 + (float)random.NextDouble();

        shard.Position = Vector2.Zero;
        shard.Rotation = angle;
        shard.Modulate = Colors.White;
        await Group.Of([
            shard.TweenPositionX(landing.X, flight, options => options.Ease = EaseType.QuartOut),
            shard.TweenPositionY(landing.Y, flight, options =>
            {
                // This curve dips below zero before landing at 1, forming a launch arc.
                options.EaseFunction = progress => (2 + lift) * progress * progress - (1 + lift) * progress;
            }),
            shard.TweenRotation(spin, flight, options => options.Ease = EaseType.QuadOut),
            shard.TweenModulateAlpha(0, 0.3 * Tempo, options => options.Delay = 0.6 * Tempo),
        ]).End;
    }

    private async Task FloatBonus()
    {
        bonus.Modulate = Colors.White;
        await Group.Of([
            bonus.TweenPositionY(-95, 0.7 * Tempo, options =>
            {
                options.From = -50;
                options.Ease = EaseType.QuartOut;
            }),
            bonus.TweenModulateAlpha(0, 0.3 * Tempo, options => options.Delay = 0.4 * Tempo),
        ]).End;
    }

    /// <summary>Rolls the displayed score up from whatever it currently shows.</summary>
    private async Task AddToScore()
    {
        total += Points;
        await Group.Of([
            Stage.TweenFloat(total, 0.5 * Tempo, options =>
            {
                options.From = shown;
                options.Ease = EaseType.CubicOut;
                options.OnUpdate = (_, value) =>
                {
                    shown = value;
                    score.Text = $"{value:000}";
                };
            }),
            score.TweenScale(Vector2.One, 0.6 * Tempo, options =>
            {
                options.From = new Vector2(1.45f, 1.45f);
                options.Ease = EaseType.ElasticOut;
            }),
        ]).End;
    }

    // A decaying oscillation that finishes at the starting value.
    private static float Shake(float progress) =>
        MathF.Sin(progress * 42) * (1 - progress) * (1 - progress);
}
