# BrewLab Postman Collection

Complete Postman collection for testing the BrewLab API with automatic token management.

## ?? Files Included

- **BrewLab.postman_collection.json** - Main API collection with all endpoints
- **BrewLab.postman_environment.json** - Local development environment
- **BrewLab.postman_environment.production.json** - Production environment template

## ?? Quick Start

### 1. Import Collection

1. Open Postman
2. Click **Import** button (top left)
3. Drag and drop `BrewLab.postman_collection.json`
4. Click **Import**

### 2. Import Environment

1. Click **Import** again
2. Drag and drop `BrewLab.postman_environment.json`
3. Click **Import**
4. Select **BrewLab Local** from the environment dropdown (top right)

### 3. Start Testing!

The collection is ready to use. Follow the **Testing Workflow** below.

## ?? Testing Workflow

### Step 1: Authentication

**Register a New User:**
1. Open `Auth` ? `Register`
2. Click **Send**
3. ? JWT token is automatically saved to environment

**Or Login with Existing User:**
1. Open `Auth` ? `Login`
2. Update email/password in body if needed
3. Click **Send**
4. ? JWT token is automatically saved to environment

**Verify Authentication:**
1. Open `Auth` ? `Get Current User (Me)`
2. Click **Send**
3. Should return your user details

### Step 2: Create Coffees

**Create First Coffee:**
1. Open `Coffees` ? `Create Coffee - Ethiopian Yirgacheffe`
2. Click **Send**
3. ? Coffee ID is automatically saved to environment variable `coffee_id`

**Create More Coffees (Optional):**
- `Create Coffee - Colombian Supremo`
- `Create Coffee - Sumatra Mandheling`

**View All Coffees:**
1. Open `Coffees` ? `Get All Coffees`
2. Click **Send**
3. See all your coffees listed

**View Single Coffee:**
1. Open `Coffees` ? `Get Coffee by ID`
2. Click **Send** (uses `{{coffee_id}}` from environment)

### Step 3: Create Experiments

**Prerequisites:** You must have created at least one coffee first!

**Create Experiments for a Coffee:**
1. Open `Experiments` ? `Create Experiment - Pour Over`
2. Click **Send**
3. Try other brewing methods:
   - `Create Experiment - French Press`
   - `Create Experiment - Aeropress`
   - `Create Experiment - Espresso`

**View All Experiments for Coffee:**
1. Open `Experiments` ? `Get Experiments for Coffee`
2. Click **Send**
3. See all experiments for the current `{{coffee_id}}`

## ?? API Endpoints Reference

### Authentication Endpoints

| Method | Endpoint | Auth Required | Description |
|--------|----------|---------------|-------------|
| POST | `/api/auth/register` | No | Register new user |
| POST | `/api/auth/login` | No | Login existing user |
| GET | `/api/auth/me` | Yes | Get current user info |

### Coffee Endpoints

| Method | Endpoint | Auth Required | Description |
|--------|----------|---------------|-------------|
| GET | `/api/coffees` | Yes | Get all user's coffees |
| GET | `/api/coffees/{id}` | Yes | Get specific coffee |
| POST | `/api/coffees` | Yes | Create new coffee |

### Experiment Endpoints

| Method | Endpoint | Auth Required | Description |
|--------|----------|---------------|-------------|
| GET | `/api/experiment/{coffeeId}` | Yes | Get all experiments for a coffee |
| POST | `/api/experiment` | Yes | Create new experiment |

## ?? Automatic Token Management

The collection includes **automatic JWT token handling**:

1. **Register** or **Login** ? Token automatically saved to `{{jwt_token}}`
2. All protected endpoints use: `Authorization: Bearer {{jwt_token}}`
3. No manual token copying needed! ??

## ?? Environment Variables

The collection uses these environment variables:

| Variable | Description | Auto-Set |
|----------|-------------|----------|
| `base_url` | API base URL | Manual |
| `jwt_token` | JWT authentication token | ? Auto (on login/register) |
| `user_email` | Logged-in user's email | ? Auto (on login/register) |
| `user_name` | Logged-in user's name | ? Auto (on login/register) |
| `coffee_id` | Last created coffee ID | ? Auto (on coffee creation) |
| `experiment_id` | Last created experiment ID | ? Auto (on experiment creation) |

