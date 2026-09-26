# tweens.gd

Tween libraries for Godot, inspired by Jeffrey Lanters' **unity-tweens**. A typed C# NuGet package and a planned GDScript addon with equivalent functionality.

| Project | Location | Status |
| --- | --- | --- |
| C# NuGet package `tweens.gd` | `csharp/` | Implemented; examples below |
| GDScript Godot addon | `addons/tweens_gd/` (planned) | Backend and performance evaluation pending |
| Public documentation | [docs/](docs/README.md) | C# guides/reference and GDScript status; deployment pending |

Internal working documents live in the gitignored `docs-internal/` directory. The following documentation describes the C# implementation.

Reuse definitions, control independent playback handles, and compose animations with `async`/`await`.

Targets **.NET 10 / GodotSharp 4.7.2**. The included testbed pins its 2dog versions in `testbed/Directory.Build.props`. The library has no dependency on 2dog, native engine packages, or editor assemblies; your application supplies the engine. Use matching GodotSharp/engine versions. Other Godot versions and trimmed/AOT/web exports have not been validated.

## Run the testbed

```powershell
dotnet build tweens.gd.slnx
dotnet test testbed/testbed.tests/testbed.tests.csproj
dotnet run --project testbed/testbed.2dog
```

The testbed is an eight-page gallery with 32 examples: squash and stretch, choreography, motion and paths, interface, drawing and particles, a 3D stage, materials, and shaders. Change the duration, restart the current page, or read the animation tasks beside each example. Examples await tweens and groups directly; leaving a page ends its tweens through their node lifetimes. From `testbed/`, `dotnet test` and `dotnet run --project testbed.2dog` also work.

## Run the library tests

```powershell
dotnet test tests/tweens.gd.tests/tweens.gd.tests.csproj
./tests/Measure-Coverage.ps1
```

`tests/` is a minimal Godot project whose 2dog test suite covers the library itself: timelines, easing, callbacks, faults, groups, the generated `Tweens.*` definitions, every adapter and convenience overload, lifetimes, the automatic runner, and shaders. Godot runs one engine per test process, so engine-shutdown tests (`-p:TestSuite=Lifecycle`) and shader-default tests that need a display (`-p:TestSuite=Rendering`) build and run separately. `Measure-Coverage.ps1` runs the headless suites (add `-Rendering` for the third), and reports line and branch coverage of the library.

## Use the library

Add a project reference to `csharp/tweens.gd.csproj` (as the testbed does), or consume a locally built package:

```powershell
dotnet pack csharp/tweens.gd.csproj -c Release -o artifacts/packages
```

Create a tween from `_Ready` or later on a node inside the tree:

```csharp
using Godot;
using tweens.gd;

var movement = sprite.Tween(new Tweens.Position2D
{
    To = new Vector2(400, 180),
    Duration = 0.6,
    Ease = EaseType.CubicOut,
});

var reason = await movement.End;
if (reason == Reason.Completed)
    GD.Print("Arrived");
```

The first tween installs one internal runner under the SceneTree root through deferred attachment. No autoload, scene script file, or special host-loop code is required. Definitions are readonly record structs; handles are managed objects, and property adapters use typed getters/setters rather than reflection or string property paths.

### Reuse and control

```csharp
var fade = new Tweens.ModulateAlpha { To = 0, Duration = 0.3 };
var first = sprite.Tween(fade);
var second = label.Tween(fade with { Delay = 0.15 });
first.Pause();
first.Resume();
second.Cancel();
sprite.CancelTweens(includeChildren: true);
```

Built-in definitions are readonly record structs in the root `Tweens` namespace. A `with` expression copies the configuration and changes only the listed properties; the original remains available for reuse. Each start captures its own initial property value and owns its playback state. Omitted `From`/`To` use that captured value. Delegates and the objects they capture remain shared references. Godot Curves are duplicated for each playback.

Multiple tweens on the same property are allowed: the last application in insertion order wins. Axis and alpha adapters read the other components at application time so independent component tweens compose correctly.

Reusable fields need no factory or configurator:

```csharp
private readonly Tweens.PathFollow2DVOffset offset = new() { To = 20 };

// Inside an animation method:
var movement = follower.Tween(offset with { Duration = seconds, Delay = delay });
```

`TweenOptions` is immutable too. Share timing with `Options = timing` and put
individual overrides afterward. Convenience callbacks use mutable
`TweenOptionsBuilder` instances. The older `*Tween` class definitions remain
supported but are hidden from IntelliSense; use `Tweens.*` for new definitions.

