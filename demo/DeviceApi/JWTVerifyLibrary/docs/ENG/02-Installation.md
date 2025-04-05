# Installation

**Version:** 1.0.0  
**Author:** R&D Engineer Aykut Mürkit  
**Company:** ISBAK 2025

---

This section outlines the installation steps and the process of integrating JWTVerifyLibrary into your project.

## Prerequisites

Before using JWTVerifyLibrary, ensure that you have the following requirements:

- **.NET SDK 8.0** or newer
- **ASP.NET Core 8.0** or newer

## NuGet Package Installation

JWTVerifyLibrary can be easily installed via NuGet.

### Using Package Manager Console

```powershell
Install-Package JWTVerifyLibrary
```

### Using .NET CLI

```bash
dotnet add package JWTVerifyLibrary
```

### Using Visual Studio

1. Right-click on your project in Solution Explorer
2. Select "Manage NuGet Packages..."
3. Click on the "Browse" tab
4. Search for "JWTVerifyLibrary"
5. Select the package and click the "Install" button

## Adding as a Project Reference

If you're not using NuGet or want to make changes to the source code, you can add JWTVerifyLibrary as a project reference.

1. Download or clone the JWTVerifyLibrary project
2. Right-click on your solution and select "Add > Existing Project..."
3. Navigate to and select the JWTVerifyLibrary.csproj file
4. Right-click on your main project and select "Add > Project Reference..."
5. Check the JWTVerifyLibrary project and click the "OK" button

## Configuration

For JWTVerifyLibrary to work properly, you need to configure some settings in your configuration file.

### Adding Settings to appsettings.json

Add the following JWT settings to your appsettings.json file:

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

### Creating a JWTVerifyLibrarySettings.json File

Alternatively, you can create a dedicated JWTVerifyLibrarySettings.json file. This approach allows you to separate JWT configuration settings from the main configuration file.

1. Create a file named "JWTVerifyLibrarySettings.json" in the root directory of your project
2. Add the following content:

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

3. Set the file properties to "Copy to Output Directory" as "Copy always":

```xml
<ItemGroup>
  <None Update="JWTVerifyLibrarySettings.json">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

## Security Advice

From a security perspective, it's important to consider the following recommendations:

- **Secret Key:** Use a Secret that is at least 32 characters long, complex, and randomly generated
- **Environment Variables:** In production, store your Secret key in environment variables rather than in appsettings.json
- **HTTPS:** Always use HTTPS when using JWT
- **Key Rotation:** Regularly rotate your Secret keys

## Troubleshooting

Common issues encountered during installation and their solutions:

1. **Dependency Conflicts:** 
   
   Solution: Ensure that other components in your project are using compatible versions with the packages that JWTVerifyLibrary depends on.

2. **Configuration File Not Found:** 
   
   Solution: Verify that the JWTVerifyLibrarySettings.json file is in the correct location and is being copied to the output directory.

3. **.NET Version Incompatibility:** 
   
   Solution: Ensure your project is using .NET 8.0 or a newer version.

---

[◀ Introduction](01-Introduction.md) | [Home](README.md) | [Next: Quick Start ▶](03-Quick-Start.md) 