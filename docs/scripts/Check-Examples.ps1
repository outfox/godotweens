# Compile the C# code fences actually displayed by the documentation.
$ErrorActionPreference = 'Stop'
$docsRoot = Split-Path $PSScriptRoot -Parent
$repoRoot = Split-Path $docsRoot -Parent
$exampleOutput = Join-Path $repoRoot "artifacts/docs-examples/$([Guid]::NewGuid())"
New-Item -ItemType Directory -Path $exampleOutput -Force | Out-Null
$libraryProject = [System.Security.SecurityElement]::Escape((Join-Path $repoRoot 'csharp/tweens.gd.csproj'))
@"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <NoWarn>CS1998</NoWarn>
  </PropertyGroup>
  <ItemGroup><ProjectReference Include="$libraryProject" /></ItemGroup>
</Project>
"@ | Set-Content (Join-Path $exampleOutput 'Examples.csproj')

$exampleCount = 0
foreach ($page in Get-ChildItem (Join-Path $docsRoot 'src/content/docs') -Recurse -File | Where-Object { $_.Extension -in '.md', '.mdx' }) {
    $body = [IO.File]::ReadAllText($page.FullName).Replace("`r`n", "`n")
    foreach ($fence in [regex]::Matches($body, '(?m)^```csharp[^\n]*\n(?<code>[\s\S]*?)^```\s*$')) {
        $exampleCount++
        $code = $fence.Groups['code'].Value
        $line = ($body.Substring(0, $fence.Groups['code'].Index) -split "`n").Count
        $source = $page.FullName.Replace('\', '/')
        if ($code -match '(?m)^public (?:partial |sealed |static )?class ') {
            $compilation = "#line $line `"$source`"`n$code"
        } else {
            # Context is documented on each page. These are compile-only parameters,
            # not engine objects created or exercised by this check.
            $imports = @('using Godot;', 'using tweens.gd;') + @([regex]::Matches($code, '(?m)^using [\w.]+;') | ForEach-Object { $_.Value })
            $code = [regex]::Replace($code, '(?m)^using [\w.]+;', '')
            $compilation = @"
$(($imports | Select-Object -Unique) -join "`n")
public partial class DocumentationExample$exampleCount : Node
{
    public async Task Run(Sprite2D sprite, Label label, Camera2D camera,
        StandardMaterial3D material, StandardMaterial3D shared,
        MeshInstance3D mesh, ShaderMaterial shaderMaterial, Node owner,
        CancellationToken cancellationToken)
    {
#line $line "$source"
$code
    }
}
"@
        }
        Set-Content (Join-Path $exampleOutput "Example$exampleCount.cs") $compilation
    }
}
if ($exampleCount -eq 0) { throw 'No C# examples found.' }
dotnet build (Join-Path $exampleOutput 'Examples.csproj') -c Release --nologo
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Output "Compiled all $exampleCount C# documentation examples."
