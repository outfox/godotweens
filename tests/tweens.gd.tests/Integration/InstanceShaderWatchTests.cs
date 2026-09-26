// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss

using Godot;
using tweens.gd.Tests.Support;
using twodog.Testing;
using twodog.Testing.Xunit;

namespace tweens.gd.Tests.Integration;

/// <summary>The binding snapshot that faults instance-uniform playback when effective materials change.</summary>
[Collection<HeadlessCollection>]
public class InstanceShaderWatchTests(HeadlessFixture godot)
{
    private static ShaderMaterial Shaded(SceneScope scope, string type = "spatial")
        => scope.Track(new ShaderMaterial { Shader = scope.Track(new Shader { Code = $"shader_type {type};" }) });

    private static ArrayMesh Surfaces(SceneScope scope, int count)
    {
        var mesh = scope.Track(new ArrayMesh());
        using var box = new BoxMesh();
        for (var i = 0; i < count; i++) mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, box.GetMeshArrays());
        return mesh;
    }

    private static void Faults(InstanceShaderWatch watch, string message)
        => Assert.Contains(message, Assert.Throws<InvalidOperationException>(watch.Validate).Message);

    [Fact]
    public void CanvasItemsWatchTheirEffectiveMaterial()
    {
        using var scope = new SceneScope(godot);
        var parent = scope.Add(new ColorRect { Material = Shaded(scope, "canvas_item") });
        var child = new ColorRect { UseParentMaterial = true };
        parent.AddChild(child);
        var orphan = scope.Add(new ColorRect());

        using var inherited = new InstanceShaderWatch(child);
        using var unassigned = new InstanceShaderWatch(orphan);
        inherited.Validate();
        unassigned.Validate();
        parent.Material = Shaded(scope, "canvas_item");
        orphan.Material = Shaded(scope, "canvas_item");
        Faults(inherited, "material binding changed");
        Faults(unassigned, "material binding changed");
    }

    [Fact]
    public void MeshInstancesWatchTheirActiveSurfaceMaterials()
    {
        using var scope = new SceneScope(godot);
        var mesh = Surfaces(scope, 2);
        mesh.SurfaceSetMaterial(0, Shaded(scope));
        mesh.SurfaceSetMaterial(1, scope.Track(new StandardMaterial3D()));
        var node = scope.Add(new MeshInstance3D { Mesh = mesh, MaterialOverlay = Shaded(scope) });
        using var watch = new InstanceShaderWatch(node);
        watch.Validate();
        node.SetSurfaceOverrideMaterial(1, Shaded(scope));
        Faults(watch, "material binding changed");
    }

    [Fact]
    public void MaterialOverridesHideSurfaceChanges()
    {
        using var scope = new SceneScope(godot);
        var node = scope.Add(new MeshInstance3D { Mesh = Surfaces(scope, 2), MaterialOverride = Shaded(scope) });
        using var watch = new InstanceShaderWatch(node);
        node.SetSurfaceOverrideMaterial(1, Shaded(scope));
        watch.Validate();
        node.MaterialOverride = Shaded(scope);
        Faults(watch, "material binding changed");
    }

    [Fact]
    public void MeshChangesFaultPlayback()
    {
        using var scope = new SceneScope(godot);
        var mesh = Surfaces(scope, 1);
        var swapped = scope.Add(new MeshInstance3D { Mesh = mesh });
        var grown = scope.Add(new MeshInstance3D { Mesh = mesh });
        using var swapWatch = new InstanceShaderWatch(swapped);
        swapped.Mesh = Surfaces(scope, 1);
        Faults(swapWatch, "mesh binding changed");

        using var growWatch = new InstanceShaderWatch(grown);
        using var box = new BoxMesh();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, box.GetMeshArrays());
        Faults(growWatch, "mesh binding changed");
    }

    [Fact]
    public void MultiMeshInstancesWatchTheirMeshSurfaces()
    {
        using var scope = new SceneScope(godot);
        var mesh = Surfaces(scope, 1);
        mesh.SurfaceSetMaterial(0, Shaded(scope));
        var multimesh = scope.Track(new MultiMesh { Mesh = mesh });
        var node = scope.Add(new MultiMeshInstance3D { Multimesh = multimesh });
        var empty = scope.Add(new MultiMeshInstance3D());
        using var watch = new InstanceShaderWatch(node);
        using var emptyWatch = new InstanceShaderWatch(empty);
        watch.Validate();
        emptyWatch.Validate();
        mesh.SurfaceSetMaterial(0, Shaded(scope));
        Faults(watch, "material binding changed");
    }

    [Fact]
    public void GeometryWithoutMeshWatchesOverrideAndOverlay()
    {
        using var scope = new SceneScope(godot);
        var node = scope.Add(new Sprite3D());
        using var watch = new InstanceShaderWatch(node);
        watch.Validate();
        node.MaterialOverlay = Shaded(scope);
        Faults(watch, "material binding changed");
    }

    [Fact]
    public void DisposedMaterialsFaultPlayback()
    {
        using var scope = new SceneScope(godot);
        var material = new ShaderMaterial { Shader = scope.Track(new Shader { Code = "shader_type spatial;" }) };
        var node = scope.Add(new MeshInstance3D { MaterialOverride = material });
        using var watch = new InstanceShaderWatch(node);
        material.Dispose();
        Faults(watch, "disposed during playback");
    }

    [Fact]
    public void NextPassesAreWatched()
    {
        using var scope = new SceneScope(godot);
        var next = new ShaderMaterial { Shader = scope.Track(new Shader { Code = "shader_type spatial;" }) };
        var material = Shaded(scope);
        material.NextPass = next;
        var repassed = Shaded(scope);
        repassed.NextPass = Shaded(scope);
        var node = scope.Add(new MeshInstance3D { MaterialOverride = material, MaterialOverlay = repassed });
        using var watch = new InstanceShaderWatch(node);
        watch.Validate();
        repassed.NextPass = Shaded(scope);
        Faults(watch, "pass binding changed");
        repassed.NextPass = null;

        using var nextWatch = new InstanceShaderWatch(scope.Add(new MeshInstance3D { MaterialOverride = material }));
        next.Dispose();
        Faults(nextWatch, "pass binding changed");
    }

    [Fact]
    public void ShaderEditsAreWatched()
    {
        using var scope = new SceneScope(godot);
        var material = Shaded(scope);
        var node = scope.Add(new MeshInstance3D { MaterialOverride = material });
        using var watch = new InstanceShaderWatch(node);
        material.Shader.Code = "shader_type spatial; instance uniform float amount;";
        Assert.Contains("shader binding changed", Assert.Throws<InvalidOperationException>(watch.Validate).Message);
    }

    [Fact]
    public void MaterialsWithoutShadersCannotBeWatched()
    {
        using var scope = new SceneScope(godot);
        var bare = scope.Add(new MeshInstance3D { MaterialOverride = scope.Track(new ShaderMaterial()) });
        Assert.Throws<ArgumentException>(() => new InstanceShaderWatch(bare));

        var material = Shaded(scope);
        material.NextPass = scope.Track(new ShaderMaterial());
        var chained = scope.Add(new MeshInstance3D { MaterialOverride = material });
        Assert.Throws<ArgumentException>(() => new InstanceShaderWatch(chained));
    }
}
