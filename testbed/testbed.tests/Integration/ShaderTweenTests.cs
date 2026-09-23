// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using godotweens;
using twodog.Testing;
using twodog.Testing.Xunit;

namespace testbed.Tests;

[Collection<RenderingCollection>]
[Trait("Category", "Rendering")]
public class ShaderTweenTests(Fixture godot)
{
    [Fact]
    public void UniformDefaultIsCapturedAndAbsentOverrideIsRestored()
    {
        using var shader = new Shader { Code = "shader_type canvas_item; uniform float amount = 0.25; void fragment() { COLOR = vec4(amount); }" };
        using var material = new ShaderMaterial { Shader = shader };
        using var scheduler = new TweenScheduler();
        var handle = scheduler.Add(material, new ShaderParameterTween<float>("amount") { To = 0.75f, Duration = 1, Fill = FillMode.None });
        Assert.Equal(0.25f, handle.Value);
        scheduler.Update(0.5);
        Assert.Null(handle.Error);
        Assert.Equal(0.5f, material.GetShaderParameter("amount").AsSingle());
        scheduler.Update(0.5);
        Assert.Equal(TweenState.Completed, handle.State);
        Assert.Equal(Variant.Type.Nil, material.GetShaderParameter("amount").VariantType);
    }

    private const string Uniforms = "uniform float scalar = 0.25; uniform int count = 2; uniform vec2 v2 = vec2(0.25); uniform vec3 v3 = vec3(0.25); uniform vec4 v4 = vec4(0.25); uniform vec4 tint : source_color = vec4(0.25);";

    [Theory]
    [InlineData("float")] [InlineData("double")] [InlineData("int")]
    [InlineData("Vector2")] [InlineData("Vector3")] [InlineData("Vector4")] [InlineData("Color")]
    public void AllSupportedUniformTypesCaptureInterpolateAndRestore(string type)
    {
        using var shader = new Shader { Code = "shader_type canvas_item; " + Uniforms };
        using var material = new ShaderMaterial { Shader = shader };
        switch (type)
        {
            case "float": Check("scalar", 0.25f, 0.75f, 0.5f); break;
            case "double": Check("scalar", 0.25d, 0.75d, 0.5d); break;
            case "int": Check("count", 2, 5, 4); break;
            case "Vector2": Check("v2", Vector2.One * 0.25f, Vector2.One * 0.75f, Vector2.One * 0.5f); break;
            case "Vector3": Check("v3", Vector3.One * 0.25f, Vector3.One * 0.75f, Vector3.One * 0.5f); break;
            case "Vector4": Check("v4", Vector4.One * 0.25f, Vector4.One * 0.75f, Vector4.One * 0.5f); break;
            case "Color": Check("tint", new Color(0.25f, 0.25f, 0.25f, 0.25f), new Color(0.75f, 0.75f, 0.75f, 0.75f), new Color(0.5f, 0.5f, 0.5f, 0.5f)); break;
        }
        void Check<[MustBeVariant] T>(string name, T initial, T to, T middle) where T : struct
        {
            var handle = material.TweenShaderParameter(name, to, 1, godot.Tree, d => d.Fill = FillMode.None);
            Assert.Equal(initial, handle.Value);
            var scheduler = TweenRuntime.GetRunner(godot.Tree).Scheduler;
            scheduler.Update(0.5); Assert.Null(handle.Error); Assert.Equal(middle, material.GetShaderParameter(name).As<T>());
            scheduler.Update(0.5); Assert.Equal(TweenState.Completed, handle.State);
            Assert.Equal(Variant.Type.Nil, material.GetShaderParameter(name).VariantType);
            material.SetShaderParameter(name, Variant.From(initial));
            var explicitOverride = material.TweenShaderParameter(name, to, 1, godot.Tree, d => d.Fill = FillMode.None);
            scheduler.Update(1); Assert.Equal(TweenState.Completed, explicitOverride.State);
            Assert.Equal(initial, material.GetShaderParameter(name).As<T>());
        }
    }

