# SPDX-License-Identifier: MIT
# SPDX-FileCopyrightText: 2026 Moritz Voss

param([Parameter(Mandatory)][string] $Tag)

$ErrorActionPreference = 'Stop'
# NuGet release tags use three numeric components and optional SemVer prerelease identifiers.
# Build metadata is excluded because it does not distinguish versions on nuget.org.
$number = '(0|[1-9][0-9]*)'
$identifier = '(0|[1-9][0-9]*|[0-9]*[A-Za-z-][0-9A-Za-z-]*)'
if ($Tag -cnotmatch "^v$number\.$number\.$number(-$identifier(\.$identifier)*)?$") {
    throw "Invalid release tag '$Tag'. Use v1.2.3 or v1.2.3-rc.1."
}
$version = $Tag.Substring(1)
$prerelease = $version.Contains('-').ToString().ToLowerInvariant()
if ($env:GITHUB_OUTPUT) {
    "version=$version" >> $env:GITHUB_OUTPUT
    "prerelease=$prerelease" >> $env:GITHUB_OUTPUT
}
Write-Output $version
