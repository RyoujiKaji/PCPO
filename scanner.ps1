$sonarHostUrl = "http://localhost:9000"
$sonarToken = "sqp_4833b8330d3266fe267d02e78d4e11b48fc529bd"
$baseDir = Get-Location

dotnet tool install --tool-path .tools dotnet-sonarscanner

.tools\dotnet-sonarscanner begin `
    /key:lb1 `
    /d:sonar.host.url=$sonarHostUrl `
    /d:sonar.login=$sonarToken `
    /d:sonar.qualitygate.wait=false `
    /d:sonar.sources=.

dotnet build 

.tools\dotnet-sonarscanner end `
    /d:sonar.login=$sonarToken
