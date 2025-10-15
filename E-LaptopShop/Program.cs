using System.Text.Json.Serialization;
using E_LaptopShop.Application;
using E_LaptopShop.Application.Common;
using E_LaptopShop.Application.Models;
using E_LaptopShop.Application.Services;
using E_LaptopShop.Helpers;
using E_LaptopShop.Infra;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

    var env = builder.Environment;
    builder.Configuration
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)                      // file thật local (không commit)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true) // Development/Production (không commit)
        .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);               // tuỳ dev

    if (env.IsDevelopment())
        builder.Configuration.AddUserSecrets<Program>(optional: true);  // secrets dev

    builder.Configuration.AddEnvironmentVariables();
// Add Swagger with JWT Support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "E-LaptopShop API", Version = "v1" });
    
    // JWT Authorization in Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below."
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // File upload support
    c.OperationFilter<FileUploadOperationFilter>();
});

// Add layer dependency
builder.Services.AddApplicationServices();  // Application layer first
builder.Services.AddInfraServices(builder.Configuration);  // Infrastructure layer second

// ✨ JWT Configuration - 2025 Best Practices
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.AddScoped<IJwtService, JwtService>();

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
if (jwtSettings == null)
{
    throw new InvalidOperationException("JWT Settings not found in configuration");
}

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = jwtSettings.SaveToken;
    options.RequireHttpsMetadata = jwtSettings.RequireHttpsMetadata;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ValidateIssuer = jwtSettings.ValidateIssuer,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = jwtSettings.ValidateAudience,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = jwtSettings.ValidateLifetime,
        ClockSkew = TimeSpan.FromMinutes(jwtSettings.ClockSkewMinutes),
        RequireExpirationTime = true
    };

    // 2025 Enhancement: Enhanced JWT Events
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new { message = "You are not authorized" });
            return context.Response.WriteAsync(result);
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new { message = "You do not have permission" });
            return context.Response.WriteAsync(result);
        }
    };
});

// Authorization
builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});
//big file upload
builder.Services.Configure<FormOptions>(options =>
{
    // 50 MB trong bytes
    options.MultipartBodyLengthLimit = 52428800;
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});
var uploadsDirectory = Path.Combine(builder.Environment.ContentRootPath, "uploads");
if (!Directory.Exists(uploadsDirectory))
{
    Directory.CreateDirectory(uploadsDirectory);
}
var imageDirectory = Path.Combine(uploadsDirectory, "image");
if (!Directory.Exists(imageDirectory))
{
    Directory.CreateDirectory(imageDirectory);
}

var videoDirectory = Path.Combine(uploadsDirectory, "video");
if (!Directory.Exists(videoDirectory))
{
    Directory.CreateDirectory(videoDirectory);
}

var documentDirectory = Path.Combine(uploadsDirectory, "document");
if (!Directory.Exists(documentDirectory))
{
    Directory.CreateDirectory(documentDirectory);
}
var otherDirectory = Path.Combine(uploadsDirectory, "other");
if (!Directory.Exists(otherDirectory))
{
    Directory.CreateDirectory(otherDirectory);
}
var tempUploadsDirectory = Path.Combine(builder.Environment.ContentRootPath, "temp-uploads");
if (!Directory.Exists(tempUploadsDirectory))
{
    Directory.CreateDirectory(tempUploadsDirectory);
}
var app = builder.Build();

    
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-LaptopShop v1");
        c.RoutePrefix = string.Empty;
    });
}

// Add global exception handler
app.UseGlobalExceptionHandler();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "uploads")),
    RequestPath = "/uploads"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "uploads", "image")),
    RequestPath = "/image"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "uploads", "video")),
    RequestPath = "/video"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "uploads", "document")),
    RequestPath = "/document"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "uploads", "other")),
    RequestPath = "/other"
});
app.UseHttpsRedirection();

app.UseCors("AllowAll");

// ✨ JWT Middleware Pipeline - ORDER MATTERS!
app.UseAuthentication();  // 🔐 Must come before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
