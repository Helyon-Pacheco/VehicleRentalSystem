using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace VehicleRentalSystem.Messaging.Configurations;

public static class HealthCheckConfig
{
    public static IServiceCollection AddHealthCheckConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();
        var hcUIBuilder = services.AddHealthChecksUI();

        var postgreConnectionString = configuration.GetSection("DatabaseSettings:DefaultConnection").Value
            ?? throw new InvalidOperationException("DatabaseSettings:DefaultConnection is not configured.");
        var rabbitMqHostName = configuration["RabbitMqSettings:HostName"]
            ?? throw new InvalidOperationException("RabbitMqSettings:HostName is not configured.");
        var rabbitMqPort = configuration["RabbitMqSettings:Port"]
            ?? throw new InvalidOperationException("RabbitMqSettings:Port is not configured.");
        var rabbitMqUserName = configuration["RabbitMqSettings:UserName"]
            ?? throw new InvalidOperationException("RabbitMqSettings:UserName is not configured.");
        var rabbitMqPassword = configuration["RabbitMqSettings:Password"]
            ?? throw new InvalidOperationException("RabbitMqSettings:Password is not configured.");
        
        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

        var connectionLazy = new Lazy<Task<IConnection>>(async () =>
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri($"amqp://{rabbitMqUserName}:{rabbitMqPassword}@{rabbitMqHostName}:{rabbitMqPort}"),
            };

            return await factory.CreateConnectionAsync();
        });

        services.AddSingleton(sp =>
        {
            return connectionLazy.Value.GetAwaiter().GetResult();
        });

        hcBuilder
            .AddNpgSql(postgreConnectionString)
            .AddRabbitMQ(name: "rabbitmq");

        hcUIBuilder
            .AddInMemoryStorage();

        return services;
    }

    public static IApplicationBuilder UseHealthCheckConfiguration(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            endpoints.MapHealthChecksUI(options =>
            {
                options.UIPath = "/health-ui";
                options.ApiPath = "/health-ui-api";
            });
        });

        return app;
    }
}
