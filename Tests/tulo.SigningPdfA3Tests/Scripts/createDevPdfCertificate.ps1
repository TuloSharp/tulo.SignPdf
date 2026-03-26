param(
    [Parameter(Mandatory = $true)]
    [Alias("d")]
    [string]$DestinationPath,

    [Parameter(Mandatory = $true)]
    [Alias("n")]
    [string]$Name,

    [Parameter(Mandatory = $true)]
    [Alias("p")]
    [string]$Password
)

$baseScriptPath = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent

function CreateOpenSSLConfig {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ConfigPath,

        [Parameter(Mandatory = $true)]
        [string]$CommonName
    )

    Write-Host "Creating OpenSSL configuration file" -ForegroundColor Blue

    $configText = @"
[ req ]
default_bits       = 2048
prompt             = no
default_md         = sha256
distinguished_name = dn
x509_extensions    = v3_cert

[ dn ]
CN = $CommonName

[ v3_cert ]
basicConstraints = critical,CA:FALSE
keyUsage = critical,digitalSignature
subjectKeyIdentifier = hash
authorityKeyIdentifier = keyid,issuer
"@

    $configText | Out-File -FilePath $ConfigPath -Encoding ascii
    Write-Host "Created config at $ConfigPath" -ForegroundColor Green
}

if (-not (Test-Path -Path $DestinationPath)) {
    Write-Host "Creating directory: $DestinationPath" -ForegroundColor Blue
    New-Item -Path $DestinationPath -ItemType Directory | Out-Null
}

Set-Location -Path $DestinationPath
Write-Host "Setting directory to $DestinationPath" -ForegroundColor Blue

$certBaseName = $Name.Trim()
if ([string]::IsNullOrWhiteSpace($certBaseName)) {
    throw "Name must not be empty."
}

$configFile = Join-Path $DestinationPath "$certBaseName.openssl.conf"
$keyFile    = Join-Path $DestinationPath "$certBaseName.key"
$crtFile    = Join-Path $DestinationPath "$certBaseName.crt"
$pfxFile    = Join-Path $DestinationPath "$certBaseName.pfx"

CreateOpenSSLConfig -ConfigPath $configFile -CommonName $certBaseName

# PowerShell path -> WSL path
$driveLetter = $baseScriptPath.Substring(0, 1).ToLower()
$remainingPath = $baseScriptPath.Substring(2).Replace('\', '/')
$wslScriptPath = "/mnt/$driveLetter$remainingPath"

# Destination path -> WSL path
$destDriveLetter = $DestinationPath.Substring(0, 1).ToLower()
$destRemainingPath = $DestinationPath.Substring(2).Replace('\', '/')
$wslDestinationPath = "/mnt/$destDriveLetter$destRemainingPath"

wsl "$wslScriptPath/createCertificate.sh" `
    -d "$wslDestinationPath" `
    -n "$certBaseName" `
    -p "$Password"

$created = $LASTEXITCODE

if ($created -eq 0) {
    Write-Host "Certificate created successfully" -ForegroundColor Green
    Write-Host "KEY: $keyFile" -ForegroundColor Green
    Write-Host "CRT: $crtFile" -ForegroundColor Green
    Write-Host "PFX: $pfxFile" -ForegroundColor Green
}
elseif ($created -eq 1) {
    Write-Host "Destination path is invalid" -ForegroundColor Red
}
elseif ($created -eq 2) {
    Write-Host "Name is invalid" -ForegroundColor Red
}
else {
    Write-Host "Unknown error occurred. Exit code: $created" -ForegroundColor Red
}