# BrewLab API Test Script
# This script tests the complete API flow with the new architecture

$baseUrl = "http://localhost:5000"
$ErrorActionPreference = "Stop"

Write-Host "================================" -ForegroundColor Cyan
Write-Host "  BrewLab API Test Suite" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Generate random email to avoid conflicts
$randomEmail = "test$(Get-Random -Minimum 1000 -Maximum 9999)@example.com"

try {
    # Test 1: Register User
    Write-Host "Test 1: Register User" -ForegroundColor Yellow
    Write-Host "  Email: $randomEmail" -ForegroundColor Gray

    $registerBody = @{
        name = "Test User"
        email = $randomEmail
        password = "Test123!"
    } | ConvertTo-Json

    $register = Invoke-RestMethod -Uri "$baseUrl/api/auth/register" -Method POST -Body $registerBody -ContentType "application/json"

    if ($register.success) {
        Write-Host "  ? Registration successful" -ForegroundColor Green
        Write-Host "  Token: $($register.data.token.Substring(0, 20))..." -ForegroundColor Gray
        $token = $register.data.token
    } else {
        Write-Host "  ? Registration failed: $($register.errorMessage)" -ForegroundColor Red
        exit 1
    }

    Write-Host ""

    # Test 2: Login
    Write-Host "Test 2: Login" -ForegroundColor Yellow

    $loginBody = @{
        email = $randomEmail
        password = "Test123!"
    } | ConvertTo-Json

    $login = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -Body $loginBody -ContentType "application/json"

    if ($login.success) {
        Write-Host "  ? Login successful" -ForegroundColor Green
        Write-Host "  User: $($login.data.name)" -ForegroundColor Gray
    } else {
        Write-Host "  ? Login failed: $($login.errorMessage)" -ForegroundColor Red
        exit 1
    }

    Write-Host ""

    # Setup headers for authenticated requests
    $headers = @{
        "Authorization" = "Bearer $token"
        "Content-Type" = "application/json"
    }

    # Test 3: Create Coffee
    Write-Host "Test 3: Create Coffee" -ForegroundColor Yellow

    $coffeeBody = @{
        name = "Ethiopian Yirgacheffe"
        brand = "Blue Bottle"
        roast = "Light"
        origin = "Ethiopia"
        tastingNotes = "Floral, citrus, tea-like"
    } | ConvertTo-Json

    $coffee = Invoke-RestMethod -Uri "$baseUrl/api/coffees" -Method POST -Body $coffeeBody -Headers $headers

    if ($coffee.success) {
        Write-Host "  ? Coffee created successfully" -ForegroundColor Green
        Write-Host "  ID: $($coffee.data.id)" -ForegroundColor Gray
        Write-Host "  Name: $($coffee.data.name)" -ForegroundColor Gray
        $coffeeId = $coffee.data.id
    } else {
        Write-Host "  ? Coffee creation failed: $($coffee.errorMessage)" -ForegroundColor Red
        exit 1
    }

    Write-Host ""

    # Test 4: Get All Coffees
    Write-Host "Test 4: Get All Coffees" -ForegroundColor Yellow

    $coffees = Invoke-RestMethod -Uri "$baseUrl/api/coffees" -Method GET -Headers $headers

    if ($coffees.success) {
        Write-Host "  ? Retrieved coffees successfully" -ForegroundColor Green
        Write-Host "  Count: $($coffees.data.Count)" -ForegroundColor Gray
        foreach ($c in $coffees.data) {
            Write-Host "    - $($c.name) by $($c.brand)" -ForegroundColor Gray
        }
    } else {
        Write-Host "  ? Failed to get coffees: $($coffees.errorMessage)" -ForegroundColor Red
        exit 1
    }

    Write-Host ""

    # Test 5: Get Coffee by ID
    Write-Host "Test 5: Get Coffee by ID" -ForegroundColor Yellow

    $coffeeById = Invoke-RestMethod -Uri "$baseUrl/api/coffees/$coffeeId" -Method GET -Headers $headers

    if ($coffeeById.success) {
        Write-Host "  ? Retrieved coffee by ID successfully" -ForegroundColor Green
        Write-Host "  Name: $($coffeeById.data.name)" -ForegroundColor Gray
        Write-Host "  Roast: $($coffeeById.data.roast)" -ForegroundColor Gray
    } else {
        Write-Host "  ? Failed to get coffee: $($coffeeById.errorMessage)" -ForegroundColor Red
        exit 1
    }

    Write-Host ""

    # Test 6: Create Experiment
    Write-Host "Test 6: Create Experiment" -ForegroundColor Yellow

    $experimentBody = @{
        coffeeId = $coffeeId
        brewMethod = "V60"
        coffeeWeight = 18.5
        waterWeight = 300.0
        brewTime = "00:02:30"
        remark = "Perfect extraction"
        aroma = 5
        acidity = 4
        body = 3
        overall = 9
    } | ConvertTo-Json

    $experiment = Invoke-RestMethod -Uri "$baseUrl/api/experiment" -Method POST -Body $experimentBody -Headers $headers

    if ($experiment.success) {
        Write-Host "  ? Experiment created successfully" -ForegroundColor Green
        Write-Host "  ID: $($experiment.data.id)" -ForegroundColor Gray
        Write-Host "  Method: $($experiment.data.brewMethod)" -ForegroundColor Gray
        Write-Host "  Overall Score: $($experiment.data.overall)/10" -ForegroundColor Gray
    } else {
        Write-Host "  ? Experiment creation failed: $($experiment.errorMessage)" -ForegroundColor Red
        exit 1
    }

    Write-Host ""

    # Test 7: Get Experiments for Coffee
    Write-Host "Test 7: Get Experiments for Coffee" -ForegroundColor Yellow

    $experiments = Invoke-RestMethod -Uri "$baseUrl/api/experiment/$coffeeId" -Method GET -Headers $headers

    if ($experiments.success) {
        Write-Host "  ? Retrieved experiments successfully" -ForegroundColor Green
        Write-Host "  Count: $($experiments.data.Count)" -ForegroundColor Gray
        foreach ($e in $experiments.data) {
            Write-Host "    - $($e.brewMethod): Overall $($e.overall)/10" -ForegroundColor Gray
        }
    } else {
        Write-Host "  ? Failed to get experiments: $($experiments.errorMessage)" -ForegroundColor Red
        exit 1
    }

    Write-Host ""

    # Test 8: Error Scenario - Invalid Coffee ID
    Write-Host "Test 8: Error Scenario - Invalid Coffee ID" -ForegroundColor Yellow

    $invalidId = [Guid]::NewGuid()
    $errorTest = Invoke-RestMethod -Uri "$baseUrl/api/coffees/$invalidId" -Method GET -Headers $headers

    if (-not $errorTest.success) {
        Write-Host "  ? Error handling works correctly" -ForegroundColor Green
        Write-Host "  Status: HTTP 200 (as expected)" -ForegroundColor Gray
        Write-Host "  Success: false" -ForegroundColor Gray
        Write-Host "  Error Message: $($errorTest.errorMessage)" -ForegroundColor Gray
    } else {
        Write-Host "  ? Error handling failed - should return success=false" -ForegroundColor Red
        exit 1
    }

    Write-Host ""

    # Test 9: Error Scenario - Experiment for Non-existent Coffee
    Write-Host "Test 9: Error Scenario - Experiment for Invalid Coffee" -ForegroundColor Yellow

    $invalidExperimentBody = @{
        coffeeId = [Guid]::NewGuid()
        brewMethod = "V60"
        coffeeWeight = 18.5
        waterWeight = 300.0
        brewTime = "00:02:30"
        remark = "Test"
        aroma = 5
        acidity = 4
        body = 3
        overall = 9
    } | ConvertTo-Json

    $errorExperiment = Invoke-RestMethod -Uri "$baseUrl/api/experiment" -Method POST -Body $invalidExperimentBody -Headers $headers

    if (-not $errorExperiment.success) {
        Write-Host "  ? Error handling works correctly" -ForegroundColor Green
        Write-Host "  Status: HTTP 200 (as expected)" -ForegroundColor Gray
        Write-Host "  Success: false" -ForegroundColor Gray
        Write-Host "  Error Message: $($errorExperiment.errorMessage)" -ForegroundColor Gray
    } else {
        Write-Host "  ? Error handling failed - should return success=false" -ForegroundColor Red
        exit 1
    }

    Write-Host ""
    Write-Host "================================" -ForegroundColor Cyan
    Write-Host "  ? All Tests Passed!" -ForegroundColor Green
    Write-Host "================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Summary:" -ForegroundColor Yellow
    Write-Host "  - All responses returned HTTP 200" -ForegroundColor Gray
    Write-Host "  - Success responses have success=true and data" -ForegroundColor Gray
    Write-Host "  - Error responses have success=false and errorMessage" -ForegroundColor Gray
    Write-Host "  - Architecture refactoring verified successfully!" -ForegroundColor Gray
    Write-Host ""

} catch {
    Write-Host ""
    Write-Host "================================" -ForegroundColor Red
    Write-Host "  ? Test Failed!" -ForegroundColor Red
    Write-Host "================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Error Details:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Make sure:" -ForegroundColor Yellow
    Write-Host "  1. The API is running (dotnet run)" -ForegroundColor Gray
    Write-Host "  2. Database is set up and running" -ForegroundColor Gray
    Write-Host "  3. API is accessible at $baseUrl" -ForegroundColor Gray
    Write-Host ""
    exit 1
}
