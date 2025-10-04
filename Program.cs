using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using AvikstromPortfolio.Configuration;
using AvikstromPortfolio.Data;
using Serilog;
using AvikstromPortfolio.Hubs;
using AvikstromPortfolio.Services.FlightBoard;
using AvikstromPortfolio.Services.F1;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddAzureWebAppDiagnostics(); // For console logging in Azure
builder.Host.UseSerilog(); // For file logging
Log.Warning("App startup: Serilog initialized at {UtcNow}", DateTime.UtcNow);
Log.Information("Logging to: {Path}", configuration["Serilog:WriteTo:1:Args:path"]);

// Load configuration from appsettings.json and environment-specific files
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<PortfolioDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null
        )
    );

    if (builder.Environment.IsDevelopment())
    {
        options.LogTo(Console.WriteLine, [
            DbLoggerCategory.Database.Command.Name,
            DbLoggerCategory.Infrastructure.Name
        ], LogLevel.Information)
        .EnableSensitiveDataLogging();
    }
});


// Add services to the container. For Newtonsoft.Json
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver
    {
        NamingStrategy = new SnakeCaseNamingStrategy()
    };
});

builder.Services.Configure<ExternalScriptsOptions>(builder.Configuration.GetSection("ExternalScripts"));

// ********* F1 Service *********
builder.Services.Configure<F1InfoApiOptions>(builder.Configuration.GetSection("F1InfoApi"));
builder.Services.AddHttpClient<IF1InfoService, F1InfoService>();
builder.Services.AddSingleton<IF1NationalityMapper, F1NationalityMapper>();
// ******************************

// ********* Flight Service *********
builder.Services.AddSignalR();
builder.Services.Configure<FlightInfoApiOptions>(builder.Configuration.GetSection("FlightInfoApiOptions"));
builder.Services.Configure<FlightBoardPollingOptions>(builder.Configuration.GetSection("FlightBoardPolling"));
builder.Services.AddHttpClient<IFlightInfoService, FlightInfoService>();
builder.Services.AddScoped<IFlightBoardBroadcaster, FlightBoardBroadcaster>();
builder.Services.AddScoped<IFlightBoardPartialRenderer, FlightBoardPartialRenderer>();
builder.Services.AddSingleton<IFlightBoardPollingActivator, FlightBoardPollingActivator>();
// ******************************

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Shows full error details in development mode
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseStatusCodePagesWithRedirects("/Home/Error/{0}");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<FlightBoardHub>("/flightboardhub");

app.Run();
