#requires -Version 7
<#
.SYNOPSIS
  Mirrors the Arabic documentation (docs/ar) against the English one (docs/en)
  chapter by chapter. Every H2/H3 heading in an English chapter must have a
  matching heading-level in its Arabic twin, in the same order. H1 titles are
  excluded because they are translations. Heading text inside fenced code
  blocks is ignored.
.PARAMETER DocsRoot
  Repo docs folder. Defaults to <repo>/docs.
.OUTPUTS
  Prints one line per chapter with the compared level pattern, then a summary.
  Exits 1 if any chapter diverges.
.EXAMPLE
  pwsh scripts/docs/check-doc-parity.ps1
#>
param(
    [string]$DocsRoot = (Join-Path (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path 'docs')
)

$EnglishDir = Join-Path $DocsRoot 'en'
$ArabicDir  = Join-Path $DocsRoot 'ar'
if (-not (Test-Path $EnglishDir) -or -not (Test-Path $ArabicDir)) {
    Write-Error "Both docs/en and docs/ar must exist. DocsRoot=$DocsRoot"
    exit 2
}

function Get-HeadingLevels([string]$Path) {
    $levels = [System.Collections.Generic.List[int]]::new()
    $inFence = $false
    foreach ($line in [System.IO.File]::ReadAllLines($Path)) {
        $trimmed = $line.Trim()
        if ($trimmed -match '^```') { $inFence = -not $inFence; continue }
        if ($inFence) { continue }
        $m = [regex]::Match($trimmed, '^(#{1,6})\s+')
        if ($m.Success) { [void]$levels.Add($m.Groups[1].Value.Length) }
    }
    return $levels.ToArray()
}

$enFiles = Get-ChildItem $EnglishDir -File
$pageLines = [System.Collections.Generic.List[string]]::new()
[void]$pageLines.Add('Chapter            EN pattern                      AR pattern')
[void]$pageLines.Add('-' * 60)
$failures = [System.Collections.Generic.List[string]]::new()

foreach ($en in $enFiles) {
    $ar = Join-Path $ArabicDir $en.Name
    if (-not (Test-Path $ar)) {
        [void]$failures.Add("$($en.Name): missing in docs/ar")
        [void]$pageLines.Add('{0,-18} MISSING IN AR' -f $en.Name)
        continue
    }
    $enLevels   = @(Get-HeadingLevels -Path $en.FullName)
    $arLevels   = @(Get-HeadingLevels -Path $ar)
    if ($enLevels.Count -eq 0) { continue } # placeholder chapters with no headings
    $enBody = @($enLevels | Where-Object { $_ -ge 2 }) # drop H1 title
    $arBody = @($arLevels | Where-Object { $_ -ge 2 })

    $enPat = [string]::Join('-', $enBody)
    $arPat = [string]::Join('-', $arBody)
    [void]$pageLines.Add('{0,-22} {1,-31} {2}' -f ($en.Name.Replace('.md', ''), $enPat, $arPat))
    if (($arPat -ne $enPat) -or ($arBody.Count -ne $enBody.Count)) {
        [void]$failures.Add('{0}: EN [{1}] vs AR [{2}]' -f ($en.Name, $enPat, $arPat))
    }
}

$pageLines | ForEach-Object { Write-Host $_ }
Write-Host ''
if ($failures.Count -gt 0) {
    Write-Host "MIRROR PARITY FAILED ($($failures.Count) chapter(s)):" -ForegroundColor Red
    $failures | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
    exit 1
}
Write-Host "Mirror parity OK: $($enFiles.Count) chapters, ar/en heading structure identical." -ForegroundColor Green