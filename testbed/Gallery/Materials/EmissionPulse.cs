// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd;
namespace testbed;

public sealed class EmissionPulse : GalleryEffect
{
    private StandardMaterial3D material = null!;

    public override string Title => "Emission";
    public override string Caption => "Emission color and energy multiplier.";

    protected override void Build()
    {
        var scene = World();
        Floor(scene.View, -0.9f);
        material = Surface(new Color("2a3a50"));
        material.EmissionEnabled = true;
        material.Emission = new Color(0.08f, 0.22f, 0.15f);
        material.EmissionEnergyMultiplier = 0.15f;
        var torus = Mesh(scene.View, new TorusMesh { InnerRadius = 0.45f, OuterRadius = 0.8f }, material);
        torus.RotationDegrees = new Vector3(65, 0, 15);
    }

    protected override void Animate()
    {
        Keep(material.TweenEmission(new Color(0.12f, 0.08f, 0.3f), Seconds, Stage, Cycle));
        Keep(material.TweenEmissionEnergyMultiplier(2, Seconds, Stage, Cycle));
    }
}
