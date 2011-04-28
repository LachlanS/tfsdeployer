function Set-TfsNotificationJobDelay {
    [CmdletBinding()]
    param (
        [parameter(Mandatory=$true)]
        [string]
        $Url,
     
        [ValidateRange(0, 86400)]   
        [int]
        $DelaySeconds = 120
    )

    Add-Type -AssemblyName 'Microsoft.TeamFoundation.Client, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'

    $DefaultPath = '/Configuration/JobService/DefaultDelayedJobDelay'
    $Path = '/Service/Integration/Settings/NotificationJobDelay'
    $Connection = New-Object -TypeName Microsoft.TeamFoundation.Client.TfsTeamProjectCollection -ArgumentList $Url
    #$Connection = New-Object -TypeName Microsoft.TeamFoundation.Client.TfsConfigurationServer -ArgumentList $Url
    $Connection.EnsureAuthenticated()

    $Registry = $Connection.GetService([Microsoft.TeamFoundation.Framework.Client.ITeamFoundationRegistry])

    $OldDelaySeconds = 120
    $DefaultDelay = $Registry.GetValue($DefaultPath, $OldDelaySeconds)
    $OldDelaySeconds = $Registry.GetValue($Path, $OldDelaySeconds)

    $RestartRequired = $false
    if ($OldDelaySeconds -ne $DelaySeconds) {
        $Registry.SetValue($Path, $DelaySeconds)
        $RestartRequired = $true
    } else {
        Write-Verbose "Notification job delay already set to $OldDelaySeconds seconds."
    }
    
    New-Object -TypeName PSObject -Property @{
        Url = $Url
        OldDelaySeconds = $OldDelaySeconds
        DelaySeconds = $DelaySeconds
        RestartRequired = $RestartRequired
    }
}

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
