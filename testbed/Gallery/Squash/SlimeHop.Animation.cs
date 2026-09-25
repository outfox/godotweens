// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in SlimeHop.cs.
public sealed partial class SlimeHop
{
    private async Task AnimateAsync()
    {
        var blinking = Group.Of(Blink()).End;
        await Task.WhenAll(blinking, Repeat(Hop));
    }

    private async Task<bool> Hop()
    {
        var air = 0.32 * Tempo;
        if (Math.Abs(slime.Position.X + direction * Stride) > Bounds) direction = -direction;
        var target = slime.Position.X + direction * Stride;

        _ = Run(LookAhead().End);
        if (await Crouch().End != Reason.Completed) return false;
        if (await Launch().End != Reason.Completed) return false;

        _ = Run(Travel(target, air * 2));
        if (!await Rise(air)) return false;
        if (!await Fall(air)) return false;

        _ = Run(Splash(target));
        if (await Squash().End != Reason.Completed) return false;
        return await Recover().End == Reason.Completed;
    }

    private const float Ground = 52, Apex = Ground - 78, Stride = 90, Bounds = 136;
    private static readonly Vector2 Crouched = new(1.38f, 0.6f), Launched = new(0.68f, 1.45f), Falling = new(0.82f, 1.25f),
        Squashed = new(1.6f, 0.5f);

    private TweenInstance Blink() =>
        eyes.TweenScaleY(0.1f, 0.07, options =>
        {
            options.UsePingPong = true;
            options.Repeats = TweenOptions.Infinite;
            options.RepeatInterval = 2.2;
            options.Delay = 0.9;
        });

    private TweenInstance LookAhead() =>
        pupils.TweenPositionX(direction * 3, 0.2 * Tempo, options => options.Ease = EaseType.BackOut);

    private TweenInstance Crouch() =>
        slime.TweenScale(Crouched, 0.32 * Tempo, options => options.Ease = EaseType.SineOut);

    private TweenInstance Launch() =>
        slime.TweenScale(Launched, 0.08 * Tempo, options => options.Ease = EaseType.QuadOut);

    private async Task Travel(float target, double duration)
    {
        var tweens = new List<TweenInstance>();
        tweens.Add(slime.TweenPositionX(target, duration, options => options.Ease = EaseType.SineInOut));
        if (++hops % 2 == 0)
            tweens.Add(body.TweenRotation(direction * MathF.Tau, duration, options =>
            {
                options.From = 0;
                options.Ease = EaseType.CubicInOut;
            }));
        await Group.Of([.. tweens]).End;
    }

    private async Task<bool> Rise(double air)
    {
        return await Group.Of([
            slime.TweenPositionY(Apex, air, options => options.Ease = EaseType.QuadOut),
            slime.TweenScale(Vector2.One, air, options => options.Ease = EaseType.QuadOut),
        ]).End == Reason.Completed;
    }

    private async Task<bool> Fall(double air)
    {
        return await Group.Of([
            slime.TweenPositionY(Ground, air, options => options.Ease = EaseType.QuadIn),
            slime.TweenScale(Falling, air, options => options.Ease = EaseType.QuadIn),
        ]).End == Reason.Completed;
    }

    private TweenInstance Squash() =>
        slime.TweenScale(Squashed, 0.06 * Tempo, options => options.Ease = EaseType.QuadOut);

    private TweenInstance Recover() =>
        slime.TweenScale(Vector2.One, 0.75 * Tempo, options => options.Ease = EaseType.ElasticOut);

    /// <summary>Flings the droplets outward along the upper half of an ellipse.</summary>
    private async Task Splash(float x)
    {
        var tweens = new List<TweenInstance>();
        var duration = 0.4 * Tempo;
        var origin = new Vector2(x, Ground - 6);
        for (var i = 0; i < drops.Length; i++)
        {
            var angle = MathF.PI + (i + 0.5f) / drops.Length * MathF.PI;
            var landing = origin + new Vector2(MathF.Cos(angle) * 58, MathF.Sin(angle) * 34);
            drops[i].Position = origin;
            drops[i].Scale = Vector2.One * (i % 2 == 0 ? 1 : 0.7f);
            tweens.Add(drops[i].TweenPosition(landing, duration, options => options.Ease = EaseType.QuartOut));
            tweens.Add(drops[i].TweenModulateAlpha(0, duration, options =>
            {
                options.From = 1;
                options.Ease = EaseType.CubicIn;
            }));
        }
        await Group.Of([.. tweens]).End;
    }
}
