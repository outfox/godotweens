// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd;
namespace testbed;

public sealed class ParentedRotation : GalleryEffect
{
    private MeshInstance3D cube = null!;

    public override string Title => "Global Quaternion";
    public override string Caption => "Global rotation and local scale under a rotated parent.";

    protected override void Build()
    {
        var scene = World();
        Floor(scene.View, -1.05f);
        var parent = scene.View.Add(new Node3D { Rotation = new Vector3(0.2f, 0.3f, 0.2f) });
        cube = Mesh(parent, new BoxMesh { Size = new Vector3(1.2f, 1.2f, 1.2f) }, Surface(Palette.Mint));
        Mesh(parent, new SphereMesh { Radius = 0.16f, Height = 0.32f }, Surface(Palette.Amber), new Vector3(1, 0, 0));
    }

    protected override void Animate()
    {
        var orientation = Quaternion.FromEuler(new Vector3(0.5f, 2.5f, 0.8f));
        Keep(cube.TweenGlobalQuaternion(orientation, Seconds, Cycle));
        Keep(cube.TweenScale(new Vector3(1.4f, 0.7f, 1.1f), Seconds, Cycle));
    }
}
