using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Trailarr.Core;
using Trailarr.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add configuration
builder.Services.Configure<MovieLibraryOptions>(builder.Configuration.GetSection("MovieLibrary"));

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ??
                new[] { "http://localhost:3000" }
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add Trailarr services
builder.Services.AddTrailarrCore(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.Logger.LogInformation("Running in Development mode");
}

app.UseCors();
app.UseRouting();

app.Logger.LogInformation("Starting Trailarr API on {Url}", "http://localhost:5000");

// Map endpoints
app.MapHealthChecks("/health");
app.MapControllers();

await app.RunAsync();
