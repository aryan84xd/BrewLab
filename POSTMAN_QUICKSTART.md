# BrewLab API - Quick Reference

## ?? Import to Postman

1. Open Postman
2. Click **Import** ? Select files:
   - `BrewLab.postman_collection.json`
   - `BrewLab.postman_environment.json`
3. Select **BrewLab Local** environment from dropdown

## ? Quick Test (3 steps)

```
1. Auth ? Register          ? Send (token saved automatically ?)
2. Coffees ? Create Coffee  ? Send (coffee ID saved ?)
3. Experiments ? Create     ? Send (uses saved coffee ID ?)
```

## ?? All Endpoints

### Auth (No token required)
```
POST   /api/auth/register     Register new user
POST   /api/auth/login        Login
GET    /api/auth/me          Get current user (requires token)
```

### Coffees (Token required)
```
GET    /api/coffees          Get all coffees
GET    /api/coffees/{id}     Get coffee by ID
POST   /api/coffees          Create coffee
```

### Experiments (Token required)
```
GET    /api/experiment/{coffeeId}    Get experiments for coffee
POST   /api/experiment               Create experiment
```

## ?? Environment Variables

Automatically set by scripts:
- `{{jwt_token}}` - After login/register
- `{{coffee_id}}` - After creating coffee
- `{{experiment_id}}` - After creating experiment

## ?? Sample Bodies

**Register/Login:**
```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

**Create Coffee:**
```json
{
  "name": "Ethiopian Yirgacheffe",
  "brand": "Blue Bottle",
  "roast": "Light",
  "origin": "Ethiopia",
  "tastingNotes": "Floral, citrus"
}
```

**Create Experiment:**
```json
{
  "coffeeId": "{{coffee_id}}",
  "brewMethod": "Pour Over",
  "coffeeWeight": 15.5,
  "waterWeight": 250.0,
  "brewTime": "03:30:00",
  "remark": "Perfect cup",
  "aroma": 8,
  "acidity": 7,
  "body": 6,
  "overall": 8
}
```

## ?? Common Issues

**401 Unauthorized**: Run Login/Register first
**404 Coffee not found**: Create a coffee first
**Connection refused**: Start API (`dotnet run`)

## ?? Change API URL

In environment settings, update `base_url`:
- Local: `http://localhost:5000`
- Production: `https://your-api.com`

---
See `POSTMAN_GUIDE.md` for detailed documentation
