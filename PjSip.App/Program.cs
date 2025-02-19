using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PjSip.App.Data;
using PjSip.App.Models;
using PjSip.App.Services;
using PjSip.App.Utils;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .SetFileLoadExceptionHandler(c => { /* Handle load errors */ });

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PjSip API",
        Version = "v1",
        Description = "API for managing SIP calls and accounts"
    });
});

// Database
builder.Services.AddDbContext<SipDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
    .AddConsole()
    .AddDebug());
var tlogger = loggerFactory.CreateLogger<ThreadSafeEndpoint>();
ThreadSafeEndpoint.Initialize(tlogger);


// Application Services
builder.Services.AddScoped<SipManagerService>();
builder.Services.AddSingleton<SipManager>();
builder.Services.AddScoped<AgentConfigService>();
var app = builder.Build();

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PjSip API v1"));
}

// Database Migration
await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SipDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Applying database migrations...");
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("Database migrations completed");
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Failed to apply database migrations");
        throw;
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "dist")),
    RequestPath = ""
});

app.MapFallbackToFile("index.html", new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "dist"))
});

// Global Error Handling
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (error?.Error != null)
        {
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                Error = "An unexpected error occurred",
                Details = app.Environment.IsDevelopment() ? error.Error.Message : null
            }));
        }
    });
});

app.Run();