### Async completion

`End` is a lazily allocated, shared `Task<Reason>`. Multiple callers can await it, including after the tween ends. Reasons are `Completed`, `Cancelled`, `TargetFreed`, `OwnerExited`, and `RunnerDisposed`. A direct `Free()` may be observed as `OwnerExited` because Godot emits tree-exit before invalidating the native instance; `QueueFree()` is identified as `TargetFreed`.

```csharp
await sprite.Tween(new Tweens.Position2D { To = destination, Duration = 0.5 }).End;
await sprite.Tween(new Tweens.ModulateAlpha { To = 0, Duration = 0.2 }).End;

await sprite.Tween(new Tweens.Scale2D { To = Vector2.One, Duration = 0.2 }, new Tweens.ModulateAlpha { To = 1, Duration = 0.2 }).End;
await Group.Of(first, second).End;
await first.AwaitDecommissionAsync(cancellationToken);
```

The wait token cancels **only that wait**. Call `Cancel()` to cancel playback. Awaited steps continue each other's timelines: tweens started where an await of `End` resumes inherit the time the finished tween overshot its end, so sequences do not drift. A `Group` plays several tweens as one step; if one stops early, it cancels the others. Check completion reasons before starting a follow-up animation when cancellation should stop a sequence; the [sequences guide](docs/src/content/docs/csharp/sequences.md) covers chaining, staggering, waits, pausing, and step timing. Errors in interpolation, easing, setters, or callbacks fault the completion task and are reported through `TweenScheduler.UnhandledException`; the automatic runner reports them with `GD.PushError`. Other tweens continue. Terminal callbacks and `OnFinally` run at most once, with cleanup and task settlement even when callbacks fail. Multiple failures are retained in an `AggregateException`.

Create/control tweens on Godot's main thread. `End` is settled from that thread; normal Godot async callers retain their synchronization context. Do not use `.Wait()`, `.Result`, `Task.Run`, or `ConfigureAwait(false)` around engine access. No coroutine API is provided. Use Godot's `ToSignal` for unrelated engine-signal waits.

### Timing and lifetime

| Option | Behavior |
| --- | --- |
| `Duration` | Seconds per leg, stored as double. Zero duration completes on the first eligible update. |
| `Delay` | Initial wait before starting; only unused delta advances playback. |
| `Repeats` | Cycles after the first, default 0. `TweenOptions.Infinite` (-1) repeats until cancelled; a fully zero-time infinite cycle is rejected. |
| `UsePingPong` | Forward and backward legs form one cycle. |
| `PingPongInterval` | Wait at the far endpoint before returning. |
| `RepeatInterval` | Wait between cycles, never after the final cycle. |
| `Offset` | Starting seconds into the first forward leg, within `[0, Duration]`. Delay still comes first. |
| `Ease` | All 31 upstream ease functions. Back/elastic overshoot remains unclamped. |
| `EaseFunction` / `Curve` | One custom source may override `Ease`; specifying both is rejected. Curve samples use normalized time 0–1. |
| `ProcessMode` | `Process` by default; `Physics` opts into physics updates. |
| `PauseMode` | `Bound` follows owner `CanProcess()`, `SceneTree` follows tree pause only, `Always` ignores both. Instance pause always wins. |
| `UseUnscaledTime` | Process mode uses monotonic engine ticks; physics uses `1 / PhysicsTicksPerSecond` per tick. This does not change pause policy. |

Negative/non-finite timing is rejected. Large deltas skip directly to the correct phase, including across multiple cycles; time is not discarded at boundaries. `OnUpdate` samples once per eligible tick, plus delay fill/restoration when applicable: skipped cycles do not synthesize callbacks for every intermediate boundary. Long frames catch up in full; no hidden time clamp is imposed. Unscaled physics uses fixed simulation ticks, not wall-clock duration during catch-up.

`FillMode` flags are `ApplyFromDuringDelay`, `RetainFinalValue`, `Both`, and `None`. Default is `RetainFinalValue`. Without retention, natural completion restores the captured initial property value. Cancellation keeps the most recently applied value. A ping-pong tween's final value is its starting endpoint.

Callback order: `OnAdd`, optional delay-fill `OnUpdate`, `OnStart` once, updates, then `OnEnd` and `OnFinally`. Cancellation substitutes `OnCancel` for `OnEnd`; faults run `OnFinally`. Terminal state is visible before terminal callbacks. New tweens created by update callbacks begin on the next eligible update. `Progress` is the current leg's normalized position, reversing during ping-pong; it is not overall progress through all cycles.

