#requires -Version 7
<#
.SYNOPSIS
  Verifies the contents of a packed Jeninnet.Assertions package without publishing it.
.DESCRIPTION
  Checks the four target-framework assets, XML documentation, README/icon, nuspec
  metadata (repository commit, license, dependency groups), and the sibling symbol
  package. Fails when source, test, or private files leak into either archive.
.PARAMETER PackageDirectory
  Folder containing the .nupkg and .snupkg files. Defaults to artifacts/packages.
.PARAMETER PackageId
  Package id to verify. Defaults to Jeninnet.Assertions.
.PARAMETER Version
  Optional package version; when omitted the newest .nupkg in the folder is used.
.OUTPUTS
  Prints the checked assets and a summary. Exits 1 on any mismatch.
.EXAMPLE
  pwsh scripts/release/verify-package.ps1
.EXAMPLE
  pwsh scripts/release/verify-package.ps1 -Version 1.0.0.0
#>
param(
    [string]$PackageDirectory = (Join-Path (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path 'artifacts/packages'),
    [string]$PackageId = 'Jeninnet.Assertions',
    [string]$Version
)

Add-Type -AssemblyName System.IO.Compression.FileSystem

$targetFrameworks = @('netstandard2.0', 'net6.0', 'net8.0', 'net10.0', 'net11.0')
$failures = [System.Collections.Generic.List[string]]::new()

function Add-Failure([string]$Message) {
    [void]$failures.Add($Message)
}

function Get-ZipEntries([string]$Path) {
    $archive = [System.IO.Compression.ZipFile]::OpenRead($Path)
    try {
        return @($archive.Entries | ForEach-Object { $_.FullName })
    }
    finally {
        $archive.Dispose()
    }
}

function Read-ZipText([string]$Path, [string]$EntryName) {
    $archive = [System.IO.Compression.ZipFile]::OpenRead($Path)
    try {
        $entry = $archive.GetEntry($EntryName)
        if ($null -eq $entry) {
            return $null
        }

        $reader = [System.IO.StreamReader]::new($entry.Open())
        try {
            return $reader.ReadToEnd()
        }
        finally {
            $reader.Dispose()
        }
    }
    finally {
        $archive.Dispose()
    }
}

if (-not (Test-Path $PackageDirectory)) {
    Write-Error "Package directory not found: $PackageDirectory. Run dotnet pack first."
    exit 2
}

$packages = @(Get-ChildItem $PackageDirectory -File -Filter "$PackageId.*.nupkg" |
    Where-Object {
        # Match only the exact package ID followed by a numeric version. A plain wildcard
        # also matches sibling packages such as Jeninnet.Assertions.MSTest.1.1.0.nupkg.
        $_.Name -match ('^' + [regex]::Escape($PackageId) + '\.\d+(\.\d+){1,3}\.nupkg$')
    } |
    Where-Object { $_.Name -notlike '*.snupkg' } |
    Sort-Object LastWriteTime -Descending)
if ($Version) {
    $packages = @($packages | Where-Object { $_.Name -eq "$PackageId.$Version.nupkg" })
}

if ($packages.Count -eq 0) {
    Write-Error "No $PackageId .nupkg found in $PackageDirectory$(if ($Version) { " for version $Version" })."
    exit 2
}

$package = $packages[0]
$packageVersion = $package.Name.Substring($PackageId.Length + 1, $package.Name.Length - $PackageId.Length - 1 - '.nupkg'.Length)
$symbolPackage = Join-Path $package.DirectoryName "$PackageId.$packageVersion.snupkg"
if (-not (Test-Path $symbolPackage)) {
    Add-Failure "Symbol package not found: $symbolPackage"
}

$entries = Get-ZipEntries -Path $package.FullName
$symbolEntries = if (Test-Path $symbolPackage) { Get-ZipEntries -Path $symbolPackage } else { @() }

foreach ($framework in $targetFrameworks) {
    $assembly = "lib/$framework/$PackageId.dll"
    $documentation = "lib/$framework/$PackageId.xml"
    if ($entries -notcontains $assembly) {
        Add-Failure "Missing assembly asset: $assembly"
    }

    if ($entries -notcontains $documentation) {
        Add-Failure "Missing XML documentation: $documentation"
    }

    $pdb = "lib/$framework/$PackageId.pdb"
    if ((Test-Path -LiteralPath $symbolPackage) -and ($symbolEntries -notcontains $pdb)) {
        Add-Failure "Missing symbol asset: $pdb"
    }
}

foreach ($required in @('README.md', 'icon.png', "$PackageId.nuspec")) {
    if ($entries -notcontains $required) {
        Add-Failure "Missing package file: $required"
    }
}

foreach ($entry in $entries) {
    if ($entry -match '(^|/)(private|tests?|src)/' -or $entry -like '*.cs' -or $entry -like '*.csproj' -or $entry -like '*.slnx') {
        Add-Failure "Unexpected source/test file in package: $entry"
    }
}

$nuspecText = Read-ZipText -Path $package.FullName -EntryName "$PackageId.nuspec"
if ($null -ne $nuspecText) {
    try {
        [xml]$nuspec = $nuspecText
        $metadata = $nuspec.package.metadata
        if ($metadata.id -ne $PackageId) {
            Add-Failure "Nuspec id '$($metadata.id)' does not match '$PackageId'."
        }

        if ($metadata.version -ne $packageVersion) {
            Add-Failure "Nuspec version '$($metadata.version)' does not match file version '$packageVersion'."
        }

        if ($metadata.license.type -ne 'expression' -or $metadata.license.'#text' -ne 'MIT') {
            Add-Failure 'Nuspec must declare the MIT license expression.'
        }

        if ($metadata.repository.type -ne 'git' -or [string]::IsNullOrWhiteSpace($metadata.repository.commit)) {
            Add-Failure 'Nuspec repository must include type="git" and a commit hash.'
        }

        if ($metadata.readme -ne 'README.md' -or $metadata.icon -ne 'icon.png') {
            Add-Failure 'Nuspec must reference README.md and icon.png.'
        }

        $groups = @($metadata.dependencies.group)
        $groupFrameworks = @($groups | ForEach-Object { $_.targetFramework } | Sort-Object)
        $expectedFrameworks = @('net11.0', 'net10.0', 'net6.0', 'net8.0', '.NETStandard2.0') | Sort-Object
        if (($groupFrameworks -join ';') -ne ($expectedFrameworks -join ';')) {
            Add-Failure "Nuspec dependency groups '$($groupFrameworks -join ', ')' do not match the four target frameworks."
        }

        $forbidden = @('MSTest', 'MSTest.TestAdapter', 'MSTest.TestFramework', 'xunit', 'xunit.assert', 'NUnit', 'TUnit')
        foreach ($group in $groups) {
            foreach ($dependency in @($group.dependency)) {
                if ($null -eq $dependency) { continue }
                if ($dependency.id -in $forbidden) {
                    Add-Failure "Test-framework dependency leaked into the package: $($dependency.id)"
                }

                $isJson = $dependency.id -eq 'System.Text.Json'
                $isJsonFramework = $group.targetFramework -in @('net6.0', '.NETStandard2.0')
                if ($isJson -and -not $isJsonFramework) {
                    Add-Failure "System.Text.Json should only be a dependency on net6.0 and netstandard2.0, not $($group.targetFramework)."
                }
            }
        }
    }
    catch {
        Add-Failure "Could not parse the nuspec: $($_.Exception.Message)"
    }
}

$frameworkSummary = ($targetFrameworks | ForEach-Object { "lib/$_/$PackageId.dll" }) -join ', '
Write-Output "Package: $($package.Name) ($([math]::Round($package.Length / 1MB, 2)) MB)"
Write-Output "Frameworks: $frameworkSummary"
Write-Output "Symbols: $(if (Test-Path $symbolPackage) { Split-Path -Leaf $symbolPackage } else { '<missing>' })"
Write-Output "Entries: $($entries.Count) package file(s), $($symbolEntries.Count) symbol file(s)"

if ($failures.Count -gt 0) {
    Write-Output ''
    $failures | ForEach-Object { Write-Output "FAIL: $_" }
    Write-Error "Package verification failed with $($failures.Count) issue(s)."
    exit 1
}

Write-Output 'Package verification OK: assets, docs, metadata, and symbols match the release contract.'
exit 0
