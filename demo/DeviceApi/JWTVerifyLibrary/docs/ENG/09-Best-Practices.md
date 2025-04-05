# Best Practices

**Version:** 1.0.0  
**Author:** R&D Engineer Aykut Mürkit  
**Company:** ISBAK 2025

---

This section contains best practices and recommendations for using JWTVerifyLibrary in your projects most efficiently and securely.

## Security

### Token Signing Key (Secret Key)

- **Use a Strong Secret Key**: Use a randomly generated secret key with a minimum length of 256 bits (32 characters).

```csharp
// ❌ Weak secret key - DO NOT USE
"MysecretKey123"

// ✅ Strong secret key - USE THIS
"a5J#9$rTp2!kLzX&cB7E@qYm8NwDvF3h"
```

- **Store in Environment Variables**: Store your secret key in environment variables rather than in your appsettings.json file.

```csharp
// Program.cs
builder.Configuration["JwtSettings:Secret"] = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
```

- **Rotate Regularly**: Rotate your secret key regularly in production environments (e.g., every 90 days).

### HTTPS Usage

- **Always Use Over HTTPS**: JWT tokens should never be sent over the network unencrypted.

```csharp
// Program.cs
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseHttpsRedirection();
```

- **Use HTTPS Even in Development**: Using HTTPS even in local development environments helps you detect production issues early.

### Lifetime and Renewal

- **Use Short-Lived Access Tokens**: Limit access token lifetime to a short period like 15-60 minutes.

```json
// appsettings.json
{
  "JwtSettings": {
    "AccessTokenExpirationInMinutes": 30,
    "RefreshTokenExpirationInDays": 7
  }
}
```

- **Implement Refresh Token Strategy**: Implement a secure refresh token strategy to prevent users from constantly having to log in despite short-lived access tokens.

- **Use Absolute and Sliding Expiration**: Consider using absolute expiration for access tokens and sliding expiration for refresh tokens.

### Claims and Authorization

- **Use Minimum Required Claims**: Only store claims in tokens that you actually need.

```csharp
// ❌ Token with excessive data - DO NOT USE
var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, user.Id),
    new Claim(ClaimTypes.Name, user.Username),
    new Claim(ClaimTypes.Email, user.Email),
    new Claim(ClaimTypes.GivenName, user.FirstName),
    new Claim(ClaimTypes.Surname, user.LastName),
    new Claim("PhoneNumber", user.PhoneNumber),
    new Claim("Address", user.Address),
    // Many more user details...
};

// ✅ Token with only necessary claims - USE THIS
var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, user.Id),
    new Claim(ClaimTypes.Name, user.Username),
    new Claim(ClaimTypes.Role, user.Role)
};
```

- **Use Role-based and Policy-based Authorization**: Use the [Authorize] attribute with policies for security at the controller or endpoint level.

```csharp
// Controller class
[Authorize(Policy = "RequireAdminRole")]
public class AdminController : ControllerBase
{
    // Controller methods
}

// Policy definition in Program.cs
services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
});
```

## Performance

### Service Configuration

- **Use Singleton**: Register JwtService as a singleton in your DI container.

```csharp
// Program.cs
services.AddSingleton<IJwtService, JwtService>();
```

- **Cache TokenValidationParameters**: Instead of creating validation parameters each time, create them once and reuse.

```csharp
// JwtService.cs
private readonly TokenValidationParameters _validationParameters;

public JwtService(IOptions<JwtSettings> settings)
{
    var jwtSettings = settings.Value;
    _validationParameters = new TokenValidationParameters
    {
        // Parameters...
    };
}
```

### Memory Optimization

- **Avoid Unnecessary Token Validations**: Avoid validating the token multiple times. Store validation results in HttpContext.Items.

```csharp
// Middleware.cs
if (!context.Items.ContainsKey("TokenValidated"))
{
    // Token validation
    context.Items["TokenValidated"] = true;
}
```

- **Use StringSegment**: Use StringSegment when processing the Authorization header to minimize string splitting and concatenation operations.

