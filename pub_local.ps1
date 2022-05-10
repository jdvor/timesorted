function Get-NugetLocalFeedPath {
    if (Test-Path 'D:\LocalNugetPackages') {
        return 'D:\LocalNugetPackages';
    }
    if (Test-Path 'C:\LocalNugetPackages') {
        return 'C:\LocalNugetPackages';
    }
    if (Test-Path 'E:\LocalNugetPackages') {
        return 'E:\LocalNugetPackages';
    }
    $nlfd = Join-Path $env:TEMP 'LocalNugetPackages'
    if (-not (Test-Path $nlfd)) {
        New-Item -ItemType Directory -Path $nlfd
    }
    return $nlfd
}

$NugetPackagesRoot = (Invoke-Expression 'nuget locals global-packages -list' | Out-String).Replace('global-packages:', '').Trim()
$NugetLocalFeed = Get-NugetLocalFeedPath

Push-Location (Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)

[regex]$rx = '^(?<name>[^\d]+)\.(?<ver>\d+(\.\d+)+)\.nupkg$'
Get-ChildItem ./publish -Include '*.nupkg' -Recurse | ForEach-Object {
    $nugetFilename = Split-Path $_.FullName -Leaf
    $match = $rx.Match($nugetFilename)
    $name = $match.Groups['name'].Value
    $ver = $match.Groups['ver'].Value
    (& nuget delete $name $ver -Source $NugetPackagesRoot -NonInteractive -Verbosity quiet) | Out-Null
    (& nuget delete $name $ver -Source $NugetLocalFeed -NonInteractive -Verbosity quiet) | Out-Null
    & nuget add $_.FullName -Source $NugetLocalFeed -NonInteractive
}

Pop-Location
