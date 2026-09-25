// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in BouncingBall.cs.
public sealed partial class BouncingBall
{
    private const float Ground = 62, Apex = Ground - 96, Stride = 75, Bounds = 151;
    private static readonly Vector2 Crouched = new(0.72f, 1.32f), Falling = new(0.8f, 1.25f), Squashed = new(1.55f, 0.55f);

    private TweenInstance Crouch() =>
        ball.TweenScale(Crouched, 0.07 * Tempo, t => t.Ease = EaseType.QuadOut);

    private TweenInstance[] Travel(float target, double duration) =>
    [
        spin.TweenRotation(spin.Rotation + direction * MathF.PI, duration),
        ball.TweenPositionX(target, duration),
        shadow.TweenPositionX(target, duration),
    ];

    private TweenInstance[] Rise(double air)
    {
        void Rising(TweenOptions t) => t.Ease = EaseType.QuadOut;
        return
        [
            ball.TweenPositionY(Apex, air, Rising),
            ball.TweenScale(Vector2.One, air, Rising),
            shadow.TweenScale(new Vector2(0.4f, 0.4f), air, Rising),
        ];
    }

    private TweenInstance[] Fall(double air)
    {
        void Dropping(TweenOptions t) => t.Ease = EaseType.QuadIn;
        return
        [
            ball.TweenPositionY(Ground, air, Dropping),
            ball.TweenScale(Falling, air, Dropping),
            shadow.TweenScale(Vector2.One, air, Dropping),
        ];
    }

    private TweenInstance Squash() =>
        ball.TweenScale(Squashed, 0.06 * Tempo, t => t.Ease = EaseType.QuadOut);

    private IEnumerable<TweenInstance> Ripple(float x)
    {
        var duration = 0.5 * Tempo;
        ring.Position = new Vector2(x, Ground);
        yield return ring.TweenScale(new Vector2(2.2f, 2.2f), duration, t => { t.From = new Vector2(0.4f, 0.4f); t.Ease = EaseType.QuartOut; });
        yield return ring.TweenModulateAlpha(0, duration, t => { t.From = 1; t.Ease = EaseType.QuadIn; });
    }

    private IEnumerable<TweenInstance> KickUpDust(float x)
    {
        var duration = 0.45 * Tempo;
        for (var i = 0; i < dust.Length; i++)
        {
            var side = i % 2 == 0 ? -1 : 1;
            var row = i / 2;
            var landing = new Vector2(x + side * (26 + row * 16), Ground - 10 - row * 5);
            dust[i].Position = new Vector2(x + side * 14, Ground - 3);
            dust[i].Scale = Vector2.One * (1.4f - row * 0.3f);
            yield return dust[i].TweenPosition(landing, duration, t => t.Ease = EaseType.QuartOut);
            yield return dust[i].TweenModulateAlpha(0, duration, t => { t.From = 0.9f; t.Ease = EaseType.QuadIn; });
        }
    }
}
