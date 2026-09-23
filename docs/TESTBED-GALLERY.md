# Testbed gallery

Run the interactive testbed:

~~~powershell
dotnet run --project testbed/testbed.2dog
~~~

The gallery contains six pages and 24 examples. Each page is a separate C# Control subtree; only the current page is instantiated. Small SubViewports isolate the cameras, lights and 3D worlds in individual cards.

| Page | Examples and combinations |
| --- | --- |
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

Page indices are zero-based (0–5):

~~~powershell
# Open directly on the material page.
dotnet run --project testbed/testbed.2dog -- --gallery-page 4

# Capture every page and quit, using a deterministic frame step.
dotnet run --project testbed/testbed.2dog -- --gallery-snapshots artifacts/gallery --rendering-method gl_compatibility --fixed-fps 60

# The existing single-page screenshot command also accepts a page selection.
dotnet run --project testbed/testbed.2dog -- --gallery-page 5 --snapshot artifacts/shaders.png --rendering-method gl_compatibility
~~~

The gallery capture writes 01.png through 06.png after 60 frames per page. The compatibility renderer walkthrough has been visually checked on all six pages.

## Extend the gallery

The shell lives in [TweenDemo.cs](../testbed/TweenDemo.cs). Shared page helpers and lifecycle handling live in [GalleryPage.cs](../testbed/Gallery/GalleryPage.cs); each page has its own file in [testbed/Gallery](../testbed/Gallery).

Derive a page from GalleryPage, create its visuals in Build, and start its tweens in Animate. Use Keep for each playback handle, Own for resources created by the page, and Cycle for the shared repeating timing options. Material tweens bind to the page node so they stop with it. Do not dispose resources borrowed from another owner. Add the page to the shell's page names/factory and the navigation/rendering tests.

## Tests

~~~powershell
dotnet test testbed/testbed.tests/testbed.tests.csproj -c Release
dotnet test testbed/testbed.tests/testbed.tests.csproj -c Release -p:RenderingTests=true
~~~

Headless tests exercise repeated navigation, rapid deferred page selection, pause/cancel/restart, and teardown during an awaited sequence. Rendering tests exercise every page with live native objects, including shader playback and particle pause/resume. Run these as separate builds/processes; do not use --no-build when switching suites.
