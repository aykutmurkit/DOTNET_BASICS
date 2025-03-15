using Microsoft.EntityFrameworkCore;
using test.Core;
using test.DTOs;
using test.Entities;
using test.Extensions;
using test.Middleware;
using test.Repositories;
using test.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddSwaggerServices();

// Add database, repository and application services
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddRepositoryServices();
builder.Services.AddApplicationServices();

// Add validation services for both architectures
builder.Services.AddGenericValidationServices();
builder.Services.AddNonGenericValidationServices();

// Add rate limiting services
builder.Services.AddRateLimitingServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwaggerServices();

app.UseHttpsRedirection();

// Add validation middleware
app.UseValidationMiddleware();

// Add rate limiting middleware
app.UseRateLimiting();

app.UseAuthorization();

app.MapControllers();

// Ensure database is created and seeded
app.EnsureDatabaseCreated(recreateDatabase: true);

app.Run();
