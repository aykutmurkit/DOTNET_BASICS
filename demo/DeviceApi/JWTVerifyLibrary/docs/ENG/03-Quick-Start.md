# Quick Start

**Version:** 1.0.0  
**Author:** R&D Engineer Aykut Mürkit  
**Company:** ISBAK 2025

---

This section is designed to help you quickly integrate JWTVerifyLibrary into your project and see basic usage examples.

## 1. Add the Library

First, add JWTVerifyLibrary to your project. For detailed installation information, see the [Installation](02-Installation.md) section.

```powershell
Install-Package JWTVerifyLibrary
```

## 2. Add Configuration

Add JWT settings to your appsettings.json file:

```json
{
  "JwtSettings": {
    "Secret": "YourSecretKey12345678901234567890",
    "Issuer": "YourApplicationName",
    "Audience": "YourTargetAudience",
    "AccessTokenExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 7
  }
}
```

## 3. Register Services in Program.cs

In your Program.cs or Startup.cs file, add the following:

```csharp
using JWTVerifyLibrary.Extensions;

// ...

// Add JWT services and validation
builder.Services.AddJwtVerification(builder.Configuration);

// ...

// Add JWT middleware to pipeline (before UseAuthentication)
app.UseJwtVerification();
app.UseAuthentication();
app.UseAuthorization();
```

## 4. Use [Authorize] Attribute in Controllers

Use the [Authorize] attribute to protect controllers or actions that require JWT validation:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SampleController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult PublicEndpoint()
        {
            return Ok("This endpoint is public!");
        }

        [HttpGet("protected")]
        [Authorize] // Requires JWT validation
        public IActionResult ProtectedEndpoint()
        {
            return Ok("Only authenticated users can see this!");
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")] // For users with specific role
        public IActionResult AdminEndpoint()
        {
            return Ok("Only users with Admin role can see this!");
        }
    }
}
```

## 5. Making Requests with JWT

Send your JWT token in HTTP requests with the Authorization header:

```http
GET /api/sample/protected HTTP/1.1
Host: yourapi.com
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Curl Example

```bash
curl -X GET "https://yourapi.com/api/sample/protected" -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### Using Swagger UI

To use JWT authentication through Swagger UI:

1. Open your Swagger UI interface
2. Click the "Authorize" button in the top right corner
3. In the popup window, enter your JWT token in the "Bearer" field
4. Click the "Authorize" button and close the popup
5. You can now test protected endpoints

## 6. Swagger Configuration

To integrate JWT authentication with Swagger:

```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "YourApi", Version = "v1" });
    
    // JWT authentication configuration for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
```

## 7. Complete Example

Below is a complete MinimalAPI example showing basic setup and usage of JWTVerifyLibrary:

```csharp
using JWTVerifyLibrary.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT Test API", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Add JWTVerifyLibrary
builder.Services.AddJwtVerification(builder.Configuration);

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// JWT verification middleware
app.UseJwtVerification();

app.UseAuthentication();
app.UseAuthorization();

// Test endpoints
app.MapGet("/public", () => "This endpoint is public!")
   .WithName("GetPublic");

app.MapGet("/protected", [Authorize] (HttpContext context) =>
{
    var userId = context.User.FindFirst("nameid")?.Value;
    var username = context.User.FindFirst("unique_name")?.Value;
    
    return $"Welcome, {username}! Your user ID: {userId}";
})
.WithName("GetProtected");

app.MapGet("/admin", [Authorize(Roles = "Admin")] () => "This endpoint is only for users with Admin role!")
   .WithName("GetAdmin");

app.Run();
```

## Next Steps

Now that you've learned the basic setup and usage, you can explore:

- [Basic Usage](04-Basic-Usage.md) - More usage examples
- [Advanced Features](05-Advanced-Features.md) - Advanced features of the library
- [Architecture](06-Architecture.md) - Internal structure of the library
- [API Reference](07-API-Reference.md) - All classes and methods

---

[◀ Installation](02-Installation.md) | [Home](README.md) | [Next: Basic Usage ▶](04-Basic-Usage.md) 