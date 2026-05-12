# Local Testing Guide

## Prerequisites

1. **PostgreSQL Database**
   - Install PostgreSQL if you don't have it: https://www.postgresql.org/download/
   - Default port: 5432
   - Default credentials: postgres/postgres

2. **.NET 9 SDK**
   - Already installed (your project builds successfully)

---

## Setup Steps

### Step 1: Configure PostgreSQL

**Option A: Use Default Settings**
If your PostgreSQL uses:
- Host: `localhost`
- Port: `5432`
- Username: `postgres`
- Password: `postgres`

Then you're all set! The `appsettings.Development.json` is already configured.

**Option B: Custom PostgreSQL Settings**
Edit `appsettings.Development.json` and update the connection string:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=YOUR_HOST;Database=brewlab;Username=YOUR_USERNAME;Password=YOUR_PASSWORD;Port=YOUR_PORT"
}
```

### Step 2: Create Database

Open PostgreSQL command line (psql) or pgAdmin and run:

```sql
CREATE DATABASE brewlab;
```

Or use PowerShell:
```powershell
# Using psql command
psql -U postgres -c "CREATE DATABASE brewlab;"
```

### Step 3: Create Tables

Run this SQL script in your `brewlab` database:

```sql
-- Create Users table
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" UUID PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Email" VARCHAR(255) NOT NULL UNIQUE,
    "PasswordHash" VARCHAR(500) NOT NULL
);

-- Create Coffees table
CREATE TABLE IF NOT EXISTS "Coffees" (
    "Id" UUID PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Brand" VARCHAR(255) NOT NULL,
    "Roast" VARCHAR(100) NOT NULL,
    "Origin" VARCHAR(255),
    "TastingNotes" TEXT,
    "UserId" UUID NOT NULL,
    FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
);

