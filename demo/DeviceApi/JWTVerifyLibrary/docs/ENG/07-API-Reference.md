# API Reference

**Version:** 1.0.0  
**Author:** R&D Engineer Aykut Mürkit  
**Company:** ISBAK 2025

---

This document details all public APIs of the JWTVerifyLibrary.

## JwtService

The `JwtService` class provides core functionalities for validating and processing JWT tokens.

### Interface

```csharp
public interface IJwtService
{
    bool ValidateToken(string token, out JwtSecurityToken? validatedToken);
    IEnumerable<Claim> GetTokenClaims(string token);
    ClaimsPrincipal GetClaimsPrincipal(string token);
}
```

### Methods

#### ValidateToken

```csharp
bool ValidateToken(string token, out JwtSecurityToken? validatedToken)
```

**Description**: Validates the authenticity of a JWT token.

**Parameters**:
- `token` (string): The JWT token to validate.
- `validatedToken` (out JwtSecurityToken?): If validation is successful, contains the validated token object.

**Return Value**:
- `bool`: `true` if token validation is successful, otherwise `false`.

**Examples**:

```csharp
// Validating a JWT token
string jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";

bool isValid = jwtService.ValidateToken(jwtToken, out var validatedToken);

if (isValid)
{
    Console.WriteLine("Token is valid!");
    Console.WriteLine($"Expiration: {validatedToken.ValidTo}");
}
else
{
    Console.WriteLine("Token is invalid or expired.");
}
```

**Exception Cases**:
- `ArgumentNullException`: When token is null or empty.
- `SecurityTokenException`: When token format is invalid or validation fails.

#### GetTokenClaims

```csharp
IEnumerable<Claim> GetTokenClaims(string token)
```

**Description**: Extracts all claims from a validated JWT token.

**Parameters**:
- `token` (string): The JWT token to extract claims from.

**Return Value**:
- `IEnumerable<Claim>`: List of claims extracted from the token.

**Examples**:

```csharp
// Getting claims from a JWT token
string jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";

IEnumerable<Claim> claims = jwtService.GetTokenClaims(jwtToken);

foreach (var claim in claims)
{
    Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
}
```

**Exception Cases**:
- `ArgumentNullException`: When token is null or empty.
- `SecurityTokenException`: When token format is invalid or validation fails.

#### GetClaimsPrincipal

```csharp
ClaimsPrincipal GetClaimsPrincipal(string token)
```

**Description**: Creates a ClaimsPrincipal object from a validated JWT token.

**Parameters**:
- `token` (string): The JWT token to create ClaimsPrincipal from.

**Return Value**:
- `ClaimsPrincipal`: Identity created based on the token.

**Examples**:

```csharp
// Creating ClaimsPrincipal from JWT token
string jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";

ClaimsPrincipal principal = jwtService.GetClaimsPrincipal(jwtToken);

// Getting user ID
string? userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
```

**Exception Cases**:
- `ArgumentNullException`: When token is null or empty.
- `SecurityTokenException`: When token format is invalid or validation fails.

## JwtMiddleware

The `JwtMiddleware` class is an ASP.NET Core middleware component for intercepting HTTP requests and validating JWT tokens.

### Class Structure

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
        // Middleware processing logic
    }
}
```

### Methods

#### InvokeAsync

```csharp
public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
```

**Description**: The main method of the middleware that runs for each HTTP request.

**Parameters**:
- `context` (HttpContext): The context of the current HTTP request.
- `jwtService` (IJwtService): Service for JWT token operations.

**Process Flow**:
1. Extracts Bearer token from Authorization header.
2. Validates the token.
3. If validation is successful, creates user identity and assigns it to `HttpContext.User`.
4. Proceeds to the next middleware in the pipeline.

## ServiceCollectionExtensions

The `ServiceCollectionExtensions` class provides extension methods for adding JWT validation services to an ASP.NET Core application.

### Methods

#### AddJwtVerification

```csharp
public static IServiceCollection AddJwtVerification(
    this IServiceCollection services, 
    IConfiguration configuration)
