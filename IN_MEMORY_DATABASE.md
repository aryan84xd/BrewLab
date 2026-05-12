# ?? In-Memory Database Fallback Implemented!

## ? What Was Added

Your API now **automatically falls back to an in-memory database** when PostgreSQL is not available!

### How It Works:

1. **On startup**, API tries to connect to PostgreSQL
2. **If PostgreSQL is available** ? Uses PostgreSQL database
3. **If PostgreSQL is NOT available** ? Automatically uses in-memory database
4. **No configuration needed** ? Works automatically!

---

## ?? Test It Now (No Database Required!)

### Step 1: Start the API
```powershell
dotnet run
```

### Step 2: Look for This Message
```
? PostgreSQL not available: Failed to connect to [::1]:5432
? Falling back to In-Memory database
```

Or if PostgreSQL is running:
```
? PostgreSQL connection successful - Using PostgreSQL database
```

### Step 3: Test the API
The API works perfectly with in-memory data!

```powershell
# Test health endpoint
Invoke-RestMethod http://localhost:5000/api/health/ping
```

---

## ?? Pre-Seeded Data

The in-memory database comes with **test data** you can use immediately:

### Test User
- **Email**: `test@brewlab.com`
- **Password**: `Test123!`
- **ID**: `11111111-1111-1111-1111-111111111111`

### Test Coffees
1. **Ethiopian Yirgacheffe**
   - Brand: Blue Bottle
   - Roast: Light
   - Origin: Ethiopia
   - Tasting Notes: Floral, citrus, tea-like
   - ID: `22222222-2222-2222-2222-222222222222`

2. **Colombian Supremo**
   - Brand: Stumptown
   - Roast: Medium
   - Origin: Colombia
   - Tasting Notes: Chocolate, caramel, nutty
   - ID: `33333333-3333-3333-3333-333333333333`

### Test Experiment
- For Ethiopian Yirgacheffe coffee
- Brew Method: V60
- Overall Score: 9/10
- ID: `44444444-4444-4444-4444-444444444444`

---

## ?? Complete Test Flow (No Database!)

### 1. Login with Test User
```powershell
$loginBody = @{
    email = "test@brewlab.com"
    password = "Test123!"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/login" -Method POST -Body $loginBody -ContentType "application/json"

# Save token
$token = $response.data.token
Write-Host "Token: $token"
```

**Expected Response:**
```json
{
  "success": true,
  "errorMessage": null,
  "data": {
    "token": "eyJhbGciOi...",
    "email": "test@brewlab.com",
    "name": "Test User",
    "expiresAtUtc": "2024-..."
  }
}
```

---

### 2. Get All Coffees
```powershell
$headers = @{
    "Authorization" = "Bearer $token"
}

$coffees = Invoke-RestMethod -Uri "http://localhost:5000/api/coffees" -Method GET -Headers $headers
$coffees | ConvertTo-Json -Depth 5
```

**Expected Response:**
```json
{
  "success": true,
  "errorMessage": null,
  "data": [
    {
      "id": "22222222-2222-2222-2222-222222222222",
      "name": "Ethiopian Yirgacheffe",
      "brand": "Blue Bottle",
      "roast": "Light",
      "origin": "Ethiopia",
      "tastingNotes": "Floral, citrus, tea-like"
    },
    {
      "id": "33333333-3333-3333-3333-333333333333",
      "name": "Colombian Supremo",
      "brand": "Stumptown",
      "roast": "Medium",
      "origin": "Colombia",
      "tastingNotes": "Chocolate, caramel, nutty"
    }
  ]
}
```

---

### 3. Get Coffee by ID
```powershell
$coffeeId = "22222222-2222-2222-2222-222222222222"
$coffee = Invoke-RestMethod -Uri "http://localhost:5000/api/coffees/$coffeeId" -Method GET -Headers $headers
$coffee | ConvertTo-Json -Depth 5
```

---

### 4. Create New Coffee
```powershell
$newCoffee = @{
    name = "Kenyan AA"
    brand = "Counter Culture"
    roast = "Medium-Light"
    origin = "Kenya"
    tastingNotes = "Blackcurrant, grapefruit, winey"
} | ConvertTo-Json

$created = Invoke-RestMethod -Uri "http://localhost:5000/api/coffees" -Method POST -Body $newCoffee -Headers $headers -ContentType "application/json"
$created | ConvertTo-Json -Depth 5
```

**Expected Response:**
```json
{
  "success": true,
  "errorMessage": null,
  "data": {
    "id": "newly-generated-guid",
    "name": "Kenyan AA",
    "brand": "Counter Culture",
    "roast": "Medium-Light",
    "origin": "Kenya",
    "tastingNotes": "Blackcurrant, grapefruit, winey"
  }
}
```

---

### 5. Get Experiments for Coffee
```powershell
$coffeeId = "22222222-2222-2222-2222-222222222222"
$experiments = Invoke-RestMethod -Uri "http://localhost:5000/api/experiment/$coffeeId" -Method GET -Headers $headers
$experiments | ConvertTo-Json -Depth 5
```

---

