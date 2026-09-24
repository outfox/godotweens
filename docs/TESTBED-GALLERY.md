# Testbed gallery

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

## Extend the gallery

The shell lives in [TweenDemo.cs](../testbed/TweenDemo.cs). Each page has a folder in [testbed/Gallery](../testbed/Gallery) holding a small page class and one file per example card.

An example derives from [GalleryEffect](../testbed/Gallery/GalleryEffect.cs): give it a Title and Caption, create its visuals in Build, and start its tweens in Animate. Build draws into Stage directly, or into a 2D View() or 3D World() created over it; the builders in [GalleryEffect.Stage.cs](../testbed/Gallery/GalleryEffect.Stage.cs) cover common shapes, meshes and textures. Use Keep for each playback handle, Own for resources the effect creates, and Cycle (or CycleAfter for delayed echoes) for the shared repeating timing. Async choreography assigns Sequence, usually via Repeat, and checks Finished after each await so Stop ends it. Material tweens bind to Stage so they stop with the page. Do not dispose resources borrowed from another owner. Colors come from [Palette](../testbed/Gallery/Palette.cs).

List the effect in its page's CreateEffects; cards are numbered in that order. For a new page, derive from GalleryPage, add it to the shell's page table and to the navigation/rendering tests.

## Tests

~~~powershell
dotnet test testbed/testbed.tests/testbed.tests.csproj -c Release
dotnet test testbed/testbed.tests/testbed.tests.csproj -c Release -p:RenderingTests=true
~~~

Headless tests exercise repeated navigation, rapid deferred page selection, pause/cancel/restart, and teardown during an awaited sequence. Rendering tests exercise every page with live native objects, including shader playback and particle pause/resume. Run these as separate builds/processes; do not use --no-build when switching suites.
