// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using godotweens;
namespace testbed;

public sealed class UvScroll : GalleryEffect
{
    private StandardMaterial3D material = null!;

    public override string Title => "UV Transform";
    public override string Caption => "UV1 offset X and scale.";

    protected override void Build()
    {
        var scene = World();
        scene.Camera.Position = new Vector3(0, 0, 2.7f);
        scene.Camera.LookAt(Vector3.Zero);

        material = Surface(Colors.White);
        material.AlbedoTexture = Checker(64, 16, Palette.Mint, new Color("304875"));
        material.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest;
        Mesh(scene.View, new QuadMesh { Size = new Vector2(3.8f, 1.35f) }, material);
    }

    protected override void Animate()
    {
        var slow = Seconds * 2;
        Keep(material.TweenUv1OffsetX(1, slow, Stage, Cycle));
        Keep(material.TweenUv1Scale(new Vector3(2.5f, 2.5f, 1), slow, Stage, Cycle));
    }
}
