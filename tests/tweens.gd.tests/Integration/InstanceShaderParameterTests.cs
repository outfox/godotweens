// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd.Tests.Support;
using twodog.Testing;
using twodog.Testing.Xunit;

namespace tweens.gd.Tests.Integration;

/// <summary>Node-owned instance uniforms. Headless shaders report no defaults, so tests start from overrides.</summary>
[Collection<HeadlessCollection>]
public class InstanceShaderParameterTests(HeadlessFixture godot)
{
    private static ShaderMaterial Material(SceneScope scope, bool spatial)
        => scope.Track(new ShaderMaterial
        {
            Shader = scope.Track(new Shader
            {
                Code = $"shader_type {(spatial ? "spatial" : "canvas_item")}; instance uniform float amount; instance uniform vec2 pair;",
            }),
        });

    private static Node Node(SceneScope scope, bool spatial, ShaderMaterial material)
    {
        Node node = spatial
            ? new MeshInstance3D { Mesh = scope.Track(new BoxMesh()), MaterialOverride = material }
            : new ColorRect { Material = material };
        scope.Add(node);
        Set(node, "amount", 0.25f);
        return node;
    }

    private static void Set(Node node, string name, Variant value)
    {
        if (node is CanvasItem canvas) canvas.SetInstanceShaderParameter(name, value);
        else ((GeometryInstance3D)node).SetInstanceShaderParameter(name, value);
    }

    private static float Get(Node node) => (node is CanvasItem canvas
        ? canvas.GetInstanceShaderParameter("amount")
        : ((GeometryInstance3D)node).GetInstanceShaderParameter("amount")).AsSingle();

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void OverridesAnimateAndRestore(bool spatial)
    {
        using var scope = new SceneScope(godot);
        var node = Node(scope, spatial, Material(scope, spatial));
        TweenInstance tween = node is CanvasItem canvas
            ? canvas.TweenInstanceShaderParameter("amount", 1f, 1, d => d.Fill = FillMode.None)
            : ((GeometryInstance3D)node).TweenInstanceShaderParameter("amount", 1f, 1, d => d.Fill = FillMode.None);
        scope.Advance(0.5);
        Assert.Equal(0.625f, Get(node));
        scope.Advance(0.5);
        Assert.Equal(Reason.Completed, tween.CompletionReason);
        Assert.Equal(0.25f, Get(node));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void OptionsOverloadsCopyOptions(bool spatial)
    {
        using var scope = new SceneScope(godot);
        var node = Node(scope, spatial, Material(scope, spatial));
        var options = new TweenOptions { Duration = 10, Delay = 0.5 };
        TweenInstance tween = node is CanvasItem canvas
            ? canvas.TweenInstanceShaderParameter("amount", 1f, 1, options)
            : ((GeometryInstance3D)node).TweenInstanceShaderParameter("amount", 1f, 1, options);
        scope.Advance(1);
        Assert.Equal(0.5f, tween.Progress);
        Assert.Equal(0.625f, Get(node));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void InvalidConfigurationsAreRejected(bool spatial)
    {
        using var scope = new SceneScope(godot);
        using var scheduler = new TweenScheduler();
        var node = Node(scope, spatial, Material(scope, spatial));
        void Rejects<TException>(string parameter, float? from = null, bool vector = false) where TException : Exception
        {
            if (node is CanvasItem canvas)
                Assert.Throws<TException>(() => vector
                    ? scheduler.Add(canvas, new Tweens.CanvasItemInstanceShaderParameter<Quaternion>(parameter))
                    : scheduler.Add(canvas, new Tweens.CanvasItemInstanceShaderParameter<float>(parameter) { From = from }));
            else
                Assert.Throws<TException>(() => vector
                    ? scheduler.Add((GeometryInstance3D)node, new Tweens.GeometryInstanceShaderParameter<Quaternion>(parameter))
                    : scheduler.Add((GeometryInstance3D)node, new Tweens.GeometryInstanceShaderParameter<float>(parameter) { From = from }));
        }
        Rejects<ArgumentException>("");
        Rejects<ArgumentException>("missing");
        Rejects<ArgumentException>("pair");
        Rejects<ArgumentException>("amount", float.NaN);
        Rejects<NotSupportedException>("amount", vector: true);
        if (node is CanvasItem item)
            Assert.Throws<ArgumentException>(() => scheduler.Add(item, new Tweens.CanvasItemInstanceShaderParameter<float>("amount") { To = float.NaN }));
        Assert.Equal(0, scheduler.ActiveCount);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ReplacedMaterialsFaultWithoutWritingTheNewBinding(bool spatial)
    {
        using var scope = new SceneScope(godot);
        using var scheduler = new TweenScheduler();
        var node = Node(scope, spatial, Material(scope, spatial));
        TweenInstance tween = node is CanvasItem canvas
            ? scheduler.Add(canvas, new Tweens.CanvasItemInstanceShaderParameter<float>("amount") { To = 1, Duration = 1 })
            : scheduler.Add((GeometryInstance3D)node, new Tweens.GeometryInstanceShaderParameter<float>("amount") { To = 1, Duration = 1 });
        var replacement = Material(scope, spatial);
        if (node is CanvasItem item) item.Material = replacement;
        else ((GeometryInstance3D)node).MaterialOverride = replacement;
        scheduler.Update(0.5);
        Assert.IsType<InvalidOperationException>(tween.Error);
        Assert.Equal(0.25f, Get(node));
    }
}
