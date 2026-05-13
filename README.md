# ? BrewLab API

A modern coffee experiment tracking API built with .NET 9, featuring a clean architecture with automatic database fallback and standardized error handling.

## ?? Quick Start

### Prerequisites
- .NET 9 SDK
- PostgreSQL (optional - uses in-memory database if unavailable)

### Run Locally
```bash
# Clone the repository
git clone https://github.com/aryan84xd/BrewLab.git
cd BrewLab

# Run the API 
dotnet run

# API will be available at http://localhost:5000
# Swagger UI at http://localhost:5000/swagger (development only)
```

### Test Credentials (In-Memory Database)
- **Email**: `test@brewlab.com`
- **Password**: `Test123!`

---

## ?? Features

### Core Features
- ? **User Authentication** - JWT-based auth with register/login
- ? **Coffee Management** - Create and track coffee varieties
- ? **Experiment Tracking** - Record brew experiments with ratings
- ? **Automatic Database Fallback** - Uses in-memory DB when PostgreSQL unavailable
- ? **Standardized Responses** - All endpoints return HTTP 200 with success/error fields

### Architecture
- ? **Clean Architecture** - Request ? DTO ? DBO ? Database ? DBO ? DTO ? Response
- ? **No Circular Dependencies** - Clear separation of concerns
- ? **Environment-Aware** - Automatically adapts to Development/Production
- ? **CORS Configured** - Development (all origins) and Production (restricted)

---

## ??? Architecture

### Data Flow
```
Client Request
    ?
Controller (Request Models)
    ?
Service (DTOs - Data Transfer Objects)
    ?
Repository (DBOs - Database Objects)
    ?
Database (PostgreSQL or In-Memory)
    ?
Repository (DBOs)
    ?
Service (DTOs)
    ?
Controller (Response Models)
    ?
Client Response (Always HTTP 200 with ApiResponse wrapper)
```

### Response Format
All endpoints return a standardized response:

**Success:**
```json
{
  "success": true,
  "errorMessage": null,
  "data": {
    // Your actual data here
  }
}
```

**Error:**
```json
{
  "success": false,
  "errorMessage": "Descriptive error message",
  "data": null
}
```

---

## ?? API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login and get JWT token
- `GET /api/auth/me` - Get current user info (requires auth)

### Coffee Management
- `GET /api/coffees` - Get all user's coffees (requires auth)
- `GET /api/coffees/{id}` - Get specific coffee (requires auth)
- `POST /api/coffees` - Create new coffee (requires auth)

### Experiment Tracking
- `GET /api/experiment/{coffeeId}` - Get experiments for coffee (requires auth)
- `POST /api/experiment` - Create new experiment (requires auth)

### Health Check
- `GET /api/health` - API health and architecture info
- `GET /api/health/ping` - Simple ping test
- `GET /api/health/error-test` - Test error response format

---

## ?? Testing

### Using Postman
Import the Postman collection from `POSTMAN_COLLECTION.json`

1. Import collection into Postman
2. Run "Register User" or use test credentials
3. Run "Login" to get JWT token
4. Token automatically saved to environment variable
5. Test other endpoints

### Using Swagger UI (Development Only)
```bash
# Start API in development mode
dotnet run

# Open browser
http://localhost:5000/swagger
```

### Using cURL
```bash
# Health check
curl http://localhost:5000/api/health/ping

# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@brewlab.com","password":"Test123!"}'

# Get coffees (replace TOKEN with actual token)
curl http://localhost:5000/api/coffees \
  -H "Authorization: Bearer TOKEN"
```

---

## ??? Database

### Automatic Fallback
The API automatically detects database availability:

**PostgreSQL Available:**
- Uses PostgreSQL for persistent storage
- Connection string from configuration

**PostgreSQL Unavailable:**
- Automatically falls back to in-memory database
- Pre-seeded with test data
- Data persists while API is running

### PostgreSQL Setup (Optional)

1. Install PostgreSQL
2. Create database:
```sql
CREATE DATABASE brewlab;
```

3. Run setup script:
```sql
-- Run Database/setup.sql
```

4. Update connection string in `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=brewlab;Username=postgres;Password=yourpassword"
  }
}
```

### Pre-Seeded Test Data (In-Memory)

**Test User:**
- Email: test@brewlab.com
- Password: Test123!

**Test Coffees:**
- Ethiopian Yirgacheffe (Light roast)
- Colombian Supremo (Medium roast)

**Test Experiment:**
- V60 brew method for Ethiopian coffee

---

## ?? Deployment

### Environment Variables

**Required:**
```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Host=your-db;Database=brewlab;Username=user;Password=pass
Jwt__Key=your-secure-key-at-least-32-characters-long
```

**Optional:**
```bash
Jwt__Issuer=YourIssuer
Jwt__Audience=YourAudience
Jwt__ExpirationMinutes=60
PORT=5000
```

### Production Checklist

