using Microsoft.EntityFrameworkCore;
using test.Core;
using test.DTOs;
using test.Entities;
using test.Repositories;
using test.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

// Register Services
builder.Services.AddScoped<IService<Device, DeviceDto, CreateDeviceDto, UpdateDeviceDto>, DeviceService>();
builder.Services.AddScoped<IService<ApnName, ApnNameDto, CreateApnNameDto, UpdateApnNameDto>, SettingService>();
builder.Services.AddScoped<IService<ApnPassword, ApnPasswordDto, CreateApnPasswordDto, UpdateApnPasswordDto>, ApnPasswordService>();
builder.Services.AddScoped<IService<ApnAddress, ApnAddressDto, CreateApnAddressDto, UpdateApnAddressDto>, ApnAddressService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureDeleted(); // Drop existing database
    context.Database.EnsureCreated(); // Create new database with seed data
}

app.Run();
