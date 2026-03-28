param(
    [Parameter(Mandatory = $true)]
    [string]$ProjectName,

    [string]$Runtime = "win-x64",
    [string]$Configuration = "Release",
    [string]$DotnetVersion = "8.0.x",
    [string]$Tag,
    [switch]$CleanPublishDir
)

$ErrorActionPreference = "Stop"

# Script location: RepoRoot\Deploy\Local\build-local.ps1
# Repo root = two levels up from script folder
$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path

$ProjectPath = Join-Path $RepoRoot "Src\$ProjectName\$ProjectName.csproj"
$PublishDir  = Join-Path $RepoRoot "publish\$ProjectName"
$ConfigFile  = Join-Path $RepoRoot "NuGet.config"
$ExecutableName = "$ProjectName.exe"

Write-Host "RepoRoot       : $RepoRoot"
Write-Host "ProjectName    : $ProjectName"
Write-Host "ProjectPath    : $ProjectPath"
Write-Host "PublishDir     : $PublishDir"
Write-Host "ExecutableName : $ExecutableName"
Write-Host "Runtime        : $Runtime"
Write-Host "Configuration  : $Configuration"
Write-Host "ConfigFile     : $ConfigFile"

if (-not (Test-Path $ProjectPath)) {
    throw "Project file not found: $ProjectPath"
}

if (-not (Test-Path $ConfigFile)) {
    throw "nuget.config not found: $ConfigFile"
}

if ($CleanPublishDir -and (Test-Path $PublishDir)) {
    Remove-Item $PublishDir -Recurse -Force
}

New-Item -ItemType Directory -Force -Path $PublishDir | Out-Null

dotnet restore $ProjectPath --configfile $ConfigFile
if ($LASTEXITCODE -ne 0) {
    throw "dotnet restore failed."
}

if ($Tag) {
    [xml]$csproj = Get-Content $ProjectPath
    $version = $csproj.Project.PropertyGroup.Version | Select-Object -First 1

    if (-not $version) {
        throw "No <Version> found in project file."
    }

    $expectedTag = "v$version"
    if ($Tag -ne $expectedTag) {
        throw "Tag mismatch. Provided: $Tag | Expected: $expectedTag"
    }
}

dotnet publish $ProjectPath `
    -c $Configuration `
    -r $Runtime `
    --no-self-contained `
    -o $PublishDir `
    /p:PublishSingleFile=true `
    /p:DebugType=None `
    /p:DebugSymbols=false

if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed."
}

$ExePath = Join-Path $PublishDir $ExecutableName

if (-not (Test-Path $ExePath)) {
    throw "Expected executable was not created: $ExePath"
}

Write-Host ""
Write-Host "Build completed successfully."
Write-Host "EXE: $ExePath"