// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using godotweens;
namespace testbed;

public sealed class AlbedoFade : GalleryEffect
{
    private StandardMaterial3D material = null!;

    public override string Title => "Albedo Alpha";
    public override string Caption => "Alpha tween with transparency enabled. RGB is preserved.";

    protected override void Build()
    {
        var scene = World();
        var backdrop = Surface(Colors.White);
        backdrop.AlbedoTexture = Checker(64, 16, Palette.Mint, new Color("304875"));
        Mesh(scene.View, new QuadMesh { Size = new Vector2(3.5f, 1.8f) }, backdrop, new Vector3(0, 0, -0.6f));

        material = Surface(Palette.Blue);
        material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        Mesh(scene.View, new SphereMesh { Radius = 0.8f, Height = 1.6f }, material);
    }

    protected override void Animate() => Keep(material.TweenAlbedoAlpha(0.08f, Seconds, Stage, Cycle));
}
