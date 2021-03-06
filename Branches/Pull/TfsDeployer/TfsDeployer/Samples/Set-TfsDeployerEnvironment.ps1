<#
.SYNOPSIS
    Initializes the same set of $TfsDeployer* global variables exposed to deployment scripts by TFS Deployer using
    the most recent good build for a specified build definition name.

.PARAMETER CollectionUri
    The uri of the TFS Team Project Collection containing the build to use for populating the global variables.
    
.PARAMETER TeamProjectName
    The name of the TFS Team Project within the specified collection.
    
.PARAMETER BuildDefinitionName
    The name of the TFS Build Definition which will provide the last good build data.
    
#>

#requires -version 2.0
param (
    [parameter(Mandatory=$true)]
    [string]
    $CollectionUri,
    
    [parameter(Mandatory=$true)]
    [string]
    $TeamProjectName,

    [parameter(Mandatory=$true)]
    [string]
    $BuildDefinitionName,
    
    [string]
    $OriginalQuality = '',
    
    [string]
    $NewQuality = 'Released'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

'Microsoft.TeamFoundation.Client',
'Microsoft.TeamFoundation.Build.Client' |
    ForEach-Object {
        Add-Type -AssemblyName "$_, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    }
    
$Collection = New-Object -Type Microsoft.TeamFoundation.Client.TfsTeamProjectCollection -ArgumentList $CollectionUri
$Collection.EnsureAuthenticated()

$BuildServer = $Collection.GetService([Microsoft.TeamFoundation.Build.Client.IBuildServer])
$Definition = $BuildServer.GetBuildDefinition($TeamProjectName, $BuildDefinitionName)

if (-not $Definition.LastGoodBuildUri) {
    throw "Could not find last good build for build definition $BuildDefinitionName"
}

$global:TfsDeployerComputer = $Env:COMPUTERNAME
$global:TfsDeployerOriginalQuality = $OriginalQuality
$global:TfsDeployerNewQuality = $NewQuality

$DeployerProcess = $Definition.Process | Select-Object -Property Description, Parameters, ServerPath, TeamProject
$DeployerDef = $Definition | Select-Object -Property DefaultDropLocation, Description, Enabled, Id, LastBuildUri,
    LastGoodBuildLabel, LastGoodBuildUri, Name, ProcessParameters, @{N='Process'; E={$DeployerProcess}}

$global:TfsDeployerBuildDetail = $BuildServer.GetBuild($Definition.LastGoodBuildUri) | 
    Select-Object -Property BuildControllerUri, BuildDefinitionUri, BuildFinished, BuildNumber,
        DropLocation, DropLocationRoot, FinishTime, IsDeleted, KeepForever, LabelName, LastChangedBy,
        LastChangedOn, LogLocation, ProcessParameters, Quality, RequestedBy, RequestedFor, ShelvesetName,
        SourceGetVersion, StartTime, Status, TeamProject, Uri, @{N='BuildDefinition';E={$DeployerDef}}
