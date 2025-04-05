# Versioning

**Version:** 1.0.0  
**Author:** R&D Engineer Aykut Mürkit  
**Company:** ISBAK 2025

---

This section explains the versioning policy of JWTVerifyLibrary, how updates are distributed, and backward compatibility guarantees.

## Version Numbering

JWTVerifyLibrary follows the [Semantic Versioning (SemVer)](https://semver.org/) scheme. Version numbers are defined in the following format:

```
X.Y.Z
```

Where:
- **X** = Major Version: When you make incompatible API changes
- **Y** = Minor Version: When you add functionality in a backward-compatible manner
- **Z** = Patch: When you make backward-compatible bug fixes

Examples:
- `1.0.0` - Initial stable release
- `1.1.0` - New features added, backward compatible
- `1.1.1` - Bug fixes made
- `2.0.0` - New version with backward-incompatible changes

## Pre-releases and Build Metadata

For development pre-releases and test versions, the following formats can be used:

- **Pre-release**: `X.Y.Z-alpha.N`, `X.Y.Z-beta.N`, `X.Y.Z-rc.N`
- **Build metadata**: `X.Y.Z+YYYYMMDDHHMMSS`

Examples:
- `1.0.0-alpha.1` - First alpha release
- `1.0.0-beta.2` - Second beta release
- `1.0.0-rc.1` - First release candidate
- `1.0.0+20250115103045` - Build from January 15, 2025

## Version History

### 1.0.0 (January 15, 2025)

- Initial stable release
- Core JWT token validation functionality
- ASP.NET Core integration
- Middleware support
- Extension methods
- Simple configuration mechanism

### 0.9.0 (December 10, 2024)

- Release Candidate version
- Finalization of public APIs
- Performance improvements
- Documentation additions

### 0.5.0 (October 1, 2024)

- Beta version
- JWT validation functions
- Initial middleware implementation

### 0.1.0 (August 15, 2024)

- Alpha version
- Basic project structure

## Upgrade Guide

Upgrade guide for different version numbers:

### Patch Versions (Z increment)

Patch versions only contain bug fixes and are completely backward compatible. You can update without changing your code.

```xml
<PackageReference Include="JWTVerifyLibrary" Version="1.0.1" />
```

### Minor Versions (Y increment)

Minor versions add new features but don't change existing functionality. It's generally a safe upgrade, but it's good to review new behaviors.

```xml
<PackageReference Include="JWTVerifyLibrary" Version="1.1.0" />
```

Check the documentation for new features and optionally use them.

### Major Versions (X increment)

Major versions contain backward-incompatible changes. Before upgrading, you should:

1. Read the release notes carefully
2. Review the changes in the changelog
3. Test your application
4. Update your code to be compatible with the new API if necessary

For example, moving from 1.x.x to 2.0.0:

```xml
<PackageReference Include="JWTVerifyLibrary" Version="2.0.0" />
```

## Backward Compatibility Policy

### Guarantees

- **Patch versions**: Full backward compatibility guaranteed.
- **Minor versions**: Backward compatibility for public APIs and behaviors. Your existing code will continue to work as long as it doesn't interact with new features.
- **Major versions**: Backward compatibility not guaranteed. Changes to your code may be required.

### Scope of Changes

When using our API, you can expect the following changes:

#### In Patch Versions:
- Bug fixes
- Security updates
- Changes in internal implementation (public API doesn't change)
- Documentation improvements

#### In Minor Versions:
- New features
- New APIs
- New extensibility points
- Deprecation warnings (but functionality is preserved)

#### In Major Versions:
- Changing or removing APIs
- Behavioral changes
- Changes in supported .NET versions
- Removal of features deprecated in previous versions

## Support Lifecycle

Our support policy for JWTVerifyLibrary versions:

- **Major versions**: The latest major version and the previous major version are supported.
- **Minor versions**: Only the latest minor version for each supported major version is actively supported.
- **Patch versions**: Critical security updates are applied to all supported versions.

For example, if version 2.3.0 is currently available:
- 2.3.0 - Full support
- 2.0.0-2.2.x - Security updates only
- 1.x.x - Critical security updates only for the latest minor version (1.5.x)
- 0.x.x - Not supported

## LTS (Long-Term Support) Versions

Some versions may be designated as long-term support (LTS). These versions are supported for a longer period than the standard support period.

LTS versions are marked as follows:
- 1.0.0-LTS
- 2.0.0-LTS

LTS versions are ideal for enterprise applications requiring stability and long-term support.

## Upgrade Helper Tools

JWTVerifyLibrary provides some tools to help with major version upgrades:

- **UpgradeAssistant**: Performs automatic code transformations from old version to new version.
- **MigrationGuide.md**: Document containing detailed upgrade steps for each major version.
- **ApiCompatibilityChecker**: Tool to check your code's compatibility with the new API.

## Deprecation Policy

Before features are deprecated:

1. The feature is marked as `[Obsolete]` in a minor version update
2. A warning message is added in the next minor version
3. It is preserved with warning for at least one major version
4. It may be removed in the next major version

Example deprecation process:

```csharp
// In version 1.5.0
[Obsolete]
public void OldMethod() { ... }

// In version 1.6.0
[Obsolete("This method will be removed in version 2.0.0. Use NewMethod() instead.")]
public void OldMethod() { ... }

// In version 2.0.0
// OldMethod is no longer available
```

## Version Control and Distribution

JWTVerifyLibrary versions are distributed as follows:

1. **NuGet Packages**: All official releases are published on [NuGet.org](https://www.nuget.org)
2. **GitHub Releases**: Source code for each version is tagged on [GitHub](https://github.com/isbak/JWTVerifyLibrary/releases)
3. **Distribution Frequency**:
   - Patch versions: As needed (for bug fixes)
   - Minor versions: Every 2-3 months
   - Major versions: 1-2 times per year

---

[◀ API Reference](07-API-Reference.md) | [Home](README.md) | [Next: Best Practices ▶](09-Best-Practices.md) 