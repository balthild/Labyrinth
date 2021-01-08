Push-Location $PSScriptRoot/..
Remove-Item -Recurse -Force dist/win64
dotnet clean Labyrinth
dotnet publish Labyrinth -r win-x64 -c Release --self-contained -o dist/win64 /p:PublishSingleFile=true /p:PublishTrimmed=true
Pop-Location
