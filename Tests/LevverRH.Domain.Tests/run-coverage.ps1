# Script para executar testes com cobertura de código

Write-Host "🧪 Executando testes com cobertura de código..." -ForegroundColor Cyan

# Limpar relatórios antigos
if (Test-Path "TestResults") {
    Remove-Item "TestResults" -Recurse -Force
}

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage" --results-directory:"TestResults"

# Gerar relatório HTML
Write-Host "`n📊 Gerando relatório de cobertura..." -ForegroundColor Cyan
reportgenerator -reports:"TestResults\*\coverage.cobertura.xml" -targetdir:"TestResults\CoverageReport" -reporttypes:Html

# Abrir relatório no navegador
Write-Host "`n✅ Relatório gerado! Abrindo navegador..." -ForegroundColor Green
Start-Process "TestResults\CoverageReport\index.html"

Write-Host "`n📈 Resumo da cobertura:" -ForegroundColor Yellow
Get-Content "TestResults\CoverageReport\Summary.txt" -ErrorAction SilentlyContinue
