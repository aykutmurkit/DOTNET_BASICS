# LogLibrary Architecture

## Version 1.0.0
**Author:** R&D Engineer Aykut Mürkit, İsbak

## Overview

LogLibrary is designed with a layered architecture to provide flexibility, extensibility and separation of concerns. The library consists of several key components:

```
┌─────────────────────────────────────────────────────┐
│                   Client Application                 │
└───────────────────────┬─────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────┐
│                     ILogService                      │
└───────────────────────┬─────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────┐
│                    LogService                        │
└──────┬─────────────────┬──────────────────┬─────────┘
       │                 │                  │
       ▼                 ▼                  ▼
┌─────────────┐  ┌─────────────┐  ┌──────────────────┐
│ Console Log │  │   File Log  │  │  MongoDB Log     │
└─────────────┘  └─────────────┘  └────────┬─────────┘
                                           │
                                           ▼
                               ┌──────────────────────┐
                               │   MongoDbContext     │
                               └──────────┬───────────┘
                                          │
                                          ▼
                               ┌──────────────────────┐
                               │ Custom Serializers   │
                               │ - JObjectSerializer  │
                               │ - JArraySerializer   │
                               │ - BsonDocumentSer.   │
                               └──────────────────────┘
```

## Components

### 1. Core Layer

Contains domain models and interfaces:

- **LogEntry**: The main model representing a log record
- **ILogService**: Interface defining logging operations
- **ILogRepository**: Interface for data access operations

### 2. Services Layer

Contains the implementation of business logic:

- **LogService**: Implements the ILogService interface, handles log creation and formatting

### 3. Data Layer

Manages data access and persistence:

- **MongoDbContext**: Manages MongoDB connection and serialization
- **LogRepository**: Implements ILogRepository for MongoDB operations

### 4. Custom Serialization

Special serializers for handling complex data types:

- **JObjectSerializer**: Handles JObject serialization without type discriminators
- **JArraySerializer**: Handles JArray serialization without type discriminators 
- **BsonDocumentSerializer**: Custom serializer for generic objects

## Sequence Diagrams

### Basic Logging Flow

```
┌───────────┐     ┌────────────┐     ┌──────────────┐    ┌────────────┐
│ Controller│     │ LogService │     │LogRepository │    │MongoDbContext│
└─────┬─────┘     └──────┬─────┘     └──────┬───────┘    └──────┬─────┘
      │                  │                   │                   │
      │  LogInfoAsync    │                   │                   │
      │─────────────────>│                   │                   │
      │                  │                   │                   │
      │                  │  CreateLogEntry   │                   │
      │                  │   (constructs     │                   │
      │                  │    LogEntry)      │                   │
      │                  │                   │                   │
      │                  │   SaveLogAsync    │                   │
      │                  │──────────────────>│                   │
      │                  │                   │    InsertOne      │
      │                  │                   │──────────────────>│
      │                  │                   │                   │
      │                  │                   │                   │
      │                  │                   │    Serialization  │
      │                  │                   │                   │
      │                  │                   │   DB Operation    │
      │                  │                   │                   │
      │                  │                   │<─ ─ ─ ─ ─ ─ ─ ─ ─ │
      │                  │<─ ─ ─ ─ ─ ─ ─ ─ ─│                   │
      │<─ ─ ─ ─ ─ ─ ─ ─ ─│                   │                   │
      │                  │                   │                   │
```

## MongoDB Serialization

The custom serialization system in LogLibrary is designed to handle complex data types like JObject without adding type information:

### Before (with type discriminators):
```json
{
  "Data": {
    "_t": "Newtonsoft.Json.Linq.JObject",
    "_v": {
      "userId": 123,
      "action": "login"
    }
  }
}
```

### After (with custom serialization):
```json
{
  "Data": {
    "userId": 123,
    "action": "login"
  }
}
```

## Extension Points

LogLibrary is designed to be extensible:

1. **Custom Log Targets**: Implement ILogRepository for new storage targets
2. **Custom Serializers**: Add serializers for additional complex types
3. **Log Filtering**: Implement custom log filtering logic in LogService

---

© İsbak, 2023. All rights reserved. 