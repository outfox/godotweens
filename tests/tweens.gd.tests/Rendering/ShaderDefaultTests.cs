// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using twodog.Testing;
using twodog.Testing.Xunit;

namespace tweens.gd.Tests.Rendering;

/// <summary>Uniforms without overrides start from the shader default and restore the absence of an override.</summary>
[Collection<RenderingCollection>]
public class ShaderDefaultTests(Fixture godot)
{
    [Fact]
    public void MaterialUniformsStartFromTheirDefaultAndRemoveTheOverride()
    {
        using var shader = new Shader { Code = "shader_type canvas_item; uniform float amount = 0.25;" };
        using var material = new ShaderMaterial { Shader = shader };
        using var scheduler = new TweenScheduler();
        var tween = scheduler.Add(material, new Tweens.ShaderParameter<float>("amount") { To = 1, Duration = 1, Fill = FillMode.None });
        Assert.Equal(0.25f, tween.Value);
        scheduler.Update(0.5);
        Assert.Equal(0.625f, material.GetShaderParameter("amount").AsSingle());
        scheduler.Update(0.5);
        Assert.Equal(Reason.Completed, tween.CompletionReason);
        Assert.Equal(Variant.Type.Nil, material.GetShaderParameter("amount").VariantType);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void InstanceUniformsStartFromTheirDefaultAndRemoveTheOverride(bool spatial)
    {
        using var shader = new Shader
        {
            Code = $"shader_type {(spatial ? "spatial" : "canvas_item")}; instance uniform float amount = 0.25;",
        };
        using var material = new ShaderMaterial { Shader = shader };
        using var mesh = new BoxMesh();
        Node node = spatial ? new MeshInstance3D { Mesh = mesh, MaterialOverride = material } : new ColorRect { Material = material };
        godot.Tree.Root.AddChild(node);
        try
        {
            using var scheduler = new TweenScheduler();
            TweenInstance tween = node is CanvasItem canvas
                ? scheduler.Add(canvas, new Tweens.CanvasItemInstanceShaderParameter<float>("amount") { To = 1, Duration = 1, Fill = FillMode.None })
                : scheduler.Add((GeometryInstance3D)node, new Tweens.GeometryInstanceShaderParameter<float>("amount") { To = 1, Duration = 1, Fill = FillMode.None });
            Assert.False(HasOverride(node));
            scheduler.Update(0.5);
            Assert.Equal(0.625f, Get(node).AsSingle());
            Assert.True(HasOverride(node));
            scheduler.Update(0.5);
            Assert.Equal(Reason.Completed, tween.CompletionReason);
            Assert.Equal(0.25f, Get(node).AsSingle());
            Assert.False(HasOverride(node));
        }
        finally { node.Free(); }

        static Variant Get(Node node) => node is CanvasItem canvas
            ? canvas.GetInstanceShaderParameter("amount")
            : ((GeometryInstance3D)node).GetInstanceShaderParameter("amount");

        // Without an override the getter reports the default; only the property's storage flag tells them apart.
        static bool HasOverride(Node node)
        {
            foreach (var property in node.GetPropertyList())
                using (property)
                    if (property["name"].AsString() == "instance_shader_parameters/amount")
                        return ((PropertyUsageFlags)property["usage"].AsInt64() & PropertyUsageFlags.Storage) != 0;
            throw new InvalidOperationException("The instance uniform is not listed.");
        }
    }
}
