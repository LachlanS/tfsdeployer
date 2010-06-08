#requires -version 2.0
param (
	[switch]
	$Test,
	
	[switch]
	$Release
)

$ErrorActionPreference = 'Stop'
Set-PSDebug -Strict

$PSScriptRoot = $MyInvocation.MyCommand.Path | Resolve-Path | Split-Path

$Target = 'Build'
if ($Test) { $Target = 'Test' }

$Configuration = 'Debug'
if ($Release) { $Configuration = 'Release' }

$MSBuildToolsPath = (Get-ItemProperty -Path HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0).MSBuildToolsPath
$MSBuildExe = Join-Path -Path $MSBuildToolsPath -ChildPath MSBuild.exe

$TfsDeployerMSBuild = Join-Path -Path $PSScriptRoot -ChildPath TfsDeployer.msbuild

& $MSBuildExe $TfsDeployerMSBuild /p:Configuration=$Configuration /t:$Target
