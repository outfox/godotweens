// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in BouncingBall.cs.
public sealed partial class BouncingBall
{
    private async Task<bool> Bounce()
    {
        var air = 0.36 * Tempo;
        if (Math.Abs(ball.Position.X + direction * Stride) > Bounds) direction = -direction;
        var target = ball.Position.X + direction * Stride;

        if (await Crouch().End != Reason.Completed) return false;

        _ = Run(Travel(target, air * 2));

        if (!await Rise(air)) return false;
        if (!await Fall(air)) return false;

        _ = Run(Ripple(target));
        _ = Run(KickUpDust(target));
        return await Squash().End == Reason.Completed;
    }

    private const float Ground = 62, Apex = Ground - 96, Stride = 75, Bounds = 151;
    private static readonly Vector2 Crouched = new(0.72f, 1.32f), Falling = new(0.8f, 1.25f), Squashed = new(1.55f, 0.55f);

    private TweenInstance Crouch() =>
        ball.TweenScale(Crouched, 0.07 * Tempo, options => options.Ease = EaseType.QuadOut);

    private async Task<bool> Travel(float target, double duration)
    {
        return await Group.Of([
            spin.TweenRotation(spin.Rotation + direction * MathF.PI, duration),
            ball.TweenPositionX(target, duration),
            shadow.TweenPositionX(target, duration),
        ]).End == Reason.Completed;
    }

    private async Task<bool> Rise(double air)
    {
        return await Group.Of([
            ball.TweenPositionY(Apex, air, options => options.Ease = EaseType.QuadOut),
            ball.TweenScale(Vector2.One, air, options => options.Ease = EaseType.QuadOut),
            shadow.TweenScale(new Vector2(0.4f, 0.4f), air, options => options.Ease = EaseType.QuadOut),
        ]).End == Reason.Completed;
    }

    private async Task<bool> Fall(double air)
    {
        return await Group.Of([
            ball.TweenPositionY(Ground, air, options => options.Ease = EaseType.QuadIn),
            ball.TweenScale(Falling, air, options => options.Ease = EaseType.QuadIn),
            shadow.TweenScale(Vector2.One, air, options => options.Ease = EaseType.QuadIn),
        ]).End == Reason.Completed;
    }

    private TweenInstance Squash() =>
        ball.TweenScale(Squashed, 0.06 * Tempo, options => options.Ease = EaseType.QuadOut);

    private async Task Ripple(float x)
    {
        var duration = 0.5 * Tempo;
        ring.Position = new Vector2(x, Ground);
        await Group.Of([
            ring.TweenScale(new Vector2(2.2f, 2.2f), duration, options =>
            {
                options.From = new Vector2(0.4f, 0.4f);
                options.Ease = EaseType.QuartOut;
            }),
            ring.TweenModulateAlpha(0, duration, options =>
            {
                options.From = 1;
                options.Ease = EaseType.QuadIn;
            }),
        ]).End;
    }

    private async Task KickUpDust(float x)
    {
        var tweens = new List<TweenInstance>();
        var duration = 0.45 * Tempo;
        for (var i = 0; i < dust.Length; i++)
        {
            var side = i % 2 == 0 ? -1 : 1;
            var row = i / 2;
            var landing = new Vector2(x + side * (26 + row * 16), Ground - 10 - row * 5);
            dust[i].Position = new Vector2(x + side * 14, Ground - 3);
            dust[i].Scale = Vector2.One * (1.4f - row * 0.3f);
            tweens.Add(dust[i].TweenPosition(landing, duration, options => options.Ease = EaseType.QuartOut));
            tweens.Add(dust[i].TweenModulateAlpha(0, duration, options =>
            {
                options.From = 0.9f;
                options.Ease = EaseType.QuadIn;
            }));
        }
        await Group.Of([.. tweens]).End;
    }
}
