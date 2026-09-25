# SPDX-License-Identifier: MIT
# SPDX-FileCopyrightText: 2026 Moritz Voss

param([Parameter(Mandatory)][string] $Version)

$ErrorActionPreference = 'Stop'
$packageDirectory = Join-Path $PSScriptRoot '../../artifacts/packages' | Resolve-Path
$package = Join-Path $packageDirectory "tweens.gd.$Version.nupkg"
$symbols = Join-Path $packageDirectory "tweens.gd.$Version.snupkg"
if (!(Test-Path $symbols)) { throw "Missing symbol package: $symbols" }

Add-Type -AssemblyName System.IO.Compression.FileSystem
$symbolArchive = [System.IO.Compression.ZipFile]::OpenRead($symbols)
try {
    $pdbPath = 'lib/net10.0/tweens.gd.pdb'
    $pdb = $symbolArchive.GetEntry($pdbPath)
    if (!$pdb) { throw "Symbol package is missing $pdbPath" }
    if ($pdb.Length -eq 0) { throw "Symbol package contains an empty $pdbPath" }
} finally {
    $symbolArchive.Dispose()
}

$archive = [System.IO.Compression.ZipFile]::OpenRead($package)
try {
    foreach ($path in @('LICENSE', 'README.md', 'THIRD-PARTY-NOTICES.md', 'lib/net10.0/tweens.gd.dll', 'lib/net10.0/tweens.gd.xml')) {
        if (!$archive.GetEntry($path)) { throw "Package is missing $path" }
    }
    $reader = [System.IO.StreamReader]::new($archive.GetEntry('tweens.gd.nuspec').Open())
    try { $manifest = [xml]$reader.ReadToEnd() } finally { $reader.Dispose() }
    $metadata = $manifest.package.metadata
    if ($metadata.id -cne 'tweens.gd' -or $metadata.version -cne $Version) {
        throw 'Package ID or version is incorrect.'
    }
    if ($metadata.license.type -ne 'expression' -or $metadata.license.InnerText -ne 'MIT') {
        throw 'Package must declare the MIT license expression.'
    }
    $dependencies = @($metadata.dependencies.group.dependency)
    if ($dependencies.Count -ne 1 -or $dependencies[0].id -ne 'GodotSharp') {
        throw 'The library package must depend only on GodotSharp.'
    }
} finally {
    $archive.Dispose()
}

# Build against the packed artifact, without a project reference or the testbed.
$consumer = Join-Path $PSScriptRoot '../../artifacts/package-consumer'
New-Item $consumer -ItemType Directory -Force | Out-Null
@"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>
  <ItemGroup><PackageReference Include="tweens.gd" Version="[$Version]" /></ItemGroup>
</Project>
"@ | Set-Content (Join-Path $consumer 'Consumer.csproj')
@'
// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2026 Moritz Voss
using Godot;
using tweens.gd;
public static class Consumer
{
    public static System.Threading.Tasks.Task<TweenCompletionReason> Animate(Node2D node) =>
        node.Tween(new Position2DTween { To = new Vector2(100, 50), Duration = 0.25 }).Completion;
}
'@ | Set-Content (Join-Path $consumer 'Consumer.cs')
# Map this ID exclusively to the just-built package and use a fresh cache for each check.
$escapedPackageDirectory = [System.Security.SecurityElement]::Escape($packageDirectory.Path)
@"
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$escapedPackageDirectory" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
    <clear />
    <packageSource key="local"><package pattern="tweens.gd" /></packageSource>
    <packageSource key="nuget.org"><package pattern="*" /></packageSource>
  </packageSourceMapping>
</configuration>
"@ | Set-Content (Join-Path $consumer 'NuGet.config')
$cache = Join-Path $PSScriptRoot "../../artifacts/package-cache/$([Guid]::NewGuid())"
dotnet restore (Join-Path $consumer 'Consumer.csproj') --configfile (Join-Path $consumer 'NuGet.config') --packages $cache --force
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build (Join-Path $consumer 'Consumer.csproj') -c Release --no-restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
