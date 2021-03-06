#requires -version 2.0
[CmdletBinding()]
param (
    [parameter(
        Mandatory=$true,
        ValueFromPipeline=$true,
        ValueFromPipelineByPropertyName=$true
    )]
    [ValidateScript({ $_ | Test-Path -PathType Leaf })]
    [string[]]
    $Path
)

begin {
    $ErrorActionPreference = 'Stop'
    Set-PSDebug -Strict

    $OldNamespace = 'http://www.readify.net/TFSDeployer/DeploymentMappings20061026'
    $NewNamespace = 'http://www.readify.net/TfsDeployer/DeploymentMappings20100214'
}

process {
    foreach ($p in $Path) {
        $FullName = ($p | Resolve-Path).Path # TODO fix wildcard issue here
        $DeploymentPath = $FullName | Split-Path
        if (($DeploymentPath | Split-Path -Leaf) -eq 'Deployment') {
            $BuildName = $DeploymentPath | Split-Path | Split-Path -Leaf
        } else {
            $BuildName = $null
            Write-Warning "Expected parent directory Deployment for $p"
        }
        $Old = (Get-Content -Path $FullName) -as [xml]
        if (-not $Old.DeploymentMappings) {
            throw "Expected DeploymentMappings root node in $p"
        }
        if ($Old.DeploymentMappings.xmlns -ne $OldNamespace) {
            throw "Expected namespace $OldNamespace in $p"
        }
        
        $New = "<DeploymentMappings xmlns=`"$NewNamespace`" />" -as [xml]
        
        foreach ($OldMap in $Old.DeploymentMappings.Mapping) {
            $NewMap = $New.ImportNode($OldMap, $true)
            if ($NewMap.HasAttribute('xmlns')) {
                $NewMap.RemoveAttribute('xmlns')
            }
            if ($BuildName) {
                $Pattern = '^' + [Regex]::Escape($BuildName) + '$'
                $NewMap.SetAttribute('BuildDefinitionPattern', $Pattern)
            }
        }
        
        $NewPath = $FullName + '.new'
        $New.Save($NewPath )
        Write-Output "$p converted to $NewPath"
    }
}
