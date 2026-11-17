# ==============================================================================
# SmartRestaurant — Cryptographic Pilot License Generator Utility
# Generates ECDSA P-256 digitally-signed offline .lic license files for Pilot Store
# ==============================================================================

param(
    [string]$CustomerId = "CUST-PILOT-STORE-01",
    [string]$InstallationId = "*",
    [string]$Edition = "Enterprise",
    [int]$ValidityDays = 365,
    [string]$OutputFile = "$PSScriptRoot/../pilot-store-license.lic"
)

$ErrorActionPreference = "Stop"

$root = Resolve-Path "$PSScriptRoot/.."
$cliProject = "$root/src/SmartRestaurant.LicenseCli/SmartRestaurant.LicenseCli.csproj"
$outFullPath = [System.IO.Path]::GetFullPath($OutputFile)

dotnet run --project $cliProject -- --customer $CustomerId --installation $InstallationId --edition $Edition --days $ValidityDays --out "$outFullPath"
