$ErrorActionPreference = "Stop"
$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot "..\..")
Set-Location $RepoRoot

if (-not (Test-Path ".env")) {
    Copy-Item ".env.example" ".env"
    Write-Host "Created .env from .env.example. Review secrets before exposing publicly." -ForegroundColor Yellow
}

Write-Host "Checking Docker Engine..." -ForegroundColor Cyan
docker info --format "{{.ServerVersion}}" 2>$null | Out-Null
if ($LASTEXITCODE -ne 0) {
    $desktopExe = "C:\Program Files\Docker\Docker\Docker Desktop.exe"
    if (Test-Path $desktopExe) {
        Write-Host "Starting Docker Desktop..." -ForegroundColor Yellow
        Start-Process $desktopExe
        for ($i = 0; $i -lt 30; $i++) {
            Start-Sleep -Seconds 3
            docker info --format "{{.ServerVersion}}" 2>$null | Out-Null
            if ($LASTEXITCODE -eq 0) { break }
        }
    }
}

docker info --format "Docker Engine ready: {{.ServerVersion}}"
if ($LASTEXITCODE -ne 0) {
    throw "Docker Engine is not running. Please open Docker Desktop as Administrator or start 'com.docker.service', then re-run this script."
}

Write-Host "Building and starting containers (sqlserver, api, web)..." -ForegroundColor Cyan
docker compose up -d --build

Write-Host "Waiting for API & Web healthcheck on http://localhost:8080..." -ForegroundColor Cyan
for ($i = 0; $i -lt 30; $i++) {
    try {
        $resp = Invoke-RestMethod -Uri "http://localhost:8080/api/health" -TimeoutSec 3
        if ($resp.data.status -eq "Healthy") {
            Write-Host "Deployment live at http://localhost:8080 (API Healthy)" -ForegroundColor Green
            docker compose ps
            exit 0
        }
    } catch { }
    Start-Sleep -Seconds 3
}

docker compose logs --tail 50 api
throw "API healthcheck did not become ready within timeout."
