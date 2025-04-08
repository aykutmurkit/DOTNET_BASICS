# Database Seeding Process

**Version:** 1.0.0  
**Author:** R&D Engineer Aykut Mürkit  
**Company:** ISBAK 2025

---

This document explains the detailed structure, working principles, implementation methods, and troubleshooting strategies of the database seed processes in the DeviceApi project. The seeding process ensures the automatic creation of necessary data during the initial setup of the application or when the database is reset.

## Table of Contents

1. [Overview](#1-overview)
2. [Seeding Architecture](#2-seeding-architecture)
3. [Seeder Types and Execution Order](#3-seeder-types-and-execution-order)
4. [Data Relationships and Structures](#4-data-relationships-and-structures)
5. [Seed Process Flow](#5-seed-process-flow)
6. [Performance Optimization](#6-performance-optimization)
7. [Troubleshooting](#7-troubleshooting)
8. [Adding New Seeders](#8-adding-new-seeders)
9. [Advanced Topics](#9-advanced-topics)
10. [Best Practices](#10-best-practices)

## 1. Overview

DeviceApi provides an automatic database seeding mechanism for the project to quickly become operational in different environments (development, testing, demo). This mechanism allows:

- New developers to quickly obtain a functioning system
- Test environments to be initialized with consistent data sets
- Demo environments to be prepared with realistic data
- Easy database reset and restart during development

The seeding process automatically runs when the `DatabaseSettings:ResetDatabaseOnStartup` configuration value is set to `true` in the `Program.cs` file. This process completely deletes the database, recreates it, and then adds the basic data.

## 2. Seeding Architecture

### 2.1 Core Components

The seeding architecture consists of the following core components:

- **ISeeder Interface**: The contract that all seed classes must implement
- **DatabaseSeeder**: The main class that coordinates all seed operations
- **Specific Seeder Classes**: Classes that perform customized seed operations for each data type
- **SeederOrder Enum**: Enumeration that determines the execution order of seed operations
- **SeederExtensions**: Helper methods that facilitate seed operations

### 2.2 ISeeder Interface

The basic interface that all seeders must implement:

```csharp
public interface ISeeder
{
    int Order { get; }
    Task SeedAsync(AppDbContext context);
}
```

This interface contains two core elements:
- **Order**: The priority value that determines the execution order of the seeder
- **SeedAsync**: The asynchronous method that adds seed data to the database

### 2.3 Architectural Structure

The seeding architecture uses a layered structure that follows the data hierarchy and dependencies:

```
┌───────────────────────────────────────────────────────────────┐
│                        Program.cs                              │
│              (Initiates the seed process at startup)          │
└───────────────────────────────┬───────────────────────────────┘
                                │
                                ▼
┌───────────────────────────────────────────────────────────────┐
│                      DatabaseSeeder                            │
│        (Finds, sorts, and executes all seeders)               │
└───────────────────────────────┬───────────────────────────────┘
                                │
                                ▼
┌──────────────────────────────────────────────────────────────┐
│             Finding ISeeder classes using Reflection         │
└──────────────────────────────┬───────────────────────────────┘
                               │
                ┌──────────────┼─────────────┬─────────────────┐
                │              │             │                 │
                ▼              ▼             ▼                 ▼
┌───────────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────────┐
│  Reference Data   │ │    Base     │ │ Core Entity │ │ Relational      │
│(AlignmentTypeSeeder)│ │(StationSeeder)│ │(DeviceSeeder) │ │ Entities       │
└───────────────────┘ └─────────────┘ └─────────────┘ └─────────────────┘
```

## 3. Seeder Types and Execution Order

### 3.1 Seeder Types

Seeders in DeviceApi are functionally divided into four main categories:

1. **Reference Data Seeders**: For basic constants and enums (AlignmentTypeSeeder)
2. **Base Data Seeders**: For the system's core entities (StationSeeder, PlatformSeeder)
3. **Business Entity Seeders**: For core business entities (DeviceSeeder, DeviceSettingsSeeder)
4. **Relational Data Seeders**: For data related to business entities (MessageSeeders)

### 3.2 Seeder Execution Order

Seeders run in a specific order to correctly establish relationships between tables:

| Order | Seeder Name | Order Value | Dependencies | Description |
| ---- | ---------- | ------------ | ------------- | -------- |
| 1 | AlignmentTypeSeeder | 1 | None | Adds basic alignment types |
| 2 | StationSeeder | 3 | None | Creates station data |
| 3 | PlatformSeeder | 4 | StationSeeder | Creates platform data |
| 4 | PredictionSeeder | 5 | PlatformSeeder | Creates train prediction data |
| 5 | DeviceSeeder | 5 | PlatformSeeder | Creates device data |
| 6 | DeviceSettingSeeder | 6 | DeviceSeeder | Creates device settings |
| 7 | FullScreenMessageSeeder | 40 | DeviceSeeder | Creates full screen messages |
| 8 | ScrollingScreenMessageSeeder | 41 | DeviceSeeder | Creates scrolling screen messages |
| 9 | BitmapScreenMessageSeeder | 42 | DeviceSeeder | Creates bitmap screen messages |
| 10 | PeriodicMessageSeeder | 43 | DeviceSeeder | Creates periodic messages |

### 3.3 Dependency Graph

Dependency relationships and execution order between seeders:

```
AlignmentTypeSeeder (1) 
          │
          ▼
StationSeeder (3)
          │
          ▼
PlatformSeeder (4)
      ┌───┴────┐
      │        │
      ▼        ▼
PredictionSeeder (5)  DeviceSeeder (5)
                           │
                           ▼
                  DeviceSettingSeeder (6)
                           │
                           ▼
                 FullScreenMessageSeeder (40)
                           │
                           ▼
               ScrollingScreenMessageSeeder (41)
                           │
                           ▼
                BitmapScreenMessageSeeder (42)
                           │
                           ▼
                 PeriodicMessageSeeder (43)
```

## 4. Data Relationships and Structures

### 4.1 Data Hierarchy

Seed data created in DeviceApi has the following hierarchical structure:

```
Station 1─┐
          ├── Platform 1───┬─── Prediction 1, 2, 3 (Different trains)
          │                ├─── Device 1 ────┬─── Device Settings 1
          │                │                 ├─── Full Screen Message 1
          │                │                 ├─── Scrolling Screen Message 1
          │                │                 ├─── Bitmap Screen Message 1
          │                │                 └─── Periodic Message 1
          │                └─── Device 2 ────┬─── Device Settings 2
          │                                  └─── ...
          │
          └── Platform 2───┬─── Prediction 4, 5 (Different trains)
                           ├─── Device 3 ────┬─── ...
                           └─── Device 4 ────┴─── ...
```

### 4.2 Sample Data Structure

A typical station and its platform devices are configured as follows:

- **Station A (Central)**
  - **Platform 1 (Main Entrance)**
    - *Device 1:* LED Display (192.168.1.101:8001)
    - *Device 2:* Information Screen (192.168.1.102:8001)
  - **Platform 2 (Side Entrance)**
    - *Device 3:* LED Display (192.168.1.103:8001)
    - *Device 4:* Information Screen (192.168.1.104:8001)

- **Station B (East)**
  - **Platform 3 (Main)**
    - *Device 5:* LED Display (192.168.1.105:8001)
    - *Device 6:* Information Screen (192.168.1.106:8001)

This structure reflects a real metro station scenario and provides appropriate data for testing, development, and demo purposes.

## 5. Seed Process Flow

### 5.1 General Process Flow

The seeding process is carried out with the following steps:

1. The application starts and checks the `DatabaseSettings:ResetDatabaseOnStartup` value
2. If the value is `true`, the database is deleted and recreated
3. The `DatabaseSeeder.SeedAsync()` method is called
4. DatabaseSeeder finds all classes of type `ISeeder` using reflection
5. Seeders are sorted by `Order` value
6. Each seeder is executed in sequence
7. All seeding operations are logged in detail

### 5.2 Technical Implementation

The seeding process is realized with the following code in the `DatabaseSeeder` class:

```csharp
public async Task SeedAsync()
{
    using var scope = _serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Find all classes implementing the ISeeder interface
    var seeders = GetSeeders();

    foreach (var seeder in seeders.OrderBy(s => s.Order))
    {
        try
        {
            // Logging operations
            _logger.LogInformation("Seeding: Starting {SeederName}...", seeder.GetType().Name);
            await _logService.LogInfoAsync($"Seeding: Starting {seeder.GetType().Name}...",
                "DatabaseSeeder.SeedAsync",
                new { SeederName = seeder.GetType().Name });
            
            // Perform the seed operation
            await seeder.SeedAsync(context);
            
            // Logging operations
            _logger.LogInformation("Seeding: {SeederName} completed successfully.", seeder.GetType().Name);
            await _logService.LogInfoAsync($"Seeding: {seeder.GetType().Name} completed successfully.",
                "DatabaseSeeder.SeedAsync",
                new { SeederName = seeder.GetType().Name });
        }
        catch (Exception ex)
        {
            // Error logging
            _logger.LogError(ex, "Seeding: Error occurred during {SeederName}: {ErrorMessage}", 
                seeder.GetType().Name, ex.Message);
            await _logService.LogErrorAsync($"Seeding: Error occurred during {seeder.GetType().Name}",
                "DatabaseSeeder.SeedAsync", ex);
            throw;
        }
    }
}
```

## 6. Performance Optimization

### 6.1 Bulk Data Insertion

For performance improvement in large data sets:

- **SQL Batch Operations**: Bulk data insertion with SQL commands is commonly used
- **StringBuilder Usage**: Multiple SQL statements are created at once
- **IDENTITY_INSERT Usage**: Ensures preservation of ID values and correct establishment of relationships

### 6.2 Context Management

For efficient use of Entity Framework:

- **Context Cleaning**: ChangeTracker is cleaned after each seeder runs
- **Reduce Eager Loading**: EF Core's tracking mechanism is disabled for bulk insertion
- **Parallel Execution**: Independent seeders can potentially run in parallel

## 7. Troubleshooting

### 7.1 Common Errors and Solutions

| Error | Possible Cause | Solution |
| ---- | ----------- | ----- |
| The INSERT statement conflicted with the FOREIGN KEY constraint | Related tables are populated in the wrong order | Check the Order values of seeders |
| Cannot insert explicit value for identity column | IDENTITY_INSERT is not enabled | Add SET IDENTITY_INSERT [Table] ON; to the SQL query |
| Violation of PRIMARY KEY constraint | Attempting to add a record with an ID value that already exists | Ensure ID values are unique |
| Timeout expired | Long-running operation on a large data set | Reduce the data set size or adjust the batch size |

### 7.2 Log Examination

When debugging seeding operations:

- Examine application logs to find which seeder encountered an error
- Access detailed information through structured logs using LogLibrary
- Monitor database operations with SQL profiler

### 7.3 Forcing Seed Execution

To force the seeding process during development:

```json
// appsettings.Development.json
{
  "DatabaseSettings": {
    "ResetDatabaseOnStartup": true
  }
}
```

## 8. Adding New Seeders

### 8.1 Step-by-Step Seeder Creation

Steps to follow when adding a new seeder:

1. Add your new seeder's order value to the `SeederOrder` enum
2. Create a new class implementing the `ISeeder` interface
3. Set the `Order` property to an appropriate value based on dependencies
4. In the `SeedAsync` method:
   - Check if data already exists in the table
   - Check dependencies (data in other tables)
   - Add the data
   - Clean the context

### 8.2 Template Code Example

Basic template for a new seeder:

```csharp
using Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Data.Seeding
{
    public class NewDataSeeder : ISeeder
    {
        public int Order => (int)SeederOrder.NewData; // Define in the Enum
        
        public async Task SeedAsync(AppDbContext context)
        {
            // Check for existing data
            if (await context.NewDataTable.AnyAsync())
            {
                return; // Skip if data already exists
            }
            
            // Check dependencies
            var devices = await context.Devices.ToListAsync();
            if (!devices.Any())
            {
                throw new InvalidOperationException("NewData cannot be added before Devices are created!");
            }
            
            // Create SQL command
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SET IDENTITY_INSERT [NewDataTable] ON;");
            
            // Add data
            foreach (var device in devices)
            {
                queryBuilder.AppendLine(
                    $"INSERT INTO [NewDataTable] ([Id], [DeviceId], [Name], [Value]) VALUES " +
                    $"({devices.IndexOf(device) + 1}, {device.Id}, 'Sample {device.Name}', 42.5);"
                );
            }
            
            queryBuilder.AppendLine("SET IDENTITY_INSERT [NewDataTable] OFF;");
            
            // Execute SQL command
            await context.Database.ExecuteSqlRawAsync(queryBuilder.ToString());
            
            // Clean context cache
            foreach (var entry in context.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }
        }
    }
}
```

## 9. Advanced Topics

### 9.1 Random Test Data

For creating realistic test data:

- **Bogus** library: Can be used to generate fake data
- **Random values**: Use random generators for variable data
- **Realistic data sets**: Define data ranges that reflect real scenarios

### 9.2 Environment-Based Seed Data

For creating seed data specific to different environments (Development, Test, Production):

```csharp
public async Task SeedAsync(AppDbContext context)
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    
    switch (environment)
    {
        case "Development":
            await SeedDevelopmentDataAsync(context);
            break;
        case "Staging":
            await SeedStagingDataAsync(context);
            break;
        case "Production":
            // Don't perform seeding in prod or add minimal data
            await SeedMinimalDataAsync(context);
            break;
    }
}
```

### 9.3 Data Set Distribution

Managing seed data in large projects:

- **JSON/XML data files**: Store seed data outside of code
- **Container images**: Distribution with seed data included
- **CI/CD integration**: Generate seed data in automated build processes

## 10. Best Practices

Recommended best practices for seeding operations:

1. **Minimalist Approach**: Only add data needed for testing and that is meaningful
2. **Idempotent Design**: Ability to run the same seeder multiple times
3. **Performance Focus**: Use bulk insertion methods for large data sets
4. **Order Management**: Carefully design the dependency graph and assign appropriate Order values
5. **Detailed Logging**: Include comprehensive debugging information in seeders
6. **Secure Data**: Test data should not contain real credentials or sensitive personal information
7. **Test Coverage**: Seed data should allow testing all important areas of application flow

---

## Resources

- [Entity Framework Core Data Seeding Documentation](https://docs.microsoft.com/en-us/ef/core/modeling/data-seeding)
- [SQL Server Bulk Insert Performance Optimization](https://docs.microsoft.com/en-us/sql/t-sql/statements/bulk-insert-transact-sql)
- [Bogus - Fake Data Generation Library](https://github.com/bchavez/Bogus)

---

[◀ Architecture](06-Architecture.md) | [Home](README.md) | [Next: Configuration ▶](08-Configuration.md) 