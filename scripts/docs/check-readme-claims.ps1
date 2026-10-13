#requires -Version 7
<#
.SYNOPSIS
  Verifies that every simple backticked PascalCase identifier in the README's
  "## Available Assertions" section (the collapsible groups) is real: it must
  either be a public API member name in the committed baseline, appear as part
  of one (e.g. the option verbs "InternalFields" inside "IncludingInternalFields"),
  or be an allowlisted BCL type/keyword used in prose. This keeps the README
  from advertising assertions that do not exist.
.PARAMETER ReadmePath
  Repo README.md. Defaults to <repo>/README.md.
.PARAMETER ApiBaselinePath
  Committed API baseline. Defaults to
  <repo>/tests/Jeninnet.Assertions.Tests/ApiBaselines/Jeninnet.Assertions.Api.txt.
.OUTPUTS
  Prints each unresolved identifier, then a summary. Exits 1 if any claim is
  not backed by the API surface.
.EXAMPLE
  pwsh scripts/docs/check-readme-claims.ps1
#>
param(
    [string]$ReadmePath      = (Join-Path (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path 'README.md'),
    [string]$ApiBaselinePath = (Resolve-Path (Join-Path $PSScriptRoot '..\..\tests\Jeninnet.Assertions.Tests\ApiBaselines\Jeninnet.Assertions.Api.txt')).Path
)

if (-not (Test-Path $ReadmePath) -or -not (Test-Path $ApiBaselinePath)) {
    Write-Error "README ($ReadmePath) or baseline ($ApiBaselinePath) not found."
    exit 2
}

function Get-PublicNames([string]$BaselinePath) {
    $names = [System.Collections.Generic.List[string]]::new()
    $currentType = $null
    foreach ($line in [System.IO.File]::ReadAllLines($BaselinePath)) {
        if ($line.StartsWith('//')) { continue }
        $trim = $line.TrimStart()
        if ($line.StartsWith('type ')) {
            $t = $line.Substring(5).Trim()
            foreach ($marker in @('<', '(')) {
                $at = $t.IndexOf($marker)
                if ($at -ge 0) { $t = $t.Substring(0, $at) }
            }
            $dot = $t.LastIndexOf('.')
            if ($dot -ge 0) { $t = $t.Substring($dot + 1) }
            [void]$names.Add($t)
            $currentType = $t
            continue
        }
        if ($null -eq $currentType) { continue }
        $name = $null
        if ($trim.StartsWith('method ')) {
            $name = ($trim.Substring(7).Split('(', 2))[0].Split(' ')[-1]
        }
        elseif ($trim.StartsWith('property ')) {
            if ($trim -match '^property (static )?[^ ]+ ([A-Za-z_][\w]*?)(\[.*\])? \{') { $name = $Matches[2] }
        }
        else {
            if ($trim -match '^(field|event) (static )?(const )?(readonly )?[^ ]+ ([A-Za-z_][\w]*)$') { $name = $Matches[5] }
        }
        if ($name) {
            $at = $name.IndexOf('<')
            if ($at -ge 0) { $name = $name.Substring(0, $at) }
            [void]$names.Add($name)
        }
    }
    return $names
}

$allowlist = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
foreach ($w in @('bool', 'object', 'string', 'Guid', 'Func', 'TimeOnly', 'Memory', 'ReadOnlyMemory', 'ReadOnlySpan', 'Span')) {
    [void]$allowlist.Add($w)
}

$public = Get-PublicNames -BaselinePath $ApiBaselinePath
$fullSurface = ($public | Sort-Object -Unique)

$readme = [System.IO.File]::ReadAllText($ReadmePath)
$sectionStart = $readme.IndexOf('## Available Assertions')
if ($sectionStart -lt 0) { Write-Error "'## Available Assertions' section not found in README."; exit 2 }
$sectionEnd = $readme.IndexOf('## ', $sectionStart + 5)
if ($sectionEnd -lt 0) { $sectionEnd = $readme.Length }
$section = $readme.Substring($sectionStart, $sectionEnd - $sectionStart)

$tokens = [regex]::Matches($section, '`([^`]+)`') |
    ForEach-Object { $_.Groups[1].Value } |
    Where-Object { $_ -match '^[A-Z][A-Za-z0-9]*$' } |
    Sort-Object -Unique

$unresolved = [System.Collections.Generic.List[string]]::new()
foreach ($token in $tokens) {
    $isMember   = $fullSurface -contains $token
    $isEmbedded = $fullSurface | Where-Object { $_.IndexOf($token, [System.StringComparison]::Ordinal) -ge 0 } | Select-Object -First 1
    if (-not $isMember -and $null -eq $isEmbedded -and -not $allowlist.Contains($token)) {
        [void]$unresolved.Add($token)
    }
}

Write-Host "Checked $($tokens.Count) identifiers from the Available Assertions section."
if ($unresolved.Count -gt 0) {
    Write-Host "UNRESOLVED CLAIMS ($($unresolved.Count)):" -ForegroundColor Red
    $unresolved | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
    exit 1
}
Write-Host "README claims OK: every backticked identifier is backed by the public API." -ForegroundColor Green