# Test CORS Configuration
# This script tests if CORS is properly configured

Write-Host "================================" -ForegroundColor Cyan
Write-Host "  CORS Configuration Test" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5000"

# Test 1: Check if API is running
Write-Host "1. Checking if API is running..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/health/ping" -Method GET -UseBasicParsing -ErrorAction Stop
    Write-Host "   ? API is running" -ForegroundColor Green
} catch {
    Write-Host "   ? API is not running!" -ForegroundColor Red
    Write-Host "   Please start the API first: dotnet run" -ForegroundColor Yellow
    exit 1
}
Write-Host ""

# Test 2: Check CORS headers with OPTIONS request
Write-Host "2. Testing CORS preflight (OPTIONS)..." -ForegroundColor Yellow
try {
    $headers = @{
        "Origin" = "http://localhost:5173"
        "Access-Control-Request-Method" = "GET"
        "Access-Control-Request-Headers" = "content-type,authorization"
    }

    $response = Invoke-WebRequest -Uri "$baseUrl/api/health/ping" -Method OPTIONS -Headers $headers -UseBasicParsing -ErrorAction Stop

    $corsHeader = $response.Headers["Access-Control-Allow-Origin"]
    if ($corsHeader) {
        Write-Host "   ? CORS headers present" -ForegroundColor Green
        Write-Host "   Access-Control-Allow-Origin: $corsHeader" -ForegroundColor Gray

        $allowMethods = $response.Headers["Access-Control-Allow-Methods"]
        if ($allowMethods) {
            Write-Host "   Access-Control-Allow-Methods: $allowMethods" -ForegroundColor Gray
        }
    } else {
        Write-Host "   ? CORS headers not found in OPTIONS response" -ForegroundColor Yellow
    }
} catch {
    Write-Host "   ? Could not test OPTIONS request" -ForegroundColor Yellow
    Write-Host "   This might be normal for some configurations" -ForegroundColor Gray
}
Write-Host ""

# Test 3: Check CORS headers with GET request
Write-Host "3. Testing CORS with GET request..." -ForegroundColor Yellow
try {
    $headers = @{
        "Origin" = "http://localhost:5173"
    }

    $response = Invoke-WebRequest -Uri "$baseUrl/api/health/ping" -Method GET -Headers $headers -UseBasicParsing -ErrorAction Stop

    $corsHeader = $response.Headers["Access-Control-Allow-Origin"]
    if ($corsHeader) {
        Write-Host "   ? CORS working for GET requests" -ForegroundColor Green
        Write-Host "   Access-Control-Allow-Origin: $corsHeader" -ForegroundColor Gray
    } else {
        Write-Host "   ? CORS header not present" -ForegroundColor Yellow
    }

    # Check response
    $content = $response.Content | ConvertFrom-Json
    if ($content.success) {
        Write-Host "   ? API responding correctly" -ForegroundColor Green
    }
} catch {
    Write-Host "   ? GET request failed" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Gray
}
Write-Host ""

# Test 4: Test from different origins
Write-Host "4. Testing multiple origins..." -ForegroundColor Yellow

$origins = @(
    "http://localhost:5000",
    "http://localhost:5173",
    "http://localhost:4173"
)

foreach ($origin in $origins) {
    try {
        $headers = @{ "Origin" = $origin }
        $response = Invoke-WebRequest -Uri "$baseUrl/api/health/ping" -Method GET -Headers $headers -UseBasicParsing -ErrorAction Stop
        $corsHeader = $response.Headers["Access-Control-Allow-Origin"]

        if ($corsHeader) {
            Write-Host "   ? $origin - Allowed" -ForegroundColor Green
        } else {
            Write-Host "   ? $origin - No CORS header" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "   ? $origin - Failed" -ForegroundColor Red
    }
}
Write-Host ""

# Test 5: Check browser console for CORS errors
Write-Host "5. Browser Testing Instructions:" -ForegroundColor Yellow
Write-Host "   Open browser console (F12) and run:" -ForegroundColor Gray
Write-Host ""
Write-Host "   fetch('$baseUrl/api/health/ping')" -ForegroundColor White
Write-Host "     .then(r => r.json())" -ForegroundColor White
Write-Host "     .then(d => console.log('Success:', d))" -ForegroundColor White
Write-Host "     .catch(e => console.error('CORS Error:', e));" -ForegroundColor White
Write-Host ""

Write-Host "================================" -ForegroundColor Cyan
Write-Host "  Summary" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Current CORS Configuration:" -ForegroundColor Yellow
Write-Host "  • Development: Allow all origins" -ForegroundColor Gray
Write-Host "  • Production: Specific origins only" -ForegroundColor Gray
Write-Host ""
Write-Host "Allowed Origins (Production):" -ForegroundColor Yellow
Write-Host "  • http://localhost:5173" -ForegroundColor Gray
Write-Host "  • http://localhost:4173" -ForegroundColor Gray
Write-Host "  • http://localhost:5000" -ForegroundColor Gray
Write-Host "  • http://127.0.0.1:5000" -ForegroundColor Gray
Write-Host "  • https://brew-lab-frontend.vercel.app" -ForegroundColor Gray
Write-Host ""

# Test 6: Test with actual request from JavaScript context
Write-Host "6. JavaScript CORS Test:" -ForegroundColor Yellow
Write-Host "   Creating test HTML file..." -ForegroundColor Gray

$testHtml = @"
<!DOCTYPE html>
<html>
<head>
    <title>CORS Test</title>
</head>
<body>
    <h1>BrewLab CORS Test</h1>
    <button onclick="testCors()">Test CORS</button>
    <pre id="result"></pre>

    <script>
        async function testCors() {
            const result = document.getElementById('result');
            result.textContent = 'Testing...';

            try {
                const response = await fetch('$baseUrl/api/health/ping');
                const data = await response.json();
                result.textContent = 'SUCCESS!\n\n' + JSON.stringify(data, null, 2);
                result.style.color = 'green';
            } catch (error) {
                result.textContent = 'CORS ERROR!\n\n' + error.message;
                result.style.color = 'red';
                console.error('CORS Error:', error);
            }
        }

        // Auto-test on load
        window.onload = () => {
            setTimeout(testCors, 500);
        };
    </script>
</body>
</html>
"@

$testHtml | Out-File -FilePath "cors-test.html" -Encoding UTF8
Write-Host "   ? Test file created: cors-test.html" -ForegroundColor Green
Write-Host "   Opening in browser..." -ForegroundColor Gray
Start-Process "cors-test.html"

Write-Host ""
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "If you see CORS errors in browser:" -ForegroundColor Yellow
Write-Host "  1. Make sure API is running in Development mode" -ForegroundColor Gray
Write-Host "  2. Check browser console for specific error" -ForegroundColor Gray
Write-Host "  3. Verify origin matches allowed list" -ForegroundColor Gray
Write-Host ""
Write-Host "Common CORS Errors:" -ForegroundColor Yellow
Write-Host "  • 'No Access-Control-Allow-Origin header'" -ForegroundColor Gray
Write-Host "    ? API not running or CORS not configured" -ForegroundColor Gray
Write-Host "  • 'Origin not allowed by CORS'" -ForegroundColor Gray
Write-Host "    ? Add your origin to allowed list" -ForegroundColor Gray
Write-Host "  • 'Preflight request failed'" -ForegroundColor Gray
Write-Host "    ? CORS policy missing OPTIONS method" -ForegroundColor Gray
Write-Host ""
