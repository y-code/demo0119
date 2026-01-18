using System.Security.Policy;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Configuration
    .AddAzureAppConfiguration(options => {
        options
            .Connect(
                new Uri(builder.Configuration["AppConfigEndpoint"]
                    ?? throw new InvalidOperationException("AppConfigEndpoint is not set in configuration.")),
                new DefaultAzureCredential())
            .ConfigureRefresh(opt => opt.RegisterAll().SetRefreshInterval(TimeSpan.FromSeconds(30)))
            .UseFeatureFlags(opt => opt.SetRefreshInterval(TimeSpan.FromSeconds(30)))
            .Select(KeyFilter.Any, "dev");

        builder.Services.AddSingleton(options.GetRefresher());
    });
builder.Services
    .AddFeatureManagement();
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
