# timesorted

**Run tests**
```shell
dotnet test --blame-hang-timeout 10m --nologo
```

**Build strict**
(code analysis ON, warnings as errors)
```shell
dotnet clean -c Release -v quiet --nologo
dotnet build -c Release /p:RunAnalyzers=True /p:TreatWarningsAsErrors=True --nologo
```

**Create NuGet package - strict**
```shell
./pack.ps1 -VersionPrefix 1.0.1 
```

**Create NuGet package - permissive**
```shell
./pack.ps1 -VersionPrefix 1.0.1 -SkipAnalysis -AllowWarnings
```


```shell
# https://github.com/coverlet-coverage/coverlet
dotnet tool install -g coverlet.console

# https://github.com/danielpalme/ReportGenerator
dotnet tool install -g dotnet-reportgenerator-globaltool

dotnet build
coverlet .\test\Skeleton.Library.Tests\bin\Debug\net6.0\Skeleton.Library.Tests.dll -t dotnet -a "test --no-build" --exclude "[*]namespace*"

dotnet test --collect:"XPlat Code Coverage"

reportgenerator --reports:"./TestResults/../coverage.cobertura.xml" --targetdir:"" --reporttypes:Html
```
