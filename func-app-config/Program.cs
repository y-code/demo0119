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
            .Select(KeyFilter.Any, "dev")
            .ConfigureRefresh(opt => opt
                // .RegisterAll()
                .Register(key: "database2:host", label: "dev", refreshAll: true)
                .SetRefreshInterval(TimeSpan.FromSeconds(30)))
            .UseFeatureFlags(opt => opt.SetRefreshInterval(TimeSpan.FromSeconds(30)));
    });

builder.Services.AddFeatureManagement();
builder.Services.AddAzureAppConfiguration();
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.UseAzureAppConfiguration();

builder.Build().Run();
