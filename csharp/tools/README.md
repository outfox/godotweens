# Structured definition generation

The property adapters in `src/Tweens/` also serve as the mutable builders for
convenience methods. `generate-structured.mjs` reads that catalog and the timing
properties in `TweenOptionsBuilder` to produce the readonly record structs in
`src/Structured/`. Shader and custom-property definitions use explicit templates
in the same generator.

After adding an adapter or changing configuration properties, run from the
repository root:

```sh
node csharp/tools/generate-structured.mjs
node csharp/tools/generate-structured.mjs --check
```

Check in the generated `.g.cs` files. Ordinary builds and package consumers do
not need Node.js. CI checks that generated definitions are current, and tests
check that every built-in adapter has an immutable typed definition.

Runtime binding state stays in the private per-playback class instance. A
structured definition creates that instance directly; a mutable class definition
creates it by snapshotting. Both use the same scheduler and property operations.
