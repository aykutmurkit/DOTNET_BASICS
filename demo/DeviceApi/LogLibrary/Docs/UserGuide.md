# LogLibrary User Guide

## Version 1.0.0
**Author:** R&D Engineer Aykut Mürkit, İsbak

## Table of Contents
1. [Introduction](#introduction)
2. [Installation](#installation)
3. [Basic Configuration](#basic-configuration)
4. [Advanced Configuration](#advanced-configuration)
5. [Logging Methods](#logging-methods)
6. [HTTP Request Logging](#http-request-logging)
7. [MongoDB Integration](#mongodb-integration)
8. [Troubleshooting](#troubleshooting)

## Introduction

LogLibrary is a comprehensive logging library for .NET applications that provides multi-target logging capabilities, structured data support, and MongoDB integration. The library is designed to be easily integrated into any .NET application and offers a simple API for logging operations.

## Installation

To install LogLibrary in your project, you can add it as a reference to your project:

```csharp
<ProjectReference Include="..\LogLibrary\LogLibrary.csproj" />
```

## Basic Configuration

LogLibrary can be configured in the `Program.cs` or `Startup.cs` file of your application:

```csharp
// Program.cs
builder.Services.AddLogLibrary(builder.Configuration);
```

Basic settings can be configured in the appsettings.json file:

```json
{
  "LogSettings": {
    "ApplicationName": "YourAppName",
    "Environment": "Development",
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "YourDatabaseName",
    "CollectionName": "Logs",
    "RetentionDays": 30
  }
}
```

## Advanced Configuration

For advanced configuration, you can customize the log settings in code:

```csharp
services.Configure<LogSettings>(options =>
{
    options.ApplicationName = "YourAppName";
    options.Environment = "Production";
    options.ConnectionString = "mongodb://user:password@mongodb.example.com:27017";
    options.DatabaseName = "LogDatabase";
    options.CollectionName = "ApplicationLogs";
    options.RetentionDays = 90;
});
```

## Logging Methods

LogLibrary provides several methods for different logging levels:

```csharp
// Basic logging
await _logService.LogInfoAsync("User logged in", "AuthController.Login", userData);
await _logService.LogWarningAsync("Login attempt failed", "AuthController.Login", attemptData);
await _logService.LogErrorAsync("Exception occurred", "PaymentService.Process", exception);
await _logService.LogDebugAsync("Debug information", "DataProcessor", debugData);
await _logService.LogCriticalAsync("System failure", "SystemMonitor", criticalException);

// With user context
await _logService.LogInfoAsync(
    "Profile updated", 
    "ProfileController.Update", 
    profileData, 
    userId: "123", 
    userName: "john.doe", 
    userEmail: "john@example.com"
);
```

## HTTP Request Logging

LogLibrary provides built-in support for HTTP request logging:

```csharp
await _logService.LogHttpAsync(
    path: "/api/users", 
    method: "POST", 
    statusCode: 201, 
    durationMs: 150, 
    traceId: Activity.Current?.Id,
    userId: User.FindFirstValue(ClaimTypes.NameIdentifier),
    userName: User.FindFirstValue(ClaimTypes.Name),
    ipAddress: HttpContext.Connection.RemoteIpAddress.ToString(),
    requestData: requestBody,
    responseData: responseBody
);
```

## MongoDB Integration

LogLibrary uses MongoDB for log storage with advanced features:

- Custom serialization for complex objects
- Automatic TTL indexes for log retention
- Efficient query support for log analysis

The library handles JObject serialization without type discriminators, making the stored data cleaner and more efficient.

## Troubleshooting

### MongoDB Connection Issues

If you're experiencing MongoDB connection issues:

1. Verify that your MongoDB server is running and accessible
2. Check the connection string format
3. Ensure you have appropriate permissions

### Missing Logs

If logs are not appearing in MongoDB:

1. Check the MongoDB connection status in the application logs
2. Verify that the `ILogService` is properly injected
3. Check MongoDB database/collection names in configuration

### Performance Issues

For performance optimization:

1. Use appropriate log levels to reduce unnecessary logging
2. Consider increasing MongoDB connection pool size for high-traffic applications
3. Add appropriate indexes for frequently queried fields in MongoDB

---

© İsbak, 2025. All rights reserved. 