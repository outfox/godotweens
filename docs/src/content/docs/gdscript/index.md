---
title: GDScript addon status
description: Planned Godot addon scope and availability; no installable GDScript release yet.
---

:::note[Planned implementation]
The GDScript addon isn't implemented yet, so there's nothing to install. The C#
documentation describes the existing library, not a GDScript API you can call.
:::

tweens.gd will also include a Godot addon for projects without .NET. It aims to
match what the C# library does: reusable definitions, independent playback,
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

The [definitions](/concepts/definitions/), [timing](/concepts/timing/), and
[lifetime](/concepts/lifetime/) pages describe the model the addon is meant to
follow. Exact parity will be documented as implementations are tested. Whether
the addon needs a native backend will be decided by measuring performance.

## Documentation to expect

Once an implementation is ready, this section will include installation, a
GDScript quickstart, signal/await completion behavior, API reference, and examples
alongside their C# equivalents. Until then, the [C# guides](/csharp/quickstart/)
show the behavior the addon is meant to match.
