using System.Net;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheckExample.HealthChecks;

public sealed class SomeApiHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;

    public SomeApiHealthCheck(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        try
        {
            var response = await _httpClient.GetAsync("https://localhost:8080/some-endpoint", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("Access to business critical emojis is A-OK");
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return HealthCheckResult.Unhealthy("Cannot authenticate to SomeAPI");
            }

            return HealthCheckResult.Degraded("Cannot connect to SomeAPI", data: new Dictionary<string, object> {
                ["Timestamp"] = DateTimeOffset.UtcNow,
                ["StatusCode"] = response.StatusCode,
                ["ResponseContents"] = await response.Content.ReadAsStringAsync(cancellationToken),
            });
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("Something went catastrophically wrong with SomeAPI", exception);
        }
    }
}
