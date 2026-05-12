# Check API Connectivity
# This script checks if the API is running and accessible

Write-Host "================================" -ForegroundColor Cyan
Write-Host "  API Connectivity Check" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Check 1: Is the process running?
Write-Host "1. Checking if BrewLab process is running..." -ForegroundColor Yellow
$process = Get-Process | Where-Object {$_.ProcessName -like "*BrewLab*"}
if ($process) {
    Write-Host "   ? BrewLab process found (PID: $($process.Id))" -ForegroundColor Green
} else {
    Write-Host "   ? BrewLab process not running" -ForegroundColor Red
    Write-Host "   Action: Run '.\start-api.ps1' to start the API" -ForegroundColor Yellow
}
Write-Host ""

# Check 2: Is port 5000 listening?
Write-Host "2. Checking if port 5000 is listening..." -ForegroundColor Yellow
try {
    $connection = Get-NetTCPConnection -LocalPort 5000 -State Listen -ErrorAction Stop
    Write-Host "   ? Port 5000 is listening" -ForegroundColor Green
} catch {
    Write-Host "   ? Port 5000 is not listening" -ForegroundColor Red
    Write-Host "   Action: Start the API with 'dotnet run'" -ForegroundColor Yellow
}
Write-Host ""

# Check 3: Can we reach the API?
Write-Host "3. Testing API connectivity..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/health/ping" -TimeoutSec 5 -ErrorAction Stop
    if ($response.StatusCode -eq 200) {
        Write-Host "   ? API is accessible and responding" -ForegroundColor Green
        $content = $response.Content | ConvertFrom-Json
        if ($content.success) {
            Write-Host "   ? New architecture is working!" -ForegroundColor Green
        }
    }
} catch [System.Net.WebException] {
    Write-Host "   ? Cannot reach API" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "   Possible causes:" -ForegroundColor Yellow
    Write-Host "     1. API is not running - Run '.\start-api.ps1'" -ForegroundColor Gray
    Write-Host "     2. Firewall blocking connection" -ForegroundColor Gray
    Write-Host "     3. Port 5000 is used by another application" -ForegroundColor Gray
} catch {
    Write-Host "   ? Error connecting to API" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Gray
}
Write-Host ""

# Check 4: Firewall rules
Write-Host "4. Checking firewall..." -ForegroundColor Yellow
$firewallRules = Get-NetFirewallRule | Where-Object {$_.DisplayName -like "*5000*" -or $_.DisplayName -like "*dotnet*"}
if ($firewallRules) {
    Write-Host "   ? Found firewall rules related to dotnet/port 5000" -ForegroundColor Gray
} else {
    Write-Host "   ? No specific firewall rules found" -ForegroundColor Gray
}
Write-Host ""

# Check 5: Port conflicts
Write-Host "5. Checking for port conflicts..." -ForegroundColor Yellow
$portInUse = Get-NetTCPConnection -LocalPort 5000 -ErrorAction SilentlyContinue
if ($portInUse) {
    Write-Host "   ? Port 5000 is in use by:" -ForegroundColor Gray
    foreach ($conn in $portInUse) {
        $proc = Get-Process -Id $conn.OwningProcess -ErrorAction SilentlyContinue
        if ($proc) {
            Write-Host "     - Process: $($proc.ProcessName) (PID: $($proc.Id))" -ForegroundColor Gray
        }
    }
} else {
    Write-Host "   ? Port 5000 is free" -ForegroundColor Green
}
Write-Host ""

Write-Host "================================" -ForegroundColor Cyan
Write-Host "  Recommendations" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

if (-not $process) {
    Write-Host "? Start the API:" -ForegroundColor Yellow
    Write-Host "  .\start-api.ps1" -ForegroundColor White
    Write-Host ""
}

Write-Host "? If API is running but not accessible:" -ForegroundColor Yellow
Write-Host "  1. Check Visual Studio output window for errors" -ForegroundColor Gray
Write-Host "  2. Try running in PowerShell instead of VS terminal:" -ForegroundColor Gray
Write-Host "     dotnet run" -ForegroundColor White
Write-Host "  3. Try a different port:" -ForegroundColor Gray
Write-Host "     dotnet run --urls `"http://localhost:5001`"" -ForegroundColor White
Write-Host ""

Write-Host "? Quick test when API is running:" -ForegroundColor Yellow
Write-Host "  Invoke-RestMethod http://localhost:5000/api/health/ping" -ForegroundColor White
Write-Host ""
