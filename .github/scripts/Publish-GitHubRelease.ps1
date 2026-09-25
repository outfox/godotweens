# SPDX-License-Identifier: MIT
# SPDX-FileCopyrightText: 2026 Moritz Voss

param(
    [Parameter(Mandatory)][string] $Tag,
    [switch] $Prerelease,
    [string] $PackageDirectory = 'packages'
)

$ErrorActionPreference = 'Stop'
$packages = @(Get-ChildItem $PackageDirectory -File | ForEach-Object FullName)
$apiUrl = if ($env:GITHUB_API_URL) { $env:GITHUB_API_URL } else { 'https://api.github.com' }
$headers = @{
    Authorization = "Bearer $env:GH_TOKEN"
    Accept = 'application/vnd.github+json'
    'X-GitHub-Api-Version' = '2022-11-28'
}
# gh release view uses the same exit code for not-found and other API failures.
# Inspect the HTTP status instead; network errors remain terminating errors.
$response = Invoke-WebRequest -Uri "$apiUrl/repos/$env:GH_REPO/releases/tags/$([Uri]::EscapeDataString($Tag))" -Headers $headers -SkipHttpErrorCheck
switch ([int]$response.StatusCode) {
    200 {
        gh release upload $Tag @packages --clobber
    }
    404 {
        $options = @('--verify-tag', '--generate-notes', '--title', "tweens.gd $Tag")
        if ($Prerelease) { $options += '--prerelease' }
        gh release create $Tag @packages @options
    }
    default {
        throw "Release lookup failed (HTTP $($response.StatusCode)); no release was created or updated."
    }
}
if ($LASTEXITCODE -ne 0) { throw "GitHub release publication failed (gh exit code $LASTEXITCODE)." }
