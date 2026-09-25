---
title: C# installation
description: Reference the C# project or build and consume its NuGet package locally.
---

The library targets .NET 10 and GodotSharp 4.7.2. Your application supplies a
matching Godot engine. Check [compatibility](/compatibility/) first; the included
2dog testbed is the validated way to try the library from this repository.

The package ID and C# namespace are both lowercase **`tweens.gd`**. Public NuGet
publication is pending; use either path below now.

## Reference the project

Clone the repository, then add a project reference from your C# application,
adjusting the path to your checkout:

```powershell
dotnet add path/to/YourGame.csproj reference path/to/tweens.gd/csharp/tweens.gd.csproj
```

This is how the included testbed consumes the library. Source lives under
`csharp/src/`; the root solution is `tweens.gd.slnx`.

## Build a local NuGet package

From the repository root:

```powershell
dotnet pack csharp/tweens.gd.csproj -c Release -o artifacts/packages
```

The current development version produces `tweens.gd.0.1.0.nupkg` and a symbol
package. In your consuming project's `NuGet.config`, add the absolute path to
that output directory alongside nuget.org, which supplies GodotSharp:

```xml
<configuration>
  <packageSources>
    <clear />
    <add key="local-tweens" value="C:/path/to/tweens.gd/artifacts/packages" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

Adjust the local path for your machine, then add this item to your application's
project file and run `dotnet restore`:

```xml
<ItemGroup>
  <PackageReference Include="tweens.gd" Version="0.1.0" />
</ItemGroup>
```

Use a project reference or a package reference, not both.

## Run the included gallery

From the repository root:

```powershell
dotnet build tweens.gd.slnx
dotnet run --project testbed/testbed.2dog
```

The testbed brings its engine through NuGet. The library itself has no 2dog,
native engine, or editor assembly dependency.

Continue with the [C# quickstart](/csharp/quickstart/).
