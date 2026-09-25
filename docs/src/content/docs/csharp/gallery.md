---
title: C# gallery
description: Run eight pages and 32 tween examples, from squash and stretch to shader uniforms.
---

Run the interactive testbed:

~~~powershell
dotnet run --project testbed/testbed.2dog
~~~

The gallery contains eight pages and 32 examples. Each page is a separate C# Control subtree; only the current page is instantiated. Small SubViewports isolate the cameras, lights and 3D worlds in individual cards.

| Page | Examples and combinations |
| --- | --- |
| Squash & stretch | Bouncing ball with stretch, squash, shadow and dust; hopping slime with anticipation and flips; clickable jelly button with confetti and a rolling score; delayed squash wave |
| Choreography | Staggered card deal with flips; easing race with delayed echoes; OnUpdate-driven spirograph; 3D jelly cube with shockwave and camera shake |
| Motion & paths | PathFollow2D progress + lateral offset + marker scale; Camera2D zoom + offset; skew + rotation + independent scale axes; awaited outward movement followed by concurrent return and rotation |
| Interface | Character reveal + text tint; progress value + color/opacity; Control offset-transform position + rotation + scale within a container; integer scrolling |
| Drawing & particles | Line2D width + color; Polygon2D color + offset + rotation; CPU particle spread + gravity + color + emitter movement; PointLight2D texture scale + energy + position |
| 3D stage | Global quaternion + scale below a transformed parent; camera FOV + offset; PathFollow3D progress + offset; spotlight angle + color + energy |
| Materials | Albedo color + roughness shared by two meshes; UV offset + scale; transparent albedo alpha; emission color + energy |
| Shaders | One uniform shared by two panels; independent CanvasItem instance uniforms on one material; typed Color + Vector2 uniforms; GeometryInstance3D displacement with an unchanged reference object |

## Controls

- Pick a page in the sidebar. Leaving a page cancels its playback and releases its scene and owned resources.
- Easing, leg duration and ping-pong apply to repeating examples. Changing a setting rebuilds the current page with fresh initial values. Some examples run at a multiple of the selected duration for a slower visual rhythm.
- Pause/resume holds all active tweens. The particle example also holds its simulation. Shaders use tweened values rather than a separate shader clock.
- Cancel holds the current samples. Restart page reconstructs the examples and starts again.
- The delivery sequence is intentionally a one-shot example of async composition; restart replays it. Cancellation and navigation settle its pending waits without starting the next step.

The material page updates explicitly shared resources. The shader page contrasts shared material uniforms with node-owned instance uniforms. Materials are configured for transparency/emission before playback; the tween library does not enable those features itself.

The default window is 1280 × 900, with canvas scaling and a scrollable page area. Shaders require a real graphics context; headless runs show an explanatory card on that page. No downloaded assets are needed: geometry, checker patterns, light textures and shaders are created by the examples.

## Navigate and capture from the desktop host

Page indices are zero-based (0–7):

~~~powershell
# Open directly on the material page.
dotnet run --project testbed/testbed.2dog -- --gallery-page 6

# Capture every page and quit, using a deterministic frame step.
dotnet run --project testbed/testbed.2dog -- --gallery-snapshots artifacts/gallery --rendering-method gl_compatibility --fixed-fps 60

# The existing single-page screenshot command also accepts a page selection.
dotnet run --project testbed/testbed.2dog -- --gallery-page 7 --snapshot artifacts/shaders.png --rendering-method gl_compatibility
~~~

The gallery capture writes 01.png through 08.png after 60 frames per page.


## Explore the source

The gallery lives in `testbed/Gallery/`, with one folder per page and one C# file
per example. Start with the [quickstart](/csharp/quickstart/) to build your own
scene, or browse the [node catalog](/csharp/nodes/) for the adapters used here.
