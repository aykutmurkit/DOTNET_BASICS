# Seeding Process

**Version:** 1.0.0  
**Author:** R&D Engineer Aykut Mürkit  
**Company:** İSBAK 2025

---

## What is Seeding?

Seeding is the process of loading initial data into the database. This process ensures that the necessary basic data is automatically created during the initial setup of the application or in the test environment.

## Seeding Classes

### 1. PlatformSeeder

```csharp
public class PlatformSeeder : ISeeder
{
    public int Order => 1;
    
    public async Task SeedAsync(ApplicationDbContext context)
    {
        // Adds platform data
    }
}
```

### 2. StationSeeder

```csharp
public class StationSeeder : ISeeder
{
    public int Order => 2;
    
    public async Task SeedAsync(ApplicationDbContext context)
    {
        // Adds station data
    }
}
```

### 3. DeviceSeeder

```csharp
public class DeviceSeeder : ISeeder
{
    public int Order => 7;
    
    public async Task SeedAsync(ApplicationDbContext context)
    {
        // Adds device data
    }
}
```

### 4. FullScreenMessageSeeder

```csharp
public class FullScreenMessageSeeder : ISeeder
{
    public int Order => 8;
    
    public async Task SeedAsync(ApplicationDbContext context)
    {
        // Adds full screen message data
    }
}
```

### 5. ScrollingScreenMessageSeeder

```csharp
public class ScrollingScreenMessageSeeder : ISeeder
{
    public int Order => 9;
    
    public async Task SeedAsync(ApplicationDbContext context)
    {
        // Adds scrolling screen message data
    }
}
```

### 6. BitmapScreenMessageSeeder

```csharp
public class BitmapScreenMessageSeeder : ISeeder
{
    public int Order => 10;
    
    public async Task SeedAsync(ApplicationDbContext context)
    {
        // Adds bitmap screen message data
    }
}
```

### 7. PeriodicMessageSeeder

```csharp
public class PeriodicMessageSeeder : ISeeder
{
    public int Order => 11;
    
    public async Task SeedAsync(ApplicationDbContext context)
    {
        // Adds periodic message data
    }
}
```

## Seeding Order

The seeding process is executed in order based on the `Order` property:

1. PlatformSeeder (Order: 1)
2. StationSeeder (Order: 2)
3. DeviceSeeder (Order: 7)
4. FullScreenMessageSeeder (Order: 8)
5. ScrollingScreenMessageSeeder (Order: 9)
6. BitmapScreenMessageSeeder (Order: 10)
7. PeriodicMessageSeeder (Order: 11)

## Running Seeding

The seeding process runs automatically when the application starts. It can also be run manually using the following command:

```bash
dotnet run --seed
```

## Special Seeding Scenarios

### 1. Seeding for Test Environment

To create special seeding data for the test environment:

```csharp
public class TestEnvironmentSeeder : ISeeder
{
    public int Order => 100; // Runs last
    
    public async Task SeedAsync(ApplicationDbContext context)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test")
        {
            // Add test data
        }
    }
}
```

### 2. Seeding for Development Environment

To create special seeding data for the development environment:

```csharp
public class DevelopmentEnvironmentSeeder : ISeeder
{
    public int Order => 100; // Runs last
    
    public async Task SeedAsync(ApplicationDbContext context)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            // Add development data
        }
    }
}
```

## Updating Seeding Data

To update seeding data:

1. Find the relevant seeder class
2. Update the `SeedAsync` method
3. Restart the application

## Important Notes

1. The seeding process does not delete existing data in the database
2. Each seeder class adds its data independently
3. The seeding order is determined based on data dependencies
4. The seeding process is executed within a transaction

## Error Handling

Possible errors during seeding:

1. **Data Conflict**: When trying to add the same data again
2. **Dependency Error**: When dependent data has not been added yet
3. **Database Error**: When the database connection is lost

Appropriate error handling mechanisms have been added for these errors.

---

[◀ Configuration](06-Configuration.md) | [Next: Versioning ▶](08-Versioning.md) 