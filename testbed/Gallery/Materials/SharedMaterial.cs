// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd;
namespace testbed;

public sealed partial class SharedMaterial : GalleryEffect
{
    private StandardMaterial3D material = null!;

    public override string Title => "Shared Material";
    public override string Caption => "Albedo color and roughness shared by both meshes.";

    protected override void Build()
    {
        var scene = World();
        Floor(scene.View, -0.75f);
        material = Surface(Palette.Mint);
        material.Roughness = 0.05f;
        Mesh(scene.View, new SphereMesh { Radius = 0.65f, Height = 1.3f }, material, new Vector3(-1, -0.1f, 0));
        var cube = Mesh(scene.View, new BoxMesh { Size = Vector3.One }, material, new Vector3(1, -0.02f, 0));
        cube.Rotation = new Vector3(0.3f, 0.5f, 0);
    }

    protected override void Animate() => TrackTweens(CreateAnimation());
}
