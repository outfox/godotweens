// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using godotweens;
namespace testbed;

/// <summary>A one-shot sequence: restart the page to replay it.</summary>
public sealed class AsyncDelivery : GalleryEffect
{
    private const float Left = -150, Right = 150, Rail = 15;
    private Polygon2D courier = null!;
    private Polygon2D[] steps = [];
    private Label status = null!;

    public override string Title => "Async Sequence";
    public override string Caption => "Await position, then run position and rotation together.";

    protected override void Build()
    {
        var view = View();
        Line(view, [new(Left, Rail), new(Right, Rail)], Palette.Track, 2);
        foreach (var x in new[] { Left, Right }) Line(view, [new(x, Rail - 10), new(x, Rail + 10)], Palette.Track, 2);

        courier = Diamond(view, new Vector2(Left, Rail), Palette.Mint, 18);
        Blob(courier, 28, 28, Palette.Mint with { A = 0.12f }).ShowBehindParent = true;
        Diamond(courier, Vector2.Zero, new Color("c3f5df"), 7);

        status = GalleryTheme.Label("Position → position + rotation", 15, Palette.Soft);
        status.Position = new Vector2(-160, -70);
        view.AddChild(status);
        steps = Enumerable.Range(0, 3).Select(i => Blob(view, 5, 5, Palette.Outline, new Vector2(110 + i * 18, -58))).ToArray();
    }

    protected override void Animate() => Sequence = Deliver(Generation);

    private async Task Deliver(int run)
    {
        try
        {
            Report(0, "1 / Position");
            var outward = Keep(courier.TweenPositionX(Right, Seconds, t => t.Ease = Ease));
            if (!await Finished(run, outward)) return;

            Report(1, "2 / Position + rotation");
            var back = Keep(courier.TweenPositionX(Left, Seconds, t => t.Ease = Ease));
            var turn = Keep(courier.TweenRotation(Mathf.Tau, Seconds, t => t.Ease = Ease));
            if (!await Finished(run, back, turn)) return;

            Report(2, "3 / Complete");
        }
        catch (Exception error)
        {
            GD.PushError(error.ToString());
        }
    }

    /// <summary>Shows the step's label and lights its progress dot, and every dot before it.</summary>
    private void Report(int step, string text)
    {
        status.Text = text;
        for (var i = 0; i < steps.Length; i++)
            Keep(steps[i].TweenColor(i <= step ? Palette.Mint : Palette.Outline, 0.2));
        Keep(steps[step].TweenScale(Vector2.One, 0.5, t => { t.From = new Vector2(2, 2); t.Ease = EaseType.ElasticOut; }));
    }
}
