$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

[System.Diagnostics.EventLog]::CreateEventSource('TfsDeployer', 'Application')