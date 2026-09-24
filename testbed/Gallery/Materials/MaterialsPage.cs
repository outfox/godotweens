// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

namespace testbed;

public partial class MaterialsPage : GalleryPage
{
    public override string Heading => "Materials";
    public override string Description => "Material property tweens update the supplied resource.";

    protected override GalleryEffect[] CreateEffects() =>
        [new SharedMaterial(), new UvScroll(), new AlbedoFade(), new EmissionPulse()];
}