    [Fact]
    public void BadUniformsFailBeforePlaybackAndDoNotChangeTheMaterial()
    {
        using var shader = new Shader { Code = "shader_type canvas_item; " + Uniforms };
        using var material = new ShaderMaterial { Shader = shader };
        using var scheduler = new TweenScheduler();
        Assert.Throws<ArgumentException>(() => scheduler.Add(material, new ShaderParameterTween<float>("missing")));
        Assert.Throws<ArgumentException>(() => scheduler.Add(material, new ShaderParameterTween<int>("scalar")));
        Assert.Throws<ArgumentException>(() => scheduler.Add(material, new ShaderParameterTween<Vector4>("tint")));
        Assert.Throws<ArgumentException>(() => scheduler.Add(material, new ShaderParameterTween<Color>("v4")));
        Assert.Throws<ArgumentException>(() => scheduler.Add(material, new ShaderParameterTween<float>(" ")));
        Assert.Throws<NotSupportedException>(() => scheduler.Add(material, new ShaderParameterTween<bool>("scalar")));
        Assert.Throws<ArgumentException>(() => scheduler.Add(material, new ShaderParameterTween<float>("scalar") { To = float.NaN }));
        Assert.Throws<ArgumentException>(() => scheduler.Add(material, new ShaderParameterTween<Vector3>("v3") { From = new Vector3(float.PositiveInfinity, 0, 0) }));
        Assert.Equal(0, scheduler.ActiveCount);
        Assert.Equal(Variant.Type.Nil, material.GetShaderParameter("scalar").VariantType);
        using var missingShader = new ShaderMaterial();
        Assert.Throws<ArgumentException>(() => scheduler.Add(missingShader, new ShaderParameterTween<float>("scalar")));
    }

