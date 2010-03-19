@echo off
setlocal
if NOT "%1" == "" set target=/t:%1

"%windir%\Microsoft.NET\Framework\v4.0.30128\MSBuild.exe" TfsDeployer.msbuild /p:Configuration=Debug %target%
