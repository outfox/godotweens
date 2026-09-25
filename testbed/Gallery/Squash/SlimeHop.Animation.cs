// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in SlimeHop.cs.
public sealed partial class SlimeHop
{
    private const float Ground = 52, Apex = Ground - 78, Stride = 90, Bounds = 136;
    private static readonly Vector2 Crouched = new(1.38f, 0.6f), Launched = new(0.68f, 1.45f), Falling = new(0.82f, 1.25f),
        Squashed = new(1.6f, 0.5f);

    private TweenInstance Blink() =>
        eyes.TweenScaleY(0.1f, 0.07, t => { t.UsePingPong = true; t.IsInfinite = true; t.RepeatInterval = 2.2; t.Delay = 0.9; });

    private TweenInstance LookAhead() =>
        pupils.TweenPositionX(direction * 3, 0.2 * Tempo, t => t.Ease = EaseType.BackOut);

    private TweenInstance Crouch() =>
        slime.TweenScale(Crouched, 0.32 * Tempo, t => t.Ease = EaseType.SineOut);

    private TweenInstance Launch() =>
        slime.TweenScale(Launched, 0.08 * Tempo, t => t.Ease = EaseType.QuadOut);

    private IEnumerable<TweenInstance> Travel(float target, double duration)
    {
        yield return slime.TweenPositionX(target, duration, t => t.Ease = EaseType.SineInOut);
        if (++hops % 2 == 0)
            yield return body.TweenRotation(direction * MathF.Tau, duration, t => { t.From = 0; t.Ease = EaseType.CubicInOut; });
    }

    private TweenInstance[] Rise(double air)
    {
        void Rising(TweenOptions t) => t.Ease = EaseType.QuadOut;
        return
        [
            slime.TweenPositionY(Apex, air, Rising),
            slime.TweenScale(Vector2.One, air, Rising),
        ];
    }

    private TweenInstance[] Fall(double air)
    {
        void Dropping(TweenOptions t) => t.Ease = EaseType.QuadIn;
        return
        [
            slime.TweenPositionY(Ground, air, Dropping),
            slime.TweenScale(Falling, air, Dropping),
        ];
    }

    private TweenInstance Squash() =>
        slime.TweenScale(Squashed, 0.06 * Tempo, t => t.Ease = EaseType.QuadOut);

    private TweenInstance Recover() =>
        slime.TweenScale(Vector2.One, 0.75 * Tempo, t => t.Ease = EaseType.ElasticOut);

    /// <summary>Flings the droplets outward along the upper half of an ellipse.</summary>
    private IEnumerable<TweenInstance> Splash(float x)
    {
        var duration = 0.4 * Tempo;
        var origin = new Vector2(x, Ground - 6);
        for (var i = 0; i < drops.Length; i++)
        {
            var angle = MathF.PI + (i + 0.5f) / drops.Length * MathF.PI;
            var landing = origin + new Vector2(MathF.Cos(angle) * 58, MathF.Sin(angle) * 34);
            drops[i].Position = origin;
            drops[i].Scale = Vector2.One * (i % 2 == 0 ? 1 : 0.7f);
            yield return drops[i].TweenPosition(landing, duration, t => t.Ease = EaseType.QuartOut);
            yield return drops[i].TweenModulateAlpha(0, duration, t => { t.From = 1; t.Ease = EaseType.CubicIn; });
        }
    }
}
