// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Linq;
using Godot;
using tweens.gd;
namespace testbed;

public sealed class CurveFollower3D : GalleryEffect
{
    private const int Beads = 24;
    private PathFollow3D leader = null!;
    private PathFollow3D[] echoes = [];

    public override string Title => "PathFollow3D";
    public override string Caption => "Progress ratio and vertical offset. Points show the path.";

    protected override void Build()
    {
        var scene = World();
        var curve = Own(new Curve3D());
        curve.AddPoint(new Vector3(-2, -0.4f, 0), Vector3.Zero, new Vector3(1.3f, 2, -1));
        curve.AddPoint(new Vector3(2, 0.4f, 0), new Vector3(-1.3f, -2, 1), Vector3.Zero);
        var path = scene.View.Add(new Path3D { Curve = curve });
        Floor(scene.View, -1.1f);

        echoes = Enumerable.Range(1, 3).Select(e =>
        {
            var echo = Follower(path);
            var glass = Surface(Palette.Amber with { A = 0.5f - e * 0.13f });
            glass.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
            Mesh(echo, new SphereMesh { Radius = 0.25f - e * 0.04f, Height = 0.5f - e * 0.08f }, glass);
            return echo;
        }).ToArray();

        leader = Follower(path);
        var gold = Surface(Palette.Amber);
        gold.EmissionEnabled = true;
        gold.Emission = Palette.Amber;
        gold.EmissionEnergyMultiplier = 0.35f;
        Mesh(leader, new SphereMesh { Radius = 0.25f, Height = 0.5f }, gold);

        var bead = Surface(new Color("7fa3bd"));
        for (var i = 0; i <= Beads; i++)
        {
            var position = curve.SampleBaked(curve.GetBakedLength() * i / Beads);
            Mesh(scene.View, new SphereMesh { Radius = 0.045f, Height = 0.09f, RadialSegments = 8, Rings = 4 }, bead, position);
        }
    }

    private static PathFollow3D Follower(Path3D path)
        => path.Add(new PathFollow3D { Loop = false, RotationMode = PathFollow3D.RotationModeEnum.None });

    protected override void Animate()
    {
        var lap = Seconds * 1.5;
        Keep(leader.TweenProgressRatio(1, lap, Cycle));
        for (var e = 0; e < echoes.Length; e++)
        {
            var trailing = CycleAfter((e + 1) * 0.08);
            Keep(echoes[e].TweenProgressRatio(1, lap, trailing));
            Keep(echoes[e].TweenVOffset(0.35f, Seconds, trailing));
        }
        Keep(leader.TweenVOffset(0.35f, Seconds, Cycle));
    }
}
