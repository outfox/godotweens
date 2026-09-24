// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

namespace testbed;

public partial class SpatialPage : GalleryPage
{
    public override string Heading => "3D";
    public override string Description => "3D transforms, camera controls, paths, and lighting.";

    protected override GalleryEffect[] CreateEffects() =>
        [new ParentedRotation(), new CameraLens(), new CurveFollower3D(), new Spotlight()];
}
