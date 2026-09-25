// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Threading.Tasks;
using Godot;
using tweens.gd;
namespace testbed;

public sealed class JellyCube : GalleryEffect
{
    private const float Ground = -0.6f, Apex = 1.0f;
    private static readonly Vector3 Crouched = new(1.35f, 0.62f, 1.35f), Launched = new(0.72f, 1.42f, 0.72f),
        Falling = new(0.84f, 1.24f, 0.84f), Squashed = new(1.5f, 0.55f, 1.5f);
    // Any combination of quarter turns leaves the cube looking identical, so rotation resets after landing.
    private static readonly Vector3[] Tumbles = [new(MathF.PI / 2, 0, 0), new(0, MathF.PI / 2, MathF.PI / 2), new(0, 0, -MathF.PI / 2)];
    private static readonly Color[] Shades = [Palette.Blue, Palette.Amber, Palette.Mint];

    private Node3D feet = null!, tumble = null!;
    private MeshInstance3D wave = null!;
    private StandardMaterial3D jelly = null!, ripple = null!;
    private Camera3D camera = null!;
    private int landings;

    public override string Title => "Jelly Cube";
    public override string Caption => "3D squash, tumble, shockwave, color shift, and camera shake.";

    protected override void Build()
    {
        var scene = World();
        camera = scene.Camera;
        camera.Position = new Vector3(0, 1.3f, 4.2f);
        camera.LookAt(new Vector3(0, 0.3f, 0));
        Floor(scene.View, Ground);

        ripple = Own(new StandardMaterial3D
        {
            AlbedoColor = Palette.Mint with { A = 0 }, Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
        });
        var ring = new TorusMesh { InnerRadius = 0.5f, OuterRadius = 0.53f, Rings = 64 };
        wave = Mesh(scene.View, ring, ripple, new Vector3(0, Ground + 0.02f, 0));

        // Scaling "feet" squashes towards the floor; the cube inside it tumbles around its own center.
        feet = scene.View.Add(new Node3D { Position = new Vector3(0, Ground, 0) });
        tumble = feet.Add(new Node3D { Position = new Vector3(0, 0.45f, 0) });
        jelly = Surface(Palette.Mint);
        jelly.Roughness = 0.2f;
        Mesh(tumble, new BoxMesh { Size = new Vector3(0.9f, 0.9f, 0.9f) }, jelly);
    }

    protected override void Animate() => Sequence = Repeat(Jump);

    private async Task<bool> Jump(int run)
    {
        var air = 0.36 * Tempo;
        if (!await Finished(run, Keep(feet.TweenScale(Crouched, 0.3 * Tempo, t => t.Ease = EaseType.SineOut)))) return false;
        if (!await Finished(run, Keep(feet.TweenScale(Launched, 0.09 * Tempo, t => t.Ease = EaseType.QuadOut)))) return false;

        var turn = Tumbles[landings % Tumbles.Length];
        Keep(tumble.TweenRotation(turn, air * 2, t => { t.From = Vector3.Zero; t.Ease = EaseType.CubicInOut; }));

        void Rising(TweenOptions t) => t.Ease = EaseType.QuadOut;
        var rise = Finished(run,
            Keep(feet.TweenPositionY(Apex, air, Rising)),
            Keep(feet.TweenScale(Vector3.One, air, Rising)));
        if (!await rise) return false;

        void Dropping(TweenOptions t) => t.Ease = EaseType.QuadIn;
        var fall = Finished(run,
            Keep(feet.TweenPositionY(Ground, air, Dropping)),
            Keep(feet.TweenScale(Falling, air, Dropping)));
        if (!await fall) return false;

        tumble.Rotation = Vector3.Zero;
        Land();
        if (!await Finished(run, Keep(feet.TweenScale(Squashed, 0.06 * Tempo, t => t.Ease = EaseType.QuadOut)))) return false;
        return await Finished(run, Keep(feet.TweenScale(Vector3.One, 0.8 * Tempo, t => t.Ease = EaseType.ElasticOut)));
    }

    /// <summary>Shifts the cube's color, sends a shockwave over the floor and shakes the camera.</summary>
    private void Land()
    {
        Keep(jelly.TweenAlbedoColor(Shades[landings++ % Shades.Length], 0.3 * Tempo, Stage));

        var spread = 0.7 * Tempo;
        Keep(wave.TweenScale(new Vector3(2.6f, 1, 2.6f), spread, t =>
        {
            t.From = new Vector3(0.9f, 1, 0.9f);
            t.Ease = EaseType.QuartOut;
        }));
        Keep(ripple.TweenAlbedoAlpha(0, spread, Stage, t => { t.From = 0.9f; t.Ease = EaseType.QuadIn; }));

        var shake = 0.4 * Tempo;
        Keep(camera.TweenVOffset(0.06f, shake, t => { t.From = 0; t.EaseFunction = Shake; }));
        Keep(camera.TweenHOffset(0.035f, shake, t => { t.From = 0; t.EaseFunction = w => Shake(MathF.Min(1, w * 1.3f)); }));
    }
}
