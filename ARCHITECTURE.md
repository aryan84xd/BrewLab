# BrewLab Architecture

## Overview
This project has been refactored from Entity Framework to use raw **Npgsql** with a clean layered architecture.

## Architecture Layers

```
Controller ? Service ? Repository ? Database
```

### 1. **Controllers** (`Controllers/`)
- Handle HTTP requests/responses
- Validate user authentication
- Call services for business logic
- New V2 controllers:
  - `AuthControllerV2.cs`
  - `CoffeesControllerV2.cs`
  - `ExperimentControllerV2.cs`

### 2. **Services** (`Services/`)
- Business logic layer
- Coordinate between controllers and repositories
- Services:
  - `AuthService.cs`
  - `CoffeeService.cs`
  - `ExperimentService.cs`

### 3. **Repositories** (`Repositories/`)
- Data access layer
- Raw SQL queries using Npgsql
- Repositories:
  - `UserRepository.cs`
  - `CoffeeRepository.cs`
  - `ExperimentRepository.cs`

### 4. **Models**

#### **DBO** (`Models/DBO/`)
Database Objects - represent database tables
- `UserDBO.cs`
- `CoffeeDBO.cs`
- `ExperimentDBO.cs`

#### **Request** (`Models/Requests/`)
Request models for API endpoints
- `CreateCoffeeRequest.cs`
- `CreateExperimentRequest.cs`

#### **Response** (`Models/Responses/`)
Response models for API endpoints
- `CoffeeResponse.cs`
- `ExperimentResponse.cs`

#### **DTOs** (`Models/DTOs/`)
Data Transfer Objects (existing)
- User DTOs
- Coffee DTOs
- Experiment DTOs

## Database Setup

1. **Run the schema script**:
   ```bash
   psql -U your_username -d your_database -f Database/schema.sql
   ```

2. **Update connection string** in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=brewlab;Username=youruser;Password=yourpass"
     }
   }
   ```

## Removed Dependencies

The following Entity Framework packages have been removed:
- `Microsoft.EntityFrameworkCore.InMemory`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Tools`
- `Npgsql.EntityFrameworkCore.PostgreSQL`
- `Microsoft.VisualStudio.Web.CodeGeneration.Design`

## Benefits

? **No circular dependency issues** - DBOs are plain objects with no navigation properties  
? **Better performance** - Direct SQL queries without ORM overhead  
? **More control** - Explicit SQL gives you full control over queries  
? **Cleaner separation** - Clear boundaries between layers  
? **Easier testing** - Mock interfaces instead of DbContext  

## API Endpoints

### Auth V2 (`/api/authv2`)
- `POST /api/authv2/register`
- `POST /api/authv2/login`
- `GET /api/authv2/me`

### Coffees V2 (`/api/coffeesv2`)
- `GET /api/coffeesv2` - Get all user's coffees
- `GET /api/coffeesv2/{id}` - Get specific coffee
- `POST /api/coffeesv2` - Create new coffee

### Experiments V2 (`/api/experimentv2`)
- `GET /api/experimentv2/{coffeeId}` - Get experiments for a coffee
- `POST /api/experimentv2` - Create new experiment

## Migration Notes

The old controllers (`AuthController`, `CoffeesController`, `ExperimentController`) can be removed once you verify V2 controllers work correctly.

Old files to remove:
- `Data/AppDbContext.cs`
- `Data/AppDbContextFactory.cs`
- `Models/Entities/` (all entity files)
- `Migrations/` (all migration files)
- `Controllers/BaseApiController.cs` (if not needed)
