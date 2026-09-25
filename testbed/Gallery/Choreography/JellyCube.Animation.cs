// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Collections.Generic;
using Godot;
using tweens.gd;
namespace testbed;

// Scene setup and playback bookkeeping are in JellyCube.cs.
public sealed partial class JellyCube
{
    private const float Ground = -0.6f, Apex = 1.0f;
    private static readonly Vector3 Crouched = new(1.35f, 0.62f, 1.35f), Launched = new(0.72f, 1.42f, 0.72f),
        Falling = new(0.84f, 1.24f, 0.84f), Squashed = new(1.5f, 0.55f, 1.5f);
    // Any combination of quarter turns leaves the cube looking identical, so rotation resets after landing.
    private static readonly Vector3[] Tumbles = [new(MathF.PI / 2, 0, 0), new(0, MathF.PI / 2, MathF.PI / 2), new(0, 0, -MathF.PI / 2)];
    private static readonly Color[] Shades = [Palette.Blue, Palette.Amber, Palette.Mint];

    private TweenInstance Crouch() =>
        feet.TweenScale(Crouched, 0.3 * Tempo, t => t.Ease = EaseType.SineOut);

    private TweenInstance Launch() =>
        feet.TweenScale(Launched, 0.09 * Tempo, t => t.Ease = EaseType.QuadOut);

    private TweenInstance Turn(double duration)
    {
        var turn = Tumbles[landings % Tumbles.Length];
        return tumble.TweenRotation(turn, duration, t => { t.From = Vector3.Zero; t.Ease = EaseType.CubicInOut; });
    }

    private TweenInstance[] Rise(double air)
    {
        void Rising(TweenOptions t) => t.Ease = EaseType.QuadOut;
        return
        [
            feet.TweenPositionY(Apex, air, Rising),
            feet.TweenScale(Vector3.One, air, Rising),
        ];
    }

    private TweenInstance[] Fall(double air)
    {
        void Dropping(TweenOptions t) => t.Ease = EaseType.QuadIn;
        return
        [
            feet.TweenPositionY(Ground, air, Dropping),
            feet.TweenScale(Falling, air, Dropping),
        ];
    }

    private TweenInstance Squash() =>
        feet.TweenScale(Squashed, 0.06 * Tempo, t => t.Ease = EaseType.QuadOut);

    private TweenInstance Recover() =>
        feet.TweenScale(Vector3.One, 0.8 * Tempo, t => t.Ease = EaseType.ElasticOut);

    /// <summary>Shifts the cube's color, sends a shockwave over the floor and shakes the camera.</summary>
    private IEnumerable<TweenInstance> Land()
    {
        yield return jelly.TweenAlbedoColor(Shades[landings++ % Shades.Length], 0.3 * Tempo, Stage);

        var spread = 0.7 * Tempo;
        yield return wave.TweenScale(new Vector3(2.6f, 1, 2.6f), spread, t =>
        {
            t.From = new Vector3(0.9f, 1, 0.9f);
            t.Ease = EaseType.QuartOut;
        });
        yield return ripple.TweenAlbedoAlpha(0, spread, Stage, t => { t.From = 0.9f; t.Ease = EaseType.QuadIn; });

        var shake = 0.4 * Tempo;
        yield return camera.TweenVOffset(0.06f, shake, t => { t.From = 0; t.EaseFunction = Shake; });
        yield return camera.TweenHOffset(0.035f, shake, t => { t.From = 0; t.EaseFunction = w => Shake(MathF.Min(1, w * 1.3f)); });
    }
}
