# Architecture

**Version:** 1.0.0  
**Author:** R&D Engineer Aykut Mürkit  
**Company:** ISBAK 2025

---

This document describes the architecture of DeviceApi, including its key components, design patterns, and the relationships between them.

## Architectural Overview

DeviceApi uses a clean, multi-layered architecture that separates concerns and promotes maintainability, testability, and scalability. The application follows Domain-Driven Design (DDD) principles and employs the Clean Architecture approach.

### High-Level Architecture Diagram

```
┌─────────────────────────────────────────────────────────┐
│                     Client Applications                  │
│   (Web, Mobile, IoT Devices, Third-party Applications)   │
└───────────────────────────┬─────────────────────────────┘
                           │
┌───────────────────────────▼─────────────────────────────┐
│                         API Layer                        │
│             (Controllers, Middleware, Filters)           │
└───────────────────────────┬─────────────────────────────┘
                           │
┌───────────────────────────▼─────────────────────────────┐
│                      Application Layer                   │
│            (Services, DTOs, Validation, Mapping)         │
└───────────────────────────┬─────────────────────────────┘
                           │
┌───────────────────────────▼─────────────────────────────┐
│                       Domain Layer                       │
│    (Entities, Value Objects, Domain Services, Events)    │
└───────────────────────────┬─────────────────────────────┘
                           │
┌───────────────────────────▼─────────────────────────────┐
│                   Infrastructure Layer                   │
│    (Repositories, External Services, Persistence, I/O)   │
└───────────────────────────┬─────────────────────────────┘
                           │
┌───────────────────────────▼─────────────────────────────┐
│                    External Systems                      │
│     (Databases, Message Brokers, External Services)      │
└─────────────────────────────────────────────────────────┘
```

## Layers and Components

### 1. API Layer

This is the entry point for all external interactions with the system. It handles HTTP requests, routes them to the appropriate handlers, and transforms responses.

#### Key Components:

- **Controllers**: Handle incoming HTTP requests and return appropriate responses
- **Middleware**: Process requests and responses (authentication, logging, exception handling)
- **Filters**: Implement cross-cutting concerns like validation, caching, and performance monitoring
- **API Models**: Input and output models specific to the API endpoints

#### Design Principles:

- Controllers are kept thin, delegating business logic to the Application Layer
- RESTful design with proper resource naming and HTTP methods
- Versioning strategy to ensure backward compatibility
- Consistent error handling and response formats

### 2. Application Layer

This layer orchestrates the flow of data and coordinates business operations without containing business rules.

#### Key Components:

- **Services**: Implement application use cases by coordinating domain objects
- **DTOs (Data Transfer Objects)**: Objects for transferring data between layers
- **Validators**: Ensure data validity before processing
- **Mappers**: Transform between domain models and DTOs
- **Command/Query Handlers**: If using CQRS, handle commands and queries

#### Design Principles:

- Services don't contain business logic but orchestrate domain objects
- Each service has a specific responsibility (SRP)
- Transactions are managed at this level
- Domain events are dispatched and handled here

### 3. Domain Layer

This is the heart of the application, containing the business rules, domain logic, and entities.

#### Key Components:

- **Entities**: Business objects with identity and lifecycle
- **Value Objects**: Immutable objects that describe aspects of the domain
- **Domain Services**: Encapsulate domain operations that don't naturally belong to entities
- **Domain Events**: Represent significant occurrences within the domain
- **Aggregates**: Clusters of entities and value objects treated as a unit
- **Interfaces**: Define contracts for repositories and services

#### Design Principles:

- Rich domain model with business logic encapsulated in entities
- Aggregates enforce consistency boundaries
- Domain layer is independent of other layers and external concerns
- Business rules are expressed explicitly in the code

### 4. Infrastructure Layer

This layer provides implementations for external systems access, persistence, and technical services.

#### Key Components:

- **Repositories**: Implement data access for domain entities
- **Unit of Work**: Manages transactions and ensures consistency
- **External Service Clients**: Interact with external APIs and services
- **Caching**: Implements caching strategies
- **Messaging**: Handles message publishing and subscription
- **Persistence**: Implements database access and ORM configurations

#### Design Principles:

- Repositories abstract the persistence details from the domain
- Infrastructure implementations depend on interfaces defined in the domain
- Concerns like caching, logging, and messaging are implemented here
- Dependency injection to provide implementations to higher layers

## Cross-Cutting Concerns

### Authentication and Authorization

DeviceApi uses JWT (JSON Web Tokens) for authentication and a role-based authorization system:

- **Authentication Middleware**: Validates tokens and establishes identity
- **Authorization Policies**: Define permissions for various operations
- **Role Providers**: Map users to roles and permissions

### Caching Strategy

A multi-level caching approach is used to optimize performance:

- **In-Memory Cache**: For frequently accessed, short-lived data
- **Distributed Cache**: For shared data in a clustered environment
- **Entity Framework Second-Level Cache**: For database query results

### Logging and Monitoring

Comprehensive logging and monitoring are implemented using:

- **Structured Logging**: With Serilog for consistent log entries
- **Health Checks**: To verify system and dependency status
- **Performance Metrics**: Tracking key performance indicators
- **Distributed Tracing**: For tracking requests across services

### Error Handling

A centralized error handling strategy:

- **Exception Middleware**: Catches and processes exceptions
- **Problem Details Format**: Standardized error responses following RFC 7807
- **Error Codes**: Consistent error codes for client interpretation

## Data Flow

### Request Processing Flow

