# LogLib - Advanced Logging Library

**Version:** 1.0.0  
**Author:** R&D Engineer Aykut Mürkit  
**Company:** İsbak  

## Overview
LogLib is a comprehensive logging library for .NET applications providing both file and MongoDB-based logging capabilities with enhanced serialization support for complex JSON objects.

## Features

### Core Features
- Multi-target logging (Console, File, MongoDB)
- Structured logging with metadata support
- Flexible configuration options
- Asynchronous logging operations
- Custom log level support
- Rich context data capture

### MongoDB Integration
- Automatic TTL index creation
- Custom serialization for complex objects
- Support for Newtonsoft.Json.Linq types (JObject, JArray)
- Connection pooling and error handling

### Performance
- Asynchronous operations
- Batched writes
- Optimized serialization

## Configuration

### Basic Setup
```csharp
// Add to Program.cs
builder.Services.AddLogLib(builder.Configuration);
```

### appsettings.json
```json
{
  "LogSettings": {
    // File logging settings
    "LogToFile": true,
    "LogFilePath": "Logs/app.log",
    "FileSizeLimitBytes": 10485760,
    "RetainedFileCount": 7,
    
    // MongoDB settings
    "LogToMongoDB": true,
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "LogsDb",
    "CollectionName": "Logs",
    "RetentionDays": 30
  }
}
```

## Usage Examples

### Basic Logging
```csharp
// Constructor injection
private readonly ILogService _logService;

public SomeController(ILogService logService)
{
    _logService = logService;
}

// Info log
await _logService.LogInfoAsync(
    "User login successful", 
    "AuthController.Login",
    new { Username = "user123" });

// Error log
try
{
    // Some operation
}
catch (Exception ex)
{
    await _logService.LogErrorAsync(
        "Error during operation",
        "ServiceName.MethodName",
        ex);
}
```

### Advanced Context
```csharp
// Log with user context
await _logService.LogInfoAsync(
    "Profile updated", 
    "ProfileController.Update",
    new { Fields = new[] { "email", "name" } },
    userId: "123",
    userName: "user123",
    userEmail: "user@example.com");
```

## Technical Details

### MongoDB Serialization
The library implements custom serialization for MongoDB to properly handle complex object types:

- Custom serializers for `JObject` and `JArray` types
- Support for both reading legacy data with `_t`/`_v` structure and writing clean data
- Disabled type discriminators to provide cleaner data structure
- Efficient binary serialization for performance

### Implementation Highlights

```csharp
// Custom JObject serializer
private class JObjectSerializer : SerializerBase<JObject>
{
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JObject value)
    {
        if (value == null)
        {
            context.Writer.WriteNull();
            return;
        }
        
        // Direct serialization without type discriminators
        var document = BsonDocument.Parse(value.ToString());
        BsonSerializer.Serialize(context.Writer, document);
    }
    
    // ... deserialization with support for legacy formats
}
```

## Release Notes

### Version 1.0.0
- Initial release
- MongoDB integration with custom serialization
- Support for Newtonsoft.Json.Linq objects
- File and console logging

## License
Proprietary software. Copyright © 2025 İsbak. 