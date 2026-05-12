# ?? Quick Start Guide

## Setup in 3 Steps

### 1?? Setup Database
```powershell
# Run the setup script
.\setup-database.ps1
```

Or manually:
- Create PostgreSQL database named `brewlab`
- Run SQL script from `Database/setup.sql`

### 2?? Run the API
```powershell
dotnet run
```

### 3?? Test the API
```powershell
# In a new terminal
.\test-api.ps1
```

Or open Swagger UI:
```
http://localhost:5000/swagger
```

---

## Prerequisites

- ? .NET 9 SDK (already installed)
- ? PostgreSQL (with default settings: localhost:5432, user: postgres, password: postgres)

---

## Configuration

If your PostgreSQL uses different settings, update `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=YOUR_HOST;Database=brewlab;Username=YOUR_USER;Password=YOUR_PASSWORD;Port=YOUR_PORT"
  }
}
```

---

## What's New? ??

### All API responses now return HTTP 200 with this structure:

**Success:**
```json
{
  "success": true,
  "errorMessage": null,
  "data": { ... }
}
```

**Error:**
```json
{
  "success": false,
  "errorMessage": "Error description",
  "data": null
}
```

### Architecture Flow:
```
Request ? DTO ? DBO ? Database ? DBO ? DTO ? Response
```

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login and get token |
| GET | `/api/auth/me` | Get current user |
| GET | `/api/coffees` | Get all coffees |
| GET | `/api/coffees/{id}` | Get coffee by ID |
| POST | `/api/coffees` | Create new coffee |
| GET | `/api/experiment/{coffeeId}` | Get experiments |
| POST | `/api/experiment` | Create experiment |

---

## Testing Examples

### Using PowerShell:

```powershell
# 1. Register
$body = @{
    name = "Test User"
    email = "test@example.com"
    password = "Test123!"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/register" -Method POST -Body $body -ContentType "application/json"

# 2. Check response
$response.success  # true/false
$response.data     # user data with token
$response.errorMessage  # error if failed
```

### Using curl:

```bash
# Register
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"name":"Test","email":"test@example.com","password":"Test123!"}'

# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test123!"}'
```

---

## Troubleshooting

### PostgreSQL not running?
```powershell
# Check service
Get-Service -Name postgresql*

# Or start it
Start-Service postgresql-x64-16  # adjust version
```

### Port 5000 already in use?
```powershell
dotnet run --urls "http://localhost:5001"
```

### Database connection failed?
- Check PostgreSQL is running
- Verify credentials in `appsettings.Development.json`
- Test connection: `psql -U postgres -d brewlab`

---

## Documentation

- ?? [Architecture Changes](./ARCHITECTURE_CHANGES.md) - Detailed architecture guide
- ?? [API Examples](./API_RESPONSE_EXAMPLES.md) - Complete API response examples
- ?? [Testing Guide](./LOCAL_TESTING_GUIDE.md) - Comprehensive testing guide

---

## Quick Commands

```powershell
# Setup everything
.\setup-database.ps1
dotnet run

# In another terminal, test it
.\test-api.ps1

# Or use Swagger
start http://localhost:5000/swagger
```

---

## Need Help?

- Check logs in the terminal where `dotnet run` is running
- Try the test script: `.\test-api.ps1`
- Open Swagger UI for interactive testing
- Review [LOCAL_TESTING_GUIDE.md](./LOCAL_TESTING_GUIDE.md)

---

Happy Brewing! ?
