using System.Reflection;
using System.Text.Json.Serialization;
using E_LaptopShop;                           // InventoryHub
using E_LaptopShop.Application;
using E_LaptopShop.Application.Common;
using E_LaptopShop.Application.Models;
using E_LaptopShop.Application.Services;
using E_LaptopShop.Application.Services.Implementations;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Helpers;
using E_LaptopShop.Infra;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// NOTE: CreateBuilder đã tự nạp:
//   1) appsettings.json
//   2) appsettings.{Environment}.json
//   3) UserSecrets (Development only)
//   4) Environment Variables
//   5) Command-line args
// Chỉ thêm các source NGOÀI defaults — vd file optional appsettings.Local.json:
builder.Configuration
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
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
        Description = "Nhập JWT token (KHÔNG cần gõ 'Bearer '). Swagger tự thêm prefix. Ví dụ: eyJhbGci..."
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

    // XML documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Add layer dependency
builder.Services.AddApplicationServices();  // Application layer first
builder.Services.AddInfraServices(builder.Configuration);  // Infrastructure layer second

// ── SignalR — real-time inventory updates ─────────────────────────────
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval   = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
});

// Adapter cho IInventoryNotifier — Application layer dùng abstraction này
// để push realtime mà không phải reference SignalR/Web project.
builder.Services.AddScoped<E_LaptopShop.Application.Services.Interfaces.IInventoryNotifier,
                            E_LaptopShop.Hubs.SignalRInventoryNotifier>();

// ── Bulk Job Background Processing ────────────────────────────────────
// Singleton vì Channel và Registry cần share state xuyên suốt app lifetime
builder.Services.AddSingleton<E_LaptopShop.Application.Jobs.IBulkJobQueue,
                               E_LaptopShop.Application.Jobs.BulkJobQueue>();
builder.Services.AddSingleton<E_LaptopShop.Application.Jobs.BulkJobRegistry>();
// BackgroundService (Hosted Service) — tự chạy khi app start
builder.Services.AddHostedService<E_LaptopShop.Application.Jobs.BulkJobProcessor>();

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

            string message = context.AuthenticateFailure is not null
                ? "Token không hợp lệ hoặc đã hết hạn"
                : "Chưa xác thực — vui lòng đăng nhập";

            var result = System.Text.Json.JsonSerializer.Serialize(new { message });
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
    // Policy dùng cho REST API thông thường (không cần credentials)
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

    // Policy dùng cho SignalR WebSocket — PHẢI có AllowCredentials + specific origins
    // AllowAnyOrigin() không tương thích với AllowCredentials()
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins(
                "http://be-shopminhnhat.click",
                "https://be-shopminhnhat.click",
                "http://localhost:5173",    // dev FE
                "https://localhost:5173"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());           // bắt buộc cho SignalR WebSocket
});
//big file upload
builder.Services.Configure<FormOptions>(options =>
{
    // 50 MB trong bytes
    options.MultipartBodyLengthLimit = 52428800;
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});
// Auto-create required directories (deploy friendy — no manual setup needed)
var wwwrootDirectory = Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
if (!Directory.Exists(wwwrootDirectory))
    Directory.CreateDirectory(wwwrootDirectory);

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
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-LaptopShop v1");
    c.RoutePrefix = string.Empty;
});

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
// AllowFrontend áp dụng cho toàn app (bao gồm SignalR /hubs/*)
// AllowAll vẫn fallback nhưng không dùng AllowCredentials nên không ảnh hưởng REST API
app.UseCors("AllowFrontend");

// ✨ JWT Middleware Pipeline - ORDER MATTERS!
app.UseAuthentication();  // 🔐 Must come before UseAuthorization
app.UseAuthorization();

app.MapControllers();

// ── SignalR Hub endpoint ──────────────────────────────────────────────
// URL: ws://host/hubs/inventory
// FE kết nối: new HubConnectionBuilder().withUrl("/hubs/inventory")
app.MapHub<InventoryHub>("/hubs/inventory");

// Health endpoint — Quan trọng cho IIS / Azure / k8s health check
app.MapGet("/", () => Results.Ok(new
{
    name = "E-LaptopShop API",
    status = "running",
    environment = app.Environment.EnvironmentName,
    time = DateTime.UtcNow
}));

app.MapGet("/health", () => Results.Ok("Healthy"));

// Seed database — bọc try/catch để không crash startup khi DB lỗi
try
{
    await E_LaptopShop.Infra.Data.DbInitializer.SeedAsync(app.Services);
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILoggerFactory>()
                              .CreateLogger("Startup");
    logger.LogError(ex, "Database seed failed — app will continue running");
}

app.Run();
