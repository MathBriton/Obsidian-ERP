#requires -Version 5.1
# Sobe o stack completo (db + api + web) em containers, com override de producao.
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

docker compose -f docker-compose.yml -f docker-compose.prod.yml up --build -d

Write-Host ""
Write-Host "Stack no ar:"
Write-Host "  API:      http://localhost:8080/health"
Write-Host "  Frontend: http://localhost:5173"
Write-Host ""
Write-Host "Logs:   docker compose logs -f"
Write-Host "Parar:  docker compose down"
