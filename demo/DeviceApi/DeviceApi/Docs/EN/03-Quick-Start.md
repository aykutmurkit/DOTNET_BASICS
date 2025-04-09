# Quick Start

**Version:** 1.0.0  
**Author:** R&D Engineer Aykut Mürkit  
**Company:** İSBAK 2025

---

## API Overview

DeviceApi is a RESTful API developed for managing IoT devices. This guide will help you get started quickly with the API.

## Basic Endpoints

### Authentication

```http
POST /api/Auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

### Device Management

```http
# List all devices
GET /api/Devices

# Add new device
POST /api/Devices
Content-Type: application/json

{
  "name": "Test Device",
  "ip": "192.168.1.100",
  "port": 8080,
  "platformId": 1
}

# Update device
PUT /api/Devices/{id}
Content-Type: application/json

{
  "name": "Updated Device",
  "ip": "192.168.1.101",
  "port": 8081
}
```

### Message Management

#### Full Screen Messages

```http
# List all full screen messages
GET /api/FullScreenMessages

# Add new full screen message
POST /api/FullScreenMessages
Content-Type: application/json

{
  "turkishMessage": "Hello World",
  "englishMessage": "Hello World"
}
```

#### Scrolling Screen Messages

```http
# List all scrolling screen messages
GET /api/ScrollingScreenMessages

# Add new scrolling screen message
POST /api/ScrollingScreenMessages
Content-Type: application/json

{
  "turkishLines": ["Line 1", "Line 2"],
  "englishLines": ["Line 1", "Line 2"]
}
```

#### Bitmap Screen Messages

```http
# List all bitmap screen messages
GET /api/BitmapScreenMessages

# Add new bitmap screen message
POST /api/BitmapScreenMessages
Content-Type: application/json

{
  "turkishBitmap": "base64_encoded_bitmap",
  "englishBitmap": "base64_encoded_bitmap"
}
```

#### Periodic Messages

```http
# List all periodic messages
GET /api/PeriodicMessages

# Add new periodic message
POST /api/PeriodicMessages
Content-Type: application/json

{
  "message": "Periodic Message",
  "startTime": "2024-01-01T00:00:00",
  "endTime": "2024-12-31T23:59:59",
  "intervalInMinutes": 60
}
```

## Example Usage Scenarios

### Scenario 1: Adding Device and Assigning Message

1. Add a new device
2. Assign a full screen message to the device
3. Check device status

```http
# 1. Add device
POST /api/Devices
{
  "name": "New Device",
  "ip": "192.168.1.100",
  "port": 8080,
  "platformId": 1
}

# 2. Create full screen message
POST /api/FullScreenMessages
{
  "turkishMessage": "Welcome",
  "englishMessage": "Welcome"
}

# 3. Assign message to device
POST /api/Devices/{deviceId}/assign-message
{
  "messageId": 1,
  "messageType": "FullScreen"
}

# 4. Check device status
GET /api/Devices/{deviceId}/status
```

### Scenario 2: Multiple Message Management

1. Create different types of messages
2. Assign messages to devices
3. Monitor message statuses

```http
# 1. Create different types of messages
POST /api/FullScreenMessages
{
  "turkishMessage": "Full Screen Message",
  "englishMessage": "Full Screen Message"
}

POST /api/ScrollingScreenMessages
{
  "turkishLines": ["Scroll 1", "Scroll 2"],
  "englishLines": ["Scroll 1", "Scroll 2"]
}

POST /api/BitmapScreenMessages
{
  "turkishBitmap": "base64_encoded_bitmap",
  "englishBitmap": "base64_encoded_bitmap"
}

# 2. Assign messages to devices
POST /api/Devices/{deviceId}/assign-message
{
  "messageId": 1,
  "messageType": "FullScreen"
}

POST /api/Devices/{deviceId}/assign-message
{
  "messageId": 1,
  "messageType": "ScrollingScreen"
}

# 3. Check message statuses
GET /api/Devices/{deviceId}/messages
```

## Error Handling

The API uses standard HTTP status codes and error messages:

- `200 OK`: Operation successful
- `201 Created`: Resource successfully created
- `400 Bad Request`: Invalid request
- `401 Unauthorized`: Authentication required
- `403 Forbidden`: Unauthorized access
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

Example error response:

```json
{
  "success": false,
  "message": "Device not found",
  "errors": [
    {
      "code": "DeviceNotFound",
      "description": "Device with specified ID not found"
    }
  ]
}
```

## Security

To access the API:

1. Get a JWT token using the `/api/Auth/login` endpoint
2. Send the token in the `Authorization` header:
   ```
   Authorization: Bearer your_jwt_token
   ```

## Next Steps

1. Review the [Architecture](06-Architecture.md) document
2. Learn about [Data Models](05-Data-Models.md)
3. Read the [Best Practices](09-Best-Practices.md) guide

---

[◀ Installation](02-Installation.md) | [Next: Data Models ▶](05-Data-Models.md) 