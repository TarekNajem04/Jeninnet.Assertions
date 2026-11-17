#requires -Version 7
<#
.SYNOPSIS
  Verifies that MSTest Assert usage is confined to the meta test allowlist.
.DESCRIPTION
  Scans tests/ and consumers/. The meta allowlist in mstest-assert-meta.json
  names the engine and adapter tests that may use MSTest Assert as an independent
  oracle. Any other file that uses Assert.* fails the check.
.PARAMETER RepositoryRoot
  Repository root. Defaults to two levels above this script.
.PARAMETER MetaPath
  Meta allowlist JSON path. Defaults to mstest-assert-meta.json beside this script.
.EXAMPLE
  pwsh scripts/test/check-mstest-assert-usage.ps1
#>
[CmdletBinding()]
param(
    [string]$RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path,
    [string]$MetaPath = (Join-Path $PSScriptRoot 'mstest-assert-meta.json')
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

if(-not (Test-Path -LiteralPath $MetaPath)) {
    Write-Host "ERROR: Meta allowlist not found: $MetaPath"
    exit 2
}

$allowlist = Get-Content -LiteralPath $MetaPath -Raw | ConvertFrom-Json -AsHashtable
$metaSection = $allowlist.meta
if($null -eq $metaSection) {
    Write-Host "ERROR: Meta allowlist has no 'meta' object: $MetaPath"
    exit 2
}

$meta = @{}
foreach($category in @('engine', 'adapter')) {
    $entries = $metaSection[$category]
    if($null -eq $entries) {
        Write-Host "ERROR: Meta allowlist has no 'meta.$category' array: $MetaPath"
        exit 2
    }

    foreach($path in $entries) {
        if([string]::IsNullOrWhiteSpace($path)) {
            Write-Host "ERROR: Meta allowlist contains an empty path in 'meta.$category'."
            exit 2
        }

        if($meta.ContainsKey($path)) {
            Write-Host "ERROR: Meta allowlist contains '$path' more than once."
            exit 2
        }

        $meta[$path] = $category
    }
}

$failures = [System.Collections.Generic.List[string]]::new()
foreach($path in $sorted.Keys) {
    if(-not $meta.ContainsKey($path)) {
        $failures.Add("MSTest Assert outside the meta allowlist: $path ($($sorted[$path]) call(s)). Use Should() or add the file to the justified meta list.")
    }
}

$stale = @($meta.Keys | Where-Object { -not $sorted.Contains($_) } | Sort-Object)

if($failures.Count -gt 0) {
    foreach($failure in $failures) {
        Write-Host "FAIL: $failure"
    }

    Write-Host "ERROR: MSTest Assert meta check failed with $($failures.Count) issue(s)."
    exit 1
}

Write-Host "MSTest Assert meta allowlist OK: $($sorted.Count) file(s), $total call(s), $($meta.Count) allowlisted file(s)."
if($stale.Count -gt 0) {
    Write-Host "Info: $($stale.Count) allowlisted file(s) currently have no Assert usage."
}
exit 0
