# ?? CORS Configuration Fixed!

## ? What Was Fixed

### Problem:
- CORS was too restrictive (only allowed specific origins)
- Middleware order was incorrect
- Missing `AllowCredentials()` for auth

### Solution:
? **Two CORS policies:**
- **Development**: Allows ALL origins (for testing)
- **Production**: Restricted to specific origins

? **Fixed middleware order:**
- CORS before Authentication/Authorization
- Removed HTTPS redirection (caused issues)

? **Added support for:**
- Credentials (for JWT auth)
- localhost:5000 (for Swagger)
- All necessary headers and methods

---

## ?? Current CORS Configuration

### Development Mode (Default)
```csharp
app.UseCors("AllowAll"); // Allows any origin
```

**Allows:**
- ? Any origin
- ? Any header
- ? Any method
- ? Perfect for local testing

### Production Mode
```csharp
app.UseCors("AllowFrontend"); // Restricted origins
```

**Allowed Origins:**
- ? `http://localhost:5173` (Vite dev)
- ? `http://localhost:4173` (Vite preview)
- ? `http://localhost:5000` (Swagger)
- ? `http://127.0.0.1:5000` (Alternative localhost)
- ? `https://brew-lab-frontend.vercel.app` (Production)

---

## ?? Test CORS Now

### Step 1: Restart API (Important!)
```powershell
# Stop current API (Ctrl+C)
# Start fresh
dotnet run
```

### Step 2: Run CORS Test
```powershell
.\test-cors.ps1
```

This will:
- ? Test CORS headers
- ? Check multiple origins
- ? Create an HTML test page
- ? Open test in browser

### Step 3: Manual Browser Test
```javascript
// Open browser console (F12) and run:
fetch('http://localhost:5000/api/health/ping')
  .then(r => r.json())
  .then(d => console.log('Success:', d))
  .catch(e => console.error('CORS Error:', e));
```

**Expected Result:**
```json
{
  "success": true,
  "errorMessage": null,
  "data": "pong"
}
```

---

## ?? How to Identify CORS Errors

### In Browser Console (F12)

**CORS Error:**
```
Access to fetch at 'http://localhost:5000/api/...' from origin 'http://localhost:5173' 
has been blocked by CORS policy: No 'Access-Control-Allow-Origin' header is present...
```

**CORS Working:**
```
{success: true, errorMessage: null, data: {...}}
```

### In Network Tab

**CORS Error:**
- ? Request shows red
- ? Status: (failed) or CORS error
- ? No response data

**CORS Working:**
- ? Request shows 200 OK
- ? Response Headers include `Access-Control-Allow-Origin`
- ? Response data visible

---

## ??? Testing From Different Contexts

### 1. Swagger UI (Built-in Test)
```
http://localhost:5000/swagger
```
- Should work perfectly now
- Test any endpoint
- No CORS issues

### 2. React/Vue Frontend
```javascript
// In your frontend code
const response = await fetch('http://localhost:5000/api/health/ping', {
  method: 'GET',
  headers: {
    'Content-Type': 'application/json'
  }
});

const data = await response.json();
console.log(data); // Should show success: true
```

### 3. Authenticated Requests
```javascript
// With JWT token
const response = await fetch('http://localhost:5000/api/coffees', {
  method: 'GET',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  credentials: 'include' // Important for CORS with auth
});
```

### 4. POST Requests
```javascript
// Create coffee
const response = await fetch('http://localhost:5000/api/coffees', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  credentials: 'include',
  body: JSON.stringify({
    name: "Test Coffee",
    brand: "Test Brand",
    roast: "Medium",
    origin: "Test",
    tastingNotes: "Delicious"
  })
});
```

---

## ?? CORS Headers You Should See

When making requests, these headers should be present in the response:

```
Access-Control-Allow-Origin: * (in dev) or your-origin (in prod)
Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS
Access-Control-Allow-Headers: *
Access-Control-Allow-Credentials: true
```

### Check Headers in Browser:
1. Open DevTools (F12)
2. Go to Network tab
3. Make a request
4. Click on the request
5. Check "Response Headers"

---

## ?? Troubleshooting CORS Issues

### Issue 1: "No Access-Control-Allow-Origin header"

**Cause**: CORS middleware not applied or API not running

**Fix:**
```powershell
# Restart API to apply changes
dotnet run
```

---

### Issue 2: "Origin not allowed by CORS"

**Cause**: Your origin is not in the allowed list (Production mode)

**Fix**: Add your origin to Program.cs
```csharp
.WithOrigins(
    "http://localhost:5173",
    "http://localhost:4173",
    "http://localhost:5000",
    "http://your-origin-here" // Add this
)
```

Or use Development mode (already allows all):
```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run
```

---

### Issue 3: "Preflight request failed"

**Cause**: OPTIONS request not handled

**Fix**: Already fixed! CORS policy includes `.AllowAnyMethod()`

---

### Issue 4: Credentials issue with auth

**Cause**: Missing `credentials: 'include'` in fetch or `AllowCredentials()`

**Fix**: Already fixed in CORS config. In frontend, use:
```javascript
fetch(url, {
  credentials: 'include', // Add this
  headers: { 'Authorization': `Bearer ${token}` }
})
```

---

## ?? Quick Verification Checklist

- [ ] API is running (`dotnet run`)
- [ ] Running in Development mode (default)
- [ ] Can access Swagger: `http://localhost:5000/swagger`
- [ ] Health endpoint works: `http://localhost:5000/api/health/ping`
- [ ] CORS test passes: `.\test-cors.ps1`
- [ ] Browser console shows no CORS errors
- [ ] Frontend can make requests successfully

---

## ?? Frontend Integration Guide

### Update Your Frontend API Client

```javascript
// api.js or similar
const API_BASE_URL = 'http://localhost:5000';

export async function apiCall(endpoint, options = {}) {
  const url = `${API_BASE_URL}${endpoint}`;

  const config = {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...options.headers
    },
    credentials: 'include' // Important for CORS + Auth
  };

  const response = await fetch(url, config);
  const data = await response.json();

  // Check our new response format
  if (!data.success) {
    throw new Error(data.errorMessage || 'Request failed');
  }

  return data.data; // Return the actual data
}

// Example usage
export async function getCoffees(token) {
  return apiCall('/api/coffees', {
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });
}
```

---

## ?? Production Deployment

When deploying to production:

1. **Update allowed origins:**
```csharp
.WithOrigins(
    "https://your-production-domain.com",
    "https://www.your-production-domain.com"
)
```

2. **Set environment variable:**
```bash
export ASPNETCORE_ENVIRONMENT=Production
```

3. **Test CORS in production:**
```javascript
fetch('https://your-api.com/api/health/ping')
  .then(r => r.json())
  .then(console.log);
```

---

## ?? Summary

? **CORS is now properly configured!**
? **Development mode**: Allows all origins
? **Production mode**: Restricted to specific origins
? **Auth support**: Includes credentials
? **All methods/headers**: Fully supported

### Next Steps:

1. **Restart API** with the changes:
   ```powershell
   dotnet run
   ```

2. **Test CORS**:
   ```powershell
   .\test-cors.ps1
   ```

3. **Test from your frontend**:
   ```javascript
   fetch('http://localhost:5000/api/health/ping')
     .then(r => r.json())
     .then(console.log);
   ```

4. **If issues persist**, check:
   - Browser console for specific error
   - Network tab for CORS headers
   - API is running in Development mode

---

**Your API endpoints should now be accessible from any origin in Development mode!** ??
