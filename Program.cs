using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using WikstromIT.Configuration;
using WikstromIT.Data;
using WikstromIT.Helpers;
using WikstromIT.Services;


var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Load configuration from appsettings.json and environment-specific files
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<PortfolioDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver
    {
        NamingStrategy = new SnakeCaseNamingStrategy()
    };
});
builder.Services.AddHttpClient<FastF1Service>();
builder.Services.AddHttpClient<DeepSeekService>();
builder.Services.AddHttpClient<HuggingFaceService>();

// ********* F1 Service *********
builder.Services.Configure<FastF1ApiOptions>(builder.Configuration.GetSection("FastF1Api"));
builder.Services.AddHttpClient<IFastF1Service, FastF1Service>();
// ******************************


var app = builder.Build();

NationalityMapper.Initialize(Path.Combine(app.Environment.ContentRootPath, "App_Data", "Countries.json"));


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
