// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
namespace testbed;

public sealed partial class ParentedRotation : GalleryEffect
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

    protected override void Animate() => Sequence = Run(AnimateAsync());
}