```csharp
// ❌ Unnecessary string operations - DO NOT USE
var authHeader = context.Request.Headers["Authorization"].ToString();
if (authHeader.StartsWith("Bearer "))
{
    var token = authHeader.Substring(7);
    // Processing...
}

// ✅ StringSegment usage - USE THIS
var authHeader = context.Request.Headers["Authorization"];
if (authHeader.Count > 0 && authHeader[0].StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
{
    var tokenSegment = new StringSegment(authHeader[0], 7, authHeader[0].Length - 7);
    // Processing...
}
```

## Integration

### Middleware Pipeline

- **Add Middleware in the Correct Order**: Add JWTVerifyLibrary middleware before UseAuthentication() and UseAuthorization().

```csharp
// Program.cs
app.UseJwtVerification();
app.UseAuthentication();
app.UseAuthorization();
```

### Exception Management

- **Use Exception Filters**: Add global exception filters to catch and handle JWT validation errors.

```csharp
// JwtExceptionFilter.cs
public class JwtExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is SecurityTokenException)
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Invalid token" });
            context.ExceptionHandled = true;
        }
    }
}

// Program.cs
services.AddControllers(options =>
{
    options.Filters.Add<JwtExceptionFilter>();
});
```

### Logging

- **Log Token Validation Events**: Log token validation failures to detect potential security issues.

```csharp
services.AddJwtVerification(Configuration)
    .AddEventHandler(events =>
    {
        events.OnTokenValidationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtMiddleware>>();
            logger.LogWarning("JWT validation failed: {Exception}", context.Exception.Message);
            return Task.CompletedTask;
        };
    });
```

- **Don't Log PII (Personally Identifiable Information)**: Make sure your logs don't contain personal data.

## Installation and Configuration

### Configuration Management

- **Use IOptions Pattern**: Use the IOptions pattern for JWT settings.

```csharp
// Program.cs
services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));

// JwtService.cs
public JwtService(IOptions<JwtSettings> settings)
{
    _settings = settings.Value;
}
```

- **Use Fake Implementation for Testing**: Use a fake implementation of the JWT service for unit tests.

```csharp
// Test class
public class AuthControllerTests
{
    [Fact]
    public void Login_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var fakeJwtService = new FakeJwtService();
        var controller = new AuthController(fakeJwtService);
        
        // Act & Assert
    }
}
```

### References and Dependencies

- **Use Explicit Version References**: Use explicit version numbers when referencing NuGet packages.

```xml
<!-- ❌ Ambiguous version - DO NOT USE -->
<PackageReference Include="JWTVerifyLibrary" Version="1.*" />

<!-- ✅ Explicit version number - USE THIS -->
<PackageReference Include="JWTVerifyLibrary" Version="1.0.0" />
```

## Best Practices Checklist

Follow this checklist when using JWTVerifyLibrary in your projects:

1. **Security**
   - [ ] Strong and secure secret key is used (at least 32 characters, random)
   - [ ] Secret key is stored securely (environment variables, Azure Key Vault, etc.)
   - [ ] HTTPS is enforced
   - [ ] Token lifetime is set optimally (15-60 minutes)
   - [ ] Refresh token strategy is implemented

2. **Performance**
   - [ ] Services are registered correctly (singleton)
   - [ ] Caching strategies are implemented
   - [ ] Unnecessary validations are prevented

3. **Integration**
   - [ ] Middleware is added in the correct order
   - [ ] Error handling is properly configured
   - [ ] Logging strategy is implemented

4. **Configuration**
   - [ ] IOptions pattern is used
   - [ ] Configuration is designed to be testable
   - [ ] Package references are specified correctly

## Antipatterns and What to Avoid

Avoid the following practices:

- **Hardcoding Secret Keys**: Don't store secret keys as plain text in your code or configuration files.
- **Long-Lived Tokens**: Don't set access token lifetimes too long.
- **Excessive Data in Tokens**: JWT tokens are not designed to store large pieces of data.
- **Sensitive Data on Client Side**: Don't store sensitive data in tokens that you don't want to send to the client.
- **Insufficient Exception Handling**: Handle validation errors properly and log them.

## Further Resources

- [JWT Official Website](https://jwt.io/)
- [OWASP JWT Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/JSON_Web_Token_for_Java_Cheat_Sheet.html)
- [Microsoft Identity Platform](https://docs.microsoft.com/en-us/azure/active-directory/develop/)

---

[◀ Versioning](08-Versioning.md) | [Home](README.md) | [Next: Frequently Asked Questions ▶](10-FAQ.md) 