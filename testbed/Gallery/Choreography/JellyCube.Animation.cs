// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup is in JellyCube.cs.
public sealed partial class JellyCube
{
    private async Task<bool> Jump()
    {
        var air = 0.36 * Tempo;
        if (await Crouch().End != Reason.Completed) return false;
        if (await Launch().End != Reason.Completed) return false;

        _ = Run(Turn(air * 2).End);
        if (!await Rise(air)) return false;
        if (!await Fall(air)) return false;

        tumble.Rotation = Vector3.Zero;
        _ = Run(Land());
        if (await Squash().End != Reason.Completed) return false;
        return await Recover().End == Reason.Completed;
    }

    private const float Ground = -0.6f, Apex = 1.0f;
    private static readonly Vector3 Crouched = new(1.35f, 0.62f, 1.35f), Launched = new(0.72f, 1.42f, 0.72f),
        Falling = new(0.84f, 1.24f, 0.84f), Squashed = new(1.5f, 0.55f, 1.5f);
    // Any combination of quarter turns leaves the cube looking identical, so rotation resets after landing.
    private static readonly Vector3[] Tumbles = [new(MathF.PI / 2, 0, 0), new(0, MathF.PI / 2, MathF.PI / 2), new(0, 0, -MathF.PI / 2)];
    private static readonly Color[] Shades = [Palette.Blue, Palette.Amber, Palette.Mint];

    private TweenInstance Crouch() =>
        feet.TweenScale(Crouched, 0.3 * Tempo, options => options.Ease = EaseType.SineOut);

    private TweenInstance Launch() =>
        feet.TweenScale(Launched, 0.09 * Tempo, options => options.Ease = EaseType.QuadOut);

    private TweenInstance Turn(double duration)
    {
        var turn = Tumbles[landings % Tumbles.Length];
        return tumble.TweenRotation(turn, duration, options =>
        {
            options.From = Vector3.Zero;
            options.Ease = EaseType.CubicInOut;
        });
    }

    private async Task<bool> Rise(double air)
    {
        return await Group.Of([
            feet.TweenPositionY(Apex, air, options => options.Ease = EaseType.QuadOut),
            feet.TweenScale(Vector3.One, air, options => options.Ease = EaseType.QuadOut),
        ]).End == Reason.Completed;
    }

    private async Task<bool> Fall(double air)
    {
        return await Group.Of([
            feet.TweenPositionY(Ground, air, options => options.Ease = EaseType.QuadIn),
            feet.TweenScale(Falling, air, options => options.Ease = EaseType.QuadIn),
        ]).End == Reason.Completed;
    }

    private TweenInstance Squash() =>
        feet.TweenScale(Squashed, 0.06 * Tempo, options => options.Ease = EaseType.QuadOut);

    private TweenInstance Recover() =>
        feet.TweenScale(Vector3.One, 0.8 * Tempo, options => options.Ease = EaseType.ElasticOut);

    /// <summary>Shifts the cube's color, sends a shockwave over the floor and shakes the camera.</summary>
    private async Task Land()
    {
        var spread = 0.7 * Tempo;
        var shake = 0.4 * Tempo;
        await Group.Of([
            jelly.TweenAlbedoColor(Shades[landings++ % Shades.Length], 0.3 * Tempo, Stage),
            wave.TweenScale(new Vector3(2.6f, 1, 2.6f), spread, options =>
            {
                options.From = new Vector3(0.9f, 1, 0.9f);
                options.Ease = EaseType.QuartOut;
            }),
            ripple.TweenAlbedoAlpha(0, spread, Stage, options =>
            {
                options.From = 0.9f;
                options.Ease = EaseType.QuadIn;
            }),
            camera.TweenVOffset(0.06f, shake, options =>
            {
                options.From = 0;
                options.EaseFunction = Shake;
            }),
            camera.TweenHOffset(0.035f, shake, options =>
            {
                options.From = 0;
                options.EaseFunction = w => Shake(MathF.Min(1, w * 1.3f));
            }),
        ]).End;
    }

    // A decaying oscillation that finishes at the starting value.
    private static float Shake(float progress) =>
        MathF.Sin(progress * 42) * (1 - progress) * (1 - progress);
}
