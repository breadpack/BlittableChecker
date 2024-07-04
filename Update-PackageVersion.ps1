param (
    [string]$Version
)

$packageJsonPath = Join-Path $PSScriptRoot "UnityPackage\package.json"

if (-not (Test-Path $packageJsonPath)) {
    Write-Error "package.json not found at path: $packageJsonPath"
    exit 1
}

$packageJson = Get-Content $packageJsonPath -Raw | ConvertFrom-Json
$packageJson.version = $Version
$packageJson | ConvertTo-Json -Depth 32 | Set-Content $packageJsonPath

Write-Output "Updated package.json version to $Version"
