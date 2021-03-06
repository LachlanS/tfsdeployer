#requires -version 2.0

if (-not (Get-Variable -Name CodePlexReleaseServiceProxy -ErrorAction SilentlyContinue)) {
    $global:CodePlexReleaseServiceProxy = New-WebServiceProxy `
            -Uri https://www.codeplex.com/Services/ReleaseService.asmx `
            -Namespace CodePlex.ReleaseService
}

function Add-CodePlexRelease (
    [parameter(Mandatory=$true)]
    [string]
    $Name,
    
    [parameter(Mandatory=$true)]
    [string]
    $Description,
    
    [parameter(Mandatory=$true)]
    [string]
    $ProjectName,
    
    [DateTime]
    $Date = (Get-Date),
    
    [parameter(Mandatory=$true)]
    [ValidateSet('Planning', 'Alpha', 'Beta', 'Stable')]
    [string]
    $Status,
    
    [switch]
    $Public,
    
    [switch]
    $Default,
    
    [parameter(Mandatory=$true)]
    [System.Management.Automation.PSCredential]
    $Credential
) {
    if ($Status -eq 'Planning') { $Public = $false }
    if (-not $Public) { $Default = $false }

    $USCulture = [System.Globalization.CultureInfo]::CreateSpecificCulture('en-US')    
    $ReleaseId = $CodePlexReleaseServiceProxy.CreateARelease(
        $ProjectName,
        $Name,
        $Description,
        $Date.ToString('d', $USCulture),
        $Status,
        [bool]$Public,
        [bool]$Default,
        $Credential.GetNetworkCredential().UserName,
        $Credential.GetNetworkCredential().Password
    )    

    return New-Object -TypeName PSObject -Property @{
        Id = $ReleaseId
        Name = $Name
        ProjectName = $ProjectName
        Uri = "http://$ProjectName.codeplex.com/releases/view/$ReleaseId"
    }
}

function New-CodePlexReleaseFile {
    param (
        [parameter(Mandatory=$true)]
        [string]
        $Path,

        [parameter(Mandatory=$true)]
        [ValidateSet('RuntimeBinary', 'SourceCode', 'Documentation', 'Example')]
        [string]
        $FileType,
        
        [string]
        $DisplayName,
        
        [string]
        $MimeType

    )
    process {
        $ReleaseFile = New-Object -Typename CodePlex.ReleaseService.ReleaseFile
        $ReleaseFile.FileName = ($Path | Split-Path -Leaf)
        $ReleaseFile.FileData = (Get-Content -Path $Path -Encoding Byte)
        $ReleaseFile.FileType = $FileType

        if (-not $DisplayName) { $DisplayName = $ReleaseFile.FileName }
        $ReleaseFile.Name = $DisplayName 
        $ReleaseFile.MimeType = $MimeType

        return $ReleaseFile
    }
}

function Send-CodePlexReleaseFiles (
    [parameter(Mandatory=$true)]
    [string]
    $ReleaseName,
    
    [parameter(Mandatory=$true)]
    [string]
    $ProjectName,
    
    [parameter(Mandatory=$true)]
    [object[]]
    $Files,
        
    [parameter(Mandatory=$true)]
    [System.Management.Automation.PSCredential]
    $Credential
 
) {
    $CodePlexReleaseServiceProxy.UploadTheReleaseFiles(
        $ProjectName,
        $ReleaseName,
        $Files,
        $null,
        $Credential.GetNetworkCredential().UserName,
        $Credential.GetNetworkCredential().Password
    )    
}
