using Business.Interfaces;
using Business.Services;
using Core.Security;
using Core.Utilities;
using Data.Context;
using Data.Interfaces;
using Data.Repositories;
using Entities.Concrete;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı bağlantısını ekle
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT doğrulama yapılandırması
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        ClockSkew = TimeSpan.Zero
    };
});

// Servisleri kaydet
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ITwoFactorService, TwoFactorService>();

// Controller'lara validasyon filtresi ve API yanıt formatını özelleştirme
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
})
.ConfigureApiBehaviorOptions(options =>
{
    // Özel model validasyon hatası yanıtı
    options.InvalidModelStateResponseFactory = context =>
    {
        // Hata sözlüğü oluşturma
        var errors = context.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
            );

        // Standart hata yanıtı oluşturma
        var response = ApiResponse<object>.Error(
            errors, 
            "Lütfen form alanlarını kontrol ediniz", 
            StatusCodes.Status400BadRequest
        );

        return new BadRequestObjectResult(response);
    };
})
.AddJsonOptions(options =>
{
    // JSON yanıtlarda null değerli özellikleri gizle
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    // Enum değerlerini string olarak dön
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Swagger yapılandırması
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Deneme API", Version = "v1" });
    
    // JWT doğrulaması için Swagger yapılandırması
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header kullanarak kimlik doğrulama. Örnek: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// HTTPS yönlendirmesini zorlamak için
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 5001;
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Veritabanını her başlangıçta sıfırla
if (builder.Configuration.GetValue<bool>("DatabaseSettings:ResetDatabaseOnStartup"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        // Başlangıç kullanıcılarını ekle
        SeedInitialUsers(dbContext, builder.Configuration);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Başlangıç kullanıcılarını ekleme metodu
void SeedInitialUsers(AppDbContext dbContext, IConfiguration configuration)
{
    // Önce rollerin eklendiğinden emin olalım
    if (!dbContext.UserRoles.Any())
    {
        dbContext.UserRoles.AddRange(
            new UserRole { Id = 1, Name = "User" },
            new UserRole { Id = 2, Name = "Developer" },
            new UserRole { Id = 3, Name = "Admin" }
        );
        dbContext.SaveChanges(); // Rollerin önce kaydedilmesi önemli
    }
    
    var initialUsers = configuration.GetSection("InitialUsers").Get<List<InitialUser>>();
    
    if (initialUsers != null)
    {
        foreach (var initialUser in initialUsers)
        {
            // Kullanıcının zaten var olup olmadığını kontrol et
            if (dbContext.Users.Any(u => u.Username == initialUser.Username || u.Email == initialUser.Email))
                continue;
                
            string salt = PasswordHelper.CreateSalt();
            string passwordHash = PasswordHelper.HashPassword(initialUser.Password, salt);

            // Rol ID'sini belirle
            int roleId;
            switch (initialUser.Role.ToLower())
            {
                case "admin":
                    roleId = 3;
                    break;
                case "developer":
                    roleId = 2;
                    break;
                default:
                    roleId = 1; // User
                    break;
            }

            // Rolün var olduğundan emin ol
            if (!dbContext.UserRoles.Any(r => r.Id == roleId))
            {
                throw new Exception($"Role ID {roleId} bulunamadı. Kullanıcı oluşturulamıyor: {initialUser.Username}");
            }

            var user = new User
            {
                Username = initialUser.Username,
                Email = initialUser.Email,
                PasswordHash = passwordHash,
                PasswordSalt = salt,
                RoleId = roleId,
                CreatedDate = DateTime.UtcNow
            };

            dbContext.Users.Add(user);
        }
        
        dbContext.SaveChanges();
    }
}

// Başlangıç kullanıcıları için yardımcı sınıf
class InitialUser
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}
