using Microsoft.Extensions.DependencyInjection.Extensions;
using VehicleRentalSystem.Core.Interfaces.Messaging;
using VehicleRentalSystem.Messaging.Consumers;
using VehicleRentalSystem.Messaging.Services;

namespace VehicleRentalSystem.Messaging.Configurations;

public static class DependencyInjectionConfig
{
    public static void AddMessagingDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton<IMessageProducer, MessageProducer>();

        services.Scan(scan => scan
            .FromAssemblyOf<VehicleRegisteredConsumer>()
            .AddClasses(classes => classes.AssignableTo<IMessageConsumer>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
            .AddClasses(classes => classes.AssignableTo<IHostedService>())
                .AsSelf()
                .WithSingletonLifetime()
        );

        services.AddRabbitMQ(configuration);
    }
}
