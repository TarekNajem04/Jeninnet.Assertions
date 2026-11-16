#requires -Version 7
<#
.SYNOPSIS
  Enforces the MSTest Assert ratchet for the test suite.
.DESCRIPTION
  Counts textual Assert.* usage in tests/ and consumers/. Existing counts are
  grandfathered in mstest-assert-baseline.json. The check fails when a file
  outside the baseline uses Assert.* or when a baseline file increases.
.PARAMETER RepositoryRoot
  Repository root. Defaults to two levels above this script.
.PARAMETER BaselinePath
  Baseline JSON path. Defaults to mstest-assert-baseline.json beside this script.
.PARAMETER UpdateBaseline
  Rewrites the baseline from the current tree. Use only after an intentional
  migration or when adding a justified engine-test file.
.EXAMPLE
  pwsh scripts/test/check-mstest-assert-usage.ps1
.EXAMPLE
  pwsh scripts/test/check-mstest-assert-usage.ps1 -UpdateBaseline
#>
[CmdletBinding()]
param(
    [string]$RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path,
    [string]$BaselinePath = (Join-Path $PSScriptRoot 'mstest-assert-baseline.json'),
    [switch]$UpdateBaseline
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$excludedPathPatterns = @(
    '[\\/](bin|obj)[\\/]',
    '[\\/]Jeninnet\.Assertions\.Analyzer\.Tests[\\/]'
)

function Get-AssertCount([string]$path) {
    $text = [System.IO.File]::ReadAllText($path)
    return [regex]::Matches($text, '\bAssert\.').Count
}

$scanRoots = @(
    (Join-Path $RepositoryRoot 'tests'),
    (Join-Path $RepositoryRoot 'consumers')
)

$counts = [ordered]@{}
foreach($root in $scanRoots) {
    if(-not (Test-Path -LiteralPath $root)) {
        continue
    }

    foreach($file in Get-ChildItem -LiteralPath $root -Recurse -File -Filter '*.cs') {
        $relative = [System.IO.Path]::GetRelativePath($RepositoryRoot, $file.FullName).Replace('\', '/')
        if($excludedPathPatterns | Where-Object { $relative -match $_ }) {
            continue
        }

        $count = Get-AssertCount $file.FullName
        if($count -gt 0) {
            $counts[$relative] = $count
        }
    }
}

$sorted = [ordered]@{}
foreach($key in ($counts.Keys | Sort-Object)) {
    $sorted[$key] = $counts[$key]
}

$total = ($sorted.Values | Measure-Object -Sum).Sum
if($null -eq $total) {
    $total = 0
}

if($UpdateBaseline) {
    $document = [ordered]@{
        policy = 'MSTest Assert ratchet. Entries are grandfathered debt; lower or remove them as files migrate to Should().'
        files = $sorted
    }

    $json = $document | ConvertTo-Json -Depth 5
    [System.IO.File]::WriteAllText($BaselinePath, $json + [Environment]::NewLine, [System.Text.UTF8Encoding]::new($false))
    Write-Host "Updated $BaselinePath with $($sorted.Count) file(s) and $total Assert call(s)."
    exit 0
}

if(-not (Test-Path -LiteralPath $BaselinePath)) {
    Write-Host "ERROR: Baseline not found: $BaselinePath. Run with -UpdateBaseline after review."
    exit 2
}

$baseline = Get-Content -LiteralPath $BaselinePath -Raw | ConvertFrom-Json -AsHashtable
$allowed = $baseline.files
if($null -eq $allowed) {
    Write-Host "ERROR: Baseline has no 'files' object: $BaselinePath"
    exit 2
}

$failures = [System.Collections.Generic.List[string]]::new()
foreach($path in $sorted.Keys) {
    $actual = [int]$sorted[$path]
    if(-not $allowed.ContainsKey($path)) {
        $failures.Add("New MSTest Assert usage: $path ($actual call(s)). Use Should() or add a justified engine-test entry.")
        continue
    }

    $limit = [int]$allowed[$path]
    if($actual -gt $limit) {
        $failures.Add("MSTest Assert usage increased: $path (allowed $limit, found $actual). Migrate the new calls to Should().")
    }
}

$stale = @($allowed.Keys | Where-Object { -not $sorted.Contains($_) } | Sort-Object)

if($failures.Count -gt 0) {
    foreach($failure in $failures) {
        Write-Host "FAIL: $failure"
    }

    Write-Host "ERROR: MSTest Assert ratchet failed with $($failures.Count) issue(s). Run with -UpdateBaseline only after intentional migration."
    exit 1
}

Write-Host "MSTest Assert ratchet OK: $($sorted.Count) file(s), $total grandfathered call(s)."
if($stale.Count -gt 0) {
    Write-Host "Info: $($stale.Count) baseline file(s) no longer use Assert. Run with -UpdateBaseline to remove them."
}
exit 0
