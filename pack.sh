#!/usr/bin/env bash

pkgpath_raw=$(nuget locals global-packages -list)
git_branch=$(git rev-parse --abbrev-ref HEAD)

version="1.0.0"
strict="True"
pkgpath="${pkgpath_raw/global-packages: /}"
suffix=""
if [ "$git_branch" != "main" ]; then
    suffix=$(echo "$git_branch" | tr "[:upper:]" "[:lower:]" | sed s/[^a-z0-9]//g)
fi

while [[ "$#" -gt 0 ]]; do
    case $1 in
        -v|--version) version="$2"; shift ;;
        -s|--suffix) suffix="$2"; shift ;;
        -p|--packages) pkgpath="$2"; shift ;;
        -f|--forced) strict="False" ;;
        *) echo "Unknown parameter passed: $1"; exit 1 ;;
    esac
    shift
done

[[ -z "$suffix" ]] && fullversion=$version || fullversion="$version-$suffix"

echo "version: $fullversion"
echo "strict: $strict"
echo "nuget packages: $pkgpath"
echo

rm -rf ./publish
dotnet clean src/TimeSorted/TimeSorted.csproj -c Release -v quiet --nologo
dotnet pack src/TimeSorted/TimeSorted.csproj -c Release -o ./publish --nologo \
    -p:VersionPrefix="$version" \
    -p:VersionSuffix="$suffix" \
    -p:RestorePackagesPath="$pkgpath" \
    -p:TreatWarningsAsErrors=$strict \
    -p:RunAnalyzers=$strict
