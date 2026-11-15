#requires -Version 7
<#
.SYNOPSIS
  Verifies that every satellite resource file has exactly the same keys as the
  neutral English Strings.resx, with no empty values and no duplicates.
.PARAMETER LocalizationDirectory
  Folder containing Strings.resx and Strings.<culture>.resx. Defaults to the
  library localization folder.
.PARAMETER AllowPartial
  Cultures allowed to be intentionally partial (for example the French
  reference satellite). Missing/extra/empty keys in other cultures fail.
.OUTPUTS
  Prints one line per culture with missing/extra/empty counts, then a summary.
  Exits 1 when a non-allowed culture diverges or any duplicate key exists.
.EXAMPLE
  pwsh scripts/docs/check-localization.ps1
#>
param(
    [string]$LocalizationDirectory = (Join-Path (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path 'src\Jeninnet.Assertions\Localization'),
    [string[]]$AllowPartial = @('fr')
)

if (-not (Test-Path $LocalizationDirectory)) {
    Write-Error "Localization directory not found: $LocalizationDirectory"
    exit 2
}

$basePath = Join-Path $LocalizationDirectory 'Strings.resx'
if (-not (Test-Path $basePath)) {
    Write-Error "Neutral resource file not found: $basePath"
    exit 2
}

function Read-ResourceMap([string]$Path) {
    $document = [System.Xml.Linq.XDocument]::Load($Path)
    $map = [System.Collections.Generic.Dictionary[string, string]]::new([System.StringComparer]::Ordinal)
    $elements = @($document.Root.Elements('data'))
    foreach ($data in $elements) {
        $attribute = $data.Attribute('name')
        if ($null -eq $attribute) { continue }
        $name = $attribute.Value
        if ([string]::IsNullOrWhiteSpace($name)) { continue }
        if ($map.ContainsKey($name)) {
            throw "Duplicate resource key '$name' in $Path"
        }
        $valueElement = $data.Element('value')
        if ($null -eq $valueElement) {
            $map[$name] = ''
        } else {
            $map[$name] = $valueElement.Value
        }
    }
    return ,$map
}

$base = Read-ResourceMap -Path $basePath
$failures = [System.Collections.Generic.List[string]]::new()
$rows = [System.Collections.Generic.List[string]]::new()
[void]$rows.Add('culture     missing  extra  empty')
[void]$rows.Add('---------------------------------')

$satellites = Get-ChildItem $LocalizationDirectory -File -Filter 'Strings.*.resx' |
    Where-Object { $_.Name -ne 'Strings.resx' } |
    Sort-Object Name

foreach ($file in $satellites) {
    $culture = $file.Name.Substring('Strings.'.Length, $file.Name.Length - 'Strings.'.Length - '.resx'.Length)
    try {
        $translated = Read-ResourceMap -Path $file.FullName
    }
    catch {
        [void]$failures.Add("${culture}: $($_.Exception.Message)")
        [void]$rows.Add(('{0,-11} INVALID' -f $culture))
        continue
    }

    $missing = @($base.Keys | Where-Object { -not $translated.ContainsKey($_) })
    $extra = @($translated.Keys | Where-Object { -not $base.ContainsKey($_) })
    $empty = @($translated.Keys | Where-Object { [string]::IsNullOrWhiteSpace($translated[$_]) })
    $isAllowedPartial = $culture -in $AllowPartial

    [void]$rows.Add(('{0,-11} {1,7}  {2,5}  {3,5}' -f $culture, $missing.Count, $extra.Count, $empty.Count))
    if ($missing.Count -eq 0 -and $extra.Count -eq 0 -and $empty.Count -eq 0) {
        continue
    }

    if ($isAllowedPartial) {
        foreach ($key in $extra) { [void]$failures.Add("${culture}: extra key '$key'") }
        foreach ($key in $empty) { [void]$failures.Add("${culture}: empty value for '$key'") }
        continue
    }

    foreach ($key in $missing) { [void]$failures.Add("${culture}: missing key '$key'") }
    foreach ($key in $extra) { [void]$failures.Add("${culture}: extra key '$key'") }
    foreach ($key in $empty) { [void]$failures.Add("${culture}: empty value for '$key'") }
}

$rows | ForEach-Object { Write-Output $_ }
if ($failures.Count -gt 0) {
    Write-Output ''
    $failures | Select-Object -First 40 | ForEach-Object { Write-Output $_ }
    if ($failures.Count -gt 40) {
        Write-Output "... and $($failures.Count - 40) more"
    }
    Write-Error "Localization check failed with $($failures.Count) issue(s)."
    exit 1
}

Write-Output "Localization parity OK: $($satellites.Count) satellite file(s)."
exit 0
