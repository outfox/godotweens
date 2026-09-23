// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using godotweens;
namespace testbed;

public partial class SpatialPage : GalleryPage
{
    public override string Heading => "3D stage";
    public override string Description => "A small world in every card: world rotation, lens changes, a curved route, and a shifting spotlight.";
    private MeshInstance3D object3D = null!;
    private Camera3D lens = null!;
    private PathFollow3D follower = null!;
    private SpotLight3D spot = null!;
    protected override void Build()
    {
        var (view, _) = World(Card("01 / Turn in world space", "Global quaternion + scale. The object sits below a rotated parent."));
        var parent = new Node3D { Rotation = new Vector3(0.2f, 0.3f, 0.2f) }; view.AddChild(parent);
        object3D = Mesh(parent, new BoxMesh { Size = new Vector3(1.2f, 1.2f, 1.2f) }, Surface(Mint));
        Mesh(parent, new SphereMesh { Radius = 0.16f, Height = 0.32f }, Surface(Amber), new Vector3(1, 0, 0));
        var (cameraView, camera) = World(Card("02 / Through the lens", "Camera3D field of view + horizontal offset. Watch the whole arrangement reframe."));
        lens = camera;
        for (var i = -2; i <= 2; i++) Mesh(cameraView, new BoxMesh { Size = Vector3.One * 0.7f },
            Surface(i % 2 == 0 ? Mint : Blue), new Vector3(i * 1.25f, 0, -Mathf.Abs(i) * 0.6f));

        var (route, _) = World(Card("03 / A spatial route", "PathFollow3D progress ratio + vertical offset. The beads trace the curve."));
        var curve = Own(new Curve3D());
        curve.AddPoint(new Vector3(-2, -0.4f, 0), Vector3.Zero, new Vector3(1.3f, 2, -1));
        curve.AddPoint(new Vector3(2, 0.4f, 0), new Vector3(-1.3f, -2, 1), Vector3.Zero);
        var path = new Path3D { Curve = curve }; route.AddChild(path);
        follower = new PathFollow3D { Loop = false, RotationMode = PathFollow3D.RotationModeEnum.None }; path.AddChild(follower);
        Mesh(follower, new SphereMesh { Radius = 0.25f, Height = 0.5f }, Surface(Amber));
        var beads = Surface(new Color("426070"));
        for (var i = 0; i <= 24; i++) Mesh(route, new SphereMesh { Radius = 0.045f, Height = 0.09f, RadialSegments = 8, Rings = 4 },
            beads, curve.SampleBaked(curve.GetBakedLength() * i / 24));

        var (lights, _) = World(Card("04 / Paint with light", "Spot angle + light color + energy. A floor catches the changing cone."));
        foreach (var child in lights.GetChildren())
        {
            if (child is DirectionalLight3D directional) directional.Free();
            if (child is WorldEnvironment world) world.Environment.AmbientLightEnergy = 0.08f;
        }
        Mesh(lights, new PlaneMesh { Size = new Vector2(8, 6) }, Surface(new Color("788aa0")), new Vector3(0, -0.7f, 0));
        Mesh(lights, new TorusMesh { InnerRadius = 0.4f, OuterRadius = 0.8f }, Surface(new Color("d3dce4")));
        spot = new SpotLight3D { Position = new Vector3(0, 2.8f, 2), SpotRange = 8, SpotAngle = 18,
            LightColor = Mint, LightEnergy = 0.8f }; lights.AddChild(spot); spot.LookAt(new Vector3(0, -0.5f, 0));
    }
    protected override void Animate()
    {
        Keep(object3D.TweenGlobalQuaternion(Quaternion.FromEuler(new Vector3(0.5f, 2.5f, 0.8f)), Seconds, Cycle));
        Keep(object3D.TweenScale(new Vector3(1.4f, 0.7f, 1.1f), Seconds, Cycle));
        Keep(lens.TweenFov(65, Seconds, Cycle));
        Keep(lens.TweenHOffset(0.7f, Seconds, Cycle));
        Keep(follower.TweenProgressRatio(1, Seconds * 1.5, Cycle));
        Keep(follower.TweenVOffset(0.35f, Seconds, Cycle));
        Keep(spot.TweenSpotAngle(52, Seconds, Cycle));
        Keep(spot.TweenLightColor(Blue, Seconds, Cycle));
        Keep(spot.TweenLightEnergy(3, Seconds, Cycle));
    }
}
