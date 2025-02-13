using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PjSip.App.Data;
using PjSip.App.Services;
using PjSip.App.Utils;

var builder = WebApplication.CreateBuilder(args);
var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Program>();
logger.LogInformation("Initializing ThreadSafeEndpoint");
ThreadSafeEndpoint.Initialize(logger);
// Add services to the container
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
// Configure logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

// Configure database
builder.Services.AddDbContext<SipDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddSingleton<SipManager>();
builder.Services.AddScoped<SipManagerService>();
builder.Services.AddScoped<AgentManager>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PjSip API V1");
        c.RoutePrefix = string.Empty;
    });
}

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SipDbContext>();
    dbContext.Database.EnsureCreated();
    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Global exception handler
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (error != null)
        {
            var ex = error.Error;
            await context.Response.WriteAsJsonAsync(new 
            { 
                error = "An unexpected error occurred",
                detail = app.Environment.IsDevelopment() ? ex.Message : null
            });
        }
    });
});

app.Run();
