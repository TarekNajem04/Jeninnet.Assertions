#requires -Version 7
<#
.SYNOPSIS
  Compares Jeninnet.Assertions' public assertion surface with pinned peer packages.

.DESCRIPTION
  Downloads the pinned NuGet packages listed in peer-versions.json, reflects their
  public assemblies, and compares the subjects accepted by Should() plus the verbs
  exposed by the returned assertion types.

  The Jeninnet side is read from the committed public-API baseline by default, so
  the comparison works from a clean checkout without building or restoring. Pass
  -OurAssembly to reflect a built assembly instead.

  This is a measurement tool, not a build or test dependency. Outputs are written
  under artifacts/parity/ (gitignored).

.PARAMETER VersionsFile
  JSON file containing pinned package versions and probe subjects.

.PARAMETER BaselinePath
  Committed public-API baseline used when -OurAssembly is not supplied.

.PARAMETER OutputDirectory
  Directory for report.md and report.json. Defaults to <repo>/artifacts/parity.

.PARAMETER OurAssembly
  Optional path to a built Jeninnet.Assertions.dll. When supplied, the tool reflects
  it instead of reading the baseline.

.PARAMETER Refresh
  Re-downloads packages and re-extracts them even when cached.

.PARAMETER Quiet
  Suppresses progress messages.

.OUTPUTS
  Writes report.md and report.json. Exits 0 on success, 2 on a fatal input error.

.EXAMPLE
  pwsh scripts/parity/compare.ps1

.EXAMPLE
  pwsh scripts/parity/compare.ps1 -Refresh -OurAssembly src/Jeninnet.Assertions/bin/Release/net11.0/Jeninnet.Assertions.dll
