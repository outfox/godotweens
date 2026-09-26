---
title: Material tweens
description: Animate shared material properties with explicit ownership and rendering prerequisites.
---

Material tweens write directly to the resource you supply. If several nodes share that resource, all of them see the change. Playback never clones, reassigns, or disposes your material, shader, textures, or mesh.

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

A tree-scoped tween keeps running when a mesh is removed or changes its material assignment. It retains the originally supplied resource. With an owner, tree exit cancels with `OwnerExited`; `owner.CancelTweens()` cancels only that owner's automatic tweens (optionally its descendants), including material tweens. Other owners and tree-scoped tweens are unaffected.

Disposed resource targets cancel with `TargetFreed`, even while paused. Runner/tree teardown settles pending work with `RunnerDisposed`. Cleanup releases playback bindings, never the supplied resources. Handles retain their `Target` for inspection.

With no owner, `Bound` and `SceneTree` pause modes follow tree pause. With an owner, `Bound` follows `owner.CanProcess()`; `SceneTree` follows tree pause. `Always` ignores those policies, while pausing the handle always stops advancement. All native-resource access requires Godot's main thread.

Manual scheduling uses `scheduler.Add(material, definition)` or `scheduler.Add(material, definition, owner)`. Without an owner, a manual scheduler has no tree pause policy. Dispose it when finished.

## Built-in material properties

All 25 adapters target `BaseMaterial3D`, supporting both `StandardMaterial3D` and `OrmMaterial3D`.

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

Each UV property also has X/Y/Z variants, such as `Tweens.MaterialUv1OffsetX` / `TweenUv1OffsetX`. Component setters preserve other components at each write, including concurrent edits. Every convenience method accepts a SceneTree or an owner Node, plus an optional configuration callback.

Set rendering features explicitly: alpha fading needs a suitable transparency mode; emission needs `EmissionEnabled`; normal strength needs a normal map and `NormalEnabled`. `EmissionIntensity` requires `rendering/lights_and_shadows/use_physical_light_units`. UV animation only becomes visible with suitable textures/mapping. Tweens do not change these modes or flags. Native setters retain their usual limits and renderer-specific behavior. See [BaseMaterial3D](https://docs.godotengine.org/en/stable/classes/class_basematerial3d.html).

For independent ordinary-material values, duplicate once during scene setup, assign that duplicate, and use it for subsequent tweens:

```csharp
var unique = (StandardMaterial3D)shared.Duplicate();
mesh.MaterialOverride = unique;
unique.TweenAlbedoColor(Colors.Red, 1, mesh);
unique.TweenRoughness(0.2f, 1, mesh);
```


Continue with [shader uniforms](/csharp/shaders/) for shared and per-instance parameters.
