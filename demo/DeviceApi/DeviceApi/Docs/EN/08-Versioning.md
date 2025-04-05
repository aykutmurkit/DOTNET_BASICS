# Versioning

**Version:** 1.0.0  
**Author:** R&D Engineer Aykut Mürkit  
**Company:** ISBAK 2025

---

This document outlines the versioning strategy for DeviceApi, ensuring predictable updates and backwards compatibility.

## Versioning Policy

DeviceApi follows Semantic Versioning (SemVer) 2.0.0, which provides a clear structure for version numbers and their meaning.

### Version Format

All versions follow the format: **MAJOR.MINOR.PATCH**

- **MAJOR** version increases for incompatible API changes
- **MINOR** version increases for backward-compatible functionality additions
- **PATCH** version increases for backward-compatible bug fixes

### Additional Labels

For pre-release versions, we may append a hyphen and a series of dot-separated identifiers:

- Alpha releases: `1.0.0-alpha.1`, `1.0.0-alpha.2`, etc.
- Beta releases: `1.0.0-beta.1`, `1.0.0-beta.2`, etc.
- Release candidates: `1.0.0-rc.1`, `1.0.0-rc.2`, etc.

## API Versioning

DeviceApi implements multiple strategies for API versioning to provide flexibility to clients.

### URL Versioning

The primary versioning mechanism is through the URL path:

```
https://api.example.com/v1/devices
https://api.example.com/v2/devices
```

### Header-Based Versioning

Clients can also specify the API version through a custom HTTP header:

```
X-API-Version: 1.0
```

### Accept Header Versioning

Content negotiation using the Accept header is supported:

```
Accept: application/json;version=1.0
```

### Default Version

If no version is specified, the API defaults to the most recent stable version.

## Version Lifecycle

Each API version progresses through the following stages:

1. **Preview**: Initial version for early testing and feedback
2. **Active**: Fully supported with regular updates and improvements
3. **Deprecated**: Still functioning but scheduled for removal
4. **Sunset**: No longer supported or available

### Deprecation Policy

When an API version or feature is deprecated:

1. Advance notice will be provided at least 6 months before sunset
2. Documentation will clearly mark deprecated features
3. API responses will include deprecation warnings in headers
4. The changelog will list all deprecated features

## Version History

### 1.0.0 (Current) - 2025-01-15

- Initial stable release of DeviceApi
- Complete device management capabilities
- Authentication and authorization system
- Real-time data collection and processing

### 0.9.0 (Beta) - 2024-11-01

- Beta release for partner testing
- Added pagination, filtering, and sorting
- Improved error handling and documentation

### 0.5.0 (Alpha) - 2024-08-15

- Alpha release for internal testing
- Basic device registration and management
- Initial API structure and data models

## Upgrade Guide

When upgrading between versions, consider the following:

### Upgrading from 0.9.x to 1.0.0

- Authentication tokens now expire after 1 hour (previously 24 hours)
- Device data endpoint structure standardized to `/devices/{id}/data`
- Added required fields for device registration: `macAddress` and `firmwareVersion`

### Upgrading from 0.5.x to 0.9.0

- Authentication mechanism changed from Basic Auth to JWT
- Response format standardized across all endpoints
- Pagination parameters changed from `limit`/`offset` to `pageSize`/`page`

## Backward Compatibility Promise

DeviceApi makes the following compatibility promises:

### What We Won't Change in a Minor/Patch Version

- Existing endpoint URLs
- Required request parameters
- Response field meanings (though we may add new fields)
- Authentication mechanisms
- Error codes for existing conditions

### What May Change in a Minor Version

- Addition of new endpoints
- Addition of optional parameters
- Addition of response fields
- Extended enum values
- Performance improvements

### What May Change in a Major Version

- Removal of endpoints
- Changes to parameter requirements
- Changes to response structure
- Authentication mechanism changes
- Error code adjustments

## Version Support Policy

DeviceApi follows a predictable support schedule:

- Each major version is supported for a minimum of 24 months
- At least two major versions are supported at any time
- Security updates are provided for all supported versions
- Bug fixes are prioritized for the most recent major version

### End-of-Life Schedule

| Version | Release Date | End of Active Support | End of Life |
|---------|--------------|------------------------|-------------|
| 1.0.x   | 2025-01-15   | 2026-01-15            | 2027-01-15  |
| 0.9.x   | 2024-11-01   | 2025-01-15            | 2025-07-15  |
| 0.5.x   | 2024-08-15   | 2024-11-01            | 2025-01-15  |

## Versioning Best Practices for Clients

To ensure a smooth experience with DeviceApi, we recommend:

1. **Always specify a version**: Don't rely on the default version, which may change
2. **Subscribe to release notifications**: Stay informed about upcoming changes
3. **Test against new versions early**: Take advantage of preview releases
4. **Plan migrations**: Schedule time to migrate to new versions before old ones sunset
5. **Automate compatibility testing**: Set up test suites that validate against the API

## Internal Versioning Strategy

For developers contributing to DeviceApi:

### Database Schema Migrations

Database changes are tracked using Entity Framework Core migrations. Each migration is versioned with a timestamp and descriptive name:

```csharp
public partial class AddDeviceLocationTracking_20250110 : Migration
{
    // migration code
}
```

### Library Dependencies

Package dependencies are specified with exact versions or appropriate version ranges:

```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.*" />
```

### Source Control

Git tags are created for each release and follow the SemVer scheme:

```bash
git tag -a v1.0.0 -m "Release 1.0.0"
git push origin v1.0.0
```

## API Change Management Process

New API changes follow this process:

1. **Proposal**: New features are documented and reviewed
2. **Implementation**: Changes are developed in a feature branch
3. **Review**: API changes undergo technical and design review
4. **Testing**: Automated tests validate backward compatibility
5. **Documentation**: API changes are fully documented
6. **Release Notes**: Changes are summarized in release notes
7. **Deployment**: New version is deployed and monitored

---

[◀ Configuration](07-Configuration.md) | [Home](README.md) | [Next: Best Practices ▶](09-Best-Practices.md) 