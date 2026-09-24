// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Threading.Tasks;
using Godot;
using godotweens;
namespace testbed;

public partial class MotionPage : GalleryPage
{
    public override string Heading => "Motion & paths";
    public override string Description => "Paths, camera controls, transforms, and async sequences.";
    private PathFollow2D follower = null!;
    private Polygon2D ship = null!, shape = null!, courier = null!;
    private Camera2D camera = null!;
    private Label sequence = null!;
    protected override void Build()
    {
        var view = View(Card("01 / PathFollow2D", "Progress ratio, vertical offset, and marker scale."));
        var curve = Own(new Curve2D());
        curve.AddPoint(new(-160, 35), Vector2.Zero, new(90, -130));
        curve.AddPoint(new(160, -35), new(-90, 130), Vector2.Zero);
        var path = new Path2D { Curve = curve }; view.AddChild(path);
        Line(view, curve.GetBakedPoints(), new Color("4d8190"), 3);
        follower = new PathFollow2D { Loop = false }; path.AddChild(follower);
        ship = Diamond(follower, Vector2.Zero, Mint);
        Diamond(view, new(-160, 35), Muted, 4); Diamond(view, new(160, -35), Muted, 4);

        var cameraView = View(Card("02 / Camera2D", "Zoom and offset applied to a static scene."));
        camera = cameraView.GetChild<Camera2D>(0);
        for (var x = -400; x <= 400; x += 40) Line(cameraView, [new(x, -200), new(x, 200)], new Color("2a3b50"), 1);
        for (var y = -200; y <= 200; y += 40) Line(cameraView, [new(-400, y), new(400, y)], new Color("2a3b50"), 1);
        Line(cameraView, [new(0, -200), new(0, 200)], new Color("41597a"), 1.5f);
        Line(cameraView, [new(-400, 0), new(400, 0)], new Color("41597a"), 1.5f);
        for (var i = -2; i <= 2; i++) Diamond(cameraView, new(i * 90, i % 2 * 35), i == 0 ? Amber : Blue, 12);

        var transforms = View(Card("03 / Combined Transforms", "Skew, rotation, and independent X/Y scale."));
        shape = Diamond(transforms, Vector2.Zero, Blue, 34);
        Line(shape, [Vector2.Zero, new(42, 0)], Amber, 3);

        var chainView = View(Card("04 / Async Sequence", "Await position, then run position and rotation together."));
        Line(chainView, [new(-150, 15), new(150, 15)], new Color("4d8190"), 2);
        courier = Diamond(chainView, new(-150, 15), Mint, 18);
        sequence = Text("Position → position + rotation", 15, Soft);
        sequence.Position = new Vector2(-160, -65); chainView.AddChild(sequence);
    }
    protected override void Animate()
    {
        Keep(follower.TweenProgressRatio(1, Seconds, Cycle));
        Keep(follower.TweenVOffset(20, Seconds, Cycle));
        Keep(ship.TweenScale(new Vector2(1.6f, 1.6f), Seconds, Cycle));
        Keep(camera.TweenZoom(new Vector2(1.8f, 1.8f), Seconds, Cycle));
        Keep(camera.TweenOffset(new Vector2(90, 25), Seconds, Cycle));
        Keep(shape.TweenSkew(0.5f, Seconds, Cycle));
        Keep(shape.TweenRotation(Mathf.Pi, Seconds, Cycle));
        Keep(shape.TweenScaleX(1.8f, Seconds, Cycle));
        Keep(shape.TweenScaleY(0.6f, Seconds, Cycle));
        SequenceTask = Deliver(Generation);
    }
    private async Task Deliver(int run)
    {
        try
        {
            sequence.Text = "1 / Position";
            var outward = Keep(courier.TweenPositionX(150, Seconds, d => d.Ease = Ease));
            if (await outward.Completion != TweenCompletionReason.Completed || run != Generation) return;
            sequence.Text = "2 / Position + rotation";
            var back = Keep(courier.TweenPositionX(-150, Seconds, d => d.Ease = Ease));
            var turn = Keep(courier.TweenRotation(Mathf.Tau, Seconds, d => d.Ease = Ease));
            var results = await Task.WhenAll(back.Completion, turn.Completion);
            if (run != Generation || results[0] != TweenCompletionReason.Completed || results[1] != TweenCompletionReason.Completed) return;
            sequence.Text = "3 / Complete";
        }
        catch (Exception error) { GD.PushError(error.ToString()); }
    }
}
