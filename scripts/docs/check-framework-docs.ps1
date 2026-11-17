#requires -Version 7
<#
.SYNOPSIS
  Verifies that current documentation reflects the .NET 11 / C# 15 toolchain and
  every shipped target framework.
.DESCRIPTION
  Compares each src/*/README.md against the TargetFramework(s) in its csproj, checks
  the root README and current en/ar book chapters for the core target list, and
  rejects stale ".NET 10 only" wording. Historical release notes and recorded
  benchmark environments are excluded because they are accurate as written.
.PARAMETER RepositoryRoot
  Repository root. Defaults to two levels above this script.
.EXAMPLE
  pwsh scripts/docs/check-framework-docs.ps1
#>
[CmdletBinding()]
param(
    [string]$RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Read-Text([string]$relativePath) {
    $path = Join-Path $RepositoryRoot $relativePath
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Missing documentation file: $relativePath"
    }
    return [System.IO.File]::ReadAllText($path)
}

function Get-ProjectFrameworks([string]$csprojPath) {
    $text = [System.IO.File]::ReadAllText($csprojPath)
    $matches = [regex]::Matches($text, '<TargetFrameworks?>([^<]+)</TargetFrameworks?>')
    if ($matches.Count -eq 0) {
        throw "No TargetFramework(s) found in $csprojPath"
    }

    $frameworks = [System.Collections.Generic.List[string]]::new()
    foreach ($match in $matches) {
        foreach ($framework in $match.Groups[1].Value.Split(';', [System.StringSplitOptions]::RemoveEmptyEntries)) {
            $framework = $framework.Trim()
            if (-not $frameworks.Contains($framework)) {
                $frameworks.Add($framework)
            }
        }
    }
    return $frameworks.ToArray()
}

function Assert-ContainsAll([string]$relativePath, [string[]]$expected, [string]$label) {
    $text = Read-Text $relativePath
    $missing = @($expected | Where-Object { -not $text.Contains($_) } | Sort-Object)
    if ($missing.Count -gt 0) {
        $script:failures.Add("$relativePath is missing $label`: $($missing -join ', ')")
    }
}

$failures = [System.Collections.Generic.List[string]]::new()

$globalJson = Read-Text 'global.json' | ConvertFrom-Json
$sdkVersion = [string]$globalJson.sdk.version
if ($sdkVersion -notmatch '^11\.') {
    $failures.Add("global.json must pin an 11.x SDK; found '$sdkVersion'.")
}

$props = Read-Text 'Directory.Build.props'
if ($props -notmatch '<LangVersion>\s*15\.0\s*</LangVersion>') {
    $failures.Add('Directory.Build.props must use LangVersion 15.0.')
}

# Every shipped package README must list every framework its csproj ships.
foreach ($csproj in (Get-ChildItem -LiteralPath (Join-Path $RepositoryRoot 'src') -Recurse -File -Filter '*.csproj' | Sort-Object FullName)) {
    $projectDir = Split-Path $csproj.FullName -Parent
    $relativeDir = [System.IO.Path]::GetRelativePath($RepositoryRoot, $projectDir).Replace('\', '/')
    $readme = "$relativeDir/README.md"
    if (-not (Test-Path -LiteralPath (Join-Path $RepositoryRoot $readme))) {
        $failures.Add("Missing package README: $readme")
        continue
    }

    $frameworks = @(Get-ProjectFrameworks $csproj.FullName)
    Assert-ContainsAll $readme $frameworks 'target framework(s)'
    Assert-ContainsAll $readme @($sdkVersion) 'pinned SDK version'
    Assert-ContainsAll $readme @('C# 15', '1.2.0', '1.2.0.0') 'current version information'
}

# Current core documentation must name every core target.
$coreCsproj = Join-Path $RepositoryRoot 'src/Jeninnet.Assertions/Jeninnet.Assertions.csproj'
$coreFrameworks = @(Get-ProjectFrameworks $coreCsproj)
foreach($file in @(
    'README.md',
    'docs/en/00-index.md',
    'docs/en/01-first-steps.md',
    'docs/en/10-ecosystem.md',
    'docs/ar/00-index.md',
    'docs/ar/01-first-steps.md',
    'docs/ar/10-ecosystem.md'
)) {
    Assert-ContainsAll $file $coreFrameworks 'core target framework(s)'
}

# The Sonar preview script is part of the documented contributor toolchain.
Assert-ContainsAll 'scripts/sonar/README.md' @($sdkVersion) 'pinned SDK version'

# Reject stale "current SDK is .NET 10" wording. Benchmark environment lines such
# as ".NET SDK 10.0.404" remain valid historical measurements.
$stalePatterns = @(
    '\.NET\s+10(?:\.0)?\+',
    'current\s+\.NET\s+10\s+SDK',
    'newest\s+installed[^)]*10\.0\.x',
    '\bSDK\s+10\b(?!\.0\.)'
)
$scanFiles = @()
$scanFiles += Get-ChildItem -LiteralPath $RepositoryRoot -Recurse -File -Filter 'README.md' |
    Where-Object { $_.FullName -notmatch '[\\/](bin|obj|private)[\\/]' }
$scanFiles += Get-ChildItem -LiteralPath (Join-Path $RepositoryRoot 'docs') -Recurse -File -Filter '*.md' |
    Where-Object { $_.FullName -notmatch '[\\/]releases[\\/]' }

foreach($file in $scanFiles) {
    $relative = [System.IO.Path]::GetRelativePath($RepositoryRoot, $file.FullName).Replace('\', '/')
    $text = [System.IO.File]::ReadAllText($file.FullName)
    foreach($pattern in $stalePatterns) {
        foreach($match in [regex]::Matches($text, $pattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)) {
            $line = $text.Substring(0, $match.Index).Split("`n").Count
            $failures.Add("$relative`:$line contains stale .NET 10 wording: '$($match.Value)'")
        }
    }
}

if($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Host "FAIL: $_" }
    Write-Host "ERROR: Framework documentation check failed with $($failures.Count) issue(s)."
    exit 1
}

Write-Host "Framework documentation OK: package targets, SDK $sdkVersion, C# 15, and current target lists are consistent."
exit 0
