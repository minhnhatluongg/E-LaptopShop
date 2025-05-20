using System.Text.Json.Serialization;
using E_LaptopShop.Application;
using E_LaptopShop.Application.Common;
using E_LaptopShop.Helpers;
using E_LaptopShop.Infra;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add layer dependency
builder.Services.AddApplicationServices();  // Application layer first
builder.Services.AddInfraServices(builder.Configuration);  // Infrastructure layer second

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

app.UseAuthorization();

app.MapControllers();

app.Run();
