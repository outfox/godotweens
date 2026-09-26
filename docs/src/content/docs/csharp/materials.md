---
title: Material tweens
description: Animate shared material properties with explicit ownership and rendering prerequisites.
---

Material tweens write directly to the resource you supply. If several nodes share
that resource, all of them see the change. Playback never clones, reassigns, or
disposes your material, shader, textures, or mesh.

Import `Godot` and `tweens.gd`. The snippets run in a Node method on Godot's main
thread with configured materials and an in-tree `MeshInstance3D` named `mesh`.

## Choose the playback lifetime

```csharp
// In a Node method; material/shared are StandardMaterial3D, mesh is an in-tree MeshInstance3D.
// Shared resource, with playback scoped to this SceneTree.
var fade = material.TweenAlbedoAlpha(0, 0.5, GetTree());

// Same resource semantics, but stop when this node leaves the tree.
var roughness = material.TweenRoughness(0.2f, 1, mesh);

// Tree context with an optional owner and ordinary tween options.
var emission = material.TweenEmissionEnergyMultiplier(3, 1, GetTree(),
    d => d.Ease = EaseType.CubicOut, owner: mesh);

// Reusable definitions work with either context.
var definition = new Tweens.MaterialRoughness { To = 0.5f, Duration = 1 };
material.Tween(definition, GetTree());
material.Tween(definition, mesh);
mesh.Tween(material, definition);
```

A tree-scoped tween keeps running when a mesh is removed or gets a different
material, because it retains the resource you originally supplied. A tween with an
owner cancels with `OwnerExited` when the owner leaves the tree.
`owner.CancelTweens()` cancels only that owner's automatic tweens, material tweens
included, and optionally its descendants' tweens. Other owners and tree-scoped
tweens are unaffected.

Tweens on a disposed resource cancel with `TargetFreed`, even while paused.
Runner or tree teardown settles pending work with `RunnerDisposed`. Cleanup
releases playback bindings but never the resources you supplied, and handles
retain their `Target` for inspection.

With no owner, the `Bound` and `SceneTree` pause modes both follow tree pause.
With an owner, `Bound` follows `owner.CanProcess()` and `SceneTree` still follows
tree pause. `Always` ignores both, although pausing the handle always stops
advancement. All access to native resources has to happen on Godot's main thread.

For manual scheduling, call `scheduler.Add(material, definition)` or
`scheduler.Add(material, definition, owner)`. Without an owner, a manual scheduler
has no tree pause policy. Dispose the scheduler when you're finished with it.

## Built-in material properties

All 25 adapters target `BaseMaterial3D`, so they work with both
`StandardMaterial3D` and `OrmMaterial3D`.

| Property | Definition | Convenience method |
| --- | --- | --- |
| Albedo color | Tweens.MaterialAlbedoColor | TweenAlbedoColor |
| Albedo alpha | Tweens.MaterialAlbedoAlpha | TweenAlbedoAlpha |
| Metallic | Tweens.MaterialMetallic | TweenMetallic |
| Specular | Tweens.MaterialMetallicSpecular | TweenMetallicSpecular |
| Roughness | Tweens.MaterialRoughness | TweenRoughness |
| Emission color | Tweens.MaterialEmission | TweenEmission |
| Emission multiplier | Tweens.MaterialEmissionEnergyMultiplier | TweenEmissionEnergyMultiplier |
| Emission intensity (nits) | Tweens.MaterialEmissionIntensity | TweenEmissionIntensity |
| Normal strength | Tweens.MaterialNormalScale | TweenNormalScale |
| UV1 offset/scale | Tweens.MaterialUv1Offset / Tweens.MaterialUv1Scale | TweenUv1Offset / TweenUv1Scale |
| UV2 offset/scale | Tweens.MaterialUV2Offset / Tweens.MaterialUV2Scale | TweenUV2Offset / TweenUV2Scale |

Each UV property also has X/Y/Z variants, such as `Tweens.MaterialUv1OffsetX` /
`TweenUv1OffsetX`. A component setter leaves the other components as they are at
each write, including concurrent edits to them. Every convenience method accepts a
SceneTree or an owner Node, plus an optional configuration callback.

Tweens don't change rendering modes or flags, so enable the features you animate
yourself:

- Alpha fading needs a suitable transparency mode.
- Emission needs `EmissionEnabled`.
- Normal strength needs a normal map and `NormalEnabled`.
- `EmissionIntensity` requires `rendering/lights_and_shadows/use_physical_light_units`.
- UV animation only becomes visible with suitable textures and mapping.

Native setters keep their usual limits and renderer-specific behavior. See
[BaseMaterial3D](https://docs.godotengine.org/en/stable/classes/class_basematerial3d.html).

To give one node its own values on an ordinary material, duplicate the material
once during scene setup, assign the duplicate, and use that for later tweens:

```csharp
var unique = (StandardMaterial3D)shared.Duplicate();
mesh.MaterialOverride = unique;
unique.TweenAlbedoColor(Colors.Red, 1, mesh);
unique.TweenRoughness(0.2f, 1, mesh);
```


Continue with [shader uniforms](/csharp/shaders/) for shared and per-instance parameters.
