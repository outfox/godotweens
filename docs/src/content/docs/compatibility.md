---
title: Compatibility
description: Implementation status, engine requirements, and validated targets for tweens.gd.
---

The C# library works with .NET 10 and Godot 4.7.2 .NET today. The GDScript addon
has an experimental pure GDScript core.

| Implementation | Availability | Requirements |
| --- | --- | --- |
| C# | Implemented; project reference and local NuGet package supported | .NET 10, GodotSharp 4.7.2, matching engine |
| GDScript addon | Experimental core available from source | Tested on 2dog/Godot 4.7.2; no .NET/native extension dependency; exports pending validation |

The C# package ID and namespace are both `tweens.gd`. The local development
version is `0.1.0`, which hasn't been published to nuget.org.
[Install](/csharp/installation/) it from source or from a locally packed package.

## C# engine and platform support

| Area | Current status |
| --- | --- |
| Target framework | `net10.0` |
| Library bindings | GodotSharp 4.7.2 |
| Testbed project | Godot 4.7.2 with .NET support |
| Windows | CI builds and tests the C# solution and the testbed |
| Rendering | The testbed and dedicated shader/material tests use a real graphics context |
| Other operating systems | Not certified by the current CI |
| Other Godot versions | Not validated |
| Trimmed, AOT, mobile, and web exports | Not validated; package metadata is not an export support guarantee |

Your application supplies the engine. Use a Godot .NET engine that matches the
GodotSharp bindings. The library depends only on GodotSharp; its source generator
is a private build dependency.

## Rendering requirements

Changing a property doesn't enable a rendering feature. Configure transparency,
emission, normal maps, and particle modes before animating them. Shader default
lookup needs a working renderer, which a dummy headless renderer may not supply.
See [materials](/csharp/materials/) and [shader uniforms](/csharp/shaders/).

## GDScript availability

The addon is implemented in pure GDScript for projects without .NET. Windows
headless conformance tests run real GDScript through the pinned 2dog engine.
Standard Godot exports, older versions, mobile and Web/WASM remain unvalidated.
The initial API covers core playback and generic properties; full adapter and
group parity is still in progress. See the [GDScript guide](/gdscript/).
