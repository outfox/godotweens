// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd;
namespace testbed;

public sealed partial class CameraLens : GalleryEffect
{
    private Camera3D camera = null!;

    public override string Title => "Camera3D";
    public override string Caption => "Field of view and horizontal offset.";

    protected override void Build()
    {
        var scene = World();
        camera = scene.Camera;
        Floor(scene.View, -0.35f);
        for (var i = -2; i <= 2; i++)
        {
            var color = i % 2 == 0 ? Palette.Mint : Palette.Blue;
            var position = new Vector3(i * 1.25f, 0, -Mathf.Abs(i) * 0.6f);
            Mesh(scene.View, new BoxMesh { Size = Vector3.One * 0.7f }, Surface(color), position);
        }
    }

    protected override void Animate() => TrackTweens(CreateAnimation());
}
