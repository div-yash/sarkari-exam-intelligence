param(
    [string]$OutputRoot = ".\publish"
)

$ErrorActionPreference = "Stop"
$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot "..\..")
$OutDir = Join-Path $RepoRoot $OutputRoot
$ApiOut = Join-Path $OutDir "api"
$WebOut = Join-Path $OutDir "frontend"
$StandaloneOut = Join-Path $OutDir "standalone"

Write-Host "=== Publishing Sarkari Exam Intelligence Release Artifacts ===" -ForegroundColor Cyan

if (Test-Path $OutDir) { Remove-Item $OutDir -Recurse -Force }
New-Item -ItemType Directory -Force -Path $ApiOut, $WebOut, $StandaloneOut | Out-Null

# 1. Publish ASP.NET Core Backend API
Write-Host "[1/4] Publishing ASP.NET Core API..."
dotnet publish (Join-Path $RepoRoot "backend\src\SarkariExamIntelligence.API\SarkariExamIntelligence.API.csproj") `
    -c Release -o $ApiOut --no-restore -m:1 --disable-build-servers

Copy-Item (Join-Path $RepoRoot "deploy\iis\web.config") (Join-Path $ApiOut "web.config") -Force
New-Item -ItemType Directory -Force -Path (Join-Path $ApiOut "wwwroot\notifications") | Out-Null

# 2. Build Angular Production Bundle
Write-Host "[2/4] Building Angular Production Frontend..."
npm --prefix (Join-Path $RepoRoot "frontend") run build
Copy-Item -Path (Join-Path $RepoRoot "frontend\dist\frontend\browser\*") -Destination $WebOut -Recurse -Force
Copy-Item (Join-Path $RepoRoot "deploy\iis\frontend-web.config") (Join-Path $WebOut "web.config") -Force

# 3. Assemble Single-Server Standalone Production Bundle (API + Angular SPA)
Write-Host "[3/4] Assembling Standalone Single-Host Bundle..."
Copy-Item -Path (Join-Path $ApiOut "*") -Destination $StandaloneOut -Recurse -Force
Copy-Item -Path (Join-Path $RepoRoot "frontend\dist\frontend\browser\*") -Destination (Join-Path $StandaloneOut "wwwroot") -Recurse -Force

# 4. Generate Standalone EF Migration SQL Script
Write-Host "[4/4] Generating Idempotent SQL Server Migration Script..."
dotnet ef migrations script --idempotent `
    --project (Join-Path $RepoRoot "backend\src\SarkariExamIntelligence.Infrastructure") `
    --startup-project (Join-Path $RepoRoot "backend\src\SarkariExamIntelligence.API") `
    --no-build `
    -o (Join-Path $OutDir "schema-idempotent.sql")

Write-Host "=== Publish Complete: $OutDir ===" -ForegroundColor Green
