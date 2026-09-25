# SPDX-License-Identifier: MIT
# SPDX-FileCopyrightText: 2026 Moritz Voss

$ErrorActionPreference = 'Stop'

# Exercise the production script without network access or release mutations.
function Invoke-WebRequest {
    param($Uri, $Headers, [switch] $SkipHttpErrorCheck)
    if (!$SkipHttpErrorCheck) { throw 'Lookup must inspect HTTP error statuses.' }
    if ($releaseTestState.NetworkFailure) { throw 'Simulated network failure' }
    [pscustomobject]@{ StatusCode = $releaseTestState.StatusCode }
}
function Get-ChildItem {
    param($Path, [switch] $File)
    [pscustomobject]@{ FullName = 'tweens.gd.0.1.0.nupkg' }
    [pscustomobject]@{ FullName = 'tweens.gd.0.1.0.snupkg' }
}
function gh {
    $releaseTestState.Commands.Add(@($args))
    $global:LASTEXITCODE = $releaseTestState.ExitCode
}

$cases = @(
    @{ Status = 200; Command = 'upload' },
    @{ Status = 404; Command = 'create' },
    @{ Status = 404; Command = 'create'; Prerelease = $true },
    @{ Status = 401; Error = 'HTTP 401' },
    @{ Status = 403; Error = 'HTTP 403' },
    @{ Status = 429; Error = 'HTTP 429' },
    @{ Status = 500; Error = 'HTTP 500' },
    @{ Status = 503; Error = 'HTTP 503' },
    @{ NetworkFailure = $true; Error = 'Simulated network failure' },
    @{ Status = 200; Command = 'upload'; ExitCode = 1; Error = 'gh exit code 1' },
    @{ Status = 404; Command = 'create'; ExitCode = 1; Error = 'gh exit code 1' }
)
$savedExitCode = $global:LASTEXITCODE
try {
    foreach ($case in $cases) {
        $releaseTestState = @{
            StatusCode = $case.Status
            NetworkFailure = $case.NetworkFailure
            ExitCode = [int]$case.ExitCode
            Commands = [System.Collections.Generic.List[object]]::new()
        }
        $failure = $null
        try {
            & "$PSScriptRoot/Publish-GitHubRelease.ps1" -Tag 'v0.1.0' -Prerelease:([bool]$case.Prerelease)
        } catch { $failure = $_.Exception.Message }
        if ($case.Error) {
            if (!$failure -or !$failure.Contains($case.Error)) { throw "Expected '$($case.Error)', got '$failure'." }
        } elseif ($failure) { throw $failure }
        if ($case.Command) {
            if ($releaseTestState.Commands.Count -ne 1 -or $releaseTestState.Commands[0][1] -ne $case.Command) {
                throw "Expected exactly one gh release $($case.Command) call."
            }
            if (($releaseTestState.Commands[0] -contains '--prerelease') -ne [bool]$case.Prerelease) {
                throw 'Incorrect prerelease option.'
            }
        } elseif ($releaseTestState.Commands.Count -ne 0) { throw 'Lookup failure must not publish anything.' }
    }
} finally {
    $global:LASTEXITCODE = $savedExitCode
}
Write-Output "Passed $($cases.Count) release publishing checks."
