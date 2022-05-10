#!/usr/bin/env pwsh

param (
    [ValidatePattern('^\d{1,5}(\.\d{1,5}){2,3}$')][string] $VersionPrefix = '1.0.0.0',
    [string] $Suffix,
    [ValidateSet('Debug', 'Release')][string] $Configuration = 'Release',
    [string] $PublishDir,
    [string] $ForceNugetPackagesRoot,
    [switch] $SkipAnalysis,
    [switch] $AllowWarnings
)

$ErrorActionPreference = "Stop"

function Get-VersionSuffix {
    param ([string] $Suffix)
    if ([string]::IsNullOrEmpty($Suffix)) {
        $bn = ((Invoke-Expression 'git rev-parse --abbrev-ref HEAD') | Out-String).Trim()
    } else {
        $bn = $Suffix
    }
    if ([string]::IsNullOrEmpty($bn) -or ($bn -eq 'master') -or ($bn -eq 'main') -or ($bn -eq 'HEAD')) {
        return ''
    }
    $bn = $bn.Trim()
    if ($bn.Length -gt 30) {
        $bn = $bn.Substring(0, 30);
    }
    $bn = $bn -replace '[^\w\-]',''
    $bn = $bn.Replace('_', '-');
    return $bn.ToLower()
}

Push-Location (Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)

$VersionSuffix = Get-VersionSuffix $Suffix
$Version = if ($VersionSuffix -eq '') { $VersionPrefix } else { "$VersionPrefix-$VersionSuffix" }
$PublishDir = if ($PublishDir -eq '') { './publish/' } else { $PublishDir }
$PublishDir = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($PublishDir)
$CodeAnalysis = if ($SkipAnalysis) {'False'} else {'True'}
$Warnings = if ($AllowWarnings) {'False'} else {'True'}
if ($ForceNugetPackagesRoot -and ($ForceNugetPackagesRoot -ne '')) {
    $NugetPackagesRoot = $ForceNugetPackagesRoot
} else {
    $nugetLocalsStdout = (Invoke-Expression 'nuget locals global-packages -list' | Out-String)
    $NugetPackagesRoot = $nugetLocalsStdout.Replace('global-packages:', '').Trim()
}

Write-Host "Version: $Version" -ForegroundColor DarkGray
Write-Host "PublishDir: $PublishDir" -ForegroundColor DarkGray
Write-Host "CodeAnalysis: $CodeAnalysis" -ForegroundColor DarkGray
Write-Host "Warnings: $Warnings" -ForegroundColor DarkGray
Write-Host "NugetPackagesRoot: $NugetPackagesRoot" -ForegroundColor DarkGray

# clean up
& dotnet clean -c $Configuration -v quiet --nologo
if (Test-Path $PublishDir) {
    $null = Remove-Item -Path (Join-Path $PublishDir '*') -Recurse -Force
}

exit

# patch assemblies with date & commit configuration attribute
$commit = (& git rev-parse HEAD)
if (($LastExitCode -eq 0) -and ($commit -ne '')) {
    $date = Get-Date -Format 'yyyy-MM-ddTHH:mm:ssK'
    $content = "[assembly: System.Reflection.AssemblyConfiguration(""$commit|$date"")]"
    Get-ChildItem './src' -Include '*.csproj' -Recurse | ForEach-Object {
        $file = Join-Path (Split-Path $_.FullName) 'AssemblyInfoGenerated.cs'
        $content | Set-Content -Path $file -Force
    }
}

# build & pack
& dotnet pack -c $Configuration -o ./publish --nologo `
    /p:VersionPrefix=$VersionPrefix `
    /p:VersionSuffix=$VersionSuffix `
    /p:RestorePackagesPath=$NugetPackagesRoot `
    /p:TreatWarningsAsErrors=$Warnings `
    /p:RunAnalyzers=$CodeAnalysis

if ($LastExitCode -eq 0) {
    Write-Host "[SUCCESS]" -ForegroundColor Green
}

Pop-Location
