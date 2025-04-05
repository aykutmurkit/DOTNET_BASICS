# Installation

**Version:** 1.0.0  
**Author:** R&D Engineer Aykut Mürkit  
**Company:** ISBAK 2025

---

This section guides you through the process of setting up DeviceApi for both development and production environments.

## Prerequisites

Before installing DeviceApi, ensure that your system meets the following requirements:

### Development Environment

- **.NET SDK 8.0** or newer
- **Visual Studio 2022** (any edition) or **Visual Studio Code** with C# extensions
- **SQL Server 2019** or newer (Express edition is sufficient for development)
- **Git** for source control
- **Postman** or similar tool for API testing (optional but recommended)

### Production Environment

- **Windows Server 2019/2022** or **Linux** with Docker support
- **SQL Server 2019** or newer
- **Redis** (for distributed caching)
- **HTTPS certificate** from a trusted certificate authority
- **Docker and Docker Compose** (if deploying with containers)

## Installation Options

DeviceApi can be installed in several ways, depending on your requirements:

### Option 1: Clone from GitHub (Development)

This is the recommended approach for development environments:

```bash
# Clone the repository
git clone https://github.com/isbak/DeviceApi.git

# Navigate to the project directory
cd DeviceApi

# Restore dependencies
dotnet restore

# Build the solution
dotnet build
```

### Option 2: Docker Deployment (Development or Production)

For a quick setup using Docker:

```bash
# Clone the repository
git clone https://github.com/isbak/DeviceApi.git

# Navigate to the project directory
cd DeviceApi

# Build and start the Docker containers
docker-compose up -d
```

This will start DeviceApi along with SQL Server and Redis instances.

### Option 3: Manual Deployment to IIS (Production)

For traditional Windows hosting:

1. Build the application:
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. Create a new IIS site:
   - Open IIS Manager
   - Create a new application pool (.NET CLR Version: "No Managed Code")
   - Create a new website pointing to the published folder
   - Configure binding with your HTTPS certificate

3. Configure necessary permissions for the application pool identity

## Database Setup

DeviceApi uses Entity Framework Core for database operations. You have two options for setting up the database:

### Option 1: Using Migrations (Recommended)

```bash
# Navigate to the project directory
cd DeviceApi

# Apply migrations to create or update the database
dotnet ef database update
```

### Option 2: Manual Script Execution

If you prefer to manually control the database creation:

1. Generate the SQL script:
   ```bash
   dotnet ef migrations script -o create_database.sql
   ```

2. Execute the script against your SQL Server instance using SQL Server Management Studio or other SQL tools

## Configuration

DeviceApi uses the ASP.NET Core configuration system with settings stored in various sources:

### appsettings.json

The main configuration file is `appsettings.json`. Here's an example with the most important settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DeviceApi;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "JwtSettings": {
    "Secret": "your-super-secret-key-with-at-least-32-characters",
    "Issuer": "deviceapi",
    "Audience": "deviceapi-client",
    "AccessTokenExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 7
  },
  "RedisSettings": {
    "ConnectionString": "localhost:6379"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### Environment Variables

For production environments, it's recommended to use environment variables to override sensitive settings:

```bash
# Windows
setx DEVICEAPI_ConnectionStrings__DefaultConnection "Server=prod-db;Database=DeviceApi;User Id=app_user;Password=secure_password;"
setx DEVICEAPI_JwtSettings__Secret "production-secret-key-that-is-very-secure"

# Linux/macOS
export DEVICEAPI_ConnectionStrings__DefaultConnection="Server=prod-db;Database=DeviceApi;User Id=app_user;Password=secure_password;"
export DEVICEAPI_JwtSettings__Secret="production-secret-key-that-is-very-secure"
```

### User Secrets (Development)

During development, use .NET User Secrets to store sensitive information:

```bash
# Navigate to the project directory
cd DeviceApi

# Initialize user secrets
dotnet user-secrets init

# Set secrets
dotnet user-secrets set "JwtSettings:Secret" "dev-secret-key-for-local-testing"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=DeviceApi;Trusted_Connection=True;"
```

## Verification

To verify that the installation was successful:

1. Start the application:
   ```bash
   dotnet run
   ```

2. Open a web browser and navigate to:
   - `https://localhost:5001/swagger` (for the Swagger UI)
   - `https://localhost:5001/health` (for the health check endpoint)

3. If you see the Swagger UI and the health check returns "Healthy", the installation is successful

## Troubleshooting

Common installation issues and their solutions:

### Database Connection Issues

- **Problem**: "Cannot connect to database" error
- **Solution**: Verify SQL Server is running and the connection string is correct

### Certificate Issues

- **Problem**: HTTPS certificate errors
- **Solution**: For development, you can use:
  ```bash
  dotnet dev-certs https --clean
  dotnet dev-certs https --trust
  ```

### Port Conflicts

- **Problem**: "Address already in use" error
- **Solution**: Change the port in `Properties/launchSettings.json` or stop the process using the conflicting port

### Docker Issues

- **Problem**: "Cannot connect to Docker daemon" error
- **Solution**: Ensure Docker service is running with `docker info`

## Next Steps

Now that you have DeviceApi installed, you can:

1. Follow the [Quick Start](03-Quick-Start.md) guide to make your first API calls
2. Explore the [API Endpoints](04-API-Endpoints.md) documentation
3. Learn about [Authentication](05-Authentication.md) to secure your API

---

[◀ Introduction](01-Introduction.md) | [Home](README.md) | [Next: Quick Start ▶](03-Quick-Start.md) 