The runner uses process/physics priority **1000**, after default-priority node updates. It is not an exact Unity LateUpdate equivalent. A node's `SetProcess(false)` does not disable bound tweens; `ProcessMode` and tree pause determine `CanProcess()`.

Owner tree exit cancels playback immediately, including removal/reparenting. Freed/queued-for-deletion targets are never written. Destroying a paused owner still settles completion. Runner/tree teardown cancels and releases remaining work. `SuppressCallbacksWhenTargetInvalid` can suppress callbacks while still settling completion. Handles retain their `Target` for inspection; release handles you no longer need.

### Built-in adapters

| Family | Definitions |
| --- | --- |
| Callback values | `Tweens.Float`, `Tweens.Double`, `Tweens.Vector2`, `Tweens.Vector3`, `Tweens.Vector4`, `Tweens.Color`, `Tweens.Quaternion`, `Tweens.Rect2` |
| Node2D | `Tweens.Position2D`, `Tweens.GlobalPosition2D`, `Tweens.Scale2D`, X/Y variants; `Tweens.Rotation2D`, `Tweens.GlobalRotation2D` |
| Node3D | `Tweens.Position3D`, `Tweens.GlobalPosition3D`, `Tweens.Scale3D`, `Tweens.Rotation3D`, `Tweens.GlobalRotation3D`, X/Y/Z variants; `Tweens.Quaternion3D` |
| Control | `Tweens.ControlPosition`, `Tweens.ControlGlobalPosition`, `Tweens.ControlSize`, `Tweens.ControlScale`, X/Y variants; `Tweens.ControlRotation`, `Tweens.ControlAnchorMin`, `Tweens.ControlAnchorMax`, `Tweens.ControlOffsets` |
| Color/opacity | `Tweens.Modulate`, `Tweens.SelfModulate`, `Tweens.ModulateAlpha`, `Tweens.SelfModulateAlpha` on CanvasItem |
| Range | `Tweens.RangeValue` on ProgressBar, TextureProgressBar, sliders, and other Range nodes |
| Audio | `Tweens.AudioVolumeDb`, `Tweens.AudioVolumeLinear`, `Tweens.AudioPitchScale`, plus `2D`/`3D` suffix variants |
| Lights | `Tweens.LightColor2D`, `Tweens.LightEnergy2D`, `Tweens.LightColor3D`, `Tweens.LightEnergy3D`, `Tweens.OmniRange`, `Tweens.SpotRange`, `Tweens.SpotAngle` |

Axis suffixes follow the dimension, e.g. `Tweens.Position3DX`. Scalar and Euler rotations use **radians**; quaternion interpolation normalizes endpoints and uses shortest-path spherical interpolation. Spot angle uses Godot's **degrees**. Linear audio volume is an amplitude multiplier, `VolumeDb` is decibels, pitch is a ratio. Engine setters may clamp constrained properties (e.g. Range, audio pitch, and anchors).

Control anchors are fractions; offsets and positions are pixels. Anchor adapters use Godot's push-opposite behavior when moving an edge past its opposite edge. `Tweens.ControlOffsets` uses Vector4 `(left, top, right, bottom)`. Containers can overwrite child position/size; animate a free-layout child when necessary. Range values use the node's configured min/max/step rather than assuming a 0–1 fraction. Light and audio nodes require suitable scene resources to render light or play sound; tweening their property does not create those resources.

### Property convenience methods

Every built-in definition also has a typed extension method:

```csharp
sprite.TweenPosition(destination, 0.5, options => options.Ease = EaseType.CubicOut);
camera.TweenZoom(new Vector2(2, 2), 0.4);
label.TweenVisibleRatio(1, 1.5, options => options.From = 0);
audio.TweenVolumeDb(-20, 1);
```

Each method also accepts a `TweenOptions` value instead of a callback. Its timing options are copied at the call,
and the explicit duration argument takes precedence over `options.Duration`:

```csharp
var snappy = new TweenOptions { Ease = EaseType.BackOut, Delay = 0.1 };
sprite.TweenPosition(destination, 0.5, snappy);
sprite.TweenModulateAlpha(1, 0.3, snappy);
```

The catalog now also covers cameras, paths, 3D appearance, Control pivots and offset transforms, drawing, canvas/parallax, spatial audio, additional lights, animation, particles, decals, fog volumes, spring arms and integer frame/scroll/text properties. See the [complete node/value catalog](docs/src/content/docs/csharp/nodes.md).

