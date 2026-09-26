# SPDX-License-Identifier: MIT
# SPDX-FileCopyrightText: 2026 Moritz Voss
$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$addonDestination = Join-Path $PSScriptRoot 'addons/tweens_gd'
$fixtureDestination = Join-Path $PSScriptRoot 'conformance'
New-Item -ItemType Directory -Path $addonDestination, $fixtureDestination -Force | Out-Null
Get-ChildItem -LiteralPath (Join-Path $repositoryRoot 'addons/tweens_gd') -File |
    Copy-Item -Destination $addonDestination -Force
Get-ChildItem -LiteralPath (Join-Path $repositoryRoot 'tests/conformance') -Filter '*.json' -File |
    Copy-Item -Destination $fixtureDestination -Force
