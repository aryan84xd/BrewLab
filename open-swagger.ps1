# Open Swagger UI
# This script opens all relevant URLs for the BrewLab API

Write-Host "================================" -ForegroundColor Cyan
Write-Host "  Opening BrewLab API URLs" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Check if API is running
$apiRunning = $false
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/health/ping" -UseBasicParsing -TimeoutSec 3 -ErrorAction Stop
    $apiRunning = $true
    Write-Host "? API is running on http://localhost:5000" -ForegroundColor Green
} catch {
    Write-Host "? API is not running!" -ForegroundColor Red
    Write-Host "  Please start the API first:" -ForegroundColor Yellow
    Write-Host "    .\start-api.ps1" -ForegroundColor White
    Write-Host "    or" -ForegroundColor Gray
    Write-Host "    dotnet run" -ForegroundColor White
    exit 1
}

Write-Host ""
Write-Host "Opening URLs in your browser:" -ForegroundColor Yellow
Write-Host ""

# Open Swagger UI
Write-Host "1. Swagger UI (API Documentation)" -ForegroundColor Cyan
Write-Host "   http://localhost:5000/swagger" -ForegroundColor Gray
Start-Process "http://localhost:5000/swagger"
Start-Sleep -Seconds 1

# Open Health endpoint
Write-Host ""
Write-Host "2. Health Endpoint (Test New Architecture)" -ForegroundColor Cyan
Write-Host "   http://localhost:5000/api/health" -ForegroundColor Gray
Start-Process "http://localhost:5000/api/health"
Start-Sleep -Seconds 1

# Open Swagger JSON
Write-Host ""
Write-Host "3. Swagger JSON (OpenAPI Spec)" -ForegroundColor Cyan
Write-Host "   http://localhost:5000/swagger/v1/swagger.json" -ForegroundColor Gray

Write-Host ""
Write-Host "================================" -ForegroundColor Cyan
Write-Host "  URLs Opened Successfully!" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Available Endpoints:" -ForegroundColor Yellow
Write-Host "  • Swagger UI: http://localhost:5000/swagger" -ForegroundColor Gray
Write-Host "  • Health: http://localhost:5000/api/health" -ForegroundColor Gray
Write-Host "  • Ping: http://localhost:5000/api/health/ping" -ForegroundColor Gray
Write-Host "  • Auth: http://localhost:5000/api/auth/*" -ForegroundColor Gray
Write-Host "  • Coffees: http://localhost:5000/api/coffees" -ForegroundColor Gray
Write-Host "  • Experiments: http://localhost:5000/api/experiment" -ForegroundColor Gray
Write-Host ""
Write-Host "Tip: Use Swagger UI to test all endpoints interactively!" -ForegroundColor Cyan
Write-Host ""
