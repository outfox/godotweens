// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd.Tests.Support;
using twodog.Testing;
using twodog.Testing.Xunit;

namespace tweens.gd.Tests.Integration;

/// <summary>Material uniforms. Headless shaders report no defaults, so tests start from explicit overrides.</summary>
[Collection<HeadlessCollection>]
public class ShaderParameterTests(HeadlessFixture godot)
{
    private const string Uniforms = "shader_type canvas_item; uniform float scalar; uniform int count; "
        + "uniform vec2 pair; uniform vec3 triple; uniform vec4 quad; uniform vec4 tint : source_color;";

    private static ShaderMaterial Material(SceneScope scope, string code = Uniforms)
        => scope.Track(new ShaderMaterial { Shader = scope.Track(new Shader { Code = code }) });

    [Fact]
    public void EveryUniformTypeAnimatesAndRetainsItsValue()
    {
        using var scope = new SceneScope(godot);
        using var scheduler = new TweenScheduler();
        var material = Material(scope);
        Check(material, scheduler, "scalar", 1f, 3f, 2f);
        Check(material, scheduler, "scalar", 1d, 3d, 2d);
        Check(material, scheduler, "count", 1, 4, 3);
        Check(material, scheduler, "pair", Vector2.Zero, new Vector2(2, 4), new Vector2(1, 2));
        Check(material, scheduler, "triple", Vector3.Zero, new Vector3(2, 4, 6), new Vector3(1, 2, 3));
        Check(material, scheduler, "quad", Vector4.Zero, new Vector4(2, 4, 6, 8), new Vector4(1, 2, 3, 4));
        Check(material, scheduler, "tint", new Color(0, 0, 0, 0), new Color(1, 1, 1, 1), new Color(0.5f, 0.5f, 0.5f, 0.5f));

        static void Check<[MustBeVariant] T>(ShaderMaterial material, TweenScheduler scheduler, string name, T initial, T to, T middle)
            where T : struct
        {
            material.SetShaderParameter(name, Variant.From(initial));
            var tween = scheduler.Add(material, new Tweens.ShaderParameter<T>(name) { To = to, Duration = 1 });
            Assert.Equal(initial, tween.Value);
            scheduler.Update(0.5);
            Assert.Equal(middle, ShaderValues<T>.Read(material.GetShaderParameter(name)));
            scheduler.Update(0.5);
            Assert.Equal(to, ShaderValues<T>.Read(material.GetShaderParameter(name)));
        }
    }

    [Fact]
    public void NonRetainingPlaybackRestoresTheOverride()
    {
        using var scope = new SceneScope(godot);
        var material = Material(scope);
        material.SetShaderParameter("scalar", 0.25f);
        var owner = scope.Add(new Node());
        var tween = material.TweenShaderParameter("scalar", 1f, 1, owner, d => d.Fill = FillMode.None);
        scope.Advance(0.5);
        Assert.Equal(0.625f, material.GetShaderParameter("scalar").AsSingle());
        scope.Advance(0.5);
        Assert.Equal(Reason.Completed, tween.CompletionReason);
        Assert.Equal(0.25f, material.GetShaderParameter("scalar").AsSingle());
    }

    [Fact]
    public void ConvenienceOverloadsBindTreeOrOwner()
    {
        using var scope = new SceneScope(godot);
        var material = Material(scope);
        material.SetShaderParameter("scalar", 0f);
        var owner = scope.Add(new Node());
        var options = new TweenOptions { Duration = 10, Delay = 0.5 };
        var treeConfigured = material.TweenShaderParameter("scalar", 1f, 1, godot.Tree, d => d.Delay = 0.25);
        var treeOptions = material.TweenShaderParameter("scalar", 1f, 1, godot.Tree, options, owner);
        var ownerOptions = material.TweenShaderParameter("scalar", 1f, 1, owner, options);
        var treeDefault = material.TweenShaderParameter("scalar", 1f, 1, godot.Tree);
        scope.Advance(0.75);
        Assert.Equal(0.5f, treeConfigured.Progress);
        Assert.Equal(0.25f, treeOptions.Progress);
        Assert.Equal(0.25f, ownerOptions.Progress);
        Assert.Equal(0.75f, treeDefault.Progress);
        scope.Root.RemoveChild(owner);
        Assert.Equal(Reason.OwnerExited, treeOptions.CompletionReason);
        Assert.Equal(Reason.OwnerExited, ownerOptions.CompletionReason);
        owner.Free();
    }

