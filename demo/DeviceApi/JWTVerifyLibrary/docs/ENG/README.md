# JWTVerifyLibrary

## Overview

JWTVerifyLibrary is a powerful, lightweight, and easy-to-integrate library designed to simplify JWT (JSON Web Token) validation in ASP.NET Core applications. The library is built with a focus on security and performance and can be easily integrated into various projects.

**Version:** 1.0.0  
**Author:** R&D Engineer Aykut Mürkit  
**Company:** ISBAK 2025

---

## Table of Contents

- [Introduction](01-Introduction.md)
- [Installation](02-Installation.md)
- [Quick Start](03-Quick-Start.md)
- [Basic Usage](04-Basic-Usage.md)
- [Advanced Features](05-Advanced-Features.md)
- [Architecture](06-Architecture.md)
- [API Reference](07-API-Reference.md)
- [Versioning & Roadmap](08-Versioning.md)
- [Best Practices](09-Best-Practices.md)
- [Troubleshooting](10-Troubleshooting.md)
- [Contributing](11-Contributing.md)
- [License](12-License.md)

---

## Features

- ✅ JWT token validation and security checks
- ✅ ASP.NET Core middleware integration
- ✅ Runs on .NET 8.0
- ✅ External configuration support
- ✅ Extension methods for easy integration
- ✅ Claims extraction
- ✅ User identity assignment
- ✅ Default security settings
- ✅ Full test coverage
- ✅ Comprehensive documentation

---

## Quick Start

```csharp
// Add JWT verification to your Program.cs
using JWTVerifyLibrary.Extensions;

// Register services
builder.Services.AddJwtVerification(builder.Configuration);

// Register middleware
app.UseJwtVerification();

// Now you can use the [Authorize] attribute!
```

For more information, check out the [Quick Start](03-Quick-Start.md) guide.

---

## Support

If you have any questions or feedback, please open an issue on GitHub or submit a pull request.

---

## License

This library is licensed under the MIT License. See the [LICENSE](../../LICENSE) file for details.

---

© 2025 ISBAK. All rights reserved. 