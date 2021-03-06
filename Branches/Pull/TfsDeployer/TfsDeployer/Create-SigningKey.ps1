#requires -version 2.0

[CmdletBinding()]
param (
    [string]
    $KeyPath
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$PSScriptRoot = $MyInvocation.MyCommand.Path | Split-Path -Resolve

[System.Reflection.Assembly]::LoadFrom("$PSScriptRoot\TfsDeployer.exe") | Out-Null

[TfsDeployer.Encrypter]::CreateKey($KeyPath)
