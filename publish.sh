#!/usr/bin/env bash

[[ -z "$1" ]] && apikey=$NUGET_APIKEY_TIMESORTED || apikey=$1

if [ -z "$apikey" ]; then
    echo "No API key provided."
    exit 1
fi

shopt -s nullglob
for file in ./publish/*.{nupkg,snupkg} ; do
    dotnet nuget push "$file" -k "$apikey" -s https://api.nuget.org/v3/index.json --skip-duplicate
done
shopt -u nullglob
