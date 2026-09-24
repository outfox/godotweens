// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

namespace testbed;

public partial class SquashPage : GalleryPage
{
    public override string Heading => "Squash & stretch";
    public override string Description => "Anticipation, overshoot, and volume-preserving scale, chained with async sequences.";

    protected override GalleryEffect[] CreateEffects() =>
        [new BouncingBall(), new SlimeHop(), new JellyButton(), new SquashWave()];
}
