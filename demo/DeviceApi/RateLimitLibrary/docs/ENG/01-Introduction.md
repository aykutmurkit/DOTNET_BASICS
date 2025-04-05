# Introduction

**Version:** 1.0.0  
**Company:** DevOps 2025

---

## What is RateLimitLibrary?

RateLimitLibrary is a simple and effective rate limiting library for ASP.NET Core applications. This library is designed to protect your web APIs from high traffic and malicious request bombardments.

Rate limiting controls the number of requests that can be made to an API within a specific time frame, protecting your service's resources and ensuring fair service to all users. RateLimitLibrary makes it easy to integrate this functionality into ASP.NET Core applications.

## Why RateLimitLibrary?

### Problems Solved

RateLimitLibrary solves the following common API issues:

- **Overload Protection**: Prevents your server from crashing during high traffic periods
- **DDoS Protection**: Provides protection against malicious request bombardments
- **Resource Optimization**: Ensures balanced resource usage for your system
- **Fair Usage**: Ensures all API users utilize resources fairly
- **Cost Control**: Helps control processing costs in cloud-based systems

### Key Features

- **Global Rate Limiting**: Limiting all API requests
- **IP-Based Rate Limiting**: Limiting requests from specific IP addresses
- **Endpoint-Based Rate Limiting**: Setting custom limits for specific API endpoints
- **Time-Based Limiting**: Setting limits based on seconds, minutes, hours, or days
- **Concurrency Limits**: Controlling the number of requests processed simultaneously
- **Easy Configuration**: Simple JSON-based configuration

## Core Concepts

### Rate Limiting Strategies

RateLimitLibrary supports the following rate limiting strategies:

#### 1. Fixed Window

Determines the maximum number of requests allowed in a specific time period (e.g., every minute). The counter resets when the time period is completed.

```
│        Minute 1         │         Minute 2        │
├─────────────────────────┼─────────────────────────┤
│ Max 100 requests        │ Max 100 requests        │
```

#### 2. Concurrent Limiting

Determines the maximum number of requests that can be processed simultaneously. This is especially useful for resource-intensive operations.

```
Maximum 5 requests can be processed at the same time:
[Request 1] [Request 2] [Request 3] [Request 4] [Request 5]
```

### Limit Exceeded Behavior

When a request exceeds the defined limits, the API returns an HTTP status code 429 Too Many Requests. In this case, the client should wait for a while and try again.

## Use Cases

RateLimitLibrary is particularly useful in the following scenarios:

- **Public APIs**: APIs that are offered for free or for a fee and accessed by a large number of users
- **Microservice Architectures**: For regulating and protecting communication between services
- **Payment and Financial Transactions**: For increasing security in sensitive transactions
- **High-Traffic Websites**: For protection against DDoS attacks
- **Multi-Tenant Systems**: For ensuring fair distribution of resources among different tenants

---

[◀ Home](../README.md) | [Next: Installation ▶](02-Installation.md) 