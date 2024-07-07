%~dp0

rd/s/q ..\_output\

cd ..\src\

dotnet msbuild .\Nlnet.Sharp.Magic.Icon.sln /p:Configuration=Release

pause