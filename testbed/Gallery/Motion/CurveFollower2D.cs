// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System.Collections.Generic;
using System.Linq;
using Godot;
using tweens.gd;
namespace testbed;

public sealed partial class CurveFollower2D : GalleryEffect
{
    private static readonly Vector2 PathStart = new(-160, 35), PathEnd = new(160, -35);
    private PathFollow2D leader = null!;
    private PathFollow2D[] echoes = [];
    private Polygon2D ship = null!;

    public override string Title => "PathFollow2D";
    public override string Caption => "Progress, vertical offset, and scale, trailed by delayed echoes.";

    protected override void Build()
    {
        var view = View();
        var curve = Own(new Curve2D());
        curve.AddPoint(PathStart, Vector2.Zero, new Vector2(90, -130));
        curve.AddPoint(PathEnd, new Vector2(-90, 130), Vector2.Zero);

        var track = curve.GetBakedPoints();
        Line(view, track, Palette.Mint with { A = 0.1f }, 14);
        Line(view, track, Palette.Track, 3);
        foreach (var end in new[] { PathStart, PathEnd })
        {
            Blob(view, 5, 5, Palette.Track, end);
            Ring(view, end, 9, Palette.Track, 1.5f, 24);
        }

        // Added after the track so the followers draw on top of it.
        var path = view.Add(new Path2D { Curve = curve });
        echoes = Enumerable.Range(1, 3).Select(e =>
        {
            var echo = path.Add(new PathFollow2D { Loop = false });
            Diamond(echo, Vector2.Zero, Palette.Mint with { A = 0.55f - e * 0.15f }, 16 - e * 2.5f);
            return echo;
        }).ToArray();

        leader = path.Add(new PathFollow2D { Loop = false });
        Blob(leader, 24, 24, Palette.Mint with { A = 0.14f });
        ship = Diamond(leader, Vector2.Zero, Palette.Mint);
        Diamond(ship, Vector2.Zero, new Color("c3f5df"), 6);
    }

    protected override void Animate() => Sequence = Run(FollowPath());
}
