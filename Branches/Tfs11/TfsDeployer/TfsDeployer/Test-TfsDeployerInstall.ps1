#requires -version 2.0
[CmdletBinding()]
param ()

$ErrorActionPreference = 'Stop'
Set-PSDebug -Strict

$PSScriptRoot = $MyInvocation.MyCommand.Path | Split-Path -Resolve

Write-Verbose "Testing for executable"
$ExePath = $PSScriptRoot | Join-Path -ChildPath TfsDeployer.exe
if (-not (Test-Path -PathType Leaf -Path $ExePath)) {
    throw "File not found: $ExePath"
}
$QuotedExePath = $ExePath
if ($QuotedExePath.Contains(' ')) { $QuotedExePath = '"{0}"' -f $QuotedExePath }

Write-Verbose "Testing for Windows service"
$Service = Get-WmiObject -Class Win32_Service -Filter 'Name="TfsDeployer"' |
    Select-Object -First 1
if (-not $Service) {
    throw "Windows service not found: TfsDeployer"
}
if ($Service.PathName -ne $QuotedExePath) {
    throw "TfsDeployer service path to executable should be: $QuotedExePath" 
}
if ($Service.StartMode -ne 'Auto') {
    throw "TfsDeployer service startup type should be Automatic"
}
$ServiceAccount = $Service.StartName
if ($ServiceAccount -like '*@*') {
	$ServiceAccount = (New-Object -TypeName System.Security.Principal.WindowsIdentity -ArgumentList $ServiceAccount).Name
}

if ($ServiceAccount -eq 'LocalSystem' -or 
    $ServiceAccount -like 'NT AUTHORITY\*') {
    throw "TfsDeployer service should use a dedicated service account"
}

if ($Service.State -ne 'Running') {
    Write-Warning "TfsDeployer service is not running"
    Write-Output "Ensure $($ServiceAccount) has the 'Log on as a service' right."
    # TODO automate log as a service test via LSA PInvoke
}



if ($ServiceAccount -ne [Security.Principal.WindowsIdentity]::GetCurrent().Name) {
    Write-Output "Execute this script as $($ServiceAccount)"
    $Cred = $Host.UI.PromptForCredential($null, 'Provide credentials to run the tests with the TFS Deployer service account', $ServiceAccount, $null, 'Domain', 'ReadOnlyUserName')
    Start-Process -Credential $Cred -FilePath powershell.exe `
        -ArgumentList '-noexit','-file',"`"$($MyInvocation.MyCommand.Path | Resolve-Path)`""
    return
}

$TfsClientAssemblyName = 'Microsoft.TeamFoundation.Client, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
try {
    [System.Reflection.Assembly]::Load($TfsClientAssemblyName) | Out-Null
} catch {
    throw "Could not load TeamFoundation assembly"
}
$CollType = [Microsoft.TeamFoundation.Client.TfsTeamProjectCollection]

$ConfigPath = $ExePath + '.config'
if (-not (Test-Path -PathType Leaf -Path $ConfigPath)) {
    throw "File not found: $ConfigPath"
}
$Config = (Get-Content -Path $ConfigPath) -as [xml]

$CollUriNode = $Config.SelectSingleNode('/configuration/applicationSettings/TfsDeployer.Properties.Settings/setting[@name="TeamProjectCollectionUri"]')
if (-not $CollUriNode) {
    throw "Could not find TeamProjectCollectionUri setting in configuration file"
}
Write-Verbose "Testing Project Collection access"
$CollUri = New-Object -TypeName Uri -ArgumentList $CollUriNode.value
$Coll = New-Object -TypeName $CollType -ArgumentList $CollUri
$Coll.Connect('IncludeServices')
if (-not $Coll.HasAuthenticated) {
    throw "Could not connect to Project Collection at: $($CollUriNode.value)"
}
# TODO test project collection with correct credentials
Write-Output "Ensure $($ServiceAccount) is a member of 'Project Collection Administrators' for: $($CollUriNode.value)"

Write-Verbose "Testing HTTP namespace reservation"
$BaseAddrNode = $Config.SelectSingleNode('/configuration/applicationSettings/TfsDeployer.Properties.Settings/setting[@name="BaseAddress"]')
if ($Service.State -ne 'Running') {
    $Listener = New-Object -TypeName System.Net.HttpListener
    $Listener.Prefixes.Add(($BaseAddrNode.value -replace '^http://[^:/]+', 'http://+'))
    $Listener.Start()
    $Listener.Close()
} else {
    Write-Output "HTTP namespace reservation test skipped because TFS Deployer service is running"
}

Write-Output "Ensure $($ServiceAccount) has read access to the build drop file share"
# TODO automate impersonated test for file share access and fail with warning

Write-Verbose "Testing PowerShell execution policy"
$Policy = Get-ExecutionPolicy -List |
    Where-Object { $_.Scope -ne 'Process' -and $_.ExecutionPolicy -ne 'Undefined' } |
    Select-Object -ExpandProperty ExecutionPolicy -First 1
switch ($Policy) {
    Restricted {
        throw "PowerShell scripts cannot run with Restricted execution policy"
    }
    AllSigned {
        Write-Warning "All PowerShell scripts must be digitally signed to run with AllSigned execution policy"
    }
}

Write-Verbose "Testing mail settings"
$SmtpNode = $Config.configuration.'system.net'.mailSettings.smtp
if (-not $SmtpNode.from) {
    throw "Could not find smtp from setting in configuration file"
}
switch ($SmtpNode.deliveryMethod) {
    Network {
        $Client = New-Object -TypeName System.Net.Mail.SmtpClient
        $Client.Host = $SmtpNode.network.host
        if ($SmtpNode.network.port) { $Client.Port = $SmtpNode.network.port }
        if ($SmtpNode.network.defaultCredentials) { $Client.UseDefaultCredentials = $SmtpNode.network.defaultCredentials }
        # TODO support all smtp configuration settings
        $To = Read-Host -Prompt 'Provide a recipient email address to test mail sending'
        $Client.Send($SmtpNode.from, $To, "TFS Deployer Test at $(Get-Date)", 'Testing...')
        Write-Output "Ensure test email is received by: $To"
    }
    PickupDirectoryFromIis {
        Write-Output "Ensure $($ServiceAccount) has permission to read IIS metabase and write to mail pickup directory."
    
    }
    SpecifiedPickupDirectory {
        if (-not $SmtpNode.specifiedPickupDirectory.pickupDirectoryLocation) {
            throw "Could not find pickupDirectoryLocation setting in configuration file"
        }
        $TestFile = $SmtpNode.specifiedPickupDirectory.pickupDirectoryLocation | 
            Join-Path -ChildPath ('test.{0:B}.delete' -f [Guid]::NewGuid())
        try {
            New-Item -ItemType File -Path $TestFile | Out-Null
        } catch {
            throw "Could not write to pickupDirectoryLocation at: $($SmtpNode.specifiedPickupDirectory.pickupDirectoryLocation)"
        }
        Remove-Item -Path $TestFile
    }
}

# TODO offer to enable trace switches
# TODO offer to begin interactive testing
Write-Verbose "Test successful"
