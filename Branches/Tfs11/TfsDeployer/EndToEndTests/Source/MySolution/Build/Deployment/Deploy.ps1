$LogPath = $Env:TEMP | Join-Path -ChildPath ('{0}.log' -f $TfsDeployerBuildDetail.BuildNumber)
& { 
'Waiting for a bit...'
Start-Sleep -Seconds 3
'Waiting for a bit more...'
Start-Sleep -Seconds 4
'Timestamp: {0}' -f (Get-Date)
'$TfsDeployerBuildDetail:'
$TfsDeployerBuildDetail | Format-List -Property *
'$MyInvocation.MyCommand:'
$MyInvocation.MyCommand | Format-List -Property *
} | Tee-Object -FilePath $LogPath