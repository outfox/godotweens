// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

namespace testbed;

public partial class InterfacePage : GalleryPage
{
    public override string Heading => "Interface";
    public override string Description => "Text, range values, layout transforms, and scrolling.";

    protected override GalleryEffect[] CreateEffects() =>
        [new TextReveal(), new RangeMeter(), new OffsetTransforms(), new ScrollingList()];
}
