# JWTVerifyLibrary

## Overview

JWTVerifyLibrary is a lightweight library for JWT token verification in ASP.NET Core applications. It provides a simple and straightforward way to validate JWT tokens and integrate with the ASP.NET Core authentication pipeline.

## Features

- Easy integration with ASP.NET Core applications
- Token validation with standard security practices
- Claims extraction and identity assignment
- Middleware for request interception and validation

## Quick Start

To use this library in your ASP.NET Core application:

1. Add a reference to the JWTVerifyLibrary project in your solution
2. Ensure your application has the JWTVerifyLibrarySettings.json file in the output directory
3. Add the JWT verification services in your Program.cs or Startup.cs:
   ```csharp
   using JWTVerifyLibrary.Extensions;

   // In Program.cs
   builder.Services.AddJwtVerification(builder.Configuration);
   ```
4. Add the JWT verification middleware to your application pipeline:
   ```csharp
   app.UseJwtVerification();
   ```
5. Use standard ASP.NET Core `[Authorize]` attributes to protect your endpoints

## Configuration

The library uses the following configuration from JWTVerifyLibrarySettings.json:

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