### Changing Environment

**For Local Development:**
- Select **BrewLab Local** environment
- Default: `http://localhost:5000`

**For Production:**
1. Import `BrewLab.postman_environment.production.json`
2. Update `base_url` to your production URL
3. Select **BrewLab Production** environment

## ?? Sample Request Bodies

### Register/Login
```json
{
  "name": "John Doe",
  "email": "john.doe@example.com",
  "password": "SecurePassword123!"
}
```

### Create Coffee
```json
{
  "name": "Ethiopian Yirgacheffe",
  "brand": "Blue Bottle",
  "roast": "Light",
  "origin": "Ethiopia",
  "tastingNotes": "Floral, citrus, tea-like body"
}
```

### Create Experiment
```json
{
  "coffeeId": "{{coffee_id}}",
  "brewMethod": "Pour Over (V60)",
  "coffeeWeight": 15.5,
  "waterWeight": 250.0,
  "brewTime": "03:30:00",
  "remark": "Perfect extraction, well-balanced cup",
  "aroma": 8,
  "acidity": 7,
  "body": 6,
  "overall": 8
}
```

## ?? BrewTime Format

The `brewTime` field uses **HH:MM:SS** format:
- `"03:30:00"` = 3 minutes 30 seconds
- `"00:28:00"` = 28 seconds
- `"04:00:00"` = 4 minutes

## ?? Rating Scale

Experiments use a **1-10 rating scale** for:
- **Aroma**: Fragrance and smell intensity
- **Acidity**: Brightness and tanginess
- **Body**: Mouthfeel and texture
- **Overall**: Overall impression

## ?? Troubleshooting

### "Unauthorized" Error
- Make sure you've run **Login** or **Register** first
- Check that `{{jwt_token}}` has a value in environment
- Token may have expired - login again

### "Coffee not found for this user"
- Create a coffee first using `POST /api/coffees`
- Make sure `{{coffee_id}}` environment variable is set
- Verify you're using the correct coffee ID

### "Invalid credentials"
- Check email and password are correct
- User may not exist - try **Register** instead

### Connection Refused
- Make sure API is running: `dotnet run`
- Check `base_url` matches your API port (default: 5000)
- Verify PostgreSQL database is running

## ?? Testing Different Coffee IDs

To test experiments with different coffees:

1. Create multiple coffees
2. Copy a coffee ID from the response
3. In Postman, set environment variable manually:
   - Click environment quick look (eye icon)
   - Edit `coffee_id` value
   - Paste the new coffee ID
4. Create experiments for that coffee

## ?? Response Examples

### Successful Login Response
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAtUtc": "2024-03-08T12:00:00Z",
  "name": "John Doe",
  "email": "john.doe@example.com"
}
```

### Get Coffees Response
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Ethiopian Yirgacheffe",
    "brand": "Blue Bottle",
    "roast": "Light",
    "origin": "Ethiopia",
    "tastingNotes": "Floral, citrus, tea-like body"
  }
]
```

### Get Experiments Response
```json
[
  {
    "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "coffeeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "date": "2024-03-07T10:30:00Z",
    "brewMethod": "Pour Over (V60)",
    "coffeeWeight": 15.5,
    "waterWeight": 250.0,
    "brewTime": "03:30:00",
    "remark": "Perfect extraction, well-balanced cup",
    "aroma": 8,
    "acidity": 7,
    "body": 6,
    "overall": 8
  }
]
```

## ?? Pro Tips

1. **Run in Order**: First time testing, run requests in this order:
   - Register ? Create Coffee ? Create Experiment ? Get Experiments

2. **Use Pre-built Requests**: The collection includes ready-to-use coffee and experiment examples

3. **Scripts Included**: Token management is automatic via Postman test scripts

4. **Multiple Environments**: Switch between local and production easily

5. **Variables**: Use `{{variable}}` syntax to reference dynamic values

## ?? Support

If you encounter issues:
1. Check the API is running: `dotnet run`
2. Verify database connection in `appsettings.json`
3. Review `MIGRATION_GUIDE.md` for setup steps
4. Check Postman Console (View ? Show Postman Console) for detailed errors

---

Happy Testing! ????
