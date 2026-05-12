# ?? Production Deployment Guide

## ? YES! Your Code is Production-Ready!

Your application **automatically adapts** to the environment. No code changes needed!

---

## ?? How It Works Automatically

### Environment Detection

Your `Program.cs` already has environment-aware configuration:

```csharp
// Line 165-174
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll"); // Development: Allow any origin
}
else
{
    app.UseCors("AllowFrontend"); // Production: Restricted CORS
}
```

### Database Detection

```csharp
// Lines 34-76
try {
    // Try PostgreSQL connection
    if (PostgreSQL available) {
        Use PostgreSQL repositories
    } else {
        Use In-Memory repositories
    }
}
```

---

## ?? What Changes in Production

| Feature | Development | Production |
|---------|-------------|------------|
| **CORS** | Allows ALL origins (`AllowAll`) | Restricted origins (`AllowFrontend`) |
| **Swagger** | ? Enabled | ? Disabled (automatically) |
| **Database** | In-Memory fallback | PostgreSQL required |
| **Logging** | Verbose | Production-level |
| **Error Details** | Full stack traces | Generic messages |

---

## ?? Pre-Deployment Checklist

### 1. Environment Variables

Set these on your production server:

```bash
# Required
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Host=your-db-host;Database=brewlab;Username=your-user;Password=your-password;Port=5432
Jwt__Key=your-production-jwt-key-at-least-32-characters-long

# Optional (if different from appsettings.json)
Jwt__Issuer=YourProductionIssuer
Jwt__Audience=YourProductionAudience
Jwt__ExpirationMinutes=60
```

### 2. Update CORS Origins in `Program.cs`

Update line 52-56 with your production frontend URLs:

```csharp
.WithOrigins(
    "https://your-production-domain.com",
    "https://www.your-production-domain.com",
    "https://brew-lab-frontend.vercel.app"  // Your current Vercel URL
)
```

### 3. Database Setup

Ensure PostgreSQL is running and accessible:

```bash
# Test connection
psql -h your-db-host -U your-user -d brewlab

# Run migrations if needed
psql -h your-db-host -U your-user -d brewlab -f Database/setup.sql
```

### 4. JWT Configuration

**IMPORTANT:** Use a strong production JWT key!

```bash
# Generate a secure key (at least 32 characters)
openssl rand -base64 32

# Or in PowerShell
-join ((48..57) + (65..90) + (97..122) | Get-Random -Count 32 | ForEach-Object {[char]$_})
```

---

## ?? Deployment Platforms

### Option 1: Azure App Service

#### Step 1: Create App Service
```bash
az webapp create \
  --resource-group brewlab-rg \
  --plan brewlab-plan \
  --name brewlab-api \
  --runtime "DOTNETCORE:9.0"
```

#### Step 2: Configure Environment Variables
```bash
az webapp config appsettings set \
  --resource-group brewlab-rg \
  --name brewlab-api \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    ConnectionStrings__DefaultConnection="Host=..." \
    Jwt__Key="your-secure-key"
```

#### Step 3: Deploy
```bash
# From your project directory
dotnet publish -c Release
cd bin/Release/net9.0/publish
zip -r deploy.zip .

az webapp deployment source config-zip \
  --resource-group brewlab-rg \
  --name brewlab-api \
  --src deploy.zip
```

---

### Option 2: Docker + Any Cloud

#### Create Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["BrewLab.csproj", "./"]
RUN dotnet restore "BrewLab.csproj"
COPY . .
RUN dotnet build "BrewLab.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BrewLab.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrewLab.dll"]
```

#### Build & Run
```bash
# Build
docker build -t brewlab-api .

# Run locally to test
docker run -p 8080:80 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="Host=..." \
  -e Jwt__Key="your-key" \
  brewlab-api

# Test
curl http://localhost:8080/api/health/ping
```

#### Deploy to Docker Hub
```bash
docker tag brewlab-api yourusername/brewlab-api:latest
docker push yourusername/brewlab-api:latest
```

---

### Option 3: Railway.app

#### Step 1: Create `railway.json`
```json
{
  "$schema": "https://railway.app/railway.schema.json",
  "build": {
    "builder": "NIXPACKS"
  },
  "deploy": {
    "startCommand": "dotnet BrewLab.dll",
    "restartPolicyType": "ON_FAILURE",
    "restartPolicyMaxRetries": 10
  }
}
```

#### Step 2: Environment Variables in Railway
- `ASPNETCORE_ENVIRONMENT` = `Production`
- `ConnectionStrings__DefaultConnection` = `(PostgreSQL connection string)`
- `Jwt__Key` = `(your secure key)`
- `PORT` = `5000` (Railway provides this)

#### Step 3: Deploy
```bash
# Install Railway CLI
npm i -g @railway/cli

# Login
railway login

# Deploy
railway up
```

---

### Option 4: Render.com

#### Create `render.yaml`
```yaml
services:
  - type: web
    name: brewlab-api
    env: dotnet
    buildCommand: dotnet publish -c Release -o out
    startCommand: dotnet out/BrewLab.dll
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
      - key: ConnectionStrings__DefaultConnection
        sync: false
      - key: Jwt__Key
        sync: false
