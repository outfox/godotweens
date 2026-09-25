---
title: GDScript addon status
description: Planned Godot addon scope and availability; no installable GDScript release yet.
---

:::note[Planned implementation]
The GDScript addon is not implemented or available to install yet. The C#
documentation describes the existing library, not a GDScript API you can call.
:::

tweens.gd will also contain a Godot addon intended for projects without .NET.
The aim is equivalent functionality: reusable definitions, independent playback,
timing and easing, cancellation, node ownership, and property/material/shader
adapters.

## What is established

| Area | Current status |
| --- | --- |
| Product name | tweens.gd |
| Intended installation layout | `res://addons/tweens_gd/` |
| .NET requirement | Intended to work without .NET |
| Implementation | GDScript, possibly with GDExtension if performance requires it |
| Public classes and method names | Not finalized |
| Completion and `await` API | Not finalized; C# tasks are not the addon API |
| Minimum Godot version and export platforms | Not yet validated |
| Package/download | Not available |

The [shared concepts](/concepts/definitions/) describe the model the addon is
intended to follow. Exact parity will be documented as implementations are tested.
The choice of a native backend depends on measurements, not an assumption that
every workload requires it.

## Documentation to expect

Once an implementation is ready, this section will include installation, a
GDScript quickstart, signal/await completion behavior, API reference, and examples
alongside their C# equivalents. Until then, use the [C# gallery](/csharp/gallery/)
to explore the current behavior; running it requires the C# toolchain.
