@echo off
setlocal
if NOT "%1" == "" set target=/t:%1

"%windir%\Microsoft.NET\Framework\v3.5\MSBuild.exe" TfsDeployer.msbuild /p:Configuration=Debug %target%