#>
[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSReviewUnusedParameter', 'Refresh', Justification = 'Used by helper functions through script scope.')]
[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSReviewUnusedParameter', 'Quiet', Justification = 'Used by helper functions through script scope.')]
[CmdletBinding()]
param(
    [string]$VersionsFile = (Join-Path $PSScriptRoot 'peer-versions.json'),
    [string]$BaselinePath = (Join-Path (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path 'tests\Jeninnet.Assertions.Tests\ApiBaselines\Jeninnet.Assertions.Api.txt'),
    [string]$OutputDirectory = (Join-Path (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path 'artifacts\parity'),
    [string]$OurAssembly,
    [switch]$Refresh,
    [switch]$Quiet
)

$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$baselineFullPath = if([System.IO.Path]::IsPathRooted($BaselinePath)) { $BaselinePath } else { Join-Path $repoRoot $BaselinePath }
$outputFullPath = if([System.IO.Path]::IsPathRooted($OutputDirectory)) { $OutputDirectory } else { Join-Path $repoRoot $OutputDirectory }
$cacheDirectory = Join-Path $outputFullPath 'packages'
$extractDirectory = Join-Path $outputFullPath 'extracted'
$null = New-Item -ItemType Directory -Force -Path $outputFullPath, $cacheDirectory, $extractDirectory

function Write-Status {
    param([string]$Message)
    if(-not $Quiet) {
        Write-Information -MessageData $Message -InformationAction Continue
    }
}

function Test-FilePresent {
    param([string]$Path, [string]$Description)
    if(-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw "$Description not found: $Path"
    }
}

function Get-PackageArchive {
    param(
        [string]$PackageId,
        [string]$Version
    )

    $lowerId = $PackageId.ToLowerInvariant()
    $archive = Join-Path $cacheDirectory "$lowerId.$Version.nupkg"
    if((Test-Path -LiteralPath $archive -PathType Leaf) -and -not $Refresh) {
        Write-Status "  cache   $PackageId $Version"
        return $archive
    }

    $url = "https://api.nuget.org/v3-flatcontainer/$lowerId/$Version/$lowerId.$Version.nupkg"
    Write-Status "  fetch   $PackageId $Version"
    Invoke-WebRequest -Uri $url -OutFile $archive
    return $archive
}

function Expand-PackageArchive {
    param(
        [string]$Archive,
        [string]$Destination
    )

    if((Test-Path -LiteralPath $Destination -PathType Container) -and -not $Refresh) {
        return $Destination
    }

    if(Test-Path -LiteralPath $Destination) {
        Remove-Item -LiteralPath $Destination -Recurse -Force
    }

    $null = New-Item -ItemType Directory -Force -Path $Destination
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::ExtractToDirectory($Archive, $Destination)
    return $Destination
}

function Get-PackageDescription {
    param([string]$PackageDirectory)

    $nuspec = Get-ChildItem -LiteralPath $PackageDirectory -Filter *.nuspec -File | Select-Object -First 1
    if($null -eq $nuspec) {
        throw "No .nuspec found in $PackageDirectory"
    }

    [xml]$document = Get-Content -LiteralPath $nuspec.FullName
    $metadata = $document.package.metadata
    $licenseElement = $metadata.license
    $licenseText = if($licenseElement -is [string]) { $licenseElement } else { [string]$licenseElement.'#text' }
    $licenseType = if($licenseElement -is [string]) { '' } else { [string]$licenseElement.type }
    $licenseFirstLine = ''
    if($licenseType -eq 'file' -and -not [string]::IsNullOrWhiteSpace($licenseText)) {
        $licensePath = Join-Path $PackageDirectory $licenseText
        if(Test-Path -LiteralPath $licensePath -PathType Leaf) {
            $licenseFirstLine = Get-Content -LiteralPath $licensePath | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | Select-Object -First 1
        }
    }

    $dependencyIds = [System.Collections.Generic.HashSet[string]]::new()
    $dependencyNodes = @()
    if($null -ne $metadata.dependencies) {
        if($null -ne $metadata.dependencies.group) {
            foreach($group in @($metadata.dependencies.group)) {
                $dependencyNodes += @($group.dependency)
            }
        }
        if($null -ne $metadata.dependencies.dependency) {
            $dependencyNodes += @($metadata.dependencies.dependency)
        }
    }
    foreach($dependency in $dependencyNodes) {
        if($null -ne $dependency -and -not [string]::IsNullOrWhiteSpace([string]$dependency.id)) {
            [void]$dependencyIds.Add([string]$dependency.id)
        }
    }

    return [pscustomobject]@{
        Id = [string]$metadata.id
        Version = [string]$metadata.version
        License = if([string]::IsNullOrWhiteSpace($licenseText)) { 'not declared' } else { $licenseText }
        LicenseType = $licenseType
        LicenseFirstLine = $licenseFirstLine
        ProjectUrl = [string]$metadata.projectUrl
        RepositoryUrl = if($null -ne $metadata.repository) { [string]$metadata.repository.url } else { '' }
        Dependencies = @($dependencyIds | Sort-Object)
    }
}

function Get-TargetAssembly {
    param(
        [string]$PackageDirectory,
        [string]$PackageId
    )

    $libDirectory = Join-Path $PackageDirectory 'lib'
    if(-not (Test-Path -LiteralPath $libDirectory -PathType Container)) {
        throw "No lib/ folder in $PackageDirectory"
    }

    $preferredFrameworks = @('net11.0', 'net10.0', 'net9.0', 'net8.0', 'net6.0', 'netstandard2.1', 'netstandard2.0')
    $frameworkDirectory = $null
    foreach($framework in $preferredFrameworks) {
        $candidate = Get-ChildItem -LiteralPath $libDirectory -Directory | Where-Object { $_.Name -eq $framework } | Select-Object -First 1
        if($null -ne $candidate) {
            $frameworkDirectory = $candidate
            break
        }
    }
    if($null -eq $frameworkDirectory) {
        $frameworkDirectory = Get-ChildItem -LiteralPath $libDirectory -Directory | Select-Object -First 1
    }
    if($null -eq $frameworkDirectory) {
        throw "No target-framework folder in $libDirectory"
    }

    $assembly = Get-ChildItem -LiteralPath $frameworkDirectory.FullName -Filter "$PackageId.dll" -File | Select-Object -First 1
    if($null -eq $assembly) {
        $assembly = Get-ChildItem -LiteralPath $frameworkDirectory.FullName -Filter *.dll -File | Select-Object -First 1
    }
    if($null -eq $assembly) {
        throw "No assembly found in $($frameworkDirectory.FullName)"
    }

    return [pscustomobject]@{
        Path = $assembly.FullName
        Framework = $frameworkDirectory.Name
    }
}

function Get-ExportedTypeSafe {
    param([System.Reflection.Assembly]$Assembly)

    try {
        return @($Assembly.GetExportedTypes())
    }
    catch [System.Reflection.ReflectionTypeLoadException] {
        return @($_.Exception.Types | Where-Object { $null -ne $_ -and ($_.IsPublic -or $_.IsNestedPublic) })
    }
}

function Get-SubjectKey {
    param([System.Type]$Type)

    if($null -eq $Type) {
        return '<null>'
    }
    if($Type.IsByRef -or $Type.IsPointer) {
        return Get-SubjectKey -Type $Type.GetElementType()
    }
    if($Type.IsArray) {
        return 'Array'
    }
    if($Type.IsGenericParameter) {
        return '<T>'
    }

    $name = $Type.Name
    if($Type.IsGenericType -and $name -notmatch '`') {
        $name = $Type.GetGenericTypeDefinition().Name
    }
    return $name
}

function Get-SubjectFullName {
    param([System.Type]$Type)

    if($null -eq $Type) {
        return '<null>'
    }
    if($Type.IsGenericType -and -not $Type.IsGenericTypeDefinition) {
        return $Type.GetGenericTypeDefinition().FullName
    }
    return $Type.FullName
}

function Get-VerbName {
    param([System.Type]$Type)

    $names = [System.Collections.Generic.HashSet[string]]::new()
    if($null -eq $Type -or $Type.IsGenericParameter -or $Type.IsByRef -or $Type.IsPointer -or $Type.IsArray) {
        return @()
    }

    try {
        $methods = $Type.GetMethods([System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::Instance)
    }
    catch {
        return @()
    }

    foreach($method in $methods) {
        if($method.IsSpecialName) {
            continue
        }
        if($method.Name -in @('Equals', 'GetHashCode', 'GetType', 'ToString')) {
            continue
        }
        [void]$names.Add($method.Name)
    }

    return @($names | Sort-Object)
}

function Get-ShouldOverload {
    param([System.Reflection.Assembly]$Assembly)

    $overloads = [System.Collections.Generic.List[object]]::new()
    foreach($type in (Get-ExportedTypeSafe -Assembly $Assembly)) {
        try {
            $methods = $type.GetMethods([System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::Static -bor [System.Reflection.BindingFlags]::DeclaredOnly)
        }
        catch {
            continue
        }

        foreach($method in $methods) {
            if($method.Name -ne 'Should') {
                continue
            }
            if($method.ReturnType -eq [void]) {
                continue
            }

            try {
                $parameters = $method.GetParameters()
            }
            catch {
                continue
            }
            if($parameters.Count -eq 0) {
                continue
            }

            $subjectType = $parameters[0].ParameterType
            $assertionType = $method.ReturnType
            if($assertionType.Name -notmatch 'Assertions|Constraint') {
                continue
            }

            $overloads.Add([pscustomobject]@{
                SubjectKey = Get-SubjectKey -Type $subjectType
                SubjectFullName = Get-SubjectFullName -Type $subjectType
                SubjectDisplay = $subjectType.ToString()
                AssertionType = $assertionType.Name
                Verbs = @(Get-VerbName -Type $assertionType)
            })
        }
    }

    return @($overloads)
}

function Get-SubjectSurface {
    param([object[]]$Overloads)

    $overloadItems = if($null -eq $Overloads) { @() } else { @($Overloads) }
    $subjectTable = @{}
    foreach($overload in $overloadItems) {
        if($null -eq $overload.SubjectKey) {
            continue
        }
        if(-not $subjectTable.ContainsKey($overload.SubjectKey)) {
            $subjectTable[$overload.SubjectKey] = [pscustomobject]@{
                Key = $overload.SubjectKey
                FullName = $overload.SubjectFullName
                Display = $overload.SubjectDisplay
                AssertionTypes = [System.Collections.Generic.HashSet[string]]::new()
                Verbs = [System.Collections.Generic.HashSet[string]]::new()
            }
        }

        [void]$subjectTable[$overload.SubjectKey].AssertionTypes.Add($overload.AssertionType)
        foreach($verb in $overload.Verbs) {
            [void]$subjectTable[$overload.SubjectKey].Verbs.Add($verb)
        }
    }

    $subjects = foreach($key in $subjectTable.Keys) {
        [pscustomobject]@{
            Key = $key
            FullName = $subjectTable[$key].FullName
            Display = $subjectTable[$key].Display
            AssertionTypes = @($subjectTable[$key].AssertionTypes | Sort-Object)
            Verbs = @($subjectTable[$key].Verbs | Sort-Object)
        }
    }

    return [pscustomobject]@{
        Subjects = @($subjects | Sort-Object Key)
        AllVerbs = @(($overloadItems | ForEach-Object { $_.Verbs }) | Sort-Object -Unique)
    }
}

function Get-AssemblySurface {
    param([System.Reflection.Assembly]$Assembly)

    $types = Get-ExportedTypeSafe -Assembly $Assembly
    $overloads = Get-ShouldOverload -Assembly $Assembly
    $subjectSurface = Get-SubjectSurface -Overloads $overloads

    return [pscustomobject]@{
        TypesCount = $types.Count
        ShouldOverloads = $overloads.Count
        Subjects = $subjectSurface.Subjects
        AllVerbs = $subjectSurface.AllVerbs
    }
}

function Split-TopLevelParameter {
    param([string]$Text)

    $parts = [System.Collections.Generic.List[string]]::new()
    $depth = 0
    $start = 0
    for($index = 0; $index -lt $Text.Length; $index++) {
        $character = $Text[$index]
        if($character -eq '<' -or $character -eq '[') {
            $depth++
        }
        elseif($character -eq '>' -or $character -eq ']') {
            $depth--
        }
        elseif($character -eq ',' -and $depth -eq 0) {
            $parts.Add($Text.Substring($start, $index - $start).Trim())
            $start = $index + 1
        }
    }
    $parts.Add($Text.Substring($start).Trim())
    return @($parts)
}

function ConvertTo-SubjectKeyFromText {
    param([string]$Text)

    $trimmed = $Text.Trim()
    if($trimmed.EndsWith('[]')) {
        return 'Array'
    }
    if($trimmed -match '^T[A-Za-z0-9]*$' -and $trimmed.Length -le 16) {
        return ('<' + $trimmed + '>')
    }

    $genericStart = $trimmed.IndexOf('<')
    if($genericStart -ge 0) {
        $baseType = $trimmed.Substring(0, $genericStart)
        $argumentsText = $trimmed.Substring($genericStart + 1, $trimmed.Length - $genericStart - 2)
        $arguments = @(Split-TopLevelParameter -Text $argumentsText)
        $baseName = ($baseType -split '\.')[-1]
        return ($baseName + [char]96 + [string]$arguments.Count)
    }

    return (($trimmed -split '\.')[-1])
}

function ConvertTo-TypeNameFromText {
    param([string]$Text)

    $trimmed = $Text.Trim()
    $genericStart = $trimmed.IndexOf('<')
    if($genericStart -ge 0) {
        $trimmed = $trimmed.Substring(0, $genericStart)
    }
    return (($trimmed -split '\.')[-1])
}

function Get-BaselineSurface {
    param([string]$Path)

    Test-FilePresent -Path $Path -Description 'Public-API baseline'
    $lines = Get-Content -LiteralPath $Path
    $typeMethods = @{}
    $typeMethodsBySimpleName = @{}
    $currentType = $null
    $currentSimpleType = $null
    $typesCount = 0

    foreach($line in $lines) {
        if($line -match '^type (?<type>.+)$') {
            $currentType = $Matches['type']
            $currentSimpleType = ConvertTo-TypeNameFromText -Text $currentType
            $typeMethods[$currentType] = [System.Collections.Generic.HashSet[string]]::new()
            if(-not $typeMethodsBySimpleName.ContainsKey($currentSimpleType)) {
                $typeMethodsBySimpleName[$currentSimpleType] = [System.Collections.Generic.HashSet[string]]::new()
            }
            $typesCount++
            continue
        }

        if($null -ne $currentType -and $line -match '^  method (?<signature>.+)$') {
            $signature = $Matches['signature']
            if($signature -match '^(?<return>.+?)\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)\(') {
                [void]$typeMethods[$currentType].Add($Matches['name'])
                [void]$typeMethodsBySimpleName[$currentSimpleType].Add($Matches['name'])
            }
        }
    }

    $overloads = [System.Collections.Generic.List[object]]::new()
    foreach($line in $lines) {
        if($line -notmatch '^  method static (?<return>.+?) Should(?:<(?<generic>[^>]*)>)?\((?<parameters>.+)\)$') {
            continue
        }

        $returnType = $Matches['return'].Trim()
        $parameters = @(Split-TopLevelParameter -Text $Matches['parameters'])
        if($parameters.Count -eq 0) {
            continue
        }

        $assertionType = ConvertTo-TypeNameFromText -Text $returnType
        $verbs = if($typeMethods.ContainsKey($returnType)) {
            @($typeMethods[$returnType])
        }
        elseif($typeMethodsBySimpleName.ContainsKey($assertionType)) {
            @($typeMethodsBySimpleName[$assertionType])
        }
        else {
            @()
        }
        $overloads.Add([pscustomobject]@{
            SubjectKey = ConvertTo-SubjectKeyFromText -Text $parameters[0]
            SubjectFullName = $parameters[0]
            SubjectDisplay = $parameters[0]
            AssertionType = $assertionType
            Verbs = $verbs
        })
    }

    $subjectSurface = Get-SubjectSurface -Overloads $overloads

    return [pscustomobject]@{
        TypesCount = $typesCount
        ShouldOverloads = $overloads.Count
        Subjects = $subjectSurface.Subjects
        AllVerbs = $subjectSurface.AllVerbs
    }
}

function Get-MarkdownTable {
    param(
        [string[]]$Headers,
        [object[]]$Rows
    )

    $lines = [System.Collections.Generic.List[string]]::new()
    $lines.Add('| ' + ($Headers -join ' | ') + ' |')
    $lines.Add('|' + (($Headers | ForEach-Object { '---' }) -join '|') + '|')
    foreach($row in @($Rows)) {
        $cells = foreach($header in $Headers) {
            $value = if($row -is [System.Collections.IDictionary]) { $row[$header] } else { $row.$header }
            if($null -eq $value) { '' } else { [string]$value }
        }
        $lines.Add('| ' + ($cells -join ' | ') + ' |')
    }
    return ($lines -join "`n")
}

function Get-SafeFileName {
    param([string]$Value)
    return ($Value -replace '[^A-Za-z0-9_.-]', '_')
}

try {
    Test-FilePresent -Path $VersionsFile -Description 'Peer versions file'
    $configuration = Get-Content -LiteralPath $VersionsFile -Raw | ConvertFrom-Json
    if($null -eq $configuration.packages) {
        throw "No packages object in $VersionsFile"
    }

    $ourSource = 'baseline'
    if([string]::IsNullOrWhiteSpace($OurAssembly)) {
        Write-Status "Jeninnet source: baseline $baselineFullPath"
        $ourSurface = Get-BaselineSurface -Path $baselineFullPath
    }
    else {
        $assemblyPath = if([System.IO.Path]::IsPathRooted($OurAssembly)) { $OurAssembly } else { Join-Path $repoRoot $OurAssembly }
        Test-FilePresent -Path $assemblyPath -Description 'Jeninnet assembly'
        Write-Status "Jeninnet source: assembly $assemblyPath"
        $ourSurface = Get-AssemblySurface -Assembly ([System.Reflection.Assembly]::LoadFrom($assemblyPath))
        $ourSource = 'assembly'
    }

    $libraries = [ordered]@{}
    foreach($property in $configuration.packages.PSObject.Properties) {
        $packageId = $property.Name
        $version = [string]$property.Value
        Write-Status "Peer: $packageId $version"
        $archive = Get-PackageArchive -PackageId $packageId -Version $version
        $packageDirectory = Join-Path $extractDirectory (Get-SafeFileName "$packageId.$version")
        $null = Expand-PackageArchive -Archive $archive -Destination $packageDirectory
        $metadata = Get-PackageDescription -PackageDirectory $packageDirectory
        $target = Get-TargetAssembly -PackageDirectory $packageDirectory -PackageId $packageId
        $assembly = [System.Reflection.Assembly]::LoadFrom($target.Path)
        $surface = Get-AssemblySurface -Assembly $assembly
        $libraries[$packageId] = [pscustomobject]@{
            Metadata = $metadata
            Framework = $target.Framework
            AssemblyPath = $target.Path
            Surface = $surface
        }
    }

    $probeRows = [System.Collections.Generic.List[object]]::new()
    $probeNotes = [System.Collections.Generic.List[object]]::new()
    $probeResults = [System.Collections.Generic.List[object]]::new()
    foreach($probe in $configuration.probes) {
        $probeKey = ConvertTo-SubjectKeyFromText -Text $probe.fullName
        $row = [ordered]@{ Subject = $probe.name; Key = $probeKey }
        $probeNotes.Add([ordered]@{ Subject = $probe.name; Key = $probeKey; Note = [string]$probe.note })
        $result = [ordered]@{ Name = $probe.name; FullName = $probe.fullName; Key = $probeKey; Libraries = [ordered]@{} }

        $ourSubject = $ourSurface.Subjects | Where-Object { $_.Key -eq $probeKey } | Select-Object -First 1
        $row['Jeninnet'] = if($null -eq $ourSubject) { '&mdash;' } else { "&check; ($($ourSubject.Verbs.Count) verbs)" }
        $result.Libraries['Jeninnet'] = if($null -eq $ourSubject) { $null } else { [pscustomobject]@{ Present = $true; VerbCount = $ourSubject.Verbs.Count; AssertionTypes = $ourSubject.AssertionTypes } }

        foreach($libraryName in $libraries.Keys) {
            $subject = $libraries[$libraryName].Surface.Subjects | Where-Object { $_.Key -eq $probeKey } | Select-Object -First 1
            $row[$libraryName] = if($null -eq $subject) { '&mdash;' } else { "&check; ($($subject.Verbs.Count) verbs)" }
            $result.Libraries[$libraryName] = if($null -eq $subject) { $null } else { [pscustomobject]@{ Present = $true; VerbCount = $subject.Verbs.Count; AssertionTypes = $subject.AssertionTypes } }
        }

        $probeRows.Add($row)
        $probeResults.Add($result)
    }

    $subjectKeys = [System.Collections.Generic.HashSet[string]]::new()
    foreach($subject in $ourSurface.Subjects) { [void]$subjectKeys.Add($subject.Key) }
    foreach($libraryName in $libraries.Keys) {
        foreach($subject in $libraries[$libraryName].Surface.Subjects) { [void]$subjectKeys.Add($subject.Key) }
    }

    $subjectRows = [System.Collections.Generic.List[object]]::new()
    $subjectResults = [System.Collections.Generic.List[object]]::new()
    foreach($key in @($subjectKeys | Sort-Object)) {
        $row = [ordered]@{ Subject = $key }
        $subjectResult = [ordered]@{ Key = $key; Libraries = [ordered]@{} }

        $ourSubject = $ourSurface.Subjects | Where-Object { $_.Key -eq $key } | Select-Object -First 1
        $row['Jeninnet'] = if($null -eq $ourSubject) { '&mdash;' } else { "&check; ($($ourSubject.Verbs.Count))" }
        $subjectResult.Libraries['Jeninnet'] = if($null -eq $ourSubject) { $null } else { [pscustomobject]@{ VerbCount = $ourSubject.Verbs.Count; AssertionTypes = $ourSubject.AssertionTypes } }

        foreach($libraryName in $libraries.Keys) {
            $subject = $libraries[$libraryName].Surface.Subjects | Where-Object { $_.Key -eq $key } | Select-Object -First 1
            $row[$libraryName] = if($null -eq $subject) { '&mdash;' } else { "&check; ($($subject.Verbs.Count))" }
            $subjectResult.Libraries[$libraryName] = if($null -eq $subject) { $null } else { [pscustomobject]@{ VerbCount = $subject.Verbs.Count; AssertionTypes = $subject.AssertionTypes } }
        }

        $subjectRows.Add($row)
        $subjectResults.Add($subjectResult)
    }

    $verbDiffs = [ordered]@{}
    foreach($libraryName in $libraries.Keys) {
        $peerVerbs = $libraries[$libraryName].Surface.AllVerbs
        $missing = @($peerVerbs | Where-Object { $_ -notin $ourSurface.AllVerbs } | Sort-Object)
        $extra = @($ourSurface.AllVerbs | Where-Object { $_ -notin $peerVerbs } | Sort-Object)
        $verbDiffs[$libraryName] = [pscustomobject]@{
            MissingInJeninnet = $missing
            ExtraInJeninnet = $extra
            Shared = @($peerVerbs | Where-Object { $_ -in $ourSurface.AllVerbs } | Sort-Object)
        }
    }

    $metadataRows = [System.Collections.Generic.List[object]]::new()
    foreach($libraryName in $libraries.Keys) {
        $library = $libraries[$libraryName]
        $license = $library.Metadata.License
        if(-not [string]::IsNullOrWhiteSpace($library.Metadata.LicenseFirstLine)) {
            $license = "$license &mdash; $($library.Metadata.LicenseFirstLine)"
        }
        $metadataRows.Add([ordered]@{
            Library = $libraryName
            Version = $library.Metadata.Version
            License = $license
            Dependencies = if($library.Metadata.Dependencies.Count -eq 0) { 'none' } else { ($library.Metadata.Dependencies -join ', ') }
            TFM = $library.Framework
        })
    }

    $generatedAt = (Get-Date).ToUniversalTime().ToString('yyyy-MM-dd HH:mm:ss "UTC"')
    $markdown = [System.Collections.Generic.List[string]]::new()
    $markdown.Add('# Peer parity report')
    $markdown.Add('')
    $markdown.Add("Generated: $generatedAt")
    $markdown.Add('')
    $markdown.Add("Jeninnet source: **$ourSource**")
    if($ourSource -eq 'baseline') {
        $markdown.Add("Baseline: ``$baselineFullPath``")
    }
    else {
        $markdown.Add("Assembly: ``$OurAssembly``")
    }
    $markdown.Add('')
    $markdown.Add('> Measurement only. A missing subject or verb is evidence for a task, not a verdict on quality.')
    $markdown.Add('')
    $markdown.Add('## Surface size')
    $markdown.Add('')
    $markdown.Add('| Library | Exported types | Should overloads | Distinct verbs |')
    $markdown.Add('|---|---:|---:|---:|')
    $markdown.Add("| Jeninnet.Assertions | $($ourSurface.TypesCount) | $($ourSurface.ShouldOverloads) | $($ourSurface.AllVerbs.Count) |")
    foreach($libraryName in $libraries.Keys) {
        $surface = $libraries[$libraryName].Surface
        $markdown.Add("| $libraryName | $($surface.TypesCount) | $($surface.ShouldOverloads) | $($surface.AllVerbs.Count) |")
    }
    $markdown.Add('')
    $markdown.Add('## Pinned packages and licenses')
    $markdown.Add('')
    $markdown.Add((Get-MarkdownTable -Headers @('Library', 'Version', 'License', 'Dependencies', 'TFM') -Rows $metadataRows))
    $markdown.Add('')
    $markdown.Add('Dependency lists come from every nuspec group, not only the reflected TFM; use them as a first signal, not a package-graph audit.')
    $markdown.Add('')
    $markdown.Add('## Probe subjects')
    $markdown.Add('')
    $markdown.Add((Get-MarkdownTable -Headers @('Subject', 'Jeninnet', 'FluentAssertions', 'AwesomeAssertions', 'Shouldly', 'NFluent') -Rows $probeRows))
    $markdown.Add('')
    $markdown.Add('### Probe interpretation')
    $markdown.Add('')
    $markdown.Add((Get-MarkdownTable -Headers @('Subject', 'Key', 'Note') -Rows $probeNotes))
    $markdown.Add('')
    $markdown.Add('Shouldly and NFluent expose different entry points (`ShouldBe...` and `Check.That`), so their `Should()` subject rows are empty by design. The qualitative comparison report covers their surfaces; this harness intentionally measures the `Should()` shape used by Jeninnet, FA, and AA.')
    $markdown.Add('')
    $markdown.Add('## Subject matrix')
    $markdown.Add('')
    $markdown.Add('Cells show `&check; (verb count)` when a `Should` overload accepts the subject. `&mdash;` means no overload was found.')
    $markdown.Add('')
    $markdown.Add((Get-MarkdownTable -Headers @('Subject', 'Jeninnet', 'FluentAssertions', 'AwesomeAssertions', 'Shouldly', 'NFluent') -Rows $subjectRows))
    $markdown.Add('')
    $markdown.Add('## Verb differences (global, by name)')
    $markdown.Add('')
    foreach($libraryName in $libraries.Keys) {
        $diff = $verbDiffs[$libraryName]
        $markdown.Add("### $libraryName")
        $markdown.Add('')
        $markdown.Add("- Missing in Jeninnet: $($diff.MissingInJeninnet.Count)")
        if($diff.MissingInJeninnet.Count -gt 0) {
            $markdown.Add('  ' + (($diff.MissingInJeninnet | Select-Object -First 40) -join ', '))
        }
        $markdown.Add("- Extra in Jeninnet: $($diff.ExtraInJeninnet.Count)")
        if($diff.ExtraInJeninnet.Count -gt 0) {
            $markdown.Add('  ' + (($diff.ExtraInJeninnet | Select-Object -First 40) -join ', '))
        }
        $markdown.Add("- Shared: $($diff.Shared.Count)")
        $markdown.Add('')
    }
    $markdown.Add('## Limitations')
    $markdown.Add('')
    $markdown.Add('- Baseline mode lists verbs declared on the assertion type only; inherited members are not expanded.')
    $markdown.Add('- Reflection mode lists public instance methods, including inherited ones.')
    $markdown.Add('- Generic catch-all `Should<T>(this T)` subjects are grouped under `<T>` and are not a parity signal by themselves.')
    $markdown.Add('- Only static extension `Should` methods are counted; instance entry points such as `TypeSelector.Should()` are not.')
    $markdown.Add('- `Should` overloads returning non-assertion types are ignored.')
    $markdown.Add('- Counts are directional; property accessors and compiler-generated members differ between libraries.')
    $markdown.Add('- Packages are reflected, never executed.')
    $markdown.Add('')

    $reportPath = Join-Path $outputFullPath 'report.md'
    $jsonPath = Join-Path $outputFullPath 'report.json'
    [System.IO.File]::WriteAllText($reportPath, ($markdown -join "`n"), [System.Text.UTF8Encoding]::new($false))

    $json = [ordered]@{
        generatedAt = (Get-Date).ToUniversalTime().ToString('o')
        ourSource = $ourSource
        baseline = if($ourSource -eq 'baseline') { $baselineFullPath } else { $null }
        ourAssembly = if($ourSource -eq 'assembly') { $OurAssembly } else { $null }
        ourSurface = [ordered]@{
            typesCount = $ourSurface.TypesCount
            shouldOverloads = $ourSurface.ShouldOverloads
            distinctVerbs = $ourSurface.AllVerbs.Count
            subjects = $ourSurface.Subjects
        }
        libraries = [ordered]@{}
        probes = $probeResults
        subjects = $subjectResults
        verbDiff = $verbDiffs
    }
    foreach($libraryName in $libraries.Keys) {
        $library = $libraries[$libraryName]
        $json.libraries[$libraryName] = [ordered]@{
            metadata = $library.Metadata
            framework = $library.Framework
            surface = [ordered]@{
                typesCount = $library.Surface.TypesCount
                shouldOverloads = $library.Surface.ShouldOverloads
                distinctVerbs = $library.Surface.AllVerbs.Count
                subjects = $library.Surface.Subjects
            }
        }
    }
    [System.IO.File]::WriteAllText($jsonPath, ($json | ConvertTo-Json -Depth 12), [System.Text.UTF8Encoding]::new($false))

    Write-Information -MessageData '' -InformationAction Continue
    Write-Information -MessageData "Parity report : $reportPath" -InformationAction Continue
    Write-Information -MessageData "Machine report: $jsonPath" -InformationAction Continue
    exit 0
}
catch {
    Write-Error ($_.Exception.Message + [Environment]::NewLine + $_.ScriptStackTrace)
    exit 2
}
