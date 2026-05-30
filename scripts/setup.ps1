#requires -Version 5.1
# Setup inicial do projeto: cria .env, restaura backend e instala o frontend.
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

function New-Secret([int]$bytes = 48) {
    # Segredo aleatório criptograficamente seguro (sem +/= para nao atrapalhar o .env).
    $buffer = New-Object 'System.Byte[]' $bytes
    $rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
    $rng.GetBytes($buffer)
    return ([Convert]::ToBase64String($buffer) -replace '[+/=]', '')
}

$envPath = Join-Path $root ".env"
if (-not (Test-Path $envPath)) {
    $jwtSecret = New-Secret 48
    $dbPassword = New-Secret 18

    @"
POSTGRES_USER=obsidian
POSTGRES_PASSWORD=$dbPassword
POSTGRES_DB=obsidian_erp
POSTGRES_PORT=5433
ASPNETCORE_ENVIRONMENT=Development
API_PORT=8080
FRONTEND_PORT=5173
JWT__Issuer=obsidian-erp
JWT__Audience=obsidian-erp-client
JWT__Secret=$jwtSecret
JWT__AccessTokenMinutes=15
JWT__RefreshTokenDays=7
"@ | Set-Content -Path $envPath -Encoding utf8
    Write-Host "Criado .env com segredos gerados aleatoriamente."
}
else {
    Write-Host ".env ja existe — mantido."
}

Write-Host "Restaurando backend (.NET)..."
dotnet restore (Join-Path $root "src\backend")

Write-Host "Instalando dependencias do frontend..."
Push-Location (Join-Path $root "src\frontend")
npm ci
Pop-Location

Write-Host ""
Write-Host "Setup concluido."
Write-Host "Para rodar com seu Postgres local, configure os user-secrets da API:"
Write-Host '  dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=SEU_BANCO;Username=postgres;Password=SUA_SENHA" --project src\backend\ObsidianERP.Api'
Write-Host '  dotnet user-secrets set "Jwt:Secret" "<segredo-de-dev-com-32+caracteres>" --project src\backend\ObsidianERP.Api'
