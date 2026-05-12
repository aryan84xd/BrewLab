# Database Setup Helper Script
# Run this script to set up your PostgreSQL database for BrewLab

Write-Host "================================" -ForegroundColor Cyan
Write-Host "  BrewLab Database Setup" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# PostgreSQL connection details
$PG_HOST = "localhost"
$PG_PORT = "5432"
$PG_USER = "postgres"
$PG_PASSWORD = "postgres"
$PG_DATABASE = "brewlab"

Write-Host "Connection Details:" -ForegroundColor Yellow
Write-Host "  Host: $PG_HOST" -ForegroundColor Gray
Write-Host "  Port: $PG_PORT" -ForegroundColor Gray
Write-Host "  User: $PG_USER" -ForegroundColor Gray
Write-Host "  Database: $PG_DATABASE" -ForegroundColor Gray
Write-Host ""

# Check if psql is available
Write-Host "Checking PostgreSQL installation..." -ForegroundColor Yellow
$psqlPath = Get-Command psql -ErrorAction SilentlyContinue

if (-not $psqlPath) {
    Write-Host "? psql command not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please install PostgreSQL or add it to your PATH." -ForegroundColor Yellow
    Write-Host "Download: https://www.postgresql.org/download/" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Alternative: Run the SQL script manually using pgAdmin" -ForegroundColor Yellow
    Write-Host "Script location: Database/setup.sql" -ForegroundColor Gray
    exit 1
}

Write-Host "? PostgreSQL found at: $($psqlPath.Source)" -ForegroundColor Green
Write-Host ""

# Set password environment variable
$env:PGPASSWORD = $PG_PASSWORD

try {
    # Check if database exists
    Write-Host "Checking if database exists..." -ForegroundColor Yellow

    $checkDb = psql -h $PG_HOST -p $PG_PORT -U $PG_USER -d postgres -t -c "SELECT 1 FROM pg_database WHERE datname='$PG_DATABASE';" 2>&1

    if ($checkDb -match "1") {
        Write-Host "? Database '$PG_DATABASE' already exists" -ForegroundColor Green
        $response = Read-Host "Do you want to recreate it? This will delete all data! (y/N)"

        if ($response -eq "y" -or $response -eq "Y") {
            Write-Host "Dropping existing database..." -ForegroundColor Yellow
            psql -h $PG_HOST -p $PG_PORT -U $PG_USER -d postgres -c "DROP DATABASE IF EXISTS $PG_DATABASE;" | Out-Null
            Write-Host "? Database dropped" -ForegroundColor Green

            Write-Host "Creating database..." -ForegroundColor Yellow
            psql -h $PG_HOST -p $PG_PORT -U $PG_USER -d postgres -c "CREATE DATABASE $PG_DATABASE;" | Out-Null
            Write-Host "? Database created" -ForegroundColor Green
        }
    } else {
        Write-Host "Creating database..." -ForegroundColor Yellow
        psql -h $PG_HOST -p $PG_PORT -U $PG_USER -d postgres -c "CREATE DATABASE $PG_DATABASE;" | Out-Null
        Write-Host "? Database created" -ForegroundColor Green
    }

    Write-Host ""

    # Run setup script
    Write-Host "Creating tables..." -ForegroundColor Yellow

    $sqlScript = Get-Content "Database/setup.sql" -Raw
    $result = psql -h $PG_HOST -p $PG_PORT -U $PG_USER -d $PG_DATABASE -c $sqlScript 2>&1

    if ($LASTEXITCODE -eq 0) {
        Write-Host "? Tables created successfully" -ForegroundColor Green
    } else {
        Write-Host "? Error creating tables" -ForegroundColor Red
        Write-Host $result -ForegroundColor Red
        exit 1
    }

    Write-Host ""

    # Verify tables
    Write-Host "Verifying tables..." -ForegroundColor Yellow
    $tables = psql -h $PG_HOST -p $PG_PORT -U $PG_USER -d $PG_DATABASE -t -c "SELECT table_name FROM information_schema.tables WHERE table_schema='public' ORDER BY table_name;" 2>&1

    $expectedTables = @("Coffees", "Experiments", "Users")
    $foundTables = @()

    foreach ($table in $tables) {
        $tableName = $table.Trim()
        if ($tableName -and $tableName -in $expectedTables) {
            $foundTables += $tableName
            Write-Host "  ? $tableName" -ForegroundColor Green
        }
    }

    Write-Host ""

    if ($foundTables.Count -eq $expectedTables.Count) {
        Write-Host "================================" -ForegroundColor Cyan
        Write-Host "  ? Database Setup Complete!" -ForegroundColor Green
        Write-Host "================================" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Next steps:" -ForegroundColor Yellow
        Write-Host "  1. Update appsettings.Development.json if needed" -ForegroundColor Gray
        Write-Host "  2. Run: dotnet run" -ForegroundColor Gray
        Write-Host "  3. Open: http://localhost:5000/swagger" -ForegroundColor Gray
        Write-Host "  4. Test: .\test-api.ps1" -ForegroundColor Gray
        Write-Host ""
    } else {
        Write-Host "? Some tables are missing!" -ForegroundColor Red
        Write-Host "Expected: $($expectedTables -join ', ')" -ForegroundColor Gray
        Write-Host "Found: $($foundTables -join ', ')" -ForegroundColor Gray
        exit 1
    }

} catch {
    Write-Host ""
    Write-Host "? Setup failed!" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Common issues:" -ForegroundColor Yellow
    Write-Host "  1. PostgreSQL not running" -ForegroundColor Gray
    Write-Host "  2. Wrong username/password" -ForegroundColor Gray
    Write-Host "  3. PostgreSQL not in PATH" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Manual setup:" -ForegroundColor Yellow
    Write-Host "  1. Open pgAdmin or psql" -ForegroundColor Gray
    Write-Host "  2. Create database 'brewlab'" -ForegroundColor Gray
    Write-Host "  3. Run script from: Database/setup.sql" -ForegroundColor Gray
    Write-Host ""
    exit 1
} finally {
    # Clear password from environment
    Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue
}