```

**Description**: Adds JWT validation services and configuration to the application.

**Parameters**:
- `services` (IServiceCollection): The service collection.
- `configuration` (IConfiguration): The application configuration.

**Return Value**:
- `IServiceCollection`: The service collection, for method chaining.

**Examples**:

```csharp
// Service registration in Program.cs
builder.Services.AddJwtVerification(builder.Configuration);
```

#### AddJwtVerification (Advanced)

```csharp
public static IServiceCollection AddJwtVerification(
    this IServiceCollection services, 
    IConfiguration configuration,
    Action<JwtOptions> configureOptions)
```

**Description**: Adds JWT validation services to the application and provides custom configuration.

**Parameters**:
- `services` (IServiceCollection): The service collection.
- `configuration` (IConfiguration): The application configuration.
- `configureOptions` (Action<JwtOptions>): Callback to configure JWT options.

**Return Value**:
- `IServiceCollection`: The service collection, for method chaining.

**Examples**:

```csharp
// Advanced service registration in Program.cs
builder.Services.AddJwtVerification(builder.Configuration, options => 
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Custom parameters
    };
});
```

## ApplicationBuilderExtensions

The `ApplicationBuilderExtensions` class provides extension methods for adding JWT middleware to the ASP.NET Core application pipeline.

### Methods

#### UseJwtVerification

```csharp
public static IApplicationBuilder UseJwtVerification(this IApplicationBuilder app)
```

**Description**: Adds JWT validation middleware to the application pipeline.

**Parameters**:
- `app` (IApplicationBuilder): The application builder.

**Return Value**:
- `IApplicationBuilder`: The application builder, for method chaining.

**Examples**:

```csharp
// Middleware registration
app.UseJwtVerification();
```

**Note**: Add this middleware before `UseAuthentication()` and `UseAuthorization()`.

## JwtSettings

The `JwtSettings` class is a model class for holding JWT configuration settings.

### Properties

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

**Description**:

- `Secret`: The secret key used for JWT signing.
- `Issuer`: The entity that issues the token (usually the API or application name/URL).
- `Audience`: The recipient for whom the token is intended.
- `AccessTokenExpirationInMinutes`: The validity period of the access token in minutes.
- `RefreshTokenExpirationInDays`: The validity period of the refresh token in days.

## Event Handlers

JWTVerifyLibrary provides event handlers to customize various stages of the token validation process.

### TokenValidatedHandler

```csharp
public delegate Task TokenValidatedHandler(TokenValidatedContext context);
```

**Description**: Triggered after token is validated.

**Usage**:

```csharp
services.AddJwtVerification(Configuration)
    .AddEventHandler(events =>
    {
        events.OnTokenValidated = async context =>
        {
            // Processing after token is validated
            var userId = context.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Example: Check if user exists in database
            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
            if (!await userService.IsActiveUser(userId))
            {
                context.Fail("User is not active");
            }
        };
    });
```

### TokenValidationFailedHandler

```csharp
public delegate Task TokenValidationFailedHandler(TokenValidationFailedContext context);
```

**Description**: Triggered when token validation fails.

**Usage**:

```csharp
services.AddJwtVerification(Configuration)
    .AddEventHandler(events =>
    {
        events.OnTokenValidationFailed = async context =>
        {
            // Processing when token validation fails
            
            // Example: Log error details
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtMiddleware>>();
            logger.LogWarning("JWT validation failed: {Exception}", context.Exception.Message);
            
            // Send custom response
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Invalid token" }));
            
            // Don't run subsequent middleware
            context.Handled = true;
        };
    });
```

## Extensibility Interfaces

JWTVerifyLibrary provides various interfaces for customization and extension.

### IClaimsTransformer

```csharp
public interface IClaimsTransformer
{
    Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal);
}
```

**Description**: Used to transform claims extracted from JWT token.

**Usage**:

```csharp
// Custom claims transformer
public class CustomClaimsTransformer : IClaimsTransformer
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;
        
        if (identity == null)
            return principal;
            
        // Add additional role claim based on email claim
        var email = principal.FindFirstValue(ClaimTypes.Email);
        if (email != null && email.EndsWith("@admin.com"))
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
        }
        
        return principal;
    }
}

// Service registration
services.AddSingleton<IClaimsTransformer, CustomClaimsTransformer>();
services.AddJwtVerification(Configuration);
```

---

[◀ Architecture](06-Architecture.md) | [Home](README.md) | [Next: Versioning ▶](08-Versioning.md) 