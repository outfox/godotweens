# Structured definition generation

The property adapters in `src/Tweens/` also serve as the mutable builders for
convenience methods. The [Roslyn incremental generator](../../csharp.generators/StructuredDefinitionGenerator.cs)
discovers those adapters and the public configuration properties in
`TweenOptionsBuilder` using C# symbols, then produces the readonly record structs
in the `Tweens` namespace. Shader and custom-property definitions use explicit
templates in the same generator.

Generation runs automatically during builds and in the IDE. After adding an
adapter or changing configuration properties, build from the repository root:

```sh
dotnet build csharp/tweens.gd.csproj
```

Generated files are compiler outputs and are not checked in. To inspect them on
disk, build with `-p:EmitCompilerGeneratedFiles=true`; the default output is under
`csharp/obj/<configuration>/net10.0/generated/`. The generator is referenced only
as a build-time analyzer and is not shipped as a package dependency. Package
consumers receive the compiled definitions. CI builds the generator and library,
and tests check generation and coverage of every built-in adapter.

Runtime binding state stays in the private per-playback class instance. A
structured definition creates that instance directly; a mutable class definition
creates it by snapshotting. Both use the same scheduler and property operations.