    [Theory]
    [InlineData(false)] [InlineData(true)]
    public async Task ShaderReplacementAndEditsFaultAndSettlePlayback(bool edit)
    {
        using var shader = new Shader { Code = "shader_type canvas_item; uniform float amount = 0.25;" };
        using var replacement = new Shader { Code = "shader_type canvas_item; uniform vec3 amount = vec3(1.0);" };
        using var material = new ShaderMaterial { Shader = shader };
        using var scheduler = new TweenScheduler();
        var handle = scheduler.Add(material, new ShaderParameterTween<float>("amount") { To = 1, Duration = 1 });
        if (edit) shader.Code = replacement.Code; else material.Shader = replacement;
        scheduler.Update(0.5);
        Assert.Equal(TweenState.Faulted, handle.State);
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await handle.Completion);
        Assert.Equal(Variant.Type.Nil, material.GetShaderParameter("amount").VariantType);
        Assert.Equal(0, scheduler.ActiveCount);
    }

    [Fact]
    public void DefinitionReuseCapturesParameterAndOverrideIndependently()
    {
        using var shader = new Shader { Code = "shader_type canvas_item; uniform float amount = 0.25; uniform float other = 1.0;" };
        using var first = new ShaderMaterial { Shader = shader };
        using var second = new ShaderMaterial { Shader = shader };
        second.SetShaderParameter("amount", 0.5f);
        using var scheduler = new TweenScheduler();
        var definition = new ShaderParameterTween<float>("amount") { To = 1, Duration = 1, Fill = FillMode.None };
        var a = scheduler.Add(first, definition); var b = scheduler.Add(second, definition);
        definition.Parameter = "other"; definition.To = 0;
        scheduler.Update(0.5);
        Assert.Equal(0.625f, first.GetShaderParameter("amount").AsSingle());
        Assert.Equal(0.75f, second.GetShaderParameter("amount").AsSingle());
        scheduler.Update(0.5);
        Assert.Equal(TweenState.Completed, a.State); Assert.Equal(TweenState.Completed, b.State);
        Assert.Equal(Variant.Type.Nil, first.GetShaderParameter("amount").VariantType);
        Assert.Equal(0.5f, second.GetShaderParameter("amount").AsSingle());
        Assert.Equal(Variant.Type.Nil, first.GetShaderParameter("other").VariantType);
    }

    [Fact]
    public void OwnerOverloadUsesSharedResourceAndCancellationRetainsTheSample()
    {
        using var shader = new Shader { Code = "shader_type canvas_item; uniform float amount = 0.25;" };
        using var material = new ShaderMaterial { Shader = shader };
        var first = new ColorRect { Material = material }; var second = new ColorRect { Material = material };
        godot.Tree.Root.AddChild(first); godot.Tree.Root.AddChild(second);
        try
        {
            var tween = material.TweenShaderParameter("amount", 0.75f, 1, first);
            TweenRuntime.GetRunner(first).Scheduler.Update(0.5);
            Assert.Equal(0.5f, ((ShaderMaterial)second.Material).GetShaderParameter("amount").AsSingle());
            first.CancelTweens();
            Assert.Equal(TweenCompletionReason.Cancelled, tween.CompletionReason);
            Assert.Equal(0.5f, material.GetShaderParameter("amount").AsSingle());
            Assert.Same(material, first.Material); Assert.Same(material, second.Material);
        }
        finally { first.Free(); second.Free(); }
    }

    [Theory]
    [InlineData(false, false)] [InlineData(false, true)]
    [InlineData(true, false)] [InlineData(true, true)]
    public void InstanceUniformsAreIndependentAndRestoreOverridePresence(bool spatial, bool explicitOverride)
    {
        using var shader = new Shader { Code = "shader_type " + (spatial ? "spatial" : "canvas_item") + "; instance uniform float amount = 0.25;" };
        using var material = new ShaderMaterial { Shader = shader };
        using var mesh = new QuadMesh();
        Node first = spatial ? new MeshInstance3D { Mesh = mesh, MaterialOverride = material } : new ColorRect { Material = material };
        Node second = spatial ? new MeshInstance3D { Mesh = mesh, MaterialOverride = material } : new ColorRect { Material = material };
        godot.Tree.Root.AddChild(first); godot.Tree.Root.AddChild(second);
        try
        {
            if (explicitOverride) Set(first, "amount", 0.25f);
            Assert.Equal(explicitOverride, HasOverride(first, "amount"));
            TweenInstance tween = spatial
                ? ((GeometryInstance3D)first).TweenInstanceShaderParameter("amount", 0.75f, 1, d => d.Fill = FillMode.None)
                : ((CanvasItem)first).TweenInstanceShaderParameter("amount", 0.75f, 1, d => d.Fill = FillMode.None);
            var scheduler = TweenRuntime.GetRunner(first).Scheduler;
            scheduler.Update(0.5); Assert.Null(tween.Error);
            Assert.Equal(0.5f, Get(first, "amount").AsSingle());
            Assert.Equal(0.25f, Get(second, "amount").AsSingle());
            Assert.True(HasOverride(first, "amount"));
            scheduler.Update(0.5); Assert.Equal(TweenState.Completed, tween.State);
            Assert.Equal(0.25f, Get(first, "amount").AsSingle());
            Assert.Equal(explicitOverride, HasOverride(first, "amount"));
        }
        finally { first.Free(); second.Free(); }
    }

    [Theory]
    [InlineData(false)] [InlineData(true)]
    public async Task InstanceMaterialReplacementFaultsWithoutWritingTheNewBinding(bool spatial)
    {
        using var shader = new Shader { Code = "shader_type " + (spatial ? "spatial" : "canvas_item") + "; instance uniform float amount = 0.25;" };
        using var material = new ShaderMaterial { Shader = shader };
        using var replacement = new ShaderMaterial { Shader = shader };
        using var mesh = new QuadMesh();
        Node node = spatial ? new MeshInstance3D { Mesh = mesh, MaterialOverride = material } : new ColorRect { Material = material };
        godot.Tree.Root.AddChild(node);
        try
        {
            TweenInstance tween = spatial
                ? ((GeometryInstance3D)node).TweenInstanceShaderParameter("amount", 1f, 1)
                : ((CanvasItem)node).TweenInstanceShaderParameter("amount", 1f, 1);
            if (node is CanvasItem canvas) canvas.Material = replacement;
            else ((GeometryInstance3D)node).MaterialOverride = replacement;
            TweenRuntime.GetRunner(node).Scheduler.Update(0.5);
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await tween.Completion);
            Assert.False(HasOverride(node, "amount"));
        }
        finally { node.Free(); }
    }

    private static Variant Get(Node node, string name) => node is CanvasItem canvas
        ? canvas.GetInstanceShaderParameter(name) : ((GeometryInstance3D)node).GetInstanceShaderParameter(name);
    private static void Set(Node node, string name, Variant value)
    {
        if (node is CanvasItem canvas) canvas.SetInstanceShaderParameter(name, value);
        else ((GeometryInstance3D)node).SetInstanceShaderParameter(name, value);
    }
    private static bool HasOverride(Node node, string name)
    {
        foreach (var p in node.GetPropertyList())
            using (p)
                if (p["name"].AsString() == "instance_shader_parameters/" + name)
                    return ((PropertyUsageFlags)p["usage"].AsInt64() & PropertyUsageFlags.Storage) != 0;
        throw new InvalidOperationException("Uniform metadata is missing.");
    }

    [Theory]
    [InlineData(false)] [InlineData(true)]
    public void AllInstanceValueTypesAndValidationUseTheNativeUniformContract(bool spatial)
    {
        using var shader = new Shader { Code = "shader_type " + (spatial ? "spatial" : "canvas_item") + "; " + Uniforms.Replace("uniform ", "instance uniform ") };
        using var material = new ShaderMaterial { Shader = shader };
        using var mesh = new QuadMesh();
        Node node = spatial ? new MeshInstance3D { Mesh = mesh, MaterialOverride = material } : new ColorRect { Material = material };
        godot.Tree.Root.AddChild(node);
        try
        {
            Check("scalar", 0.75f, 0.5f); Check("scalar", 0.75d, 0.5d); Check("count", 5, 4);
            Check("v2", Vector2.One * 0.75f, Vector2.One * 0.5f);
            Check("v3", Vector3.One * 0.75f, Vector3.One * 0.5f);
            Check("v4", Vector4.One * 0.75f, Vector4.One * 0.5f);
            Check("tint", new Color(0.75f, 0.75f, 0.75f, 0.75f), new Color(0.5f, 0.5f, 0.5f, 0.5f));
            Assert.Throws<ArgumentException>(() => Start("missing", 1f));
            Assert.Throws<ArgumentException>(() => Start("scalar", 1));
            Assert.Throws<ArgumentException>(() => Start("scalar", double.PositiveInfinity));
            Assert.Throws<NotSupportedException>(() => Start("scalar", true));
            var removed = Start("scalar", 1f); removed.Pause();
            godot.Tree.Root.RemoveChild(node);
            Assert.Equal(TweenCompletionReason.OwnerExited, removed.CompletionReason);
        }
        finally { node.Free(); }
        TweenInstance Start<T>(string name, T to) where T : struct => spatial
            ? ((GeometryInstance3D)node).TweenInstanceShaderParameter(name, to, 1, d => d.Fill = FillMode.None)
            : ((CanvasItem)node).TweenInstanceShaderParameter(name, to, 1, d => d.Fill = FillMode.None);
        void Check<[MustBeVariant] T>(string name, T to, T middle) where T : struct
        {
            var handle = Start(name, to);
            var scheduler = TweenRuntime.GetRunner(node).Scheduler;
            scheduler.Update(0.5); Assert.Null(handle.Error); Assert.Equal(middle, Get(node, name).As<T>());
            scheduler.Update(0.5); Assert.Equal(TweenState.Completed, handle.State); Assert.False(HasOverride(node, name));
        }
    }

    [Fact]
    public async Task InheritedCanvasMaterialIsTrackedAndShaderEditsFaultInstancePlayback()
    {
        using var shader = new Shader { Code = "shader_type canvas_item; instance uniform float amount = 0.25;" };
        using var material = new ShaderMaterial { Shader = shader };
        var parent = new Node2D { Material = material }; var child = new ColorRect { UseParentMaterial = true };
        parent.AddChild(child); godot.Tree.Root.AddChild(parent);
        try
        {
            var tween = child.TweenInstanceShaderParameter("amount", 1f, 1);
            shader.Code += "\n// changed";
            TweenRuntime.GetRunner(child).Scheduler.Update(0.5);
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await tween.Completion);
        }
        finally { parent.Free(); }
    }

    [Theory]
    [InlineData("surface")] [InlineData("mesh")] [InlineData("next-pass")]
    public async Task SpatialBindingsDetectSurfaceMeshAndPassReplacement(string change)
    {
        using var shader = new Shader { Code = "shader_type spatial; instance uniform float amount = 0.25;" };
        using var material = new ShaderMaterial { Shader = shader };
        using var replacement = new ShaderMaterial { Shader = shader };
        using var baseMaterial = new StandardMaterial3D { NextPass = material };
        using var mesh = new QuadMesh { Material = change == "next-pass" ? baseMaterial : material };
        using var replacementMesh = new QuadMesh { Material = material };
        var node = new MeshInstance3D { Mesh = mesh }; godot.Tree.Root.AddChild(node);
        try
        {
            var tween = node.TweenInstanceShaderParameter("amount", 1f, 1);
            if (change == "surface") node.SetSurfaceOverrideMaterial(0, replacement);
            if (change == "mesh") node.Mesh = replacementMesh;
            if (change == "next-pass") baseMaterial.NextPass = replacement;
            TweenRuntime.GetRunner(node).Scheduler.Update(0.5);
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await tween.Completion);
        }
        finally { node.Free(); }
    }

    [Fact]
    public async Task NonFiniteShaderOutputFaultsWithoutWritingAndInvalidInitialValuesFailEarly()
    {
        using var shader = new Shader { Code = "shader_type canvas_item; uniform float amount = 0.25; uniform int count = 1;" };
        using var material = new ShaderMaterial { Shader = shader };
        using var scheduler = new TweenScheduler();
        material.SetShaderParameter("amount", float.NaN);
        Assert.Throws<ArgumentException>(() => scheduler.Add(material, new ShaderParameterTween<float>("amount")));
        material.SetShaderParameter("amount", default);
        material.SetShaderParameter("count", (long)int.MaxValue + 1);
        Assert.Throws<OverflowException>(() => scheduler.Add(material, new ShaderParameterTween<int>("count")));
        var handle = scheduler.Add(material, new ShaderParameterTween<float>("amount") { From = -float.MaxValue, To = float.MaxValue, Duration = 1 });
        scheduler.Update(0.5);
        await Assert.ThrowsAsync<ArgumentException>(async () => await handle.Completion);
        Assert.Equal(Variant.Type.Nil, material.GetShaderParameter("amount").VariantType);
    }
}
