#requires -version 2.0

$ErrorActionPreference = 'Stop'
Set-PSDebug -Strict

$PSScriptRoot = $MyInvocation.MyCommand.Path | Resolve-Path | Split-Path

$TfsClientAssemblyName = 'Microsoft.TeamFoundation.Client, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
try {
    [System.Reflection.Assembly]::Load($TfsClientAssemblyName) | Out-Null
} catch {
    throw "Could not load TeamFoundation assembly"
}
$CollType = [Microsoft.TeamFoundation.Client.TfsTeamProjectCollection]

Write-Verbose "Testing for executable"
$ExePath = $PSScriptRoot | Join-Path -ChildPath TfsDeployer.exe
if (-not (Test-Path -PathType Leaf -Path $ExePath)) {
    throw "File not found: $ExePath"
}

Write-Verbose "Testing for Windows service"
$Service = Get-WmiObject -Class Win32_Service -Filter 'Name="TfsDeployer"' |
    Select-Object -First 1
if (-not $Service) {
    throw "Windows service not found: TfsDeployer"
}
if ($Service.PathName -ne $ExePath) {
    throw "TfsDeployer service path to executable should be: $ExePath" 
}
if ($Service.StartMode -ne 'Auto') {
    throw "TfsDeployer service startup type should be Automatic"
}
if ($Service.StartName -eq 'LocalSystem' -or 
    $Service.StartName -like 'NT AUTHORITY\*') {
    throw "TfsDeployer service should use a dedicated service account"
}
Write-Verbose "Ensure $($Service.StartName) has the 'Log on as a service' right."
# TODO automate log as a service test via LSA PInvoke

if ($Service.State -ne 'Running') {
    Write-Warning "TfsDeployer service is not running"
}

$ConfigPath = $ExePath + '.config'
if (-not (Test-Path -PathType Leaf -Path $ConfigPath)) {
    throw "File not found: $ConfigPath"
}
$Config = (Get-Content -Path $ConfigPath) -as [xml]

$CollUriNode = $Config.SelectSingleNode('/configuration/applicationSettings/tfsDeployer.Properties.Settings/setting[@name="TeamProjectCollectionUri"]')
Write-Verbose "Ensure DNS and Firewalls allow connection to: $($CollUriNode.value)"
Write-Verbose "Ensure $($Service.StartName) is a member of 'Project Collection Administrators' for: $($CollUriNode.value)"
# TODO test project collection with correct credentials
#$Coll = New-Object -TypeName $CollType -ArgumentList ($CollUriNode.value, $null)

$BaseAddrNode = $Config.SelectSingleNode('/configuration/applicationSettings/tfsDeployer.Properties.Settings/setting[@name="BaseAddress"]')
Write-Verbose "Ensure $($Service.StartName) has the namespace reservation for: $($BaseAddrNode.value)"
# TODO automate urlacl test

Write-Verbose "Ensure $($Service.StartName) has read access to the build drop file share"
# TODO automate impersonated test for file share access and fail with warning

Write-Verbose "Ensure $($Service.StartName) has PowerShell execution policy set to RemoteSigned"
# TODO automate impersonated execution policy test with severity based on setting

$SmtpNode = $Config.configuration.mailSettings.smtp
Write-Verbose "Ensure $($Service.StartName) has permission to send mail"
# TODO automate impersonated mail test

# trace switches
# interactive testing