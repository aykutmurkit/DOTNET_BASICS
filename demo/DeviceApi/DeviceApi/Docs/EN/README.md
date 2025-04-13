# DeviceApi

## Overview

DeviceApi is a comprehensive REST API designed to manage IoT devices and their data. Built with ASP.NET Core, this API provides a secure, scalable, and flexible solution for device registration, management, and data collection.

**Version:** 1.0.0  
**Author:** R&D Engineer Aykut Mürkit  
**Company:** ISBAK 2025

---

## Table of Contents

- [Introduction](01-Introduction.md)
- [Installation](02-Installation.md)
- [Quick Start](03-Quick-Start.md)
- [API Endpoints](04-API-Endpoints.md)
- [Authentication](05-Authentication.md)
- [Data Models](06-Data-Models.md)
- [Architecture](06-Architecture.md)
- [Database Seeding Process](07-Seeding-Process.md)
- [Configuration](08-Configuration.md)
- [Error Handling](09-Error-Handling.md)
- [Performance Optimization](10-Performance-Optimization.md)
- [Security](11-Security.md)
- [Deployment](12-Deployment.md)
- [Monitoring](13-Monitoring.md)
- [Troubleshooting](14-Troubleshooting.md)
- [RecurringDays NULL Value Solution](10-RecurringDays-NULL-Value-Solution.md)
- [Contributing](15-Contributing.md)
- [License](16-License.md)

---

## Features

- ✅ RESTful API design with proper HTTP method semantics
- ✅ JWT authentication for secure API access
- ✅ Comprehensive device management (registration, updates, status)
- ✅ Real-time device data collection and processing
- ✅ Multi-tenant architecture for managing different device groups
- ✅ Role-based access control for administrative operations
- ✅ Pagination, filtering, and sorting for all collection endpoints
- ✅ Swagger/OpenAPI documentation
- ✅ Logging and monitoring capabilities
- ✅ Containerization support with Docker
- ✅ Performance optimized for high-volume data handling

---

## Quick Start

To get started with the DeviceApi:

```csharp
// Clone the repository
git clone https://github.com/isbak/DeviceApi.git

// Navigate to the project directory
cd DeviceApi

// Build and run the application
dotnet build
dotnet run
```

Navigate to `https://localhost:5001/swagger` to explore the API endpoints using Swagger UI.

For more information, check out the [Quick Start](03-Quick-Start.md) guide.

---

## Support

If you have any questions or feedback, please open an issue on GitHub or submit a pull request.

---

## License

This API is licensed under the MIT License. See the [LICENSE](../../LICENSE) file for details.

---

© 2025 ISBAK. All rights reserved. 