-- Create Experiments table
CREATE TABLE IF NOT EXISTS "Experiments" (
    "Id" UUID PRIMARY KEY,
    "Date" TIMESTAMP NOT NULL,
    "BrewMethod" VARCHAR(100),
    "CoffeeWeight" DECIMAL(10,2) NOT NULL,
    "WaterWeight" DECIMAL(10,2) NOT NULL,
    "BrewTime" TIME NOT NULL,
    "Remark" TEXT,
    "Aroma" INTEGER NOT NULL,
    "Acidity" INTEGER NOT NULL,
    "Body" INTEGER NOT NULL,
    "Overall" INTEGER NOT NULL,
    "CoffeeId" UUID NOT NULL,
    "UserId" UUID NOT NULL,
    FOREIGN KEY ("CoffeeId") REFERENCES "Coffees"("Id") ON DELETE CASCADE,
    FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS "IX_Coffees_UserId" ON "Coffees"("UserId");
CREATE INDEX IF NOT EXISTS "IX_Experiments_CoffeeId" ON "Experiments"("CoffeeId");
CREATE INDEX IF NOT EXISTS "IX_Experiments_UserId" ON "Experiments"("UserId");
```

### Step 4: Run the Application

Open terminal in your project directory and run:

```powershell
dotnet run
```

Or use Visual Studio:
- Press `F5` or click the "Run" button
- Or press `Ctrl+F5` to run without debugging

The application will start on:
- HTTP: `http://localhost:5000`
- Swagger UI: `http://localhost:5000/swagger`

---

## Testing the API

### Option 1: Using Swagger UI (Recommended)

1. Open browser: `http://localhost:5000/swagger`
2. You'll see all endpoints with the new `ApiResponse` wrapper
3. Test the flow:
   - Register a user: `POST /api/auth/register`
   - Login: `POST /api/auth/login` (copy the token)
   - Click "Authorize" button, enter: `Bearer YOUR_TOKEN`
   - Test protected endpoints: `POST /api/coffees`, `GET /api/coffees`, etc.

### Option 2: Using PowerShell

**1. Register a user:**
```powershell
$registerBody = @{
    name = "Test User"
    email = "test@example.com"
    password = "Test123!"
} | ConvertTo-Json

$registerResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/register" -Method POST -Body $registerBody -ContentType "application/json"

# Check response
$registerResponse
```

**2. Login and get token:**
```powershell
$loginBody = @{
    email = "test@example.com"
    password = "Test123!"
} | ConvertTo-Json

$loginResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/login" -Method POST -Body $loginBody -ContentType "application/json"

# Check response
$loginResponse

# Save token for next requests
$token = $loginResponse.data.token
```

**3. Create a coffee:**
```powershell
$coffeeBody = @{
    name = "Ethiopian Yirgacheffe"
    brand = "Blue Bottle"
    roast = "Light"
    origin = "Ethiopia"
    tastingNotes = "Floral, citrus, tea-like"
} | ConvertTo-Json

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

$coffeeResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/coffees" -Method POST -Body $coffeeBody -Headers $headers

# Check response - should have success=true
$coffeeResponse
```

**4. Get all coffees:**
```powershell
$coffeesResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/coffees" -Method GET -Headers $headers

# Check response
$coffeesResponse
```

**5. Test error scenario (invalid ID):**
```powershell
$invalidId = [Guid]::NewGuid()
$errorResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/coffees/$invalidId" -Method GET -Headers $headers

# Should return success=false with errorMessage
$errorResponse
```

### Option 3: Using Postman/Insomnia

Import this collection or create requests manually:

**Base URL:** `http://localhost:5000`

**Endpoints:**
- `POST /api/auth/register` - Register user
- `POST /api/auth/login` - Login (get token)
- `GET /api/auth/me` - Get current user (requires auth)
- `GET /api/coffees` - Get all coffees (requires auth)
- `GET /api/coffees/{id}` - Get coffee by ID (requires auth)
- `POST /api/coffees` - Create coffee (requires auth)
- `GET /api/experiment/{coffeeId}` - Get experiments (requires auth)
- `POST /api/experiment` - Create experiment (requires auth)

---

## Expected Response Format

All endpoints now return:

```json
{
  "success": true,
  "errorMessage": null,
  "data": { ... }
}
```

Or on error:

```json
{
  "success": false,
  "errorMessage": "Error description",
  "data": null
}
```

---

## Troubleshooting

### Issue: "Cannot connect to database"

**Solution:**
1. Check PostgreSQL is running:
   ```powershell
   Get-Service -Name postgresql*
   ```
2. Verify connection string in `appsettings.Development.json`
3. Test connection manually:
   ```powershell
   psql -U postgres -d brewlab
   ```

### Issue: "JWT Key not configured"

**Solution:**
The JWT key is already configured in `appsettings.Development.json`. Make sure you're running in Development mode:
```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run
```

### Issue: "Tables don't exist"

**Solution:**
Run the SQL script from Step 3 to create tables.

### Issue: "Port 5000 already in use"

**Solution:**
Change the port in command line:
```powershell
dotnet run --urls "http://localhost:5001"
```

---

## Quick Test Script

Save this as `test-api.ps1` and run it:

```powershell
# Test complete flow
$baseUrl = "http://localhost:5000"

Write-Host "1. Registering user..." -ForegroundColor Cyan
$registerBody = @{
    name = "Test User"
    email = "test$(Get-Random)@example.com"  # Random email
    password = "Test123!"
} | ConvertTo-Json

$register = Invoke-RestMethod -Uri "$baseUrl/api/auth/register" -Method POST -Body $registerBody -ContentType "application/json"
Write-Host "Success: $($register.success)" -ForegroundColor Green
$token = $register.data.token

Write-Host "`n2. Creating coffee..." -ForegroundColor Cyan
$coffeeBody = @{
    name = "Test Coffee"
    brand = "Test Brand"
    roast = "Medium"
    origin = "Test Origin"
    tastingNotes = "Delicious"
} | ConvertTo-Json

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

$coffee = Invoke-RestMethod -Uri "$baseUrl/api/coffees" -Method POST -Body $coffeeBody -Headers $headers
Write-Host "Success: $($coffee.success)" -ForegroundColor Green
$coffeeId = $coffee.data.id

Write-Host "`n3. Getting all coffees..." -ForegroundColor Cyan
$coffees = Invoke-RestMethod -Uri "$baseUrl/api/coffees" -Method GET -Headers $headers
Write-Host "Success: $($coffees.success)" -ForegroundColor Green
Write-Host "Coffee count: $($coffees.data.Count)" -ForegroundColor Yellow

Write-Host "`n4. Testing error scenario (invalid ID)..." -ForegroundColor Cyan
$invalidId = [Guid]::NewGuid()
$error = Invoke-RestMethod -Uri "$baseUrl/api/coffees/$invalidId" -Method GET -Headers $headers
Write-Host "Success: $($error.success)" -ForegroundColor $(if($error.success){"Green"}else{"Yellow"})
Write-Host "Error Message: $($error.errorMessage)" -ForegroundColor Yellow

Write-Host "`n? All tests completed!" -ForegroundColor Green
```

---

## Next Steps

1. Start the application: `dotnet run`
2. Open Swagger: `http://localhost:5000/swagger`
3. Test the new architecture with success/error responses
4. Check that all responses return HTTP 200
5. Verify that errors have `success: false` and `errorMessage` fields

Happy Testing! ??
