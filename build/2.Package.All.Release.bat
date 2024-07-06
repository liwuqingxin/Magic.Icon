%~dp0

rd/s/q ..\_output\

cd ..\src\

dotnet msbuild .\Nlnet.Sharp.Iconfont.Generator.sln /p:Configuration=Release

pause