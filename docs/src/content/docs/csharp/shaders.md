---
title: Shader uniforms
description: Typed material and per-instance uniforms, binding validation, defaults, and restoration.
---

Animate shader uniforms by name: on a shared `ShaderMaterial`, or per node with
instance uniforms. Names and types are checked before playback starts.

Import `Godot` and `tweens.gd`. These examples run in a Node method on the main
thread. `shaderMaterial` is a configured ShaderMaterial; `mesh` and `sprite`
are in-tree MeshInstance3D and Sprite2D nodes using the declared shaders.

## Ordinary shader uniforms

```csharp
// shader: uniform float dissolve = 0.25;
shaderMaterial.TweenShaderParameter("dissolve", 1f, 0.5, GetTree());
shaderMaterial.TweenShaderParameter("dissolve", 1f, 0.5, mesh);

var definition = new Tweens.ShaderParameter<float>("dissolve")
{
    To = 1,
    Duration = 0.5,
    Fill = FillMode.None,
};
shaderMaterial.Tween(definition, GetTree());
```

The material is shared as usual. Uniform names are case-sensitive and snapshotted at addition. Missing shaders, missing names, incompatible types and non-finite endpoints fail before playback registration or writes. A missing override captures the declared shader default, rather than zero.

| C# value | Required uniform metadata |
| --- | --- |
| float, double | Float (shader float) |
| int | Int (shader int) |
| Vector2 / Vector3 / Vector4 | Matching vector type |
| Color | Color (typically vec4 with source_color hint) |

Color and Vector4 are deliberately distinct. A double uses Godot's floating Variant representation, but the shader/GPU ultimately determines precision. Integers round halfway away from zero and saturate at Int32 bounds during interpolation. Non-finite samples fault playback before writing. Textures, resources, arrays, booleans, enums and quaternion representations are not supported.

On natural completion without `RetainFinalValue`, the original explicit override is restored, or the new override is removed if none originally existed. Cancellation keeps the latest sample, matching node tweens. Source definitions can be reused across materials with different initial values and override states.

Replacing, disposing, or editing the bound shader faults playback when it next attempts a sample or restoration. Any shader change signal is treated as a binding change, even if the new declaration happens to be compatible. `End` settles with the error. Start a new tween after changing the shader. No uniform metadata scan or string path parsing occurs per frame.

Default lookup requires a working renderer. Godot's dummy headless renderer can expose declarations while returning no default value; capture then fails rather than inventing one. An explicit material override can be used in headless property tests, but rendering tests are required to verify defaults and visible behavior.

## Per-instance shader uniforms

Declare an `instance uniform` in the shader when nodes sharing the same material need independent values:

```csharp
// shader: instance uniform float pulse = 0.25;
mesh.TweenInstanceShaderParameter("pulse", 1f, 0.5);
sprite.TweenInstanceShaderParameter("pulse", 0f, 0.5);
```

These methods target `GeometryInstance3D` and `CanvasItem` respectively and follow normal node lifetime/pause rules. Definitions are `Tweens.GeometryInstanceShaderParameter<T>` and `Tweens.CanvasItemInstanceShaderParameter<T>`. The same value types, validation, snapshot and restoration rules apply. An explicit override equal to the default remains explicit after restoration; an originally absent override is removed.

Effective material bindings are captured, including inherited CanvasItem materials, mesh surfaces, overrides, overlays and next passes. Replacing the mesh/material/pass or editing a bound shader faults the next write. Binding checks are conservative: changing a tracked slot can fault playback even if another slot still declares the same uniform. Godot controls instance-uniform indexing, capacity, shader compatibility and multi-material conflicts; this API does not assign or reconcile those declarations.

See [ShaderMaterial](https://docs.godotengine.org/en/stable/classes/class_shadermaterial.html), [CanvasItem](https://docs.godotengine.org/en/stable/classes/class_canvasitem.html), and [GeometryInstance3D](https://docs.godotengine.org/en/stable/classes/class_geometryinstance3d.html).
See [compatibility](/compatibility/) for rendering limits and
[playback and async](/csharp/playback/) for fault handling.
