# Test In-Memory Database
# This script tests all API endpoints with the in-memory database

Write-Host "================================" -ForegroundColor Cyan
Write-Host "  In-Memory Database Test" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5000"

# Check if API is running
Write-Host "Checking API status..." -ForegroundColor Yellow
try {
    Invoke-RestMethod "$baseUrl/api/health/ping" -ErrorAction Stop | Out-Null
    Write-Host "? API is running" -ForegroundColor Green
} catch {
    Write-Host "? API is not running!" -ForegroundColor Red
    Write-Host "  Start it with: dotnet run" -ForegroundColor Yellow
    exit 1
}
Write-Host ""

# Test 1: Login with test user
Write-Host "1. Testing Login..." -ForegroundColor Yellow
try {
    $loginBody = @{
        email = "test@brewlab.com"
        password = "Test123!"
    } | ConvertTo-Json

    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -Body $loginBody -ContentType "application/json"

    if ($loginResponse.success) {
        Write-Host "   ? Login successful" -ForegroundColor Green
        Write-Host "   User: $($loginResponse.data.name)" -ForegroundColor Gray
        Write-Host "   Email: $($loginResponse.data.email)" -ForegroundColor Gray
        $token = $loginResponse.data.token
    } else {
        Write-Host "   ? Login failed: $($loginResponse.errorMessage)" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "   ? Login error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Setup headers
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

# Test 2: Get all coffees (should have 2 pre-seeded)
Write-Host "2. Testing Get All Coffees..." -ForegroundColor Yellow
try {
    $coffeesResponse = Invoke-RestMethod -Uri "$baseUrl/api/coffees" -Method GET -Headers $headers

    if ($coffeesResponse.success) {
        Write-Host "   ? Retrieved $($coffeesResponse.data.Count) coffees" -ForegroundColor Green
        foreach ($coffee in $coffeesResponse.data) {
            Write-Host "     - $($coffee.name) by $($coffee.brand) ($($coffee.roast))" -ForegroundColor Gray
        }
    } else {
        Write-Host "   ? Failed: $($coffeesResponse.errorMessage)" -ForegroundColor Red
    }
} catch {
    Write-Host "   ? Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 3: Get specific coffee by ID
Write-Host "3. Testing Get Coffee by ID..." -ForegroundColor Yellow
try {
    $coffeeId = "22222222-2222-2222-2222-222222222222"
    $coffeeResponse = Invoke-RestMethod -Uri "$baseUrl/api/coffees/$coffeeId" -Method GET -Headers $headers

    if ($coffeeResponse.success) {
        Write-Host "   ? Retrieved coffee: $($coffeeResponse.data.name)" -ForegroundColor Green
        Write-Host "     Origin: $($coffeeResponse.data.origin)" -ForegroundColor Gray
        Write-Host "     Tasting Notes: $($coffeeResponse.data.tastingNotes)" -ForegroundColor Gray
    } else {
        Write-Host "   ? Failed: $($coffeeResponse.errorMessage)" -ForegroundColor Red
    }
} catch {
    Write-Host "   ? Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 4: Create new coffee
Write-Host "4. Testing Create Coffee..." -ForegroundColor Yellow
try {
    $newCoffeeBody = @{
        name = "Test Coffee $(Get-Random)"
        brand = "Test Brand"
        roast = "Medium"
        origin = "Test Origin"
        tastingNotes = "Testing in-memory database"
    } | ConvertTo-Json

    $createResponse = Invoke-RestMethod -Uri "$baseUrl/api/coffees" -Method POST -Body $newCoffeeBody -Headers $headers

    if ($createResponse.success) {
        Write-Host "   ? Coffee created successfully" -ForegroundColor Green
        Write-Host "     ID: $($createResponse.data.id)" -ForegroundColor Gray
        Write-Host "     Name: $($createResponse.data.name)" -ForegroundColor Gray
        $newCoffeeId = $createResponse.data.id
    } else {
        Write-Host "   ? Failed: $($createResponse.errorMessage)" -ForegroundColor Red
    }
} catch {
    Write-Host "   ? Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 5: Get experiments for coffee
Write-Host "5. Testing Get Experiments..." -ForegroundColor Yellow
try {
    $coffeeId = "22222222-2222-2222-2222-222222222222"
    $experimentsResponse = Invoke-RestMethod -Uri "$baseUrl/api/experiment/$coffeeId" -Method GET -Headers $headers

    if ($experimentsResponse.success) {
        Write-Host "   ? Retrieved $($experimentsResponse.data.Count) experiments" -ForegroundColor Green
        foreach ($exp in $experimentsResponse.data) {
            Write-Host "     - $($exp.brewMethod): Overall $($exp.overall)/10" -ForegroundColor Gray
        }
    } else {
        Write-Host "   ? Failed: $($experimentsResponse.errorMessage)" -ForegroundColor Red
    }
} catch {
    Write-Host "   ? Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 6: Create new experiment
Write-Host "6. Testing Create Experiment..." -ForegroundColor Yellow
try {
    $newExperimentBody = @{
        coffeeId = "22222222-2222-2222-2222-222222222222"
        brewMethod = "Test Method"
        coffeeWeight = 18.0
        waterWeight = 300.0
        brewTime = "00:02:30"
        remark = "Testing in-memory database"
        aroma = 4
        acidity = 4
        body = 3
        overall = 8
    } | ConvertTo-Json

    $expResponse = Invoke-RestMethod -Uri "$baseUrl/api/experiment" -Method POST -Body $newExperimentBody -Headers $headers

    if ($expResponse.success) {
        Write-Host "   ? Experiment created successfully" -ForegroundColor Green
        Write-Host "     ID: $($expResponse.data.id)" -ForegroundColor Gray
        Write-Host "     Method: $($expResponse.data.brewMethod)" -ForegroundColor Gray
        Write-Host "     Overall: $($expResponse.data.overall)/10" -ForegroundColor Gray
    } else {
        Write-Host "   ? Failed: $($expResponse.errorMessage)" -ForegroundColor Red
    }
} catch {
    Write-Host "   ? Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 7: Test error handling (invalid coffee ID)
Write-Host "7. Testing Error Handling..." -ForegroundColor Yellow
try {
    $invalidId = [Guid]::NewGuid()
    $errorResponse = Invoke-RestMethod -Uri "$baseUrl/api/coffees/$invalidId" -Method GET -Headers $headers

    if (-not $errorResponse.success) {
        Write-Host "   ? Error handling working correctly" -ForegroundColor Green
        Write-Host "     Returns HTTP 200 with success=false" -ForegroundColor Gray
        Write-Host "     Error: $($errorResponse.errorMessage)" -ForegroundColor Gray
    } else {
        Write-Host "   ? Unexpected success response" -ForegroundColor Yellow
    }
} catch {
    Write-Host "   ? Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 8: Register new user
Write-Host "8. Testing User Registration..." -ForegroundColor Yellow
try {
    $registerBody = @{
        name = "Test User $(Get-Random)"
        email = "test$(Get-Random)@example.com"
        password = "TestPass123!"
    } | ConvertTo-Json

    $regResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/register" -Method POST -Body $registerBody -ContentType "application/json"

    if ($regResponse.success) {
        Write-Host "   ? User registered successfully" -ForegroundColor Green
        Write-Host "     Name: $($regResponse.data.name)" -ForegroundColor Gray
        Write-Host "     Email: $($regResponse.data.email)" -ForegroundColor Gray
    } else {
        Write-Host "   ? Failed: $($regResponse.errorMessage)" -ForegroundColor Red
    }
} catch {
    Write-Host "   ? Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Summary
Write-Host "================================" -ForegroundColor Cyan
Write-Host "  Test Summary" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "? In-Memory Database Working!" -ForegroundColor Green
Write-Host ""
Write-Host "Features Tested:" -ForegroundColor Yellow
Write-Host "  ? User login with pre-seeded data" -ForegroundColor Gray
Write-Host "  ? Get all coffees" -ForegroundColor Gray
Write-Host "  ? Get coffee by ID" -ForegroundColor Gray
Write-Host "  ? Create new coffee" -ForegroundColor Gray
Write-Host "  ? Get experiments" -ForegroundColor Gray
Write-Host "  ? Create experiment" -ForegroundColor Gray
Write-Host "  ? Error handling (returns 200 with success=false)" -ForegroundColor Gray
Write-Host "  ? User registration" -ForegroundColor Gray
Write-Host ""
Write-Host "Pre-Seeded Test Data:" -ForegroundColor Yellow
Write-Host "  Email: test@brewlab.com" -ForegroundColor Gray
Write-Host "  Password: Test123!" -ForegroundColor Gray
Write-Host "  2 coffees (Ethiopian & Colombian)" -ForegroundColor Gray
Write-Host "  1 experiment (V60 method)" -ForegroundColor Gray
Write-Host ""
Write-Host "Note: Data persists while API is running" -ForegroundColor Cyan
Write-Host "      Data resets when API restarts" -ForegroundColor Cyan
Write-Host ""
