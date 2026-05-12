# API Response Examples

## New Response Format

All API endpoints now return HTTP 200 with a standardized response structure:

```json
{
  "success": true/false,
  "errorMessage": "error description" or null,
  "data": { ... } or null
}
```

---

## Coffee Endpoints

### GET /api/coffees
**Success Response:**
```json
{
  "success": true,
  "errorMessage": null,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Ethiopian Yirgacheffe",
      "brand": "Blue Bottle",
      "roast": "Light",
      "origin": "Ethiopia",
      "tastingNotes": "Floral, citrus, tea-like"
    },
    {
      "id": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
      "name": "Colombian Supremo",
      "brand": "Stumptown",
      "roast": "Medium",
      "origin": "Colombia",
      "tastingNotes": "Chocolate, caramel, nutty"
    }
  ]
}
```

**Error Response (Unauthorized):**
```json
{
  "success": false,
  "errorMessage": "Unauthorized access.",
  "data": null
}
```

---

### GET /api/coffees/{id}
**Success Response:**
```json
{
  "success": true,
  "errorMessage": null,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Ethiopian Yirgacheffe",
    "brand": "Blue Bottle",
    "roast": "Light",
    "origin": "Ethiopia",
    "tastingNotes": "Floral, citrus, tea-like"
  }
}
```

**Error Response (Not Found):**
```json
{
  "success": false,
  "errorMessage": "Coffee not found.",
  "data": null
}
```

---

### POST /api/coffees
**Request Body:**
```json
{
  "name": "Ethiopian Yirgacheffe",
  "brand": "Blue Bottle",
  "roast": "Light",
  "origin": "Ethiopia",
  "tastingNotes": "Floral, citrus, tea-like"
}
```

**Success Response:**
```json
{
  "success": true,
  "errorMessage": null,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Ethiopian Yirgacheffe",
    "brand": "Blue Bottle",
    "roast": "Light",
    "origin": "Ethiopia",
    "tastingNotes": "Floral, citrus, tea-like"
  }
}
```

---

## Experiment Endpoints

### GET /api/experiment/{coffeeId}
**Success Response:**
```json
{
  "success": true,
  "errorMessage": null,
  "data": [
    {
      "id": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
      "coffeeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-01-15T10:30:00Z",
      "brewMethod": "V60",
      "coffeeWeight": 18.5,
      "waterWeight": 300.0,
      "brewTime": "00:02:30",
      "remark": "Perfect extraction",
      "aroma": 5,
      "acidity": 4,
      "body": 3,
      "overall": 9
    }
  ]
}
```

**Error Response (Coffee Not Found):**
```json
{
  "success": false,
  "errorMessage": "Coffee not found for this user.",
  "data": null
}
```

---

### POST /api/experiment
**Request Body:**
```json
{
  "coffeeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "brewMethod": "V60",
  "coffeeWeight": 18.5,
  "waterWeight": 300.0,
  "brewTime": "00:02:30",
  "remark": "Perfect extraction",
  "aroma": 5,
  "acidity": 4,
  "body": 3,
  "overall": 9
}
```

**Success Response:**
```json
{
  "success": true,
  "errorMessage": null,
  "data": {
    "id": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
    "coffeeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "date": "2024-01-15T10:30:00Z",
    "brewMethod": "V60",
    "coffeeWeight": 18.5,
    "waterWeight": 300.0,
    "brewTime": "00:02:30",
    "remark": "Perfect extraction",
    "aroma": 5,
    "acidity": 4,
    "body": 3,
    "overall": 9
  }
}
```

**Error Response (Invalid Coffee):**
```json
{
  "success": false,
  "errorMessage": "Coffee not found for the user.",
  "data": null
}
```

---

## Authentication Endpoints

### POST /api/auth/register
**Request Body:**
```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

**Success Response:**
```json
{
  "success": true,
  "errorMessage": null,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "john@example.com",
    "name": "John Doe",
    "expiresAtUtc": "2024-01-15T10:30:00Z"
  }
}
```

**Error Response (Email Exists):**
```json
{
  "success": false,
  "errorMessage": "Email already registered.",
  "data": null
}
```

---

### POST /api/auth/login
**Request Body:**
```json
{
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

**Success Response:**
```json
{
  "success": true,
  "errorMessage": null,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "john@example.com",
    "name": "John Doe",
    "expiresAtUtc": "2024-01-15T10:30:00Z"
  }
}
```

**Error Response (Invalid Credentials):**
```json
{
  "success": false,
  "errorMessage": "Invalid credentials.",
  "data": null
}
```

---

### GET /api/auth/me
**Success Response:**
```json
{
  "success": true,
  "errorMessage": null,
  "data": {
    "id": "6fa85f64-5717-4562-b3fc-2c963f66afa9",
    "name": "John Doe",
    "email": "john@example.com"
  }
}
```

**Error Response (Unauthorized):**
```json
{
  "success": false,
  "errorMessage": "Unauthorized access.",
  "data": null
}
```

---

## Frontend Integration Tips

### Axios Example
```typescript
try {
  const response = await axios.get('/api/coffees');

  // Response is always 200, check success field
  if (response.data.success) {
    const coffees = response.data.data;
    // Handle success
  } else {
    // Handle error with response.data.errorMessage
    showError(response.data.errorMessage);
  }
} catch (error) {
  // Handle network errors only
  showError('Network error occurred');
}
```

### Fetch Example
```typescript
const response = await fetch('/api/coffees/{id}');
const result = await response.json();

if (result.success) {
  const coffee = result.data;
  // Handle success
} else {
  // Handle error
  console.error(result.errorMessage);
}
```

### React Query Example
```typescript
const { data, error } = useQuery({
  queryKey: ['coffee', id],
  queryFn: async () => {
    const response = await fetch(`/api/coffees/${id}`);
    const result = await response.json();

    if (!result.success) {
      throw new Error(result.errorMessage);
    }

    return result.data;
  }
});
```
