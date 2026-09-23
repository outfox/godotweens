# Material and shader tweens

Material tweens write directly to the resource you supply. If several nodes share that resource, all of them see the change. Playback never clones, reassigns, or disposes your material, shader, textures, or mesh.

## Choose the playback lifetime

```csharp
// Shared resource, with playback scoped to this SceneTree.
var fade = material.TweenAlbedoAlpha(0, 0.5, GetTree());

// Same resource semantics, but stop when this node leaves the tree.
var roughness = material.TweenRoughness(0.2f, 1, mesh);

// Tree context with an optional owner and ordinary tween options.
var emission = material.TweenEmissionEnergyMultiplier(3, 1, GetTree(),
    d => d.Ease = EaseType.CubicOut, owner: mesh);

// Reusable definitions work with either context.
var definition = new MaterialRoughnessTween { To = 0.5f, Duration = 1 };
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
| Albedo color | MaterialAlbedoColorTween | TweenAlbedoColor |
| Albedo alpha | MaterialAlbedoAlphaTween | TweenAlbedoAlpha |
| Metallic | MaterialMetallicTween | TweenMetallic |
| Specular | MaterialMetallicSpecularTween | TweenMetallicSpecular |
| Roughness | MaterialRoughnessTween | TweenRoughness |
| Emission color | MaterialEmissionTween | TweenEmission |
| Emission multiplier | MaterialEmissionEnergyMultiplierTween | TweenEmissionEnergyMultiplier |
| Emission intensity (nits) | MaterialEmissionIntensityTween | TweenEmissionIntensity |
| Normal strength | MaterialNormalScaleTween | TweenNormalScale |
| UV1 offset/scale | MaterialUv1OffsetTween / MaterialUv1ScaleTween | TweenUv1Offset / TweenUv1Scale |
| UV2 offset/scale | MaterialUV2OffsetTween / MaterialUV2ScaleTween | TweenUV2Offset / TweenUV2Scale |

Each UV property also has X/Y/Z variants, such as `MaterialUv1OffsetXTween` / `TweenUv1OffsetX`. Component setters preserve other components at each write, including concurrent edits. Every convenience method accepts a SceneTree or an owner Node, plus an optional configuration callback.

Set rendering features explicitly: alpha fading needs a suitable transparency mode; emission needs `EmissionEnabled`; normal strength needs a normal map and `NormalEnabled`. `EmissionIntensity` requires `rendering/lights_and_shadows/use_physical_light_units`. UV animation only becomes visible with suitable textures/mapping. Tweens do not change these modes or flags. Native setters retain their usual limits and renderer-specific behavior. See [BaseMaterial3D](https://docs.godotengine.org/en/stable/classes/class_basematerial3d.html).

For independent ordinary-material values, duplicate once during scene setup, assign that duplicate, and use it for subsequent tweens:

```csharp
var unique = (StandardMaterial3D)shared.Duplicate();
mesh.MaterialOverride = unique;
unique.TweenAlbedoColor(Colors.Red, 1, mesh);
unique.TweenRoughness(0.2f, 1, mesh);
```

## Ordinary shader uniforms

```csharp
// shader: uniform float dissolve = 0.25;
shaderMaterial.TweenShaderParameter("dissolve", 1f, 0.5, GetTree());
shaderMaterial.TweenShaderParameter("dissolve", 1f, 0.5, mesh);

var definition = new ShaderParameterTween<float>("dissolve")
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

Replacing, disposing, or editing the bound shader faults playback when it next attempts a sample or restoration. Any shader change signal is treated as a binding change, even if the new declaration happens to be compatible. Completion settles with the error. Start a new tween after changing the shader. No uniform metadata scan or string path parsing occurs per frame.

Default lookup requires a working renderer. Godot's dummy headless renderer can expose declarations while returning no default value; capture then fails rather than inventing one. An explicit material override can be used in headless property tests, but rendering tests are required to verify defaults and visible behavior.

## Per-instance shader uniforms

Declare an `instance uniform` in the shader when nodes sharing the same material need independent values:

```csharp
// shader: instance uniform float pulse = 0.25;
mesh.TweenInstanceShaderParameter("pulse", 1f, 0.5);
sprite.TweenInstanceShaderParameter("pulse", 0f, 0.5);
```

These methods target `GeometryInstance3D` and `CanvasItem` respectively and follow normal node lifetime/pause rules. Definitions are `GeometryInstanceShaderParameterTween<T>` and `CanvasItemInstanceShaderParameterTween<T>`. The same value types, validation, snapshot and restoration rules apply. An explicit override equal to the default remains explicit after restoration; an originally absent override is removed.

Effective material bindings are captured, including inherited CanvasItem materials, mesh surfaces, overrides, overlays and next passes. Replacing the mesh/material/pass or editing a bound shader faults the next write. Binding checks are conservative: changing a tracked slot can fault playback even if another slot still declares the same uniform. Godot controls instance-uniform indexing, capacity, shader compatibility and multi-material conflicts; this API does not assign or reconcile those declarations.

See [ShaderMaterial](https://docs.godotengine.org/en/stable/classes/class_shadermaterial.html), [CanvasItem](https://docs.godotengine.org/en/stable/classes/class_canvasitem.html), and [GeometryInstance3D](https://docs.godotengine.org/en/stable/classes/class_geometryinstance3d.html).

## Validation

```powershell
# Deterministic core, node and material contracts; no display required.
dotnet test testbed/testbed.tests/testbed.tests.csproj -c Release

# Separate process with a real rendering fixture; requires a working graphics device/display.
dotnet test testbed/testbed.tests/testbed.tests.csproj -c Release -p:RenderingTests=true
```

The rendering suite checks all supported shader types, shader defaults, absent/explicit override restoration, binding changes, shared versus instance updates, and actual rendered pixels for alpha, emission, UV movement and 2D/3D instance uniforms. The default suite checks every material property and both context overloads on StandardMaterial3D and OrmMaterial3D, with a completeness guard, plus ownership, pause, disposal, callback and teardown behavior.

Add `--collect "XPlat Code Coverage"` to either command to collect coverage. Report core/runtime and adapter coverage separately; repetitive property adapters should not hide lifecycle gaps. The rendering build selects only rendering tests, avoiding multiple native engine fixtures in one process. Rebuild without `RenderingTests=true` to return to the headless suite; do not use `--no-build` when switching suites.

Validated on 2026-09-23 with 2dog 4.7.2.87: 474 headless and 31 rendering tests passed in Release, and the Release solution build succeeded. Combined handwritten-library line coverage is 98.4% (1695/1722); adapter and extension directories each have 100% line coverage. Core has 333/344 covered lines, automatic runtime 68/76, and shader binding/conversion helpers 103/109. This line union is not a claim of full branch coverage. Local results and Cobertura reports are under `artifacts/material-headless-release-final/` and `artifacts/material-rendering-release/`; `artifacts/material-coverage-summary.json` records the combined totals.
