# tweens.gd for C#

Typed C# tweens for GodotSharp and 2dog, superseding the `godotweens` package.
Reuse tween definitions, control independent playback handles, and compose
animations with `async`/`await`.

Targets .NET 10 and GodotSharp 4.7.2. Your application supplies a matching Godot
engine; the package depends only on GodotSharp. Other engine versions and
trimmed/AOT/web exports have not been validated.

```xml
<PackageReference Include="tweens.gd" Version="0.1.0" />
```

```csharp
using Godot;
using tweens.gd;

// Call from _Ready or later, on Godot's main thread.
var movement = sprite.Tween(new Position2DTween
{
    To = new Vector2(400, 180),
    Duration = 0.6,
    Ease = EaseType.CubicOut,
});

var reason = await movement.Completion;
if (reason == TweenCompletionReason.Completed)
    GD.Print("Arrived");
```

The first tween installs a runner automatically. No autoload is required.
Definitions are snapshotted on addition. Playback supports pause/resume,
cancellation, delays, loops, ping-pong, easing, and node lifetime handling.
Create and control tweens on Godot's main thread.

## Migrating from godotweens

Replace the `godotweens` package reference with `tweens.gd` and change
`using godotweens;` to `using tweens.gd;`. Update fully qualified type names and
assembly references from `godotweens` to `tweens.gd`, then restore and rebuild
all consumers. Public type names and tween behavior are unchanged. This is a
source and binary breaking rename; no legacy namespace shim is included.

The GDScript addon is planned separately and is not included in this package.
Public documentation is being prepared in the repository's Astro/Starlight site.
Until then, see the [repository README](https://github.com/outfox/godotweens#readme)
for the C# API and examples. The repository URL retains its existing name until
the GitHub rename is completed.

MIT licensed. Easing math and API inspiration come from Jeffrey Lanters'
unity-tweens; the package includes the license and third-party notices.
