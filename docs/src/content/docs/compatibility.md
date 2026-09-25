---
title: Compatibility and availability
description: Implementation status, engine requirements, and validated targets for tweens.gd.
---

| Implementation | Availability | Requirements |
| --- | --- | --- |
| C# | Implemented; project reference and local NuGet package supported | .NET 10, GodotSharp 4.7.2, matching engine |
| GDScript addon | Planned; no installable addon yet | Minimum Godot version and exports pending validation |

The C# package ID is `tweens.gd`, with namespace `tweens.gd`. The local
development version is `0.1.0`. These docs do not imply that version has been
published to nuget.org. Follow [installation](/csharp/installation/) to build from
source or use the locally packed artifact.

## C# engine and platform support

| Area | Current status |
| --- | --- |
| Target framework | `net10.0` |
| Library bindings | GodotSharp 4.7.2 |
| Included testbed | 2dog 4.7.2.91, using the versions in `testbed/Directory.Build.props` |
| Windows | CI builds/tests the C# solution and runs the headless testbed |
| Rendering | Gallery and dedicated shader/material tests use a real graphics context |
| Other operating systems | Not certified by the current CI |
| Other Godot versions | Not validated |
| Trimmed, AOT, mobile, and web exports | Not validated; package metadata is not an export support guarantee |

Your application supplies the engine. Use bindings and an engine that match; do
not assume a similarly numbered stock editor is interchangeable with the testbed
host. The library depends only on GodotSharp. Its source generator is a private
build dependency, and 2dog is used by the testbed rather than the library package.

## Rendering requirements

Changing a property does not enable a rendering feature. Configure transparency,
emission, normal maps, and particle modes before animating them. Shader default
lookup needs a working renderer; a dummy headless renderer may not supply it.
See [materials](/csharp/materials/) and [shader uniforms](/csharp/shaders/).

## GDScript availability

The addon is intended for Godot projects without .NET. Its implementation may
use GDScript alone or GDScript with GDExtension, depending on measured performance.
No native backend, supported platform list, or parity claim is established yet.
See [addon status](/gdscript/) for the intended scope.
