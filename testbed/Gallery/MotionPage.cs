// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using godotweens;
namespace testbed;

public partial class MotionPage : GalleryPage
{
    public override string Heading => "Motion & paths";
    public override string Description => "Paths, camera controls, transforms, and async sequences.";
    private PathFollow2D follower = null!;
    private PathFollow2D[] echoes = [];
    private Polygon2D ship = null!, shape = null!, shadow = null!, courier = null!;
    private Polygon2D[] steps = [];
    private Line2D beacon = null!;
    private Camera2D camera = null!;
    private Label sequence = null!;
    private static readonly Color Track = new("4d8190");
    protected override void Build()
    {
        var view = View(Card("01 / PathFollow2D", "Progress, vertical offset, and scale, trailed by delayed echoes."));
        var curve = Own(new Curve2D());
        curve.AddPoint(new(-160, 35), Vector2.Zero, new(90, -130));
        curve.AddPoint(new(160, -35), new(-90, 130), Vector2.Zero);
        Line(view, curve.GetBakedPoints(), Mint with { A = 0.1f }, 14);
        Line(view, curve.GetBakedPoints(), Track, 3);
        foreach (var end in new Vector2[] { new(-160, 35), new(160, -35) })
        {
            Blob(view, 5, 5, Track, end); var halo = Line(view, Ellipse(9, 9, 24, end), Track, 1.5f); halo.Closed = true;
        }
        // Added after the track so the followers draw on top of it.
        var path = new Path2D { Curve = curve }; view.AddChild(path);
        echoes = Enumerable.Range(1, 3).Select(e =>
        {
            var echo = new PathFollow2D { Loop = false }; path.AddChild(echo);
            Diamond(echo, Vector2.Zero, Mint with { A = 0.55f - e * 0.15f }, 16 - e * 2.5f); return echo;
        }).ToArray();
        follower = new PathFollow2D { Loop = false }; path.AddChild(follower);
        Blob(follower, 24, 24, Mint with { A = 0.14f });
        ship = Diamond(follower, Vector2.Zero, Mint);
        Diamond(ship, Vector2.Zero, new Color("c3f5df"), 6);

        var cameraView = View(Card("02 / Camera2D", "Zoom and offset applied to a static scene."));
        camera = cameraView.GetChild<Camera2D>(0);
        for (var x = -400; x <= 400; x += 40) Line(cameraView, [new(x, -200), new(x, 200)], new Color("2a3b50"), 2);
        for (var y = -200; y <= 200; y += 40) Line(cameraView, [new(-400, y), new(400, y)], new Color("2a3b50"), 2);
        Line(cameraView, [new(0, -200), new(0, 200)], new Color("41597a"), 3);
        Line(cameraView, [new(-400, 0), new(400, 0)], new Color("41597a"), 3);
        beacon = Line(cameraView, Ellipse(14, 14, 32), Amber, 2); beacon.Closed = true;
        for (var i = -2; i <= 2; i++)
        {
            var color = i == 0 ? Amber : Blue; var at = new Vector2(i * 90, i % 2 * 35);
            Blob(cameraView, 22, 22, color with { A = 0.13f }, at);
            Diamond(cameraView, at, color, 12); Diamond(cameraView, at, Colors.White with { A = 0.6f }, 4);
        }

        var transforms = View(Card("03 / Combined Transforms", "Skew, rotation, and independent X/Y scale. Outline marks the rest pose."));
        var rest = Line(transforms, [new(0, -34), new(34, 0), new(0, 34), new(-34, 0)], new Color("3a4f69"), 1.5f); rest.Closed = true;
        shadow = Diamond(transforms, new Vector2(8, 10), new Color(0, 0, 0, 0.4f), 34);
        shape = Diamond(transforms, Vector2.Zero, Blue, 34);
        Diamond(shape, Vector2.Zero, new Color("aec3ff"), 20);
        Line(shape, [Vector2.Zero, new(42, 0)], Amber, 3);
        Blob(shape, 4, 4, Amber);

        var chainView = View(Card("04 / Async Sequence", "Await position, then run position and rotation together."));
        Line(chainView, [new(-150, 15), new(150, 15)], Track, 2);
        foreach (var x in new[] { -150f, 150f }) Line(chainView, [new(x, 5), new(x, 25)], Track, 2);
        courier = Diamond(chainView, new(-150, 15), Mint, 18);
        Blob(courier, 28, 28, Mint with { A = 0.12f }).ShowBehindParent = true;
        Diamond(courier, Vector2.Zero, new Color("c3f5df"), 7);
        sequence = Text("Position → position + rotation", 15, Soft);
        sequence.Position = new Vector2(-160, -70); chainView.AddChild(sequence);
        steps = Enumerable.Range(0, 3).Select(i => Blob(chainView, 5, 5, GalleryTheme.Outline, new Vector2(110 + i * 18, -58))).ToArray();
    }
    protected override void Animate()
    {
        for (var e = 0; e < echoes.Length; e++)
        {
            var delay = (e + 1) * 0.07;
            void Echo(TweenOptions d) { Cycle(d); d.Delay = delay; }
            Keep(echoes[e].TweenProgressRatio(1, Seconds, Echo));
            Keep(echoes[e].TweenVOffset(20, Seconds, Echo));
        }
        Keep(follower.TweenProgressRatio(1, Seconds, Cycle));
        Keep(follower.TweenVOffset(20, Seconds, Cycle));
        Keep(ship.TweenScale(new Vector2(1.6f, 1.6f), Seconds, Cycle));
        Keep(camera.TweenZoom(new Vector2(1.8f, 1.8f), Seconds, Cycle));
        Keep(camera.TweenOffset(new Vector2(90, 25), Seconds, Cycle));
        Keep(beacon.TweenScale(new Vector2(2.4f, 2.4f), 1.2, d => { d.From = Vector2.One; d.Ease = EaseType.QuartOut; d.IsInfinite = true; }));
        Keep(beacon.TweenModulateAlpha(0, 1.2, d => { d.From = 1; d.Ease = EaseType.QuadIn; d.IsInfinite = true; }));
        foreach (var target in new[] { shape, shadow })
        {
            Keep(target.TweenSkew(0.5f, Seconds, Cycle));
            Keep(target.TweenRotation(Mathf.Pi, Seconds, Cycle));
            Keep(target.TweenScaleX(1.8f, Seconds, Cycle));
            Keep(target.TweenScaleY(0.6f, Seconds, Cycle));
        }
        SequenceTask = Deliver(Generation);
    }
    private void Step(int index)
    {
        for (var i = 0; i < steps.Length; i++)
            Keep(steps[i].TweenColor(i <= index ? Mint : GalleryTheme.Outline, 0.2));
        Keep(steps[index].TweenScale(Vector2.One, 0.5, d => { d.From = new Vector2(2, 2); d.Ease = EaseType.ElasticOut; }));
    }
    private async Task Deliver(int run)
    {
        try
        {
            sequence.Text = "1 / Position"; Step(0);
            var outward = Keep(courier.TweenPositionX(150, Seconds, d => d.Ease = Ease));
            if (await outward.Completion != TweenCompletionReason.Completed || run != Generation) return;
            sequence.Text = "2 / Position + rotation"; Step(1);
            var back = Keep(courier.TweenPositionX(-150, Seconds, d => d.Ease = Ease));
            var turn = Keep(courier.TweenRotation(Mathf.Tau, Seconds, d => d.Ease = Ease));
            var results = await Task.WhenAll(back.Completion, turn.Completion);
            if (run != Generation || results[0] != TweenCompletionReason.Completed || results[1] != TweenCompletionReason.Completed) return;
            sequence.Text = "3 / Complete"; Step(2);
        }
        catch (Exception error) { GD.PushError(error.ToString()); }
    }
}
