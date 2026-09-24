// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

namespace testbed;

public partial class MotionPage : GalleryPage
{
    public override string Heading => "Motion & paths";
    public override string Description => "Paths, camera controls, transforms, and async sequences.";

    protected override GalleryEffect[] CreateEffects() =>
        [new CurveFollower2D(), new CameraPan(), new CombinedTransforms(), new AsyncDelivery()];
}
