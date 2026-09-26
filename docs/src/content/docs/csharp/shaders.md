---
title: Shader uniforms
description: Typed material and per-instance uniforms, binding validation, defaults, and restoration.
---

Animate shader uniforms by name, either on a shared `ShaderMaterial` or per node
with instance uniforms. Names and types are checked before playback starts.

Import `Godot` and `tweens.gd`. These examples run in a Node method on the main
thread. `shaderMaterial` is a configured ShaderMaterial. `mesh` and `sprite` are
in-tree MeshInstance3D and Sprite2D nodes using the declared shaders.

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

The material is shared as usual, so every node using it sees the change. Uniform
names are case-sensitive and snapshotted when the tween is added. A missing
shader, an undeclared uniform, an incompatible type, or a non-finite endpoint fails
before the tween is registered or writes anything. If the material has no override
for the uniform, the tween captures the declared shader default rather than zero.

| C# value | Required uniform metadata |
| --- | --- |
| float, double | Float (shader float) |
| int | Int (shader int) |
| Vector2 / Vector3 / Vector4 | Matching vector type |
| Color | Color (typically vec4 with source_color hint) |

Color and Vector4 are deliberately distinct, so neither binds to a uniform of the
other type. A double uses Godot's floating Variant representation, but the shader
and GPU ultimately determine precision. During interpolation, integers round
halfway away from zero and saturate at Int32 bounds. Non-finite samples fault
playback before writing. Textures, resources, arrays, booleans, enums and
quaternion representations aren't supported.

On natural completion without `RetainFinalValue`, the original explicit override
is restored, or the new override is removed if none originally existed.
Cancelling keeps the latest sample, as with node tweens. You can reuse a source
definition across materials with different initial values and override states.

Replacing, disposing, or editing the bound shader faults playback the next time it
samples or restores, and `End` settles with the error. Any shader change signal
counts as a binding change, even if the new declaration happens to be compatible,
so start a new tween after changing the shader. Playback doesn't scan uniform
metadata or parse string paths per frame.

Default lookup requires a working renderer. Godot's dummy headless renderer can
expose declarations but return no default value, and then capture fails rather
than inventing one. Headless property tests can use an explicit material override,
but verifying defaults and visible behavior takes rendering tests.

## Per-instance shader uniforms

Declare an `instance uniform` in the shader when nodes sharing the same material
need independent values:

```csharp
// shader: instance uniform float pulse = 0.25;
mesh.TweenInstanceShaderParameter("pulse", 1f, 0.5);
sprite.TweenInstanceShaderParameter("pulse", 0f, 0.5);
```

These methods target `GeometryInstance3D` and `CanvasItem` respectively and follow
the normal node lifetime and pause rules. The definitions are
`Tweens.GeometryInstanceShaderParameter<T>` and
`Tweens.CanvasItemInstanceShaderParameter<T>`. Value types, validation, snapshots,
and restoration work the same as for material uniforms. After restoration, an
explicit override equal to the default stays explicit, and an originally absent
override is removed.

The tween captures the effective material bindings, including inherited CanvasItem
materials, mesh surfaces, overrides, overlays and next passes. Replacing the mesh,
a material, or a pass, or editing a bound shader, faults the next write. Binding
checks are conservative, so changing a tracked slot can fault playback even if
another slot still declares the same uniform. Godot controls instance-uniform
indexing, capacity, shader compatibility and multi-material conflicts. This API
doesn't assign or reconcile those declarations.

See [ShaderMaterial](https://docs.godotengine.org/en/stable/classes/class_shadermaterial.html), [CanvasItem](https://docs.godotengine.org/en/stable/classes/class_canvasitem.html), and [GeometryInstance3D](https://docs.godotengine.org/en/stable/classes/class_geometryinstance3d.html).
See [compatibility](/compatibility/) for rendering limits and
[playback and async](/csharp/playback/) for fault handling.
