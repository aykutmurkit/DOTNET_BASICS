# JWTVerifyLibrary Documentation

## Overview

JWTVerifyLibrary is a custom .NET library designed to simplify JWT (JSON Web Token) verification in ASP.NET Core applications. This library provides an easy-to-use solution for validating JWT tokens, extracting claims, and securing API endpoints with minimal configuration.

## Features

- Simple, one-line setup for JWT verification in any ASP.NET Core application
- Token validation with industry-standard security practices
- Claims extraction and user identity assignment
- Middleware that integrates with the ASP.NET Core authentication pipeline
- Support for configuration via appsettings.json

## Installation

1. Add a reference to the JWTVerifyLibrary project in your solution:
   ```xml
   <ItemGroup>
     <ProjectReference Include="path\to\JWTVerifyLibrary\JWTVerifyLibrary.csproj" />
   </ItemGroup>
   ```

2. Ensure your application has the JWT settings in appsettings.json:
   ```json
   {
     "JwtSettings": {
       "Secret": "YourSecretKeyHere",
       "Issuer": "YourIssuer",
       "Audience": "YourAudience",
       "AccessTokenExpirationInMinutes": 60,
       "RefreshTokenExpirationInDays": 7
     }
   }
   ```

## Usage

### Basic Setup

To integrate JWTVerifyLibrary in your ASP.NET Core application, follow these simple steps:

1. Add the required using statement at the top of your Program.cs or Startup.cs file:
   ```csharp
   using JWTVerifyLibrary.Extensions;
   ```

2. Register the JWT verification services in the `ConfigureServices` method or in the builder configuration:
   ```csharp
   // In Program.cs (minimal API)
   builder.Services.AddJwtVerification(builder.Configuration);

   // OR in Startup.cs
   services.AddJwtVerification(Configuration);
   ```

3. Add the JWT verification middleware to the application pipeline:
   ```csharp
   // In Program.cs (minimal API)
   app.UseJwtVerification();

   // Make sure to add it in the correct order in the middleware pipeline
   app.UseHttpsRedirection();
   app.UseJwtVerification();
   app.UseAuthentication();
   app.UseAuthorization();
   ```

### Advanced Configuration

You can customize the JWT verification process by working directly with the provided services:

```csharp
// Inject the JWT service into your controllers or services
private readonly IJwtService _jwtService;

public YourController(IJwtService jwtService)
{
    _jwtService = jwtService;
}

// Validate a token manually
public IActionResult ValidateToken(string token)
{
    bool isValid = _jwtService.ValidateToken(token, out var validatedToken);
    
    if (isValid)
    {
        // Token is valid
        var claims = _jwtService.GetTokenClaims(token);
        return Ok(claims);
    }
    
    return Unauthorized();
}
```

### Protecting Controllers and Actions

After setting up the library, you can use standard ASP.NET Core attributes to protect your API endpoints:

```csharp
[Authorize]
public class SecureController : ControllerBase
{
    [HttpGet]
    public IActionResult GetSecureData()
    {
        return Ok("This is secure data!");
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult AdminOnly()
    {
        return Ok("This is for admins only!");
    }
}
```

## Configuration Options

The library uses the following configuration options from appsettings.json:

| Setting | Description |
|---------|-------------|
| Secret | The secret key used to sign and verify JWT tokens |
| Issuer | The issuer of the JWT token (usually your authentication server) |
| Audience | The intended recipient of the JWT token (your API) |
| AccessTokenExpirationInMinutes | The expiration time for access tokens in minutes |
| RefreshTokenExpirationInDays | The expiration time for refresh tokens in days |

## Security Considerations

JWTVerifyLibrary implements several security best practices:

1. **Token Signature Validation**: Ensures the token hasn't been tampered with
2. **Issuer and Audience Validation**: Confirms the token was issued by a trusted source and intended for your application
3. **Lifetime Validation**: Rejects expired tokens automatically
4. **Zero Clock Skew**: No tolerance is applied to token expiration checks

## Troubleshooting

If you encounter issues with token validation:

1. Check that your JWT secret in appsettings.json matches the one used to generate tokens
2. Verify that the Issuer and Audience values match between token generation and validation
3. Ensure tokens haven't expired
4. Check that the token uses the HMAC SHA256 algorithm

## Implementation Details

The library consists of:

1. **JwtService**: Core service for token validation and claims extraction
2. **JwtMiddleware**: Middleware that intercepts requests, validates tokens, and assigns user identity
3. **Extension Methods**: For easy integration with ASP.NET Core's dependency injection and middleware pipeline

## Example Implementation

Here's a complete example of how to set up the JWTVerifyLibrary in an ASP.NET Core application:

```csharp
// Program.cs
using JWTVerifyLibrary.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add JWT verification
builder.Services.AddJwtVerification(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use JWT verification
app.UseJwtVerification();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
``` 