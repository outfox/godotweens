// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using Godot;
using tweens.gd;
namespace testbed;

public sealed partial class JellyCube : GalleryEffect
{

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
}
