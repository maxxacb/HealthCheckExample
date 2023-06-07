using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;

namespace HealthCheckExample.HealthChecks;

/// <inheritdoc />
public sealed class HealthCheckFilePublisher : IHealthCheckPublisher
{
    /// <inheritdoc />
    public async Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        var json = JsonConvert.SerializeObject(report, Formatting.Indented, new Newtonsoft.Json.Converters.StringEnumConverter());

        await File.WriteAllTextAsync($"health_{DateTimeOffset.Now:HHmmss}.json", json, cancellationToken);
    }
}
