# Quick Start

**Version:** 1.0.0  
**Author:** R&D Engineer Aykut Mürkit  
**Company:** ISBAK 2025

---

This guide will help you get started with DeviceApi quickly, showing you how to perform common operations and interact with the API.

## Prerequisites

Before starting, ensure you have:

- Completed the [Installation](02-Installation.md) steps
- A text editor or IDE for editing code
- A REST client like Postman, cURL, or similar
- Basic understanding of REST APIs and HTTP methods

## Running the API

1. Start the API server:

   ```bash
   cd DeviceApi
   dotnet run
   ```

2. The server will start on `https://localhost:5001` by default

3. Open a browser and navigate to `https://localhost:5001/swagger` to see the Swagger UI with all available endpoints

## Authentication

Most endpoints require authentication. Let's start by obtaining an authentication token:

### Step 1: Register a New User

```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "demo_user",
  "email": "demo@example.com",
  "password": "SecureP@ssw0rd123",
  "firstName": "Demo",
  "lastName": "User"
}
```

### Step 2: Login to Get a JWT Token

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "demo@example.com",
  "password": "SecureP@ssw0rd123"
}
```

The response will contain your JWT token:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "expiresIn": 3600
}
```

### Step 3: Use the Token in Subsequent Requests

Include the token in the Authorization header as a Bearer token:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Working with Devices

### Registering a New Device

```http
POST /api/devices
Content-Type: application/json
Authorization: Bearer your-token-here

{
  "name": "Temperature Sensor 1",
  "deviceType": "TEMPERATURE_SENSOR",
  "serialNumber": "TS-2025-001",
  "firmwareVersion": "1.0.0",
  "macAddress": "AA:BB:CC:DD:EE:FF",
  "location": {
    "latitude": 41.0082,
    "longitude": 28.9784,
    "address": "Istanbul, Turkey",
    "floor": 3,
    "room": "Server Room"
  },
  "properties": {
    "maxTemperature": 85,
    "minTemperature": -40,
    "accuracyLevel": "high"
  }
}
```

### Retrieving All Devices

```http
GET /api/devices?page=1&pageSize=10
Authorization: Bearer your-token-here
```

Response:

```json
{
  "totalItems": 1,
  "items": [
    {
      "id": "d290f1ee-6c54-4b01-90e6-d701748f0851",
      "name": "Temperature Sensor 1",
      "deviceType": "TEMPERATURE_SENSOR",
      "serialNumber": "TS-2025-001",
      "status": "ACTIVE",
      "createdAt": "2025-04-05T08:15:30Z",
      "lastConnectedAt": "2025-04-05T08:15:30Z"
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

### Retrieving Device Details

```http
GET /api/devices/d290f1ee-6c54-4b01-90e6-d701748f0851
Authorization: Bearer your-token-here
```

### Updating a Device

```http
PUT /api/devices/d290f1ee-6c54-4b01-90e6-d701748f0851
Content-Type: application/json
Authorization: Bearer your-token-here

{
  "name": "Temperature Sensor 1 - Updated",
  "firmwareVersion": "1.0.1",
  "status": "MAINTENANCE",
  "properties": {
    "maxTemperature": 90,
    "minTemperature": -40,
    "accuracyLevel": "high"
  }
}
```

### Deleting a Device

```http
DELETE /api/devices/d290f1ee-6c54-4b01-90e6-d701748f0851
Authorization: Bearer your-token-here
```

## Working with Device Data

### Sending Data from a Device

```http
POST /api/devices/d290f1ee-6c54-4b01-90e6-d701748f0851/data
Content-Type: application/json
Authorization: Bearer your-token-here

{
  "timestamp": "2025-04-05T10:15:30Z",
  "readings": {
    "temperature": 24.5,
    "humidity": 45.2,
    "batteryLevel": 78
  },
  "alerts": ["LOW_BATTERY"]
}
```

### Retrieving Device Data History

```http
GET /api/devices/d290f1ee-6c54-4b01-90e6-d701748f0851/data?startDate=2025-04-01T00:00:00Z&endDate=2025-04-05T23:59:59Z&page=1&pageSize=100
Authorization: Bearer your-token-here
```

## Using Filtering, Sorting, and Pagination

The API supports filtering, sorting, and pagination for collection endpoints:

### Filtering

```http
GET /api/devices?deviceType=TEMPERATURE_SENSOR&status=ACTIVE
Authorization: Bearer your-token-here
```

### Sorting

```http
GET /api/devices?sortBy=name&sortOrder=asc
Authorization: Bearer your-token-here
```

### Pagination

```http
GET /api/devices?page=2&pageSize=25
Authorization: Bearer your-token-here
```

## Working with Device Groups

### Creating a Device Group

```http
POST /api/groups
Content-Type: application/json
Authorization: Bearer your-token-here

{
  "name": "Server Room Sensors",
  "description": "All sensors in the server room",
  "devices": [
    "d290f1ee-6c54-4b01-90e6-d701748f0851"
  ]
}
```

### Adding Devices to a Group

```http
POST /api/groups/f47ac10b-58cc-4372-a567-0e02b2c3d479/devices
Content-Type: application/json
Authorization: Bearer your-token-here

{
  "deviceIds": [
    "7bba9078-8d1f-462d-81ef-24962acfe9b5",
    "32c3e9b5-0b5e-4f2d-8b9d-15a1f901aa3c"
  ]
}
```

## Error Handling

The API returns standard HTTP status codes along with descriptive error messages:

```json
{
  "status": 400,
  "message": "Invalid request parameters",
  "errors": [
    {
      "field": "name",
      "message": "Name is required"
    }
  ],
  "timestamp": "2025-04-05T10:30:45Z",
  "path": "/api/devices"
}
```

## Next Steps

Now that you're familiar with the basic operations of DeviceApi, you can:

1. Explore the complete [API Endpoints](04-API-Endpoints.md) documentation
2. Learn about [Authentication](05-Authentication.md) in detail
3. Understand the [Data Models](06-Data-Models.md)
4. Check out [Configuration](07-Configuration.md) options

For advanced topics, see:

- [Performance Optimization](09-Performance-Optimization.md)
- [Security](10-Security.md)
- [Deployment](11-Deployment.md)

---

[◀ Installation](02-Installation.md) | [Home](README.md) | [Next: API Endpoints ▶](04-API-Endpoints.md) 