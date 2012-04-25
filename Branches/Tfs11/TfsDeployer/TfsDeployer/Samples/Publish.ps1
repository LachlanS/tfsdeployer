# this is how Script Parameters defined in the DeploymentMappings.xml are accessed in PowerShell:

if ($ProductionServerName -eq "ProdSvr1") { Write-Output "Script parameter received!" ; }
