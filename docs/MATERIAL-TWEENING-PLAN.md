# Material tweening implementation

The initial material plan is implemented. See [Material and shader tweens](MATERIAL-TWEENS.md) for the API, constraints and validation commands.

The agreed resource semantics are direct mutation: a shared material remains shared, and every user sees its updates. The material is the target; SceneTree or optional owner Node determines playback lifetime. No implicit duplication or assignment helpers are included.

Delivered:

- Resource-aware lifetime checks, explicit owner binding and tree-scoped automatic playback using the existing scheduler.
- 25 BaseMaterial3D property adapters, with both tree- and owner-based extension overloads.
- Typed ShaderMaterial uniforms with metadata validation, declared defaults and override restoration.
- CanvasItem and GeometryInstance3D instance uniforms with independent values and binding-change detection.
- Per-playback preparation/restoration/cleanup hooks, deterministic lifecycle/property contracts and separate renderer/pixel tests.

Potential follow-ups remain optional: more BaseMaterial3D properties, opt-in material assignment helpers, additional shader representations and performance tuning informed by profiling. Automatic cloning, general string-property paths, overwrite arbitration and pooling remain outside this implementation.
