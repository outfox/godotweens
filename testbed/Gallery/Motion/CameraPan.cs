// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd;
namespace testbed;

public sealed partial class CameraPan : GalleryEffect
{
    private static readonly Color GridLine = new("2a3b50"), Axis = new("41597a");
    private Camera2D camera = null!;
    private Line2D beacon = null!;

    public override string Title => "Camera2D";
    public override string Caption => "Zoom and offset applied to a static scene.";

    protected override void Build()
    {
        var view = View();
        camera = view.GetChild<Camera2D>(0);

        for (var x = -400; x <= 400; x += 40) Line(view, [new(x, -200), new(x, 200)], GridLine, 2);
        for (var y = -200; y <= 200; y += 40) Line(view, [new(-400, y), new(400, y)], GridLine, 2);
        Line(view, [new(0, -200), new(0, 200)], Axis, 3);
        Line(view, [new(-400, 0), new(400, 0)], Axis, 3);

        beacon = Ring(view, Vector2.Zero, 14, Palette.Amber);
        for (var i = -2; i <= 2; i++)
        {
            var color = i == 0 ? Palette.Amber : Palette.Blue;
            var at = new Vector2(i * 90, i % 2 * 35);
            Blob(view, 22, 22, color with { A = 0.13f }, at);
            Diamond(view, at, color, 12);
            Diamond(view, at, Colors.White with { A = 0.6f }, 4);
        }
    }

    protected override void Animate() => TrackTweens(CreateAnimation());
}