- [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Configure PostgreSQL connection string
- [ ] Generate strong JWT key (32+ characters)
- [ ] Update CORS origins in `Program.cs` (line 52-56)
- [ ] Ensure PostgreSQL database is set up

### Platform-Specific

**Railway:**
```bash
# Auto-deploys from GitHub
# Add PostgreSQL plugin
# Set environment variables in dashboard
```

**Docker:**
```bash
docker build -t brewlab-api .
docker run -p 8080:80 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="..." \
  -e Jwt__Key="..." \
  brewlab-api
```

**Azure:**
```bash
az webapp create --name brewlab-api --runtime "DOTNETCORE:9.0"
az webapp config appsettings set --settings \
  ASPNETCORE_ENVIRONMENT=Production \
  ConnectionStrings__DefaultConnection="..." \
  Jwt__Key="..."
```

---

## ?? Configuration

### Development Mode
- Swagger UI enabled
- CORS allows all origins
- In-memory database fallback
- Verbose logging
- Binds to `localhost`

### Production Mode
- Swagger UI disabled
- CORS restricted to configured origins
- PostgreSQL required
- Production logging
- Binds to `0.0.0.0` (all interfaces)

### CORS Configuration

Update `Program.cs` lines 52-56 for your frontend URLs:
```csharp
.WithOrigins(
    "http://localhost:5173",           // Local development
    "https://your-frontend-url.com"    // Production frontend
)
```

---

## ?? Project Structure

```
BrewLab/
??? Controllers/          # API endpoints
?   ??? AuthController.cs
?   ??? CoffeesController.cs
?   ??? ExperimentController.cs
?   ??? HealthController.cs
??? Services/            # Business logic
?   ??? AuthService.cs
?   ??? CoffeeService.cs
?   ??? ExperimentService.cs
??? Repositories/        # Data access
?   ??? UserRepository.cs
?   ??? CoffeeRepository.cs
?   ??? ExperimentRepository.cs
?   ??? InMemory*/       # In-memory implementations
??? Models/
?   ??? Requests/        # API request models
?   ??? Responses/       # API response models
?   ??? DTOs/            # Service layer transfer objects
?   ??? DBO/             # Database objects
?   ??? Entities/        # EF Core entities (legacy)
?   ??? Common/          # ApiResponse wrapper
??? Data/
?   ??? IDbConnectionFactory.cs
?   ??? InMemoryDatabase.cs
??? Database/
    ??? setup.sql        # PostgreSQL schema
```

---

## ?? Security

### Authentication
- JWT token-based authentication
- Bcrypt password hashing
- Token expiration configurable

### Best Practices
- Environment variables for secrets
- CORS restrictions in production
- HTTPS enforced (handled by hosting platform)
- Input validation on all endpoints

---

## ?? Troubleshooting

### API Not Starting

**Issue:** Port already in use
```bash
# Find process using port 5000
netstat -ano | findstr :5000

# Kill the process (Windows)
taskkill /PID <PID> /F
```

**Issue:** Database connection failed
- Check PostgreSQL is running
- Verify connection string
- API will automatically fall back to in-memory database

### CORS Errors

**Development:**
- Should allow all origins automatically

**Production:**
- Add your frontend URL to `Program.cs` CORS configuration
- Restart the API after changes

### Deployment Issues

**Port scan timeout:**
- Fixed: API binds to `0.0.0.0` in production
- Ensure `ASPNETCORE_ENVIRONMENT=Production` is set

**JWT errors:**
- Ensure JWT key is at least 32 characters
- Verify key is set in environment variables

---

## ?? API Response Examples

### Register User
**Request:**
```json
POST /api/auth/register
{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

**Response:**
```json
{
  "success": true,
  "errorMessage": null,
  "data": {
    "token": "eyJhbGciOi...",
    "email": "john@example.com",
    "name": "John Doe",
    "expiresAtUtc": "2024-01-15T10:00:00Z"
  }
}
```

### Create Coffee
**Request:**
```json
POST /api/coffees
Authorization: Bearer <token>
{
  "name": "Ethiopian Yirgacheffe",
  "brand": "Blue Bottle",
  "roast": "Light",
  "origin": "Ethiopia",
  "tastingNotes": "Floral, citrus, tea-like"
}
```

**Response:**
```json
{
  "success": true,
  "errorMessage": null,
  "data": {
    "id": "guid-here",
    "name": "Ethiopian Yirgacheffe",
    "brand": "Blue Bottle",
    "roast": "Light",
    "origin": "Ethiopia",
    "tastingNotes": "Floral, citrus, tea-like"
  }
}
```

### Error Response
**Response:**
```json
{
  "success": false,
  "errorMessage": "Coffee not found.",
  "data": null
}
```

---

## ??? Development

### Build
```bash
dotnet build
```

### Run Tests
```bash
dotnet test
```

### Run in Development
```bash
dotnet run
```

### Run in Production Mode (Local)
```bash
$env:ASPNETCORE_ENVIRONMENT="Production"
dotnet run
```

---

## ?? License

This project is licensed under the MIT License.

---

## ?? Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

---

## ?? Support

- **Issues**: [GitHub Issues](https://github.com/aryan84xd/BrewLab/issues)
- **Email**: your-email@example.com

---

## ?? Acknowledgments

- Built with .NET 9
- Uses Npgsql for PostgreSQL
- JWT authentication with BCrypt
- Swagger/OpenAPI documentation

---

**Made with ? by [aryan84xd](https://github.com/aryan84xd)**