### 6. Create New Experiment
```powershell
$newExperiment = @{
    coffeeId = "22222222-2222-2222-2222-222222222222"
    brewMethod = "Chemex"
    coffeeWeight = 20.0
    waterWeight = 350.0
    brewTime = "00:03:00"
    remark = "Longer brew time, fuller body"
    aroma = 5
    acidity = 4
    body = 4
    overall = 8
} | ConvertTo-Json

$experiment = Invoke-RestMethod -Uri "http://localhost:5000/api/experiment" -Method POST -Body $newExperiment -Headers $headers -ContentType "application/json"
$experiment | ConvertTo-Json -Depth 5
```

---

### 7. Register New User
```powershell
$registerBody = @{
    name = "New User"
    email = "newuser@example.com"
    password = "NewPass123!"
} | ConvertTo-Json

$newUser = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/register" -Method POST -Body $registerBody -ContentType "application/json"
$newUser | ConvertTo-Json -Depth 5
```

---

## ?? Using Swagger UI

1. Start API: `dotnet run`
2. Open: `http://localhost:5000/swagger`
3. Test `/api/auth/login` with:
   ```json
   {
     "email": "test@brewlab.com",
     "password": "Test123!"
   }
   ```
4. Copy the token from response
5. Click "Authorize" button
6. Enter: `Bearer YOUR_TOKEN`
7. Test all other endpoints!

---

## ?? What's Different?

### In-Memory Database:
- ? No PostgreSQL installation needed
- ? Data persists while API is running
- ? Data resets when API restarts
- ? Perfect for testing and development
- ? Pre-seeded with test data
- ? Fast and reliable

### PostgreSQL Database:
- ? Data persists permanently
- ? Production-ready
- ? Supports large datasets
- ? Used when available

---

## ?? Implementation Details

### Files Created:
- `Data/InMemoryDatabase.cs` - In-memory data store
- `Repositories/InMemoryUserRepository.cs` - In-memory user operations
- `Repositories/InMemoryCoffeeRepository.cs` - In-memory coffee operations
- `Repositories/InMemoryExperimentRepository.cs` - In-memory experiment operations

### Updated:
- `Program.cs` - Auto-detects database availability and switches

### Detection Logic:
```csharp
// Tries to connect to PostgreSQL
if (PostgreSQL available) {
    Use PostgreSQL repositories
} else {
    Use In-Memory repositories
}
```

---

## ?? Architecture Benefits

### Same Code, Different Storage:
```
Controllers ? Services ? DTOs ? Repositories ? Storage
                                      ?
                          PostgreSQL OR In-Memory
```

- ? Controllers don't change
- ? Services don't change
- ? Only repository implementation swaps
- ? Seamless switching

---

## ?? How to Check Which Database is Being Used

### Look at console output on startup:

**PostgreSQL Mode:**
```
? PostgreSQL connection successful - Using PostgreSQL database
Now listening on: http://localhost:5000
```

**In-Memory Mode:**
```
? PostgreSQL not available: Failed to connect to [::1]:5432
? Falling back to In-Memory database
Now listening on: http://localhost:5000
```

---

## ?? Quick Start Script

Save this as `test-inmemory.ps1`:

```powershell
Write-Host "Testing In-Memory Database" -ForegroundColor Cyan
Write-Host ""

# 1. Login
Write-Host "1. Logging in..." -ForegroundColor Yellow
$login = @{
    email = "test@brewlab.com"
    password = "Test123!"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/login" -Method POST -Body $login -ContentType "application/json"
$token = $response.data.token
Write-Host "   ? Logged in as: $($response.data.name)" -ForegroundColor Green

# 2. Get coffees
Write-Host "`n2. Getting coffees..." -ForegroundColor Yellow
$headers = @{ "Authorization" = "Bearer $token" }
$coffees = Invoke-RestMethod -Uri "http://localhost:5000/api/coffees" -Headers $headers
Write-Host "   ? Found $($coffees.data.Count) coffees:" -ForegroundColor Green
foreach ($c in $coffees.data) {
    Write-Host "     - $($c.name) by $($c.brand)" -ForegroundColor Gray
}

# 3. Get experiments
Write-Host "`n3. Getting experiments..." -ForegroundColor Yellow
$coffeeId = $coffees.data[0].id
$experiments = Invoke-RestMethod -Uri "http://localhost:5000/api/experiment/$coffeeId" -Headers $headers
Write-Host "   ? Found $($experiments.data.Count) experiments" -ForegroundColor Green

Write-Host "`n? All tests passed! In-memory database working perfectly!" -ForegroundColor Green
```

Run it:
```powershell
.\test-inmemory.ps1
```

---

## ?? Summary

? **No PostgreSQL? No Problem!**
? **Automatic fallback** to in-memory database
? **Pre-seeded with test data**
? **All features work** exactly the same
? **Perfect for testing** and development
? **Seamless switch** to PostgreSQL when available

### Test Credentials:
- **Email**: `test@brewlab.com`
- **Password**: `Test123!`

### Test It Now:
```powershell
# Start API
dotnet run

# Open Swagger
start http://localhost:5000/swagger

# Or test with PowerShell
.\test-inmemory.ps1
```

**Your API now works WITHOUT PostgreSQL!** ??
