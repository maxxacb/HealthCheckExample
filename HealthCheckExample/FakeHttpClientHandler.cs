using System.Net;

namespace HealthCheckExample;

public sealed class FakeHttpClientHandler : DelegatingHandler
{
    private int _requestCount;

    public FakeHttpClientHandler()
    {
        _requestCount = 0;
    }

    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = _requestCount++;

        var reset = () => {
            _requestCount = 0;
            return new HttpResponseMessage(HttpStatusCode.OK);
        };

        return response switch {
            0 => throw new TimeoutException("The request timed out"),
            1 => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized)),
            2 => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)),
            _ => Task.FromResult(reset.Invoke()),
        };
    }
}
