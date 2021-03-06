#requires -version 2.0

[CmdletBinding()]
param (
    [string]
    $ProjectName = 'MyTeamProject',
    
    [string]
    $DefaultDropLocation = ('\\{0}\drops' -f [Environment]::MachineName)
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$PSScriptRoot = ($MyInvocation.MyCommand.Path | Split-Path | Resolve-Path).ProviderPath

$TestModulesPath = $PSScriptRoot | Join-Path -ChildPath Modules
if ($Env:PSModulePath -notlike "$TestModulesPath;*") {
    $Env:PSModulePath = "$TestModulesPath;$Env:PSModulePath"
}

Import-Module -Name CLR4PowerShell
Import-Module -Name TFS

'','.Client','.VersionControl.Client','.Build.Client' | % {
    Add-Type -AssemblyName "Microsoft.TeamFoundation$_, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
}

$DeployerPath = $PSScriptRoot | Join-Path -ChildPath ..\output\TfsDeployer
$SolutionRootSourcePath = $PSScriptRoot | Join-Path -ChildPath Source\MySolution

[xml]$DeployerConfig = Get-Content -Path $DeployerPath\TfsDeployer.exe.config
$CollectionUri = $DeployerConfig.configuration.applicationSettings.SelectSingleNode("//setting[@name='TeamProjectCollectionUri']/value").'#text'

Write-Host "Connecting to project collection..."
$Collection = New-Object -TypeName Microsoft.TeamFoundation.Client.TfsTeamProjectCollection -ArgumentList $CollectionUri 
$Collection.EnsureAuthenticated()

Write-Host "Querying team projects..."
$StructureService = $Collection.GetService([Microsoft.TeamFoundation.Server.ICommonStructureService3])
if (-not $StructureService.GetProjectFromName($ProjectName)) {
    throw "Could not find Team Project '$ProjectName'"
}

Write-Host "Configuring notification delay..."
$Result = Set-TfsNotificationJobDelay -Url $CollectionUri -DelaySeconds 10
if ($Result.RestartRequired) {
    throw "Please restart the TFS AT application pool to allow the new notification delay to take effect."
}

Write-Host "Connecting to version control service..."
$VersionControl = $Collection.GetService([Microsoft.TeamFoundation.VersionControl.Client.VersionControlServer])

Write-Host "Creating a workspace..."
$SourceRootPath = '$/{0}' -f $ProjectName
$SolutionRootPath = '{0}/MySolution' -f $SourceRootPath
$WorkingPath = $Env:TEMP | Join-Path -ChildPath ([Guid]::NewGuid())
New-Item -Path $WorkingPath -ItemType Container | Out-Null
$Workspace = $VersionControl.CreateWorkspace([Guid]::NewGuid())
$Workspace.Map($SourceRootPath, $WorkingPath)

if ($VersionControl.ServerItemExists($SolutionRootPath, 'Folder')) {
    Write-Host "Deleting old source files..."
    $GetRequest = New-Object -TypeName Microsoft.TeamFoundation.VersionControl.Client.GetRequest -ArgumentList ($SolutionRootPath, 'None', [Microsoft.TeamFoundation.VersionControl.Client.VersionSpec]::Latest)
    $Workspace.Get($GetRequest , 'None') | Out-Null
    $Workspace.PendDelete($SolutionRootPath, 'Full') | Out-Null
    $Workspace.CheckIn($Workspace.GetPendingChanges(), "Deleted old source files. $($MyInvocation.MyCommand)") | Out-Null
}

Write-Host "Copying new source files..."
$LocalSolutionRootPath = $Workspace.GetLocalItemForServerItem($SolutionRootPath)
Copy-Item -Path $SolutionRootSourcePath -Destination $LocalSolutionRootPath -Recurse

Write-Host "Customising source files..."
$DeploymentMappingsPath = $LocalSolutionRootPath | Join-Path -ChildPath Build\Deployment\DeploymentMappings.xml
$DeploymentMappings = [xml](Get-Content -Path $DeploymentMappingsPath)
$DeploymentMappings.DeploymentMappings.Mapping | ForEach-Object {
    $_.Computer = [Environment]::MachineName
}
$DeploymentMappings.Save($DeploymentMappingsPath)

$NewQuality = $DeploymentMappings.DeploymentMappings.Mapping | 
    Where-Object { $_.OriginalQuality -eq '*' } |
    Select-Object -ExpandProperty NewQuality -First 1

Write-Host "Checking in new source files..."
$Workspace.PendAdd($LocalSolutionRootPath, $true) | Out-Null
$Workspace.CheckIn($Workspace.GetPendingChanges(), "Added new source files. $($MyInvocation.MyCommand)") | Out-Null

Write-Host "Removing workspace..."
$Workspace.Delete() | Out-Null
Remove-Item -Path $WorkingPath -Recurse -Force

Write-Host "Connecting to build service..."
$BuildServer = $Collection.GetService([Microsoft.TeamFoundation.Build.Client.IBuildServer])

Write-Host "Querying build controllers..."
$Controllers = $BuildServer.QueryBuildControllers()
if (-not $Controllers) {
    throw "Could not find a Build Controller for Collection '$CollectionUri'"
}

Write-Host "Querying build qualities..."
if ($BuildServer.GetBuildQualities($ProjectName) -notcontains $NewQuality) {
    Write-Host "Adding build quality..."
    $BuildServer.AddBuildQuality($ProjectName, $NewQuality)
}

Write-Host "Configuring build process template..."
$BuildProcessTemplatePath = '{0}/Build/DefaultTemplate.xaml' -f $SolutionRootPath
$BuildProcessTemplate = $BuildServer.QueryProcessTemplates($ProjectName) | 
    Where-Object { $_.ServerPath -eq $BuildProcessTemplatePath } |
    Select-Object -First 1
if (-not $BuildProcessTemplate) {
    $BuildProcessTemplate = $BuildServer.CreateProcessTemplate($ProjectName, $BuildProcessTemplatePath)
    $BuildProcessTemplate.Save()
}    

Write-Host "Configuring build definition..."
$BuildDefName = 'MyBuild'
try { $BuildDef = $BuildServer.GetBuildDefinition($ProjectName, $BuildDefName) } catch { $BuildDef = $null }
if (-not $BuildDef) {
    $BuildDef = $BuildServer.CreateBuildDefinition($ProjectName)
    $BuildDef.Name = $BuildDefName
}
$BuildDef.BuildController = $Controllers | Select-Object -First 1
$BuildDef.ContinuousIntegrationType = 'None'
$BuildDef.DefaultDropLocation = $DefaultDropLocation | Join-Path -ChildPath $ProjectName
$BuildDef.Enabled = $true
$BuildDef.Process = $BuildProcessTemplate 


$BuildDef.ProcessParameters = Invoke-CLR4PowerShellCommand -ScriptBlock {
    param (
        [string]
        $ProcessParameters,
        [string]
        $ProjectsToBuild
    )
    
    Add-Type -AssemblyName "Microsoft.TeamFoundation.Build.Workflow, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"

    $Dict = [Microsoft.TeamFoundation.Build.Workflow.WorkflowHelpers]::DeserializeProcessParameters($ProcessParameters)
    $BuildSettingsKey = [Microsoft.TeamFoundation.Build.Workflow.ProcessParameterMetadata+StandardParameterNames]::BuildSettings
    if (-not $Dict.ContainsKey($BuildSettingsKey)) {
        $NewBuildSettings = New-Object -TypeName Microsoft.TeamFoundation.Build.Workflow.Activities.BuildSettings
        $Dict[$BuildSettingsKey] = [Microsoft.TeamFoundation.Build.Workflow.Activities.BuildSettings]$NewBuildSettings 
    }

    $BuildSettings = $Dict[$BuildSettingsKey]
    $BuildSettings.ProjectsToBuild = $ProjectsToBuild
    #$BuildSettings.PlatformConfigurations | Out-Null
    return [Microsoft.TeamFoundation.Build.Workflow.WorkflowHelpers]::SerializeProcessParameters($Dict)

} -ArgumentList $BuildDef.ProcessParameters, ('{0}/MySolution.sln' -f $SolutionRootPath)

$BuildDef.Workspace.Mappings.Clear()
$BuildDef.Workspace.Map($SolutionRootPath, '$(SourceDir)') | Out-Null
$BuildDef.Save()

Write-Host "Queuing build..."
$QueuedBuild = $BuildServer.QueueBuild($BuildDef)
Write-Host "Waiting for build to start..."
$QueuedBuild.WaitForBuildStart()
$Build = $QueuedBuild.Build
Write-Host "Waiting for build to finish..."
while (-not $Build.BuildFinished) {
    Start-Sleep -Seconds 3
    $Build.RefreshMinimalDetails()
}
if ($Build.Status -ne 'Succeeded') {
    throw "Build '$($Build.BuildNumber)' was not successful"
}


Write-Host "Starting Deployer..."
$DeployerProcess = Start-Process -FilePath $DeployerPath\TfsDeployer.exe -ArgumentList '-d' -PassThru
Start-Sleep -Seconds 10

$LogPath = $Env:TEMP | Join-Path -ChildPath ('{0}.log' -f $Build.BuildNumber)
if (Test-Path -Path $LogPath) {
    Remove-Item -Path $LogPath
}

Write-Host "Changing build quality..."
$Build.Quality = $NewQuality
$Build.Save()

Write-Host "Waiting for Deployer to respond to quality change event..."
$Expiry = (Get-Date).AddMinutes(1)
while (-not (Test-Path -Path $LogPath) -and (Get-Date) -lt $Expiry) {
    Start-Sleep -Seconds 3
}
if (Test-Path -Path $LogPath) {
    Write-Host "Deployer processed change event"
    Get-Content -Path $LogPath
}

Write-Host "Terminating Deployer..."
$DeployerProcess | Stop-Process