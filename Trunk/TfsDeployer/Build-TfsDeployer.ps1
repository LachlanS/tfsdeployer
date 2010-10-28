#requires -version 2.0
[CmdletBinding()]
param (
    [ValidateSet('Alpha', 'Beta', 'Stable')]
    [string]
    $Publish,
    
    [switch]
    $Force
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$PSScriptRoot = $MyInvocation.MyCommand.Path | Split-Path -Resolve

$Configuration = 'Debug'
if ($Publish) {
    $Configuration = 'Release' 

    Write-Output 'Connecting to TFS workspace'
    [Reflection.Assembly]::Load('Microsoft.TeamFoundation.VersionControl.Client, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a') | Out-Null
    $WorkstationType = [Microsoft.TeamFoundation.VersionControl.Client.Workstation]
    $WorkspaceInfo = $WorkstationType::Current.GetLocalWorkspaceInfo($PSScriptRoot)
    $Collection = [Microsoft.TeamFoundation.Client.TfsTeamProjectCollectionFactory]::GetTeamProjectCollection($WorkspaceInfo.ServerUri)
    $Workspace = $WorkspaceInfo.GetWorkspace($Collection)

    Write-Output 'Calculating latest changeset number'
    if ($Workspace.GetPendingChanges($PSScriptRoot, 'Full', $false)) {
        $Message = 'Commit or undo pending changes before building a release.'
        if ($Force) {
            Write-Warning $Message
        } else {
            throw $Message
        }
    }
    $ItemSpecType = [Microsoft.TeamFoundation.VersionControl.Client.ItemSpec]
    $ItemSpec = New-Object -TypeName $ItemSpecType -ArgumentList $PSScriptRoot,'Full'
    $Changeset = ($Workspace.GetLocalVersions(@($ItemSpec), $false)[0] |
        Measure-Object -Property Version -Maximum).Maximum
    $ReleaseVersion = "1.3.0.$Changeset"

    Write-Output 'Writing version number to assembly attributes'
    $SolutionInfoPath = $PSScriptRoot | Join-Path -ChildPath SolutionInfo.cs
    $UndoSolutionInfo = $Workspace.GetPendingChanges($SolutionInfoPath).Length -eq 0
    $Workspace.PendEdit($SolutionInfoPath) | Out-Null
    (Get-Content -Path $SolutionInfoPath) `
        -replace 'Version\("[^"]+"\)', "Version(`"$ReleaseVersion`")" |
        Set-Content -Path $SolutionInfoPath -Encoding UTF8
}

$Node = ''
if ([IntPtr]::Size -ne 4) { $Node = 'Wow6432Node' }
$MSBuildToolsPath = (Get-ItemProperty -Path HKLM:\SOFTWARE\$Node\Microsoft\MSBuild\ToolsVersions\4.0).MSBuildToolsPath
$MSBuildExe = Join-Path -Path $MSBuildToolsPath -ChildPath MSBuild.exe

$SolutionPath = Join-Path -Path $PSScriptRoot -ChildPath TfsDeployer.sln

Write-Output "Building $Configuration configuration"
& $MSBuildExe $SolutionPath /p:Configuration=$Configuration /p:Platform="Any CPU" /t:Build
if (-not $?) {
    throw 'Build failed.'
}

$MSTestExe = Join-Path -Path $Env:VS100COMNTOOLS -ChildPath ..\IDE\MSTest.exe

$TestContainerPath = Join-Path -Path $PSScriptRoot -ChildPath Tests.TfsDeployer\bin\$Configuration\Tests.TfsDeployer.dll

Write-Output "Executing tests"
& $MSTestExe /testContainer:"$TestContainerPath"
if (-not $?) {
    throw 'Tests failed.'
}

if ($Publish) { 
    if ($UndoSolutionInfo) {
        Write-Output 'Undoing local workspace changes to assembly attributes'
        $Workspace.Undo($SolutionInfoPath) | Out-Null
    }
    
    Write-Output 'Packaging files to be published'

    $BinariesPath = $PSScriptRoot | Join-Path -ChildPath TfsDeployer\bin\$Configuration
    $PackagePath = $Env:TEMP | Join-Path -ChildPath ([Guid]::NewGuid())
    New-Item -Path $PackagePath -ItemType Container | Out-Null
    Get-ChildItem -Path $BinariesPath\* |
        Copy-Item -Destination $PackagePath -Recurse
    Get-ChildItem -Path $PackagePath\* -Include *.pdb |
        Remove-Item
    
    $ZipPath = $PSScriptRoot | Join-Path -ChildPath "TfsDeployer-$ReleaseVersion.zip"
    
    [Reflection.Assembly]::LoadFrom(($PSScriptRoot | Join-Path -ChildPath Dependencies\ICSharpCode.SharpZipLib.dll)) | Out-Null
    $FastZip = New-Object -TypeName ICSharpCode.SharpZipLib.Zip.FastZip
    $FastZip.CreateZip($ZipPath, $PackagePath, $true, '')

    Remove-Item -Path $PackagePath -Recurse

    Write-Output 'Uploading new release to CodePlex'
    . ($PSScriptRoot | Join-Path -ChildPath Dependencies\CodePlexFunctions.ps1)
    $ReleaseFile = New-CodePlexReleaseFile -Path $ZipPath -FileType RuntimeBinary

    $CodePlexCred = (Get-Host).UI.PromptForCredential('CodePlex Login','Provide Developer or Coordinator credentials for the TFS Deployer CodePlex project','','')
    if (-not $CodePlexCred) {
        throw 'Cannot publish without CodePlex credentials.'
    }

    $ReleaseName = "TFS Deployer $ReleaseVersion"
    if ($Force) {
        $ReleaseName += ' (Forced)'
    }

    $PublicRelease = -not $Force

    $Release = Add-CodePlexRelease `
        -Name $ReleaseName `
        -Description "$Configuration build from changeset $Changeset" `
        -ProjectName 'tfsdeployer' `
        -Date (Get-Date) `
        -Status $Publish `
        -Public:$PublicRelease `
        -Credential $CodePlexCred

    Send-CodePlexReleaseFiles `
        -ReleaseName $ReleaseName `
        -ProjectName 'tfsdeployer' `
        -Files @($ReleaseFile) `
        -Credential $CodePlexCred
        
    Remove-Item -Path $ZipPath

    Write-Output "Release '$ReleaseName' published"
    Write-Output $Release.Uri
}
