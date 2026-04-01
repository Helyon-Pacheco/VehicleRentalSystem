using Azure.Storage.Blobs;
using HealthChecks.Azure.Storage.Blobs;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace VehicleRentalSystem.Api.Configurations;

public static class HealthCheckConfig
{
    public static IServiceCollection AddHealthCheckConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();
        var hcUIBuilder = services.AddHealthChecksUI();

        var redisHost = configuration["RedisSettings:Host"]
            ?? throw new InvalidOperationException("RedisSettings:Host is not configured.");
        var redisPort = configuration["RedisSettings:Port"]
            ?? throw new InvalidOperationException("RedisSettings:Port is not configured.");
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
        var azureBlobConnectionString = Environment.GetEnvironmentVariable("AZURE_CONNECTION_STRING")
            ?? throw new InvalidOperationException("AZURE_CONNECTION_STRING environment variable is not configured.");
        var azureBlobContainerName = configuration["AzureBlobStorageSettings:ContainerName"]
            ?? throw new InvalidOperationException("AzureBlobStorageSettings:ContainerName is not configured.");

        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

        var redisConfigurationOptions = new ConfigurationOptions
        {
            EndPoints = { $"{redisHost}:{redisPort}" },
            AbortOnConnectFail = false
        };

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect(redisConfigurationOptions);
        });

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

        services.AddSingleton(sp =>
        {
            return new BlobServiceClient(azureBlobConnectionString);
        });

        hcBuilder
            .AddNpgSql(postgreConnectionString)
            .AddRedis($"{redisHost}:{redisPort}", name: "redis")
            .AddRabbitMQ(name: "rabbitmq")
            .AddAzureBlobStorage(
                clientFactory: sp => sp.GetRequiredService<BlobServiceClient>(),
                optionsFactory: sp => new AzureBlobStorageHealthCheckOptions
                {
                    ContainerName = azureBlobContainerName
                },
                name: "azure_blob_storage"
            );

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
