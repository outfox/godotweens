// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using godotweens;
namespace testbed;

public partial class MaterialsPage : GalleryPage
{
    public override string Heading => "Materials";
    public override string Description => "Animate the resource itself. Shared materials change together; no material is silently duplicated.";
    private StandardMaterial3D shared = null!, textured = null!, transparent = null!, emissive = null!;
    protected override void Build()
    {
        var (twins, _) = World(Card("01 / One material, two objects", "Both meshes share albedo color + roughness tweens on the same resource."));
        shared = Surface(Mint); shared.Roughness = 0.05f;
        Mesh(twins, new SphereMesh { Radius = 0.65f, Height = 1.3f }, shared, new Vector3(-1, 0, 0));
        var cube = Mesh(twins, new BoxMesh { Size = Vector3.One }, shared, new Vector3(1, 0, 0));
        cube.Rotation = new Vector3(0.3f, 0.5f, 0);
        var (uv, camera) = World(Card("02 / Moving texture coordinates", "UV1 offset X + scale. A generated checker texture slides over the surface."));
        camera.Position = new Vector3(0, 0, 2.7f); camera.LookAt(Vector3.Zero);
        using var image = Image.CreateEmpty(64, 64, false, Image.Format.Rgba8);
        for (var y = 0; y < 64; y++) for (var x = 0; x < 64; x++)
            image.SetPixel(x, y, (x / 16 + y / 16) % 2 == 0 ? Mint : new Color("304875"));
        var checker = Own(ImageTexture.CreateFromImage(image));
        textured = Surface(Colors.White); textured.AlbedoTexture = checker;
        textured.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest;
        Mesh(uv, new QuadMesh { Size = new Vector2(3.8f, 1.35f) }, textured);
        var (fade, _) = World(Card("03 / Reveal what's behind", "Albedo alpha only. Transparency is enabled up front; RGB stays unchanged."));
        var backdrop = Surface(Colors.White); backdrop.AlbedoTexture = checker;
        Mesh(fade, new QuadMesh { Size = new Vector2(3.5f, 1.8f) }, backdrop, new Vector3(0, 0, -0.6f));
        transparent = Surface(Blue); transparent.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        Mesh(fade, new SphereMesh { Radius = 0.8f, Height = 1.6f }, transparent);
        var (glow, _) = World(Card("04 / An emissive pulse", "Emission color + energy multiplier. A visible surface pulse, without a glow post-effect."));
        emissive = Surface(new Color("162130")); emissive.EmissionEnabled = true;
        emissive.Emission = new Color(0.08f, 0.22f, 0.15f); emissive.EmissionEnergyMultiplier = 0.15f;
        Mesh(glow, new TorusMesh { InnerRadius = 0.45f, OuterRadius = 0.8f }, emissive).RotationDegrees = new Vector3(65, 0, 15);
    }
    protected override void Animate()
    {
        Keep(shared.TweenAlbedoColor(Amber, Seconds, this, Cycle));
        Keep(shared.TweenRoughness(0.95f, Seconds, this, Cycle));
        Keep(textured.TweenUv1OffsetX(1, Seconds * 2, this, Cycle));
        Keep(textured.TweenUv1Scale(new Vector3(2.5f, 2.5f, 1), Seconds * 2, this, Cycle));
        Keep(transparent.TweenAlbedoAlpha(0.08f, Seconds, this, Cycle));
        Keep(emissive.TweenEmission(new Color(0.12f, 0.08f, 0.3f), Seconds, this, Cycle));
        Keep(emissive.TweenEmissionEnergyMultiplier(2, Seconds, this, Cycle));
    }
}
