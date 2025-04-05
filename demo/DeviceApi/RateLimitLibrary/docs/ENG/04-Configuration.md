# Configuration

**Version:** 1.0.0  
**Company:** DevOps 2025

---

This document explains in detail all the available configuration options for RateLimitLibrary, their meanings, and how to set them.

## RateLimitLibrarySettings.json Structure

RateLimitLibrary reads configuration settings from the `RateLimitLibrarySettings.json` file. The general structure of this configuration file is as follows:

```json
{
  "RateLimitSettings": {
    "EnableGlobalRateLimit": true,
    "GlobalRateLimitPeriod": "1m",
    "GlobalRateLimitRequests": 100,
    "IpRateLimiting": {
      "EnableIpRateLimiting": true,
      "IpRateLimitPeriod": "1m",
      "IpRateLimitRequests": 100
    },
    "EndpointLimits": [
      {
        "Endpoint": "/api/auth/login",
        "Period": "5m",
        "Limit": 10,
        "EnableConcurrencyLimit": true,
        "ConcurrencyLimit": 5
      },
      {
        "Endpoint": "/api/device",
        "Period": "10m",
        "Limit": 3
      }
    ]
  }
}
```

## Basic Settings

### Global Rate Limiting Settings

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `EnableGlobalRateLimit` | boolean | `true` | Determines whether global rate limiting is active. If set to `true`, all API requests are limited globally. |
| `GlobalRateLimitPeriod` | string | `"1m"` | Sets the time window duration for global rate limiting. Values can be: "Xs" (seconds), "Xm" (minutes), "Xh" (hours), "Xd" (days). X should be replaced with a numerical value, e.g., "30s", "15m". |
| `GlobalRateLimitRequests` | integer | `100` | Sets the maximum number of requests allowed within the specified time window. For example, if `GlobalRateLimitPeriod` is "1m" and `GlobalRateLimitRequests` is 100, a maximum of 100 requests will be processed per minute. |

### IP-Based Rate Limiting Settings

IP-based rate limiting restricts requests based on IP addresses. This ensures that each user/device can make requests within a specific rate.

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `IpRateLimiting.EnableIpRateLimiting` | boolean | `true` | Determines whether IP-based rate limiting is active. |
| `IpRateLimiting.IpRateLimitPeriod` | string | `"1m"` | Sets the time window duration for IP-based rate limiting. Format is the same as global settings (e.g., "5m"). |
| `IpRateLimiting.IpRateLimitRequests` | integer | `100` | Sets the maximum number of requests allowed per time window for each IP address. |

## Endpoint-Based Limits

Endpoint-based rate limiting allows you to define specific limitations based on particular API endpoints. This lets you restrict some API paths more strictly than others.

Endpoint limits are configured as an array, with a separate configuration defined for each endpoint:

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `Endpoint` | string | required | The path of the API endpoint you want to limit. For example, "/api/auth/login". |
| `Period` | string | required | The time window duration for this endpoint. Format is the same as global settings (e.g., "5m"). |
| `Limit` | integer | required | The maximum number of requests allowed within the specified time window for this endpoint. |
| `EnableConcurrencyLimit` | boolean | `false` | Whether to enable concurrency limiting. This limits the number of concurrent requests to the endpoint. |
| `ConcurrencyLimit` | integer | `0` | The maximum number of concurrent requests allowed to the endpoint. Only used when `EnableConcurrencyLimit` is true. |

## Time Unit Formats

Time window durations can use the following formats:

| Format | Description | Example |
|--------|-------------|---------|
| `Xs` | X seconds | "30s" (30 seconds) |
| `Xm` | X minutes | "5m" (5 minutes) |
| `Xh` | X hours | "2h" (2 hours) |
| `Xd` | X days | "1d" (1 day) |

## Configuration Examples

### Example 1: Simple Global Limiting

A simple configuration that only enables global rate limiting:

```json
{
  "RateLimitSettings": {
    "EnableGlobalRateLimit": true,
    "GlobalRateLimitPeriod": "1m",
    "GlobalRateLimitRequests": 100,
    "IpRateLimiting": {
      "EnableIpRateLimiting": false
    },
    "EndpointLimits": []
  }
}
```

### Example 2: IP-Based Limiting Only

IP-based limiting without global limiting:

```json
{
  "RateLimitSettings": {
    "EnableGlobalRateLimit": false,
    "IpRateLimiting": {
      "EnableIpRateLimiting": true,
      "IpRateLimitPeriod": "1m",
      "IpRateLimitRequests": 50
    },
    "EndpointLimits": []
  }
}
```

### Example 3: Endpoint-Focused Limiting

Custom limits for specific endpoints:

```json
{
  "RateLimitSettings": {
    "EnableGlobalRateLimit": false,
    "IpRateLimiting": {
      "EnableIpRateLimiting": false
    },
    "EndpointLimits": [
      {
        "Endpoint": "/api/auth/login",
        "Period": "15m",
        "Limit": 5,
        "EnableConcurrencyLimit": false
      },
      {
        "Endpoint": "/api/payment/process",
        "Period": "1h",
        "Limit": 10,
        "EnableConcurrencyLimit": true,
        "ConcurrencyLimit": 2
      }
    ]
  }
}
```

### Example 4: Comprehensive Limiting

A comprehensive configuration using global, IP, and endpoint limits together:

```json
{
  "RateLimitSettings": {
    "EnableGlobalRateLimit": true,
    "GlobalRateLimitPeriod": "1m",
    "GlobalRateLimitRequests": 1000,
    "IpRateLimiting": {
      "EnableIpRateLimiting": true,
      "IpRateLimitPeriod": "1m",
      "IpRateLimitRequests": 100
    },
    "EndpointLimits": [
      {
        "Endpoint": "/api/auth/login",
        "Period": "5m",
        "Limit": 10,
        "EnableConcurrencyLimit": true,
        "ConcurrencyLimit": 5
      },
      {
        "Endpoint": "/api/users",
        "Period": "10m",
        "Limit": 50
      },
      {
        "Endpoint": "/api/reports/generate",
        "Period": "1h",
        "Limit": 5,
        "EnableConcurrencyLimit": true,
        "ConcurrencyLimit": 2
      }
    ]
  }
}
```

## Configuration Priority

RateLimitLibrary uses the following priority order:

1. Endpoint-specific limits (highest priority)
2. IP-based limits
3. Global limits (lowest priority)

If you enable all configuration types simultaneously, each request checks all applicable limits and is blocked if it is rejected by any of them.

## Changing Configuration in Code

If you want to change settings dynamically in code, you can use the RateLimitLibrary extension methods:

```csharp
// Configure RateLimiter options directly
builder.Services.AddRateLimiter(options => {
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        return RateLimitPartition.GetFixedWindowLimiter("global", _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 200, // Custom value
            Window = TimeSpan.FromMinutes(2), // Custom value
            QueueLimit = 0
        });
    });
    
    // Other custom configurations...
});
```

---

[◀ Quick Start](03-Quick-Start.md) | [Home](../README.md) | [Next: API Reference ▶](05-API-Reference.md) 