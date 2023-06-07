using HealthCheckExample;
using HealthCheckExample.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

/*
 * Playground for Health Checks
 * To run this example, you need to:
 * - Either set RabbitMQ:Uri in the appsettings.json file or run: docker run -d --hostname my-rabbit --name some-rabbit -p 5672:5672 rabbitmq:3-management
 *
 * The RabbitMQ check relies on a 3rd party library, the SomeApi check is a custom implementation.
 * Wrote a small publisher to just serialize and write to a file.
 */

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) => {
        var handler = new FakeHttpClientHandler();
        services.AddHttpClient<SomeApiHealthCheck>()
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        #region Add health checks

        services.AddHealthChecks()
            .AddCheck<SomeApiHealthCheck>("some_api_health_check")
            .AddRabbitMQ(context.Configuration.GetValue<Uri>("RabbitMQ:Uri")!);

        services.AddSingleton<IHealthCheckPublisher, HealthCheckFilePublisher>();

        services.Configure<HealthCheckPublisherOptions>(options => {
            options.Delay = TimeSpan.FromSeconds(5);
            options.Period = TimeSpan.FromMinutes(1);
        });

        #endregion
    })
    .Build();

host.Run();
