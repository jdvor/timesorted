# TimeSorted library

Time-sortable efficient value types.

* **TimeSorted.Suid** - globally unique identifier very similar to System.Guid, but its bytes and text representation is sortable by time. Optionally, a 1 byte tag can be defined for the identifier, which could be used for determining what type of entity this identifier represents or what type of source has generated it.

## Instalation

nuget package [TimeSorted](https://www.nuget.org/packages/TimeSorted)


```shell
dotnet add package TimeSorted [ -v 1.0.0 ]
```

## Badges

[![Tests](https://github.com/jdvor/timesorted/actions/workflows/test.yml/badge.svg?branch=main)](https://github.com/jdvor/timesorted/actions/workflows/test.yml)

## Useful commands (for contributors)

### Run tests
```shell
dotnet test -v minimal --nologo
```

### Build strict
(code analysis ON, warnings as errors)
```shell
dotnet clean -c Release -v quiet --nologo
dotnet build -c Release -v minimal -p:TreatWarningsAsErrors=True --nologo -clp:NoSummary
```

### Build permissive
(code analysis OFF)
```shell
dotnet clean -c Release -v quiet --nologo
dotnet build -c Release -v minimal -p:RunAnalyzers=False --nologo
```

### Tag version
```shell
git tag -a "2.0.0" -m "version 2.0.0" [ commit ]
```

### Create NuGet package (CI variant)
```shell
./pack.sh -c
```

### Create NuGet package (local development)
```shell
./pack.sh [ -v {version_prefix} ] [ -s {version_suffix} ] [ -p {nuget_package_cache} ]
```

### Publish NuGet (local development)
```shell
./pack.sh [ -v {version_prefix} ] [ -s {version_suffix} ] [ -p {nuget_package_cache} ]
./publish.sh [ {nuget_api_key} ]
```

### Test coverage & report
```shell
# https://github.com/coverlet-coverage/coverlet
dotnet tool install -g coverlet.console

# https://github.com/danielpalme/ReportGenerator
dotnet tool install -g dotnet-reportgenerator-globaltool

dotnet test --collect:"XPlat Code Coverage" --results-directory publish/coverage
reportgenerator -reports:publish/coverage/**/coverage.cobertura.xml -targetdir:publish/report -reporttypes:HtmlInline
```

Then you can find results in `./publish/report` directory.

### Run basic benchmarks and output reports to publish directory
```shell
dotnet publish bench/TimeSorted.Benchmarks/TimeSorted.Benchmarks.csproj -c Release -p:RunAnalyzers=False -o ./publish/bench -v minimal --nologo
./publish/bench/TimeSorted.Benchmarks -a publish -e GitHub -f TimeSorted.Benchmarks.*
```

Then you can find results in `./publish/results` directory.
