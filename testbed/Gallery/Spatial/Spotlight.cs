// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using godotweens;
namespace testbed;

public sealed class Spotlight : GalleryEffect
{
    private SpotLight3D spot = null!;

    public override string Title => "SpotLight3D";
    public override string Caption => "Spot angle, light color, and energy.";

    protected override void Build()
    {
        // The spot is the only light, over a dimmed ambient.
        var scene = World();
        scene.Sun.Free();
        scene.Environment.AmbientLightEnergy = 0.2f;

        Mesh(scene.View, new PlaneMesh { Size = new Vector2(8, 6) }, Surface(new Color("788aa0")), new Vector3(0, -0.7f, 0));
        Mesh(scene.View, new TorusMesh { InnerRadius = 0.4f, OuterRadius = 0.8f }, Surface(new Color("d3dce4")));

        spot = new SpotLight3D
        {
            Position = new Vector3(0, 2.8f, 2), SpotRange = 8, SpotAngle = 18, LightColor = Palette.Mint, LightEnergy = 0.8f,
            ShadowEnabled = true,
        };
        scene.View.AddChild(spot);
        spot.LookAt(new Vector3(0, -0.5f, 0));

        var bulb = Own(new StandardMaterial3D
        {
            AlbedoColor = Colors.White, EmissionEnabled = true, Emission = Colors.White, EmissionEnergyMultiplier = 3,
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
        });
        Mesh(spot, new SphereMesh { Radius = 0.12f, Height = 0.24f }, bulb);
    }

    protected override void Animate()
    {
        Keep(spot.TweenSpotAngle(52, Seconds, Cycle));
        Keep(spot.TweenLightColor(Palette.Blue, Seconds, Cycle));
        Keep(spot.TweenLightEnergy(3, Seconds, Cycle));
    }
}
