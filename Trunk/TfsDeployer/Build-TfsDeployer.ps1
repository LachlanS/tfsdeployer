#requires -version 2.0
[CmdletBinding(DefaultParameterSetName='Debug')]
param (
    [parameter(ParameterSetName='Debug')]
	[switch]
	$Test,
	
    [parameter(ParameterSetName='Debug')]
	[switch]
	$Clean,

    [parameter(ParameterSetName='Release', Mandatory=$true)]
    [ValidatePattern('\d+\.\d+\.\d+\.\d+')]
	[string]
	$ReleaseVersion

)

$ErrorActionPreference = 'Stop'
Set-PSDebug -Strict

$PSScriptRoot = $MyInvocation.MyCommand.Path | Split-Path -Resolve

$Release = $PSCmdlet.ParameterSetName -eq 'Release'

$Target = 'Build'
if ($Release -or $Test) { $Target = 'Test' }
if ($Release -or $Clean) { $Target = 'Clean,' + $Target }

$Configuration = 'Debug'
if ($Release) { 
    $Configuration = 'Release' 
    $SolutionInfoPath = $PSScriptRoot | Join-Path -ChildPath SolutionInfo.cs
    $SolutionInfoItem = Get-Item -Path $SolutionInfoPath
    if ($SolutionInfoItem.IsReadOnly) {
        $SolutionInfoItem.IsReadOnly = $false 
    }
    (Get-Content -Path $SolutionInfoPath) `
        -replace 'Version\("[^"]+"\)', "Version(`"$ReleaseVersion`")" |
        Set-Content -Path $SolutionInfoPath -Encoding UTF8
}

$Node = ''
if ([IntPtr]::Size -ne 4) { $Node = 'Wow6432Node' }
$MSBuildToolsPath = (Get-ItemProperty -Path HKLM:\SOFTWARE\$Node\Microsoft\MSBuild\ToolsVersions\4.0).MSBuildToolsPath
$MSBuildExe = Join-Path -Path $MSBuildToolsPath -ChildPath MSBuild.exe

$TfsDeployerMSBuild = Join-Path -Path $PSScriptRoot -ChildPath TfsDeployer.msbuild

& $MSBuildExe $TfsDeployerMSBuild /p:Configuration=$Configuration /p:Platform="Any CPU" /t:$Target
