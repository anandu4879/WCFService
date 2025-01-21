using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using CoreWCF.Channels;
using WCFService;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Add services to the container
var connectionString = "server=localhost;port=3306;user=root;password=Andu@123;database=db";
builder.Services.AddDbContext<DBContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// Add CoreWCF services
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

// Register Service and IService
builder.Services.AddTransient<Service>();
builder.Services.AddTransient<IService, Service>();

// Configure specific URL if needed - Changed port to 5001
builder.WebHost.UseUrls("http://localhost:5001");

var app = builder.Build();

// Configure CoreWCF endpoints
app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<Service>();
    
    var binding = new BasicHttpBinding();
    binding.Security.Mode = BasicHttpSecurityMode.None;
    binding.MaxReceivedMessageSize = 1024 * 1024; // 1MB
    binding.OpenTimeout = TimeSpan.FromMinutes(1);
    binding.CloseTimeout = TimeSpan.FromMinutes(1);
    binding.SendTimeout = TimeSpan.FromMinutes(10);
    binding.ReceiveTimeout = TimeSpan.FromMinutes(10);

    serviceBuilder.AddServiceEndpoint<Service, IService>(
        binding, 
        "/Service.svc"
    );
    
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
});

// Add error handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An unhandled exception occurred");
        throw;
    }
});

app.Run(); 