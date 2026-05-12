# ?? Complete Setup Guide - No PostgreSQL Yet

## You're Almost There! Here's what you need:

### Step 1: Install PostgreSQL

**Option A: Using Windows Installer (Recommended)**

1. Download PostgreSQL from: https://www.postgresql.org/download/windows/
2. Run the installer
3. During installation:
   - Set password for postgres user (remember this!)
   - Default port: 5432 (keep this)
   - Install Stack Builder (optional)
4. Add PostgreSQL to PATH (installer usually does this)

**Option B: Using Chocolatey**
```powershell
# If you have Chocolatey installed
choco install postgresql
```

**Option C: Using Docker (Alternative)**
```powershell
# If you prefer Docker
docker run --name brewlab-postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:16
```

---

### Step 2: Verify PostgreSQL Installation

```powershell
# Check if service is running
Get-Service -Name postgresql*

# Or check if psql command works
psql --version
```

If `psql` doesn't work, add PostgreSQL to your PATH:
```powershell
# Find PostgreSQL installation directory (usually)
# C:\Program Files\PostgreSQL\16\bin

# Add to PATH temporarily
$env:Path += ";C:\Program Files\PostgreSQL\16\bin"

# Or permanently (run as Administrator)
[Environment]::SetEnvironmentVariable(
    "Path",
    [Environment]::GetEnvironmentVariable("Path", "Machine") + ";C:\Program Files\PostgreSQL\16\bin",
    "Machine"
)
```

---

### Step 3: Setup Database

Once PostgreSQL is installed and running:

**Automated Setup:**
```powershell
.\setup-database.ps1
```

**Manual Setup:**
1. Open pgAdmin (installed with PostgreSQL)
2. Connect to localhost server
3. Right-click "Databases" ? Create ? Database
4. Name it: `brewlab`
5. Right-click `brewlab` ? Query Tool
6. Copy and paste contents of `Database/setup.sql`
7. Click Execute (F5)

**Or using psql:**
```powershell
# Create database
psql -U postgres -c "CREATE DATABASE brewlab;"

# Run setup script
psql -U postgres -d brewlab -f Database\setup.sql
```

---

### Step 4: Update Configuration (if needed)

Edit `appsettings.Development.json` with your PostgreSQL password:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=brewlab;Username=postgres;Password=YOUR_PASSWORD;Port=5432"
  }
}
```

---

### Step 5: Run the Application

```powershell
dotnet run
```

You should see:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://0.0.0.0:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

---

### Step 6: Test the API

**Open Swagger UI:**
```powershell
start http://localhost:5000/swagger
```

**Or run automated tests:**
```powershell
# In a new terminal
.\test-api.ps1
```

---

## Alternative: Test Without Database First

If you want to test the code changes without setting up database, you can create a simple in-memory version:

**Create a test controller:**

```powershell
# Create a simple test endpoint
Add-Content -Path "Controllers\HealthController.cs" -Value @"
using Microsoft.AspNetCore.Mvc;
using BrewLab.Models.Common;

namespace BrewLab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public ActionResult<ApiResponse<object>> Get()
        {
            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                status = "healthy",
                message = "API is running with new architecture!",
                timestamp = DateTime.UtcNow
            }));
        }

        [HttpGet("error")]
        public ActionResult<ApiResponse<object>> GetError()
        {
            return Ok(ApiResponse<object>.FailureResponse("This is a test error"));
        }
    }
}
"@
```

Then test:
```powershell
dotnet run

# In another terminal
Invoke-RestMethod http://localhost:5000/api/health
```

You should see:
```json
{
  "success": true,
  "errorMessage": null,
  "data": {
    "status": "healthy",
    "message": "API is running with new architecture!",
    "timestamp": "2024-01-15T10:30:00Z"
  }
}
```

---

## Quick Reference

### PostgreSQL Default Settings:
- Host: `localhost`
- Port: `5432`
- Username: `postgres`
- Password: (you set during installation)
- Database: `brewlab`

### Common PostgreSQL Commands:
```powershell
# Connect to PostgreSQL
psql -U postgres

# List databases
\l

# Connect to brewlab database
\c brewlab

# List tables
\dt

# Quit
\q
```

### Project Commands:
```powershell
# Build project
dotnet build

# Run project
dotnet run

# Clean build
dotnet clean
dotnet build

# Run on different port
dotnet run --urls "http://localhost:5001"
```

---

## Troubleshooting

### Issue: "psql not found"
**Solution:** Add PostgreSQL bin directory to PATH (see Step 2)

### Issue: "Password authentication failed"
**Solution:** Update password in `appsettings.Development.json`

### Issue: "Database brewlab does not exist"
**Solution:** Run `.\setup-database.ps1` or create manually

### Issue: "Port 5000 already in use"
**Solution:** Kill the process or use different port:
```powershell
# Find what's using port 5000
netstat -ano | findstr :5000

# Kill process (replace PID)
taskkill /PID <PID> /F

# Or use different port
dotnet run --urls "http://localhost:5001"
```

### Issue: "JWT Key not configured"
**Solution:** It's already configured in `appsettings.Development.json`. Make sure you're in Development mode:
```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run
```

---

## What's Different Now?

? **All endpoints return HTTP 200**
- No more 404, 401, 409 status codes
- Errors are indicated by `success: false` in response body

? **Clean Architecture**
- Request ? DTO ? DBO ? Database ? DBO ? DTO ? Response
- No circular dependencies

? **Consistent Error Handling**
- All responses have `success`, `errorMessage`, and `data` fields
- Frontend can easily check if operation succeeded

? **Zero Database Changes**
- Your existing database works as-is
- No migrations needed

---

## Next Steps

1. ? Install PostgreSQL
2. ? Setup database (`.\setup-database.ps1`)
3. ? Run application (`dotnet run`)
4. ? Test API (`.\test-api.ps1`)
5. ? Update frontend to check `success` field

---

## Need More Help?

- ?? [QUICKSTART.md](./QUICKSTART.md) - Quick reference
- ?? [LOCAL_TESTING_GUIDE.md](./LOCAL_TESTING_GUIDE.md) - Detailed testing guide
- ?? [API_RESPONSE_EXAMPLES.md](./API_RESPONSE_EXAMPLES.md) - API examples
- ??? [ARCHITECTURE_CHANGES.md](./ARCHITECTURE_CHANGES.md) - Architecture details

---

Happy Coding! ??
