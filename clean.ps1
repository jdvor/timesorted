#!/usr/bin/env pwsh

param (
    [switch] $IDE,
    [switch] $Git
)

Get-ChildItem .\ -Include bin,obj,TestResults,publish,coverage.json -Recurse | ForEach-Object ($_) { Remove-Item $_.FullName -Force -Recurse }

if ($IDE -eq $true) {
    if (Test-Path .vs) {
        $null = Remove-Item .vs -Force -Recurse
    }
    if (Test-Path .idea) {
        $null = Remove-Item .idea -Force -Recurse
    }
}

if (($Git -eq $true) -and (Test-Path .git)) {
    $null = Remove-Item .git -Force -Recurse
}
