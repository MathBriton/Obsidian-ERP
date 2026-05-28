#requires -Version 5.1
# Sobe backend (5026) e frontend (5173) em janelas separadas para desenvolvimento.
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot

Start-Process powershell -ArgumentList @(
    "-NoExit", "-Command",
    "Set-Location '$root\src\backend'; dotnet run --project ObsidianERP.Api --launch-profile http"
)

Start-Process powershell -ArgumentList @(
    "-NoExit", "-Command",
    "Set-Location '$root\src\frontend'; npm run dev"
)

Write-Host "Backend:  http://localhost:5026/swagger"
Write-Host "Frontend: http://localhost:5173"
