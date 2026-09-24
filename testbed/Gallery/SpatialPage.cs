// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Linq;
using Godot;
using godotweens;
namespace testbed;

public partial class SpatialPage : GalleryPage
{
    public override string Heading => "3D";
    public override string Description => "3D transforms, camera controls, paths, and lighting.";
    private MeshInstance3D object3D = null!;
    private Camera3D lens = null!;
    private PathFollow3D follower = null!;
    private PathFollow3D[] echoes = [];
    private SpotLight3D spot = null!;
    protected override void Build()
    {
        var (view, _) = World(Card("01 / Global Quaternion", "Global rotation and local scale under a rotated parent."));
        Floor(view, -1.05f);
        var parent = new Node3D { Rotation = new Vector3(0.2f, 0.3f, 0.2f) }; view.AddChild(parent);
        object3D = Mesh(parent, new BoxMesh { Size = new Vector3(1.2f, 1.2f, 1.2f) }, Surface(Mint));
        Mesh(parent, new SphereMesh { Radius = 0.16f, Height = 0.32f }, Surface(Amber), new Vector3(1, 0, 0));
        var (cameraView, camera) = World(Card("02 / Camera3D", "Field of view and horizontal offset."));
        lens = camera; Floor(cameraView, -0.35f);
        for (var i = -2; i <= 2; i++) Mesh(cameraView, new BoxMesh { Size = Vector3.One * 0.7f },
            Surface(i % 2 == 0 ? Mint : Blue), new Vector3(i * 1.25f, 0, -Mathf.Abs(i) * 0.6f));

        var (route, _) = World(Card("03 / PathFollow3D", "Progress ratio and vertical offset. Points show the path."));
        var curve = Own(new Curve3D());
        curve.AddPoint(new Vector3(-2, -0.4f, 0), Vector3.Zero, new Vector3(1.3f, 2, -1));
        curve.AddPoint(new Vector3(2, 0.4f, 0), new Vector3(-1.3f, -2, 1), Vector3.Zero);
        var path = new Path3D { Curve = curve }; route.AddChild(path);
        Floor(route, -1.1f);
        echoes = Enumerable.Range(1, 3).Select(e =>
        {
            var echo = new PathFollow3D { Loop = false, RotationMode = PathFollow3D.RotationModeEnum.None }; path.AddChild(echo);
            var glass = Surface(Amber with { A = 0.5f - e * 0.13f }); glass.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
            Mesh(echo, new SphereMesh { Radius = 0.25f - e * 0.04f, Height = 0.5f - e * 0.08f }, glass); return echo;
        }).ToArray();
        follower = new PathFollow3D { Loop = false, RotationMode = PathFollow3D.RotationModeEnum.None }; path.AddChild(follower);
        var gold = Surface(Amber); gold.EmissionEnabled = true; gold.Emission = Amber; gold.EmissionEnergyMultiplier = 0.35f;
        Mesh(follower, new SphereMesh { Radius = 0.25f, Height = 0.5f }, gold);
        var beads = Surface(new Color("7fa3bd"));
        for (var i = 0; i <= 24; i++) Mesh(route, new SphereMesh { Radius = 0.045f, Height = 0.09f, RadialSegments = 8, Rings = 4 },
            beads, curve.SampleBaked(curve.GetBakedLength() * i / 24));

        var (lights, _) = World(Card("04 / SpotLight3D", "Spot angle, light color, and energy."));
        foreach (var child in lights.GetChildren())
        {
            if (child is DirectionalLight3D directional) directional.Free();
            if (child is WorldEnvironment world) world.Environment.AmbientLightEnergy = 0.2f;
        }
        Mesh(lights, new PlaneMesh { Size = new Vector2(8, 6) }, Surface(new Color("788aa0")), new Vector3(0, -0.7f, 0));
        Mesh(lights, new TorusMesh { InnerRadius = 0.4f, OuterRadius = 0.8f }, Surface(new Color("d3dce4")));
        spot = new SpotLight3D { Position = new Vector3(0, 2.8f, 2), SpotRange = 8, SpotAngle = 18,
            LightColor = Mint, LightEnergy = 0.8f, ShadowEnabled = true }; lights.AddChild(spot); spot.LookAt(new Vector3(0, -0.5f, 0));
        var bulb = Own(new StandardMaterial3D { AlbedoColor = Colors.White, EmissionEnabled = true, Emission = Colors.White,
            EmissionEnergyMultiplier = 3, ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded });
        Mesh(spot, new SphereMesh { Radius = 0.12f, Height = 0.24f }, bulb);
    }
    protected override void Animate()
    {
        Keep(object3D.TweenGlobalQuaternion(Quaternion.FromEuler(new Vector3(0.5f, 2.5f, 0.8f)), Seconds, Cycle));
        Keep(object3D.TweenScale(new Vector3(1.4f, 0.7f, 1.1f), Seconds, Cycle));
        Keep(lens.TweenFov(65, Seconds, Cycle));
        Keep(lens.TweenHOffset(0.7f, Seconds, Cycle));
        Keep(follower.TweenProgressRatio(1, Seconds * 1.5, Cycle));
        for (var e = 0; e < echoes.Length; e++)
        {
            var delay = (e + 1) * 0.08;
            Keep(echoes[e].TweenProgressRatio(1, Seconds * 1.5, d => { Cycle(d); d.Delay = delay; }));
            Keep(echoes[e].TweenVOffset(0.35f, Seconds, d => { Cycle(d); d.Delay = delay; }));
        }
        Keep(follower.TweenVOffset(0.35f, Seconds, Cycle));
        Keep(spot.TweenSpotAngle(52, Seconds, Cycle));
        Keep(spot.TweenLightColor(Blue, Seconds, Cycle));
        Keep(spot.TweenLightEnergy(3, Seconds, Cycle));
    }
}
