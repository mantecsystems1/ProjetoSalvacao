$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot\..

if (-not (Test-Path ".env")) {
  Copy-Item ".env.example" ".env"
  Write-Host "Arquivo .env criado a partir de .env.example. Edite JWT_SIGNING_KEY antes de subir em produção." -ForegroundColor Yellow
}

docker compose up -d --build

Write-Host "API:     http://localhost:8080 (ajuste com API_PORT no .env)" -ForegroundColor Green
Write-Host "Web:     http://localhost:3000 (ajuste com WEB_PORT no .env)" -ForegroundColor Green
Write-Host "Swagger: http://localhost:8080/swagger" -ForegroundColor Green
