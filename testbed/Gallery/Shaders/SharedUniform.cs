// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using godotweens;
namespace testbed;

public sealed class SharedUniform : GalleryEffect
{
    private ShaderMaterial material = null!;

    public override string Title => "Shared Uniform";
    public override string Caption => "One float uniform updates both panels.";

    protected override void Build()
    {
        var view = View();
        material = Shader("shader_type canvas_item; uniform float amount = 0.15; " + SplitPanel.Body);
        SplitPanel.Add(view, material, SplitPanel.Left);
        SplitPanel.Add(view, material, SplitPanel.Right);
        SplitPanel.Caption(view, "SAME MATERIAL", "SAME VALUE");
    }

    protected override void Animate() => Keep(material.TweenShaderParameter("amount", 0.85f, Seconds, Stage, Cycle));
}
