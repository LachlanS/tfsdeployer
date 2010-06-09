#requires -version 2.0
param (
	[switch]
	$Test,
	
	[switch]
	$Release,

	[switch]
	$Clean
)

$ErrorActionPreference = 'Stop'
Set-PSDebug -Strict

$PSScriptRoot = $MyInvocation.MyCommand.Path | Resolve-Path | Split-Path

$Target = 'Build'
if ($Test) { $Target = 'Test' }
if ($Clean) { $Target = 'Clean,' + $Target }

$Configuration = 'Debug'
if ($Release) { $Configuration = 'Release' }

$Node = ''
if ([IntPtr]::Size -ne 4) { $Node = 'Wow6432Node' }
$MSBuildToolsPath = (Get-ItemProperty -Path HKLM:\SOFTWARE\$Node\Microsoft\MSBuild\ToolsVersions\4.0).MSBuildToolsPath
$MSBuildExe = Join-Path -Path $MSBuildToolsPath -ChildPath MSBuild.exe

$TfsDeployerMSBuild = Join-Path -Path $PSScriptRoot -ChildPath TfsDeployer.msbuild

& $MSBuildExe $TfsDeployerMSBuild /p:Configuration=$Configuration /p:Platform=x86 /t:$Target
