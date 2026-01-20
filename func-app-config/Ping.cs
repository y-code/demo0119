using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;

namespace Company.Function;

public class Ping
{
    private readonly ILogger _logger;
    private readonly IConfiguration _config;
    private readonly IFeatureManager _featureManager;

    public Ping(
      IConfiguration config,
      // REMAARK: Stop manually refreshing configuration in controller action methods
      //          Instead, use the built-in middlewaare, which does it in the pipeline
      //          ahead of reaching the function code.
      // IConfigurationRefresherProvider refresherProvider,
      IFeatureManagerSnapshot featureManager,
      ILogger<Ping> logger)
    {
        _config = config;
        _featureManager = featureManager;
        _logger = logger;
    }

    [Function("Ping")]
    public async Task<IActionResult> Run([
        HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req,
        CancellationToken cancel)
    {
        const string NO_VALUE = "<no configuration>";
        const string key1 = "database2:host";
        const string key2 = "database2:username";
        const string key3 = "database2:password";
        const string key4 = "database2:test";

        var featureEnabled = await _featureManager.IsEnabledAsync("feature-a");

        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult(
            $"key [{key1}] - Value [{_config[key1] ?? NO_VALUE}]\n" +
            $"key [{key2}] - Value [{_config[key2] ?? NO_VALUE}]\n" +
            $"key [{key3}] - Value [{_config[key3] ?? NO_VALUE}]\n" +
            $"key [{key4}] - Value [{_config[key4] ?? NO_VALUE}]\n" +
            $"Feature 'feature-a' enabled: {featureEnabled}");

    }
}
