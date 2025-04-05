# Introduction

**Version:** 1.0.0  
**Author:** R&D Engineer Aykut Mürkit  
**Company:** ISBAK 2025

---

## What is DeviceApi?

DeviceApi is a modern RESTful API designed to simplify the management of IoT (Internet of Things) devices and their associated data. Built on ASP.NET Core, it provides a comprehensive solution for device registration, monitoring, control, and data collection in an IoT ecosystem.

At its core, DeviceApi serves as a bridge between physical IoT devices and the applications that need to interact with them. It abstracts away the complexities of device communication protocols, data storage, and security concerns, providing a clean, consistent interface for developers.

## Key Concepts

### Devices

In the context of DeviceApi, a "device" represents any IoT hardware that can connect to the internet and communicate with the API. Each device has:

- A unique identifier
- Metadata (manufacturer, model, firmware version, etc.)
- Status information (online/offline, battery level, etc.)
- Configuration settings
- Associated data records

### Data Points

Devices generate data, which is captured as "data points." These can include:

- Sensor readings (temperature, humidity, pressure, etc.)
- State changes (door open/closed, motion detected, etc.)
- Operational metrics (uptime, signal strength, etc.)
- Alert conditions

### Groups and Hierarchies

Devices can be organized into logical groups, allowing for:

- Organizational separation (by department, location, function, etc.)
- Batch operations (update all devices in a group)
- Access control (limit which users can see which groups)
- Aggregated reporting

### Users and Roles

The API supports role-based access control with:

- Different permission levels (admin, operator, viewer, etc.)
- Multi-tenant isolation
- Audit logging of user actions

## Architecture Overview

DeviceApi follows a clean, layered architecture:

1. **API Layer**: HTTP endpoints for client applications
2. **Service Layer**: Business logic and workflows
3. **Data Access Layer**: Repository pattern for data operations
4. **Core Domain**: Entity definitions and business rules

### Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Authentication**: JWT-based with refresh tokens
- **Database**: Entity Framework Core with SQL Server
- **Documentation**: Swagger/OpenAPI
- **Logging**: Serilog with structured logging
- **Messaging**: Optional integration with Azure Service Bus or RabbitMQ
- **Caching**: Distributed caching with Redis

## Why DeviceApi?

### Problems Solved

DeviceApi addresses several common challenges in IoT device management:

- **Scalability**: Designed to handle thousands of devices and millions of data points
- **Security**: Comprehensive authentication, authorization, and data protection
- **Interoperability**: Standard REST interface works with any client that speaks HTTP
- **Flexibility**: Customizable device types and data schemas
- **Reliability**: Built with fault tolerance and recovery in mind

### Use Cases

DeviceApi is ideal for:

- **Smart Building Management**: Monitor and control HVAC, lighting, access control
- **Industrial IoT**: Track factory equipment, monitor production lines
- **Environmental Monitoring**: Collect data from distributed sensor networks
- **Fleet Management**: Track vehicle telemetry and maintenance needs
- **Smart City Infrastructure**: Manage street lighting, parking sensors, etc.

## Getting Started

The fastest way to understand DeviceApi is to see it in action. The [Quick Start](03-Quick-Start.md) guide will walk you through setting up a development environment and making your first API calls.

For a complete list of endpoints and their functionality, refer to the [API Endpoints](04-API-Endpoints.md) documentation.

---

[◀ Home](README.md) | [Next: Installation ▶](02-Installation.md) 