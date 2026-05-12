# Start BrewLab API
# This script helps you start the API and test it

Write-Host "================================" -ForegroundColor Cyan
Write-Host "  Starting BrewLab API" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Kill any existing BrewLab process
$existingProcess = Get-Process | Where-Object {$_.ProcessName -like "*BrewLab*"}
if ($existingProcess) {
    Write-Host "Stopping existing BrewLab process..." -ForegroundColor Yellow
    $existingProcess | Stop-Process -Force
    Start-Sleep -Seconds 2
}

# Set environment
$env:ASPNETCORE_ENVIRONMENT = "Development"

Write-Host "Starting API on http://localhost:5000" -ForegroundColor Green
Write-Host ""
Write-Host "Once started, you can:" -ForegroundColor Yellow
Write-Host "  • Open Swagger: http://localhost:5000/swagger" -ForegroundColor Gray
Write-Host "  • Test Health: http://localhost:5000/api/health" -ForegroundColor Gray
Write-Host "  • Run tests: .\test-api.ps1 (in another terminal)" -ForegroundColor Gray
Write-Host ""
Write-Host "Press Ctrl+C to stop the API" -ForegroundColor Yellow
Write-Host ""
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Start the API
dotnet run
