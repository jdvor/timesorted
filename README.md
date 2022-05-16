# timesorted

[![Tests](https://github.com/jdvor/timesorted/actions/workflows/test.yml/badge.svg?branch=main)](https://github.com/jdvor/timesorted/actions/workflows/test.yml)

**Run tests**
```shell
dotnet test -v minimal --nologo
```

**Build strict**
(code analysis ON, warnings as errors)
```shell
dotnet clean -c Release -v quiet --nologo
dotnet build -c Release -p:TreatWarningsAsErrors=True --nologo -clp:NoSummary
```

**Build permissive**
(code analysis OFF)
```shell
dotnet clean -c Release -v quiet --nologo
dotnet build -c Release -p:RunAnalyzers=False --nologo
```

**Create NuGet package - strict**
```shell
./pack.sh -v 1.0.1
```


```shell
# https://github.com/coverlet-coverage/coverlet
dotnet tool install -g coverlet.console

# https://github.com/danielpalme/ReportGenerator
dotnet tool install -g dotnet-reportgenerator-globaltool

dotnet build
coverlet .\test\TimeSorted.Tests\bin\Debug\net6.0\TimeSorted.Tests.dll -t dotnet -a "test --no-build" --exclude "[*]namespace*"

dotnet test --collect:"XPlat Code Coverage" --results-directory coverage
reportgenerator -reports:coverage/**/coverage.cobertura.xml -targetdir:coverage/report -reporttypes:HtmlInline
```
