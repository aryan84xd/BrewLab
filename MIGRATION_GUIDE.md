# Migration Guide: Entity Framework to Npgsql

## What Changed

? **Removed Entity Framework** completely  
? **Added clean architecture** with Controller ? Service ? Repository ? Database  
? **Fixed circular dependency issues** by removing navigation properties  
? **Implemented proper separation** with Request/Response/DTO/DBO models  

## Steps to Complete Migration

### 1. Run the Database Schema

First, execute the SQL schema to create the database tables:

```bash
psql -U your_username -d your_database -f Database/schema.sql
```

Or manually run the SQL from `Database/schema.sql` in your PostgreSQL database.

### 2. Update Connection String

Make sure your `appsettings.json` and `appsettings.Development.json` have the correct PostgreSQL connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=brewlab;Username=youruser;Password=yourpass"
  }
}
```

### 3. Test the API

The API endpoints remain the same:

**Auth:**
- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/auth/me`

**Coffees:**
- `GET /api/coffees` (requires auth)
- `GET /api/coffees/{id}` (requires auth)
- `POST /api/coffees` (requires auth)

**Experiments:**
- `GET /api/experiment/{coffeeId}` (requires auth)
- `POST /api/experiment` (requires auth)

### 4. Request/Response Changes

#### Creating a Coffee (POST /api/coffees)

**Old DTO:**
```json
{
  "name": "Ethiopian Yirgacheffe",
  "brand": "Blue Bottle",
  "roast": "Light",
  "origin": "Ethiopia",
  "tastingNotes": "Floral, citrus"
}
```

**New Request (same structure):**
```json
{
  "name": "Ethiopian Yirgacheffe",
  "brand": "Blue Bottle",
  "roast": "Light",
  "origin": "Ethiopia",
  "tastingNotes": "Floral, citrus"
}
```

#### Creating an Experiment (POST /api/experiment)

**Old DTO:**
```json
{
  "coffeeId": "guid-here",
  "brewMethod": "Pour Over",
  "coffeeWeight": 15.5,
  "waterWeight": 250.0,
  "brewTime": "03:30:00",
  "remark": "Great balance",
  "aroma": 8,
  "acidity": 7,
  "body": 6,
  "overall": 8
}
```

**New Request (same structure):**
```json
{
  "coffeeId": "guid-here",
  "brewMethod": "Pour Over",
  "coffeeWeight": 15.5,
  "waterWeight": 250.0,
  "brewTime": "03:30:00",
  "remark": "Great balance",
  "aroma": 8,
  "acidity": 7,
  "body": 6,
  "overall": 8
}
```

## Files Removed

The following Entity Framework-related files were removed:
- ? `Data/AppDbContext.cs`
- ? `Data/AppDbContextFactory.cs`
- ? `Controllers/BaseApiController.cs`
- ? `Migrations/` (all files)
- ?? `Models/Entities/` (kept for now, but not used)

## Files Added

**Data Layer:**
- `Data/IDbConnectionFactory.cs` - Database connection factory

**Repositories:**
- `Repositories/UserRepository.cs`
- `Repositories/CoffeeRepository.cs`
- `Repositories/ExperimentRepository.cs`

**Services:**
- `Services/AuthService.cs`
- `Services/CoffeeService.cs`
- `Services/ExperimentService.cs`

**Models:**
- `Models/DBO/UserDBO.cs`
- `Models/DBO/CoffeeDBO.cs`
- `Models/DBO/ExperimentDBO.cs`
- `Models/Requests/CreateCoffeeRequest.cs`
- `Models/Requests/CreateExperimentRequest.cs`
- `Models/Responses/CoffeeResponse.cs`
- `Models/Responses/ExperimentResponse.cs`

**Controllers (replaced):**
- `Controllers/AuthController.cs` (new version)
- `Controllers/CoffeesController.cs` (new version)
- `Controllers/ExperimentController.cs` (new version)

**Database:**
- `Database/schema.sql` - PostgreSQL schema script

## Benefits

? **No more circular dependencies** - DBOs are simple data objects  
? **Better performance** - Direct SQL queries, no ORM overhead  
? **Cleaner code** - Clear separation of concerns  
? **More control** - Full control over SQL queries  
? **Easier testing** - Mock interfaces instead of DbContext  
? **Smaller dependencies** - Removed 5 NuGet packages  

## Troubleshooting

### Build Errors
If you see Entity Framework-related errors, make sure you've restored packages:
```bash
dotnet restore
dotnet build
```

### Database Connection Issues
- Verify PostgreSQL is running
- Check connection string in appsettings.json
- Ensure database and tables are created (run schema.sql)

### Migration of Existing Data
If you have existing data in your database from Entity Framework migrations:
- The table structure should be compatible
- Column names use PascalCase with quotes (e.g., "Id", "Name")
- The schema.sql script drops and recreates tables, so backup first!

## Next Steps

1. ? Run the schema.sql script
2. ? Test all API endpoints
3. ?? Remove `Models/Entities/` folder if no longer needed
4. ?? Update your frontend to use the new response models (should be compatible)
5. ?? Update any integration tests
