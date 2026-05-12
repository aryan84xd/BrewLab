# ?? Troubleshooting "Site Cannot Be Reached"

## Quick Fix Steps

### Step 1: Check API Status
```powershell
.\check-api.ps1
```

This will diagnose the issue and tell you exactly what's wrong.

---

## Common Issues & Solutions

### Issue 1: API Not Running

**Check:**
```powershell
Get-Process | Where-Object {$_.ProcessName -like "*BrewLab*"}
```

**Fix:**
```powershell
# Option A: Use the startup script
.\start-api.ps1

# Option B: Run directly
dotnet run

# Option C: Run on different port
dotnet run --urls "http://localhost:5001"
```

---

### Issue 2: Port 5000 Already in Use

**Check what's using port 5000:**
```powershell
Get-NetTCPConnection -LocalPort 5000 -ErrorAction SilentlyContinue | ForEach-Object {
    $process = Get-Process -Id $_.OwningProcess -ErrorAction SilentlyContinue
    [PSCustomObject]@{
        ProcessName = $process.ProcessName
        PID = $_.OwningProcess
        State = $_.State
    }
}
```

**Fix Option 1 - Kill the process:**
```powershell
# Find and kill process using port 5000
$conn = Get-NetTCPConnection -LocalPort 5000 -ErrorAction SilentlyContinue
if ($conn) {
    Stop-Process -Id $conn.OwningProcess -Force
}

# Then start API
dotnet run
```

**Fix Option 2 - Use different port:**
```powershell
# Run on port 5001 instead
dotnet run --urls "http://localhost:5001"

# Then access at:
start http://localhost:5001/api/health
```

---

### Issue 3: Firewall Blocking Connection

**Check firewall:**
```powershell
Get-NetFirewallRule | Where-Object {$_.DisplayName -like "*dotnet*"}
```

**Fix - Allow dotnet through firewall:**
```powershell
# Run as Administrator
New-NetFirewallRule -DisplayName "Allow Dotnet" -Direction Inbound -Program "C:\Program Files\dotnet\dotnet.exe" -Action Allow
```

---

### Issue 4: Running from Visual Studio Terminal

Sometimes VS terminal has issues. Try this:

**Fix - Use PowerShell instead:**
1. Open regular PowerShell (not VS terminal)
2. Navigate to project directory:
   ```powershell
   cd "C:\Users\ILOM43095\source\repos\aryan84xd\BrewLab"
   ```
3. Run:
   ```powershell
   dotnet run
   ```

---

### Issue 5: Database Connection Preventing Startup

If API starts but crashes due to database:

**Fix - Check logs:**
```powershell
dotnet run
# Look for errors in output
```

**Fix - Test without database:**
The health endpoints work without database!
```powershell
# After API starts
Invoke-RestMethod http://localhost:5000/api/health/ping
```

---

## Step-by-Step Troubleshooting

### 1?? Clean Build
```powershell
dotnet clean
dotnet build
```

### 2?? Kill Existing Processes
```powershell
Get-Process | Where-Object {$_.ProcessName -like "*BrewLab*"} | Stop-Process -Force
```

### 3?? Start Fresh
```powershell
.\start-api.ps1
```

### 4?? Wait for Startup
Look for this message:
```
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```

### 5?? Test Connectivity
```powershell
# In a NEW PowerShell window
Invoke-RestMethod http://localhost:5000/api/health/ping
```

---

## Testing Connection

### Simple Ping Test
```powershell
# This should work if API is running
Invoke-RestMethod http://localhost:5000/api/health/ping

# Expected output:
# success      : True
# errorMessage : 
# data         : pong
```

### Health Check Test
```powershell
Invoke-RestMethod http://localhost:5000/api/health | ConvertTo-Json -Depth 5
```

### Using Browser
```powershell
start http://localhost:5000/api/health
start http://localhost:5000/swagger
```