1. The client sends an HTTP request to the API
2. Middleware processes the request (authentication, logging)
3. The controller receives the request and validates inputs
4. The request is mapped to a command or query
5. The appropriate application service is invoked
6. The service orchestrates domain operations using repositories and domain services
7. Domain logic executes, possibly raising domain events
8. Results are mapped back to API models
9. The controller returns an appropriate HTTP response

### Data Persistence Flow

1. Domain operations create or modify entities
2. Application services commit changes via the Unit of Work
3. Repositories translate domain objects to data model
4. Entity Framework (or other ORM) generates appropriate SQL
5. Database changes are committed in a transaction
6. Domain events may trigger additional operations
7. Caches are updated or invalidated as needed

## Technology Stack

DeviceApi is built using the following technologies:

- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 8.0
- **Database**: Microsoft SQL Server (with possible migration paths to others)
- **Caching**: Memory Cache and Redis
- **Authentication**: JWT with refresh tokens
- **Validation**: FluentValidation
- **Object Mapping**: AutoMapper
- **Documentation**: Swagger/OpenAPI
- **Messaging**: Optional integration with Azure Service Bus or RabbitMQ
- **Testing**: xUnit, Moq, and FluentAssertions

## Deployment Architecture

DeviceApi supports multiple deployment models:

### Single-Server Deployment

For small to medium deployments, the application can run on a single server with:

- API application
- SQL Server database
- Redis cache

### Microservice-Based Deployment

For larger deployments, DeviceApi can be decomposed into microservices:

- **Identity Service**: Handles authentication and user management
- **Device Registry Service**: Manages device registration and metadata
- **Telemetry Service**: Processes and stores device data
- **Analytics Service**: Provides data analysis and reporting
- **Notification Service**: Manages alerts and notifications

### Cloud Deployment

DeviceApi is designed to work well in cloud environments with:

- Containerization with Docker
- Orchestration with Kubernetes
- Cloud-native services for storage, caching, and messaging
- Auto-scaling based on load
- Geographic distribution for global deployments

## Development Considerations

### Extendability

DeviceApi is designed for extension:

- **Plugin Architecture**: For custom device types and protocols
- **Custom Handlers**: For specialized data processing
- **API Extensions**: Well-defined points for adding new endpoints
- **Middleware Pipeline**: Extensible for custom processing

### Scalability

Several strategies are employed for scalability:

- **Stateless Design**: Allows horizontal scaling
- **Asynchronous Processing**: Non-blocking I/O throughout
- **Connection Pooling**: Efficient resource utilization
- **Backpressure Handling**: Prevents system overload
- **Data Partitioning**: For very large datasets

### Testability

The architecture facilitates comprehensive testing:

- **Unit Tests**: For domain logic and services
- **Integration Tests**: For repositories and external integrations
- **API Tests**: For endpoint validation
- **Performance Tests**: For load and stress testing
- **Mocking**: Interfaces throughout the system allow for mocking

## Database Seeding Architecture

DeviceApi uses an advanced database seeding mechanism. This mechanism enables automatic generation of test, development, and demo data when the application is first initialized or when the database is reset.

### Seeding Components

The seeding architecture consists of the following components:

1. **ISeeder Interface**: The basic contract implemented by all seeder classes
2. **DatabaseSeeder**: The main class that coordinates and runs all seeders
3. **Specific Seeder Classes**: Specialized seeders for each data model
4. **SeederOrder Enum**: Constants that determine the execution order of seeding operations
5. **SeederExtensions**: Helper methods that facilitate seeding operations

### Working Principles

The seeding architecture works according to the following principles:

1. **Sequential Execution**: Seeders run in a specific order to properly establish database relationships
2. **Idempotent Design**: Seeding operations can be repeated, the same data is not added multiple times
3. **Hierarchical Relationships**: Data is created from top to bottom according to the data model hierarchy (Station->Platform->Device)
4. **Performance Optimization**: Bulk data insertion and SQL-level IDENTITY_INSERT for fast data loading

### Architectural Flow

The seeding process architecture is designed as follows:

```
┌─────────────────────┐       ┌───────────────────┐
│                     │       │                   │
│  Program.cs         │       │  appsettings.json │
│  (Seed initiator)   │       │  (Configuration)  │
│                     │       │                   │
└──────────┬──────────┘       └─────────┬─────────┘
           │                            │
           │                            │
           ▼                            ▼
┌─────────────────────────────────────────────────┐
│                                                 │
│              DatabaseSeeder                     │
│         (Find and run seeders)                  │
│                                                 │
└───────────────────────┬─────────────────────────┘
                        │
                        │
        ┌───────────────┼───────────────┐
        │               │               │
        ▼               ▼               ▼
┌────────────────┐ ┌────────────┐ ┌────────────────┐
│                │ │            │ │                │
│ Base Seeders   │ │ Core Data  │ │ Relational     │
│ (Enums, Consts)│ │ (Station)  │ │ Data           │
│                │ │            │ │ (Device, Msg)  │
└────────────────┘ └────────────┘ └────────────────┘
```

### Integration Points

The seeding architecture integrates with the following components:

- **EF Core**: Seed data is added via EF Core context or directly with SQL
- **Logging**: Seeding operations are logged in detail using LogLibrary
- **Dependency Injection**: DatabaseSeeder is accessible via the DI system
- **Configuration**: Seeding behavior can be configured via appsettings.json

For more comprehensive information about the seeding architecture, see: [Database Seeding Process](07-Seeding-Process.md)

---

[◀ Data Models](05-Data-Models.md) | [Home](README.md) | [Next: Configuration ▶](08-Configuration.md) 