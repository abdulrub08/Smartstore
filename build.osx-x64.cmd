REM Powershell -NoProfile -ExecutionPolicy Bypass -File "build.ps1"
dotnet cake --target=Deploy --runtime=osx-x64
dotnet cake --target=Zip --runtime=osx-x64
pause