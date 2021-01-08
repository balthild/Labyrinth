Push-Location $PSScriptRoot/../clashffi
go build -buildmode=c-shared -o ../Labyrinth/Libs/clashffi.dll
strip -s ../Labyrinth/Libs/clashffi.dll
Pop-Location
