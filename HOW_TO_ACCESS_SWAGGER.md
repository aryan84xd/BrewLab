# ?? How to Access Swagger UI

## Current Status: API is NOT running

You need to start the API first!

---

## ? Step-by-Step Solution

### Step 1: Start the API

**Option A: Use the startup script (Recommended)**
```powershell
.\start-api.ps1
```

**Option B: Run directly**
```powershell
dotnet run
```

**Option C: Run from Visual Studio**
- Press `F5` (Run with debugging)
- Or press `Ctrl+F5` (Run without debugging)

---

### Step 2: Wait for Startup Message

Look for this in the console:
```
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
Hosting environment: Development
```

?? **IMPORTANT**: Keep this window/terminal open! If you close it, the API stops.

---

### Step 3: Open Swagger UI

**Option A: Use the script**
```powershell
# In a NEW PowerShell window (don't close the API window!)
.\open-swagger.ps1
```

**Option B: Open manually in browser**
- Go to: http://localhost:5000/swagger
- Or: http://127.0.0.1:5000/swagger

**Option C: Use PowerShell command**
```powershell
start http://localhost:5000/swagger
```

---

## ?? Why Swagger Wasn't Working

The Swagger configuration is **correct and installed**! The issue is:

? **API was not running** - You can't access Swagger if the API isn't running
? **API window was closed** - Closing the terminal/console stops the API

---

## ? Swagger IS Configured

Your `Program.cs` has:
```csharp
app.UseSwagger();           // ? Enables Swagger JSON
app.UseSwaggerUI();         // ? Enables Swagger UI
```

All the necessary packages are installed:
- ? `Swashbuckle.AspNetCore.Swagger`
- ? `Swashbuckle.AspNetCore.SwaggerGen`
- ? `Swashbuckle.AspNetCore.SwaggerUI`

Nothing was removed! Swagger is fully functional.

---

## ?? Complete Workflow

### 1. **Start API** (Terminal 1)
```powershell
cd C:\Users\ILOM43095\source\repos\aryan84xd\BrewLab
dotnet run
```

**Keep this terminal open!** ? This is crucial

---

### 2. **Test API** (Terminal 2 - NEW window)
```powershell
# Quick test
Invoke-RestMethod http://localhost:5000/api/health/ping

# Expected output:
# success      : True
# errorMessage : 
# data         : pong
```

---

### 3. **Open Swagger** (Terminal 2 or Browser)
```powershell
start http://localhost:5000/swagger
```

---

## ?? All Available URLs

Once API is running, you can access:

| URL | Purpose |
|-----|---------|
| `http://localhost:5000/swagger` | **Swagger UI** - Interactive API docs |
| `http://localhost:5000/swagger/v1/swagger.json` | OpenAPI JSON spec |
| `http://localhost:5000/api/health` | Health check (shows new architecture) |
| `http://localhost:5000/api/health/ping` | Quick ping test |
| `http://localhost:5000/api/health/error-test` | Error response test |
| `http://localhost:5000/api/auth/register` | Register endpoint |
| `http://localhost:5000/api/auth/login` | Login endpoint |
| `http://localhost:5000/api/coffees` | Coffee endpoints |
| `http://localhost:5000/api/experiment` | Experiment endpoints |

---

## ?? What You'll See in Swagger UI

When you open Swagger, you'll see:

### 1. **Auth Controller**
- `POST /api/auth/register` - Register user
- `POST /api/auth/login` - Login
- `GET /api/auth/me` - Get current user

### 2. **Coffees Controller**
- `GET /api/coffees` - Get all coffees
- `GET /api/coffees/{id}` - Get coffee by ID
- `POST /api/coffees` - Create coffee

### 3. **Experiment Controller**
- `GET /api/experiment/{coffeeId}` - Get experiments
- `POST /api/experiment` - Create experiment

### 4. **Health Controller** (NEW!)
- `GET /api/health` - Health check
- `GET /api/health/error-test` - Test error response
- `GET /api/health/ping` - Ping test

### 5. **New Response Format**
All responses show the `ApiResponse<T>` schema:
```json
{
  "success": true/false,
  "errorMessage": "string",
  "data": { ... }
}
```

---

## ?? Troubleshooting

### Issue: "This site can't be reached"

**Cause**: API is not running

**Fix**:
```powershell
# Check if API is running
Get-Process | Where-Object {$_.ProcessName -like "*BrewLab*"}

# If nothing returned, start it:
dotnet run
```

---

### Issue: "Cannot GET /swagger"

**Cause**: Using wrong URL

**Fix**: Use the correct URLs:
- ? `http://localhost:5000/swagger` (lowercase)
- ? NOT `http://localhost:5000/Swagger`
- ? NOT `http://localhost:5000/swagger/`

---

### Issue: Swagger loads but shows "Failed to load API definition"

**Cause**: API crashed or database error

**Fix**:
1. Check console where API is running for errors
2. Health endpoints work without database:
   ```powershell
   Invoke-RestMethod http://localhost:5000/api/health/ping
   ```

---

### Issue: Can't access from Visual Studio browser

**Cause**: VS might open with HTTPS or wrong port

**Fix**:
1. Check the URL in VS browser
2. Manually navigate to: `http://localhost:5000/swagger`
3. Or use PowerShell: `start http://localhost:5000/swagger`

---

## ?? Quick Checklist

Before accessing Swagger, verify:

- [ ] API is running (`dotnet run` or press F5 in VS)
- [ ] Console shows "Now listening on: http://localhost:5000"
- [ ] API terminal/window is still open
- [ ] Can ping API: `Invoke-RestMethod http://localhost:5000/api/health/ping`
- [ ] Using correct URL: `http://localhost:5000/swagger` (lowercase)

---

## ?? Right Now - Do This:

### Terminal 1 (Start API):
```powershell
cd C:\Users\ILOM43095\source\repos\aryan84xd\BrewLab
dotnet run
```

**Wait for**: "Now listening on: http://localhost:5000"

### Terminal 2 (Test & Open):
```powershell
# Test API is responding
Invoke-RestMethod http://localhost:5000/api/health/ping

# Open Swagger
start http://localhost:5000/swagger

# Open Health endpoint
start http://localhost:5000/api/health
```

---

## ? Expected Results

### In Console (Terminal 1):
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
```

### In Browser (Swagger):
- You'll see the Swagger UI with all endpoints
- Green "Authorize" button at the top
- All controllers listed with their endpoints
- Each endpoint shows the new `ApiResponse<T>` format

### In Browser (Health):
```json
{
  "success": true,
  "errorMessage": null,
  "data": {
    "status": "healthy",
    "message": "BrewLab API is running with new architecture!",
    "timestamp": "2024-01-15T...",
    "version": "2.0",
    "architecture": "Request ? DTO ? DBO ? DB ? DBO ? DTO ? Response",
    "features": [
      "All responses return HTTP 200",
      "Success/Error fields in response body",
      "Clean layer separation",
      "No circular dependencies"
    ]
  }
}
```

---

## ?? Still Not Working?

Run the diagnostic:
```powershell
.\check-api.ps1
```

It will tell you exactly what's wrong and how to fix it.

---

**TL;DR**: Swagger IS there! You just need to **keep the API running** and access `http://localhost:5000/swagger` ??
