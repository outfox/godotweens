// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using godotweens;
using twodog.Testing;
using twodog.Testing.Xunit;
namespace testbed.Tests;

[Collection<RenderingCollection>]
[Trait("Category", "Rendering")]
public class MaterialRenderingTests(Fixture godot)
{
    private void Draw() { for (var i = 0; i < 4; i++) godot.Engine.Iteration(); RenderingServer.ForceDraw(); }
    private Color Pixel(SubViewport viewport, int x, int y)
    {
        Draw(); using var image = viewport.GetTexture().GetImage();
        Assert.False(image.IsEmpty()); return image.GetPixel(x, y);
    }
    [Theory]
    [InlineData("alpha")] [InlineData("emission")] [InlineData("uv")]
    public void MaterialTweensChangeRenderedPixels(string property)
    {
        var viewport = new SubViewport { Size = new Vector2I(128, 128), OwnWorld3D = true,
            RenderTargetUpdateMode = SubViewport.UpdateMode.Always };
        using var environment = new Godot.Environment { BackgroundMode = Godot.Environment.BGMode.Color, BackgroundColor = Colors.Black };
        using var mesh = new QuadMesh { Size = new Vector2(2, 2) };
        using var material = new StandardMaterial3D { ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded, AlbedoColor = Colors.Red };
        using var image = Image.CreateEmpty(8, 8, false, Image.Format.Rgba8);
        for (var y = 0; y < 8; y++) for (var x = 0; x < 8; x++) image.SetPixel(x, y, x < 4 ? Colors.Red : Colors.Blue);
        using var texture = ImageTexture.CreateFromImage(image);
        if (property == "alpha") material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        if (property == "emission") { material.ShadingMode = BaseMaterial3D.ShadingModeEnum.PerPixel; material.AlbedoColor = Colors.Black;
            material.EmissionEnabled = true; material.Emission = new Color(0.1f, 0, 0); }
        if (property == "uv") { material.AlbedoColor = Colors.White; material.AlbedoTexture = texture;
            material.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest; material.TextureRepeat = true; }
        viewport.AddChild(new WorldEnvironment { Environment = environment });
        viewport.AddChild(new Camera3D { Position = new Vector3(0, 0, 3), Projection = Camera3D.ProjectionType.Orthogonal, Size = 2, Current = true });
        viewport.AddChild(new MeshInstance3D { Mesh = mesh, MaterialOverride = material });
        godot.Tree.Root.AddChild(viewport);
        try
        {
            var before = Pixel(viewport, 32, 64);
            TweenInstance tween = property switch
            {
                "alpha" => material.TweenAlbedoAlpha(0.2f, 1, godot.Tree),
                "emission" => material.TweenEmissionEnergyMultiplier(5, 1, godot.Tree),
                _ => material.TweenUv1OffsetX(0.5f, 1, godot.Tree),
            };
            TweenRuntime.GetRunner(godot.Tree).Scheduler.Update(1);
            Assert.Equal(TweenState.Completed, tween.State);
            var after = Pixel(viewport, 32, 64);
            if (property == "alpha") Assert.True(before.R > after.R + 0.2f, $"Alpha: {before} -> {after}");
            if (property == "emission") Assert.True(after.R > before.R + 0.1f, $"Emission: {before} -> {after}");
            if (property == "uv") { Assert.True(before.R > 0.8f && before.B < 0.2f); Assert.True(after.B > 0.8f && after.R < 0.2f); }
        }
        finally { viewport.Free(); }
    }

    [Theory]
    [InlineData(false)] [InlineData(true)]
    public void SharedShaderInstancesRenderIndependentValues(bool spatial)
    {
        var viewport = new SubViewport { Size = new Vector2I(128, 64), OwnWorld3D = true,
            RenderTargetUpdateMode = SubViewport.UpdateMode.Always };
        using var shader = new Shader { Code = spatial
            ? "shader_type spatial; render_mode unshaded; instance uniform float amount = 0.25; void fragment(){ ALBEDO = vec3(amount, 0.0, 0.0); }"
            : "shader_type canvas_item; instance uniform float amount = 0.25; void fragment(){ COLOR = vec4(amount, 0.0, 0.0, 1.0); }" };
        using var material = new ShaderMaterial { Shader = shader };
        using var mesh = new QuadMesh { Size = Vector2.One * 2 };
        Node first;
        if (spatial)
        {
            viewport.AddChild(new Camera3D { Position = new Vector3(0, 0, 3), Projection = Camera3D.ProjectionType.Orthogonal, Size = 2, Current = true });
            first = new MeshInstance3D { Mesh = mesh, MaterialOverride = material, Position = new Vector3(-1, 0, 0) };
            viewport.AddChild(new MeshInstance3D { Mesh = mesh, MaterialOverride = material, Position = new Vector3(1, 0, 0) });
        }
        else
        {
            first = new ColorRect { Material = material, Size = new Vector2(64, 64) };
            viewport.AddChild(new ColorRect { Material = material, Size = new Vector2(64, 64), Position = new Vector2(64, 0) });
        }
        viewport.AddChild(first); godot.Tree.Root.AddChild(viewport);
        try
        {
            var leftBefore = Pixel(viewport, 32, 32); var rightBefore = Pixel(viewport, 96, 32);
            TweenInstance tween = spatial
                ? ((GeometryInstance3D)first).TweenInstanceShaderParameter("amount", 0.75f, 1)
                : ((CanvasItem)first).TweenInstanceShaderParameter("amount", 0.75f, 1);
            TweenRuntime.GetRunner(first).Scheduler.Update(1);
            var leftAfter = Pixel(viewport, 32, 32); var rightAfter = Pixel(viewport, 96, 32);
            Assert.Equal(TweenState.Completed, tween.State);
            Assert.True(leftAfter.R > leftBefore.R + 0.15f, $"Left: {leftBefore} -> {leftAfter}");
            Assert.True(rightAfter.IsEqualApprox(rightBefore), $"Right: {rightBefore} -> {rightAfter}");
        }
        finally { viewport.Free(); }
    }
}
