@echo off
set collection=%1
set username=%2

set regarch=
if /i [%processor_architecture%] equ [amd64] set regarch=Wow6432Node\
for /f "tokens=3*" %%i in ('reg query "HKLM\Software\%regarch%Microsoft\VisualStudio\10.0" /v InstallDir') do set vspath=%%i %%j

"%vspath%\tfssecurity.exe" /a+ EventSubscription $SUBSCRIPTION: CREATE_SOAP_SUBSCRIPTION n:%username% ALLOW /collection:%collection%