### Materials and shader uniforms

Material tweens update the supplied resource directly, including shared resources. Choose a SceneTree context, or bind playback to an owner node:

```csharp
material.TweenAlbedoAlpha(0, 0.5, GetTree());
material.TweenRoughness(0.2f, 1, mesh);
shaderMaterial.TweenShaderParameter("dissolve", 1f, 0.5, GetTree());
mesh.TweenInstanceShaderParameter("pulse", 1f, 0.5);
```

Includes 25 BaseMaterial3D adapters and float/double, int, vector and color shader uniforms. Defaults and explicit override state are preserved during non-retaining completion.

### Custom properties and values

```csharp
var healthTween = new Tweens.Property<MyEnemy, float>(
    enemy => enemy.Health,
    (enemy, value) => enemy.Health = value,
    Interpolators.Float)
{
    To = 0,
    Duration = 0.5,
};
enemy.Tween(healthTween); // MyEnemy derives from Node.

owner.Tween(new Tweens.Float
{
    From = 10,
    To = 100,
    Duration = 1,
    OnUpdate = (_, value) => customObject.Amount = value,
});
```

Value tweens are owned by a Node and deliver results through `OnUpdate`. Their omitted endpoints default to zero/transparent black/identity as appropriate. You may also derive from `TweenDefinition<TTarget, TValue>` and override protected `Read`, `Write`, and `Interpolate` methods. Definition snapshots are shallow; reference-valued configuration remains shared. Advanced definitions can override `Prepare`, `Restore`, and `Release` to manage per-playback bindings on the snapshot; cleanup also runs after failed preparation and must release only snapshot-owned resources.

For deterministic tests or non-node managed targets, use `TweenScheduler.Add(target, definition)` and `Update(delta, unscaledDelta, mode)`. The scheduler must be driven/disposed on its creating thread. Adding actual Nodes still requires Godot's main thread and an in-tree owner. Dispose manual schedulers to release their work. Automatic `CancelTweens` operates on the per-tree runner, not separately created manual schedulers.

## Differences from unity-tweens

The reusable definition/instance design and easing math are retained. Properties use PascalCase and Godot types. `Repeats` counts cycles after the first, with `TweenOptions.Infinite` replacing a separate flag. Fill flags describe behavior instead of retaining upstream's reversed Forwards/Backwards terminology. Native `Nullable<T>` replaces the custom nullable wrapper. Timing carries remaining delta across phase boundaries, zero duration is explicit, and terminal callbacks are idempotent.

Unity coroutine APIs, the editor inspector, component lookup, and Unity-specific audio spatial-blend/priority/reverb/pan controls are not ported. Sequence DSLs, automatic overwrite arbitration, and pooling are deferred. Global quaternion rotation is available through `Tweens.GlobalQuaternion3D` / `TweenGlobalQuaternion`; it follows Godot global-rotation scale/shear semantics. Use async composition and custom property definitions where appropriate.

## Validation and known limits

GitHub Actions builds and tests the solution, gates library coverage, verifies the
`tweens.gd` NuGet package, and uploads package artifacts. Version tags create GitHub
releases with package and symbol downloads. Maintainer release instructions and NuGet
trusted-publishing setup are kept locally in `docs-internal/RELEASING.md`.

Tests cover deterministic playback, easing and overshoot, callback mutation/faults, snapshots, async completion, main-thread continuation, node lifetime/pause, adapter families, the demo, and scheduler steady-state allocations. Material tests cover shared-resource ownership, disposal and all property/context overloads. Run `dotnet test testbed/testbed.tests/testbed.tests.csproj -c Release -p:RenderingTests=true` separately for shader contracts and rendered pixel checks (requires graphics/display). The desktop testbed has also been rendered with the OpenGL compatibility renderer. The library is packaged independently of its testbed and the gitignored Unity reference.

The testbed's trimming checks report IL2125 for unannotated GodotSharp bindings and this library; trimmed/AOT export support is not promised. An isolated two-engine 2dog restart smoke run succeeds but the second engine emits an upstream `gui/common/default_scroll_deadzone` setting warning when creating OptionButton. This is separate from tween playback and is not suppressed here.

Developer-only capture/restart checks:

```powershell
dotnet run --project testbed/testbed.2dog -- --snapshot artifacts/demo.png --rendering-method gl_compatibility
dotnet run --project testbed/testbed.2dog -- --headless --quit-after 12 --restart-check
```

The math and API inspiration are attributed in [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md), including the original MIT notice.
