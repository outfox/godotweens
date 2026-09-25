// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd;
namespace testbed;

public sealed class InstanceUniforms : GalleryEffect
{
    private ColorRect first = null!, second = null!;

    public override string Title => "Instance Uniforms";
    public override string Caption => "Independent CanvasItem values on a shared material.";

    protected override void Build()
    {
        var view = View();
        var material = Shader("shader_type canvas_item; instance uniform float amount = 0.15; " + SplitPanel.Body);
        first = view.Add(SplitPanel.Create(material, SplitPanel.Left));
        second = view.Add(SplitPanel.Create(material, SplitPanel.Right));
        second.SetInstanceShaderParameter("amount", 0.85f);
        SplitPanel.Caption(view, "INSTANCE A", "INSTANCE B");
    }

    protected override void Animate()
    {
        Keep(first.TweenInstanceShaderParameter("amount", 0.85f, Seconds, Cycle));
        Keep(second.TweenInstanceShaderParameter("amount", 0.15f, Seconds, Cycle));
    }
}
