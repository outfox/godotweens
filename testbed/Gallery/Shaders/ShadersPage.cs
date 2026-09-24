// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
namespace testbed;

public partial class ShadersPage : GalleryPage
{
    public override string Heading => "Shaders";
    public override string Description => "Shared and per-instance uniforms with typed values.";

    protected override GalleryEffect[] CreateEffects() => DisplayServer.GetName() == "headless"
        ? [new RendererRequired()]
        : [new SharedUniform(), new InstanceUniforms(), new TypedUniforms(), new VertexDisplacement()];

    private sealed class RendererRequired : GalleryEffect
    {
        public override string Title => "Renderer Required";
        public override string Caption => "Run the desktop testbed with rendering enabled.";
        protected override void Build() => Stage.AddChild(GalleryTheme.Label("Unavailable in headless mode.", 17, Palette.Amber));
        protected override void Animate() { }
    }
}
