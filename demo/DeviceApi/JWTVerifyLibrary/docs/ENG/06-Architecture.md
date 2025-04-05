# Architecture

**Version:** 1.0.0  
**Author:** R&D Engineer Aykut Mürkit  
**Company:** ISBAK 2025

---

This section explains the internal architecture, components, and operational principles of JWTVerifyLibrary.

## General Architecture

JWTVerifyLibrary is designed with a modular structure and consists of the following core components:

![JWTVerifyLibrary Architecture](../images/architecture.png)

1. **Configuration Layer**: Manages JWT settings
2. **Service Layer**: Performs core JWT validation operations
3. **Middleware Layer**: Intercepts HTTP requests and validates JWT tokens
4. **Extension Layer**: Provides extension methods for easy integration

## Components and Responsibilities

### 1. Models

This folder contains definitions for the library's data models.

**JwtSettings.cs**

Model class that holds JWT configuration settings:

```csharp
public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationInMinutes { get; set; }
    public int RefreshTokenExpirationInDays { get; set; }
}
```

### 2. Services

Provides core services for JWT operations.

**JwtService.cs**

Contains JWT token validation and processing logic:

```csharp
public interface IJwtService
{
    bool ValidateToken(string token, out JwtSecurityToken? validatedToken);
    IEnumerable<Claim> GetTokenClaims(string token);
}

public class JwtService : IJwtService
{
    // Token validation methods here
}
```

This service:
- Checks the token signature
- Detects expired tokens
- Extracts claims information from a valid token

### 3. Middleware

Intercepts HTTP requests and performs JWT validation.

**JwtMiddleware.cs**

```csharp
public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
    {
        // Extract JWT token from HTTP requests
        // Validate token
        // Create user identity if validation is successful
        // Proceed to next middleware
    }
}
```

This middleware:
- Extracts the Bearer token from HTTP request
- Validates the token
- Creates user identity if there is a valid token
- Adds identity information to HttpContext.User

### 4. Extensions

Provides extension methods for easy integration.

**ConfigurationExtensions.cs**

Extension methods for loading JWT configuration file:

```csharp
public static class ConfigurationExtensions
{
    public static IConfiguration LoadJwtVerifyLibrarySettings(this IConfiguration configuration)
    {
        // Loads configuration file
    }
}
```

**ServiceCollectionExtensions.cs**

DI extension methods for adding JWT services to the application:

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtVerification(this IServiceCollection services, 
                                                      IConfiguration configuration)
    {
        // Registers JWT services to DI container
        // Configures JWT authentication
    }
}
```

**ApplicationBuilderExtensions.cs**

Extension methods for adding JWT middleware to HTTP pipeline:

```csharp
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseJwtVerification(this IApplicationBuilder app)
    {
        // Adds JWT middleware
    }
}
```

## Process Flow

The workflow of JWTVerifyLibrary is as follows:

1. **Configuration**:
   - JwtSettings configuration is loaded
   - JWT validation parameters are created

2. **Service Registration**:
   - JWT services are registered to the DI container
   - AspNet Core authentication is configured

3. **HTTP Request Processing**:
   - Request is intercepted by JwtMiddleware
   - JWT token is extracted from Authorization header
   - Token is validated through JwtService
   - If a valid token is found, user identity is created
   - Request is passed to the next middleware for processing

## Data Flow Diagram

```
┌───────────┐     ┌───────────────┐     ┌────────────────┐
│           │     │               │     │                │
│  HTTP     │────▶│ JwtMiddleware │────▶│  JwtService    │
│  Request  │     │               │     │                │
│           │     └───────────────┘     └────────────────┘
└───────────┘              │                     │
                           │                     │
                           ▼                     │
                ┌────────────────────┐           │
                │                    │           │
                │ User Identity      │◀──────────┘
                │ Creation           │
                │                    │
                └────────────────────┘
                           │
                           │
                           ▼
                ┌────────────────────┐
                │                    │
                │ Next Middleware    │
                │ (Authorization)    │
                │                    │
                └────────────────────┘
```

## Dependencies

JWTVerifyLibrary uses the following core dependencies:

1. **Microsoft.AspNetCore.Authentication.JwtBearer**: Base infrastructure for JWT token validation
2. **Microsoft.Extensions.Configuration.Abstractions**: For configuration management
3. **Microsoft.Extensions.DependencyInjection.Abstractions**: For Dependency Injection
4. **Microsoft.Extensions.Options.ConfigurationExtensions**: For configuration binding

## Extensibility

JWTVerifyLibrary can be extended in the following ways:

1. **Custom Claim Transformers**: You can add transformers for custom claim types
2. **Event Handlers**: You can add custom handlers to various stages of the token validation process
3. **Custom Validation Rules**: You can add extra rules beyond standard validation

### Example Extension: Adding Custom Claims

```csharp
services.AddJwtVerification(Configuration)
    .AddClaimTransformation(transformer =>
    {
        transformer.OnTokenValidated = context =>
        {
            // Add additional claims to claims from token
            var claims = context.Principal.Claims.ToList();
            
            // For example, get additional information from user identity
            var userId = context.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Get additional information from database and add new claims
            claims.Add(new Claim("custom_claim", "custom_value"));
            
            return Task.CompletedTask;
        };
        
        return transformer;
    });
```

## Best Practices

Consider the following best practices when using JWTVerifyLibrary:

1. **Correct Pipeline Order**: Add JWT middleware before UseAuthentication() and UseAuthorization()
2. **Error Handling**: Handle JWT errors appropriately
3. **HTTPS**: Always use JWT over HTTPS
4. **Strong Typing**: Use strong types for JWT configuration
5. **Secret Management**: Store secret key securely in production environments

---

[◀ Advanced Features](05-Advanced-Features.md) | [Home](README.md) | [Next: API Reference ▶](07-API-Reference.md) 