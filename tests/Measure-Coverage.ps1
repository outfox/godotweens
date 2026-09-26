# SPDX-License-Identifier: MIT
# SPDX-FileCopyrightText: 2026 Moritz Voss

<#
.SYNOPSIS
Runs the library test suite with coverage and reports line and branch coverage of tweens.gd.

.DESCRIPTION
Runs the Headless and Lifecycle suites, and with -Rendering the renderer-only suite (needs a GPU and display); each
suite is its own build and process. Runs merge per line, keeping the better result. Hand-written sources and the
generated Tweens.* definitions are reported separately; the minimums apply to the hand-written sources.
#>
param(
    [string] $Configuration = 'Release',
    [switch] $NoBuild,
    [switch] $Rendering,
    [ValidateRange(0, 100)][double] $MinimumLine = 0,
    [ValidateRange(0, 100)][double] $MinimumBranch = 0,
    [string] $ResultsDirectory = (Join-Path $PSScriptRoot '../artifacts/coverage')
)

$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'tweens.gd.tests/tweens.gd.tests.csproj'
$settings = Join-Path $PSScriptRoot 'tweens.gd.tests/coverage.runsettings'
if (Test-Path $ResultsDirectory) { Remove-Item $ResultsDirectory -Recurse -Force }

$suites = @('Headless', 'Lifecycle') + $(if ($Rendering) { 'Rendering' })
foreach ($suite in $suites) {
    $arguments = @('test', $project, '-c', $Configuration, "-p:TestSuite=$suite", '--settings', $settings,
        '--collect:XPlat Code Coverage', '--results-directory', $ResultsDirectory, '--logger', "trx;LogFileName=$suite.trx")
    if ($NoBuild) { $arguments += '--no-build' }
    & dotnet @arguments
    if ($LASTEXITCODE -ne 0) { throw "The $suite tests failed with exit code $LASTEXITCODE." }
}

# Merge per file and line: a line counts as hit if any run hit it, with the best branch result of any run.
$files = @{}
foreach ($report in Get-ChildItem $ResultsDirectory -Recurse -Filter 'coverage.cobertura.xml') {
    $xml = [xml](Get-Content $report.FullName -Raw)
    foreach ($class in $xml.coverage.packages.package.classes.class) {
        if (!$files.ContainsKey($class.filename)) { $files[$class.filename] = @{} }
        $lines = $files[$class.filename]
        foreach ($line in $class.lines.line) {
            $number = [int]$line.number
            $covered = 0; $total = 0
            if ($line.'condition-coverage' -match '\((\d+)/(\d+)\)') { $covered = [int]$Matches[1]; $total = [int]$Matches[2] }
            $previous = $lines[$number]
            $lines[$number] = [pscustomobject]@{
                Hit = ([int]$line.hits -gt 0) -or ($previous -and $previous.Hit)
                Covered = [Math]::Max($covered, $(if ($previous) { $previous.Covered } else { 0 }))
                Total = [Math]::Max($total, $(if ($previous) { $previous.Total } else { 0 }))
            }
        }
    }
}
if ($files.Count -eq 0) { throw "No coverage reports were found in $ResultsDirectory." }

$summaries = foreach ($file in $files.Keys) {
    $lines = $files[$file].Values
    [pscustomobject]@{
        File = $file
        Generated = $file -match '\.g\.cs$'
        Lines = @($lines).Count
        LinesHit = @($lines | Where-Object Hit).Count
        Branches = ($lines | Measure-Object Total -Sum).Sum
        BranchesHit = ($lines | Measure-Object Covered -Sum).Sum
    }
}

function Format-Rate([double] $hit, [double] $total) { if ($total -eq 0) { 'n/a' } else { '{0:P2}' -f ($hit / $total) } }
function Get-Percent([double] $hit, [double] $total) { if ($total -eq 0) { 100 } else { 100 * $hit / $total } }

$results = foreach ($generated in $false, $true) {
    $group = @($summaries | Where-Object Generated -eq $generated)
    $lines = ($group | Measure-Object Lines -Sum).Sum; $linesHit = ($group | Measure-Object LinesHit -Sum).Sum
    $branches = ($group | Measure-Object Branches -Sum).Sum; $branchesHit = ($group | Measure-Object BranchesHit -Sum).Sum
    [pscustomobject]@{
        Name = if ($generated) { 'Generated Tweens.* definitions' } else { 'Library sources' }
        Line = Get-Percent $linesHit $lines
        Branch = Get-Percent $branchesHit $branches
        Text = "$linesHit/$lines lines ($(Format-Rate $linesHit $lines)), $branchesHit/$branches branches ($(Format-Rate $branchesHit $branches))"
    }
}

Write-Host "`ntweens.gd coverage ($Configuration; $($suites -join ', '))"
foreach ($result in $results) { Write-Host ("  {0,-32} {1}" -f $result.Name, $result.Text) }
$incomplete = $summaries | Where-Object { !$_.Generated -and ($_.LinesHit -lt $_.Lines -or $_.BranchesHit -lt $_.Branches) } | Sort-Object File
if ($incomplete) {
    Write-Host '  Incompletely covered library sources:'
    foreach ($file in $incomplete) {
        Write-Host ("    {0,-48} {1,4}/{2,-4} lines {3,4}/{4,-4} branches" -f $file.File, $file.LinesHit, $file.Lines, $file.BranchesHit, $file.Branches)
    }
}
if ($env:GITHUB_STEP_SUMMARY) {
    $table = @('| tweens.gd coverage | |', '| --- | --- |') + ($results | ForEach-Object { "| $($_.Name) | $($_.Text) |" })
    Add-Content $env:GITHUB_STEP_SUMMARY ($table -join "`n")
}

$library = $results[0]
if ($library.Line -lt $MinimumLine) { throw ('Library line coverage {0:N2}% is below the minimum of {1}%.' -f $library.Line, $MinimumLine) }
if ($library.Branch -lt $MinimumBranch) { throw ('Library branch coverage {0:N2}% is below the minimum of {1}%.' -f $library.Branch, $MinimumBranch) }