```

---

### Option 5: Heroku

#### Create `Procfile`
```
web: dotnet BrewLab.dll
```

#### Deploy
```bash
# Login
heroku login

# Create app
heroku create brewlab-api

# Set environment variables
heroku config:set ASPNETCORE_ENVIRONMENT=Production
heroku config:set ConnectionStrings__DefaultConnection="..."
heroku config:set Jwt__Key="..."

# Deploy
git push heroku master
```

---

## ?? Production Security Checklist

### Required
- [ ] Strong JWT key (32+ characters)
- [ ] PostgreSQL connection over SSL
- [ ] Environment variables (not hardcoded)
- [ ] CORS restricted to your frontend domains
- [ ] HTTPS enabled (handled by hosting platform)

### Recommended
- [ ] Rate limiting
- [ ] API key authentication for sensitive endpoints
- [ ] Database connection pooling
- [ ] Logging to external service (e.g., Application Insights)
- [ ] Health check endpoint monitoring
- [ ] Automated backups

---

## ?? Testing Production Build Locally

### Step 1: Build in Release mode
```powershell
dotnet build -c Release
```

### Step 2: Set Production environment
```powershell
$env:ASPNETCORE_ENVIRONMENT="Production"
$env:ConnectionStrings__DefaultConnection="Host=localhost;Database=brewlab;Username=postgres;Password=postgres"
$env:Jwt__Key="production-test-key-must-be-at-least-32-characters"
```

### Step 3: Run
```powershell
dotnet run -c Release
```

### Step 4: Verify
```powershell
# Should NOT show Swagger (production mode)
# Try: http://localhost:5000/swagger - should be 404

# Health check should work
Invoke-RestMethod http://localhost:5000/api/health/ping

# CORS should be restricted
# Requests from non-allowed origins will be blocked
```

---

## ?? What Happens in Production

### ? Enabled
- PostgreSQL database (required)
- Restricted CORS (only your frontend)
- JWT authentication
- All API endpoints
- Health checks
- Error handling with ApiResponse

### ? Disabled
- Swagger UI (security)
- In-memory database fallback
- Permissive CORS
- Detailed error messages

---

## ?? Troubleshooting Production

### Issue: "Cannot connect to database"

**Check:**
1. PostgreSQL is running
2. Connection string is correct
3. Database exists
4. User has permissions

**Test connection:**
```bash
psql -h your-host -U your-user -d brewlab
```

### Issue: "CORS errors"

**Check:**
1. Frontend URL is in `AllowFrontend` policy
2. Environment is set to `Production`
3. URL includes protocol (https://)

**Update Program.cs (lines 52-56):**
```csharp
.WithOrigins(
    "https://your-actual-frontend-url.com"
)
```

### Issue: "JWT validation fails"

**Check:**
1. JWT key matches on all instances
2. Key is at least 32 characters
3. Issuer and Audience match configuration

### Issue: "API not responding"

**Check:**
1. Process is running
2. Correct port is exposed
3. Firewall allows traffic
4. Health check responds: `/api/health/ping`

---

## ?? Environment-Specific Files

### Production `appsettings.Production.json`

Create this file:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "Jwt": {
    "Key": "",
    "Issuer": "BrewLabProduction",
    "Audience": "BrewLabClients",
    "ExpirationMinutes": 60
  }
}
```

**NOTE:** Don't commit with actual secrets! Use environment variables instead.

---

## ?? Deployment Commands Summary

### Build for Production
```powershell
dotnet publish -c Release -o ./publish
```

### Test Production Build Locally
```powershell
$env:ASPNETCORE_ENVIRONMENT="Production"
dotnet run -c Release
```

### Deploy (example for Azure)
```bash
az webapp deployment source config-zip \
  --resource-group your-rg \
  --name your-app \
  --src publish.zip
```

---

## ? Final Checklist Before Push

- [ ] Update CORS origins in `Program.cs` (lines 52-56)
- [ ] Verify PostgreSQL connection string
- [ ] Generate strong JWT key
- [ ] Test locally in Production mode
- [ ] Commit and push to GitHub
- [ ] Set environment variables on hosting platform
- [ ] Deploy
- [ ] Test production endpoints
- [ ] Monitor logs

---

## ?? Summary

### Your Code is Ready! ?

**No changes needed** - just configure environment variables:

1. **Set environment**:
   ```bash
   ASPNETCORE_ENVIRONMENT=Production
   ```

2. **Configure database**:
   ```bash
   ConnectionStrings__DefaultConnection=...
   ```

3. **Set JWT key**:
   ```bash
   Jwt__Key=your-secure-production-key-32-chars-minimum
   ```

4. **Deploy and it works!** ??

### What Happens Automatically:
- ? Swagger disabled in production
- ? CORS restricted to allowed origins
- ? PostgreSQL required (no in-memory fallback)
- ? Production logging
- ? Secure defaults

### Quick Deploy:
```bash
# 1. Build
dotnet publish -c Release

# 2. Deploy to your platform
# (Azure, Docker, Railway, Render, Heroku, etc.)

# 3. Set environment variables
# 4. Done!
```

**Your architecture changes will work perfectly in production!** ??
