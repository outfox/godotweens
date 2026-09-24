// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

namespace testbed;

public partial class ChoreographyPage : GalleryPage
{
    public override string Heading => "Choreography";
    public override string Description => "Staggered delays, echo trails, OnUpdate drivers, and a 3D jelly cube with camera shake.";

    protected override GalleryEffect[] CreateEffects() =>
        [new CardDeal(), new EasingRace(), new Spirograph(), new JellyCube()];
}