### Using curl (if installed)
```powershell
curl http://localhost:5000/api/health/ping
```

---

## Port Alternatives

If port 5000 doesn't work, try these:

### Port 5001
```powershell
dotnet run --urls "http://localhost:5001"
# Test: Invoke-RestMethod http://localhost:5001/api/health/ping
```

### Port 8080
```powershell
dotnet run --urls "http://localhost:8080"
# Test: Invoke-RestMethod http://localhost:8080/api/health/ping
```

### Port 3000
```powershell
dotnet run --urls "http://localhost:3000"
# Test: Invoke-RestMethod http://localhost:3000/api/health/ping
```

---

## Check Logs

### Console Output
When you run `dotnet run`, check for:
- ? **Success:** "Now listening on: http://localhost:5000"
- ? **Error:** Any red error messages about binding/ports

### Common Error Messages

**"Address already in use"**
? Port 5000 is taken. Use different port or kill the process.

**"Unable to bind to http://localhost:5000"**
? Permission issue or port conflict. Try different port.

**Database connection error**
? Normal if PostgreSQL not set up. Health endpoints still work!

---

## Nuclear Option - Complete Reset

If nothing works:

```powershell
# 1. Kill all processes
Get-Process | Where-Object {$_.ProcessName -like "*BrewLab*" -or $_.ProcessName -like "*dotnet*"} | Stop-Process -Force

# 2. Clean everything
dotnet clean
Remove-Item -Recurse -Force bin, obj -ErrorAction SilentlyContinue

# 3. Rebuild
dotnet build

# 4. Start on clean port
dotnet run --urls "http://localhost:6000"

# 5. Test
Invoke-RestMethod http://localhost:6000/api/health/ping
```

---

## Verification Checklist

Before testing, verify:

- [ ] Build succeeds: `dotnet build` (should see "Build succeeded")
- [ ] No other process on port 5000
- [ ] API starts without errors
- [ ] Can see "Now listening on: http://localhost:5000" message
- [ ] Health endpoint responds: `Invoke-RestMethod http://localhost:5000/api/health/ping`

---

## Still Not Working?

### Get Detailed Diagnostics
```powershell
# Run this and share the output
Write-Host "=== Diagnostics ===" -ForegroundColor Cyan
Write-Host "Build Status:" -ForegroundColor Yellow
dotnet build --no-restore

Write-Host "`nPort 5000 Status:" -ForegroundColor Yellow
Get-NetTCPConnection -LocalPort 5000 -ErrorAction SilentlyContinue

Write-Host "`nBrewLab Process:" -ForegroundColor Yellow
Get-Process | Where-Object {$_.ProcessName -like "*BrewLab*"}

Write-Host "`nTrying to connect:" -ForegroundColor Yellow
try {
    Invoke-RestMethod http://localhost:5000/api/health/ping -TimeoutSec 3
    Write-Host "? Connection successful!" -ForegroundColor Green
} catch {
    Write-Host "? Connection failed: $($_.Exception.Message)" -ForegroundColor Red
}
```

---

## Quick Reference

| Command | Purpose |
|---------|---------|
| `.\check-api.ps1` | Diagnose issues |
| `.\start-api.ps1` | Start API cleanly |
| `dotnet run` | Run API manually |
| `Invoke-RestMethod http://localhost:5000/api/health/ping` | Test connectivity |
| `Get-NetTCPConnection -LocalPort 5000` | Check if port is listening |
| `Get-Process \| Where-Object {$_.ProcessName -like "*BrewLab*"}` | Check if API is running |

---

## Next Steps After Fix

Once API is accessible:

1. ? Test health endpoint
2. ? Open Swagger UI: `start http://localhost:5000/swagger`
3. ? Run full tests: `.\test-api.ps1` (if database is set up)
4. ? Test your frontend integration

---

**Need immediate help?** Run `.\check-api.ps1` and it will tell you exactly what's wrong!
