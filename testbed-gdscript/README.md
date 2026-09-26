# GDScript conformance project

The project runs actual `.gd` code, with no reference to the C# tween library.
The addon itself has no .NET dependency; 2dog is only the development test host.

From the repository root:

```powershell
dotnet run --project testbed-gdscript/host -c Release
dotnet run --project testbed-gdscript/host -c Debug
dotnet test tests/tweens.gd.tests/tweens.gd.tests.csproj -c Release
```

The host build copies the current addon and shared fixtures into ignored project
directories. The launcher checks every addon script compiles, runs the tests, and
exits nonzero for assertions, captured Godot script errors, or a 120-frame timeout.
Shared timing, easing and group completion/overshoot fixtures also run through the
C# implementation in the library test suite. Group tests additionally cover shared
controls, overlapping groups, already-settled/rejected members, cancellation during
callbacks, owner/target lifetime, mixed clocks, error aggregation and reference cleanup.

To use an installed standard Godot executable instead:

```powershell
./testbed-gdscript/Stage.ps1
godot --headless --path testbed-gdscript -- --run-tests
```

This path is provided for follow-up validation; the initial local validation used
the pinned 2dog engine. The project can also be opened in the Godot editor; use
`--run-tests` in the run arguments. No editor import/cache is needed by the 2dog
test path because scripts explicitly preload their dependencies.

## Baseline benchmark

```powershell
dotnet run --project testbed-gdscript/host -c Release -- --benchmark --benchmark-output artifacts/gdscript-benchmark.json
```

Measures value callbacks, Node2D position/color and material roughness at 100,
1,000 and 10,000 concurrent tweens. Both implementations receive identical manual
linear deltas: 30 warmup updates, then 120 samples. Godot uses one parallel native
Tween, stepped with `custom_step()`; the addon uses one manual scheduler. Target
creation is excluded from creation time. The JSON records engine/CPU/build details,
creation/disposal time, median and p95 update times in microseconds.

This isolates simple steady-state updates. It does not compare equivalent lifetime
guarantees, C# performance, allocation counts, repeated churn, full-frame rendering,
shader workloads or WASM. Do not infer a supported tween count from this baseline.
