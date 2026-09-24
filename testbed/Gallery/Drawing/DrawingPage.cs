// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

namespace testbed;

public partial class DrawingPage : GalleryPage
{
    public override string Heading => "Drawing & particles";
    public override string Description => "Line, polygon, particle, and 2D light properties.";

    protected override GalleryEffect[] CreateEffects() =>
        [new GlowingRibbon(), new PolygonEchoes(), new ParticleStream(), new LightSweep()];
}