    [Fact]
    public void InvalidConfigurationsAreRejectedBeforePlayback()
    {
        using var scope = new SceneScope(godot);
        using var scheduler = new TweenScheduler();
        var material = Material(scope);
        material.SetShaderParameter("scalar", 0f);
        Assert.Throws<ArgumentException>(() => scheduler.Add(material, new Tweens.ShaderParameter<float>(" ")));
        Assert.Throws<ArgumentNullException>(() => scheduler.Add(material, new Tweens.ShaderParameter<float>(null!)));
        Assert.Throws<NotSupportedException>(() => scheduler.Add(material, new Tweens.ShaderParameter<Quaternion>("scalar")));
        Assert.Throws<ArgumentException>(() => scheduler.Add(material, new Tweens.ShaderParameter<float>("scalar") { From = float.NaN }));
        Assert.Throws<ArgumentException>(() => scheduler.Add(material, new Tweens.ShaderParameter<float>("scalar") { To = float.NaN }));
        Assert.Throws<ArgumentException>(() => scheduler.Add(material, new Tweens.ShaderParameter<float>("missing")));
        Assert.Throws<ArgumentException>(() => scheduler.Add(material, new Tweens.ShaderParameter<Vector2>("scalar")));
        Assert.Throws<ArgumentException>(() => scheduler.Add(scope.Track(new ShaderMaterial()), new Tweens.ShaderParameter<float>("scalar")));
        Assert.Equal(0, scheduler.ActiveCount);
    }

    [Fact]
    public void EditedShadersFaultPlayback()
    {
        using var scope = new SceneScope(godot);
        using var scheduler = new TweenScheduler();
        var material = Material(scope);
        material.SetShaderParameter("scalar", 0f);
        var tween = scheduler.Add(material, new Tweens.ShaderParameter<float>("scalar") { To = 1, Duration = 1 });
        material.Shader.Code = Uniforms + " uniform float extra;";
        scheduler.Update(0.5);
        Assert.Equal(TweenState.Faulted, tween.State);
        Assert.IsType<InvalidOperationException>(tween.Error);
        Assert.Equal(0f, material.GetShaderParameter("scalar").AsSingle());
    }

    [Fact]
    public void ReplacedShadersFaultPlaybackAndRestoration()
    {
        using var scope = new SceneScope(godot);
        using var scheduler = new TweenScheduler();
        var material = Material(scope);
        material.SetShaderParameter("scalar", 0f);
        var writing = scheduler.Add(material, new Tweens.ShaderParameter<float>("scalar") { To = 1, Duration = 1 });
        var restoring = scheduler.Add(material, new Tweens.ShaderParameter<float>("scalar")
        {
            To = 1, Duration = 1, Fill = FillMode.None,
            OnUpdate = (_, v) => { if (v == 1) material.Shader = scope.Track(new Shader { Code = Uniforms }); },
        });
        scheduler.Update(1);
        Assert.Equal(Reason.Completed, writing.CompletionReason);
        Assert.IsType<InvalidOperationException>(restoring.Error);
    }

    [Fact]
    public void DisposedShadersFaultPlaybackAndReleaseCleanly()
    {
        using var scope = new SceneScope(godot);
        using var scheduler = new TweenScheduler();
        var shader = new Shader { Code = Uniforms };
        var material = scope.Track(new ShaderMaterial { Shader = shader });
        material.SetShaderParameter("scalar", 0f);
        var tween = scheduler.Add(material, new Tweens.ShaderParameter<float>("scalar") { To = 1, Duration = 1 });
        material.Shader = null;
        shader.Dispose();
        scheduler.Update(0.5);
        Assert.IsType<InvalidOperationException>(tween.Error);
    }
}
