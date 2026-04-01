using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQ.Client;

namespace VehicleRentalSystem.Messaging.Configurations;

public static class RabbitMQConfig
{
    public static void AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMQSettings = configuration.GetSection("RabbitMqSettings").Get<RabbitMQSettings>()
            ?? throw new InvalidOperationException("RabbitMqSettings section is not configured in appsettings.json.");

        var factory = new ConnectionFactory()
        {
            HostName = rabbitMQSettings.HostName,
            Port = rabbitMQSettings.Port,
            UserName = rabbitMQSettings.UserName,
            Password = rabbitMQSettings.Password
        };

        services.TryAddSingleton(factory);
        services.TryAddSingleton<IConnection>(sp =>
        {
            var connFactory = sp.GetRequiredService<ConnectionFactory>();
            return connFactory.CreateConnectionAsync().GetAwaiter().GetResult();
        });
    }
}
