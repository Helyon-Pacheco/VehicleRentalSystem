using Microsoft.Extensions.DependencyInjection.Extensions;
using VehicleRentalSystem.Core.Interfaces.Messaging;
using VehicleRentalSystem.Core.Interfaces.Notifications;
using VehicleRentalSystem.Core.Interfaces.Repositories;
using VehicleRentalSystem.Core.Interfaces.Services;
using VehicleRentalSystem.Core.Interfaces.UoW;
using VehicleRentalSystem.Core.Notifications;
using VehicleRentalSystem.Infrastructure.Repositories;
using VehicleRentalSystem.Infrastructure.UoW;
using VehicleRentalSystem.Messaging.Configurations;
using VehicleRentalSystem.Messaging.Services;
using VehicleRentalSystem.RentalServices.Services;

namespace VehicleRentalSystem.RentalServices.Configuration;

public static class DependencyInjectionConfig
{
    public static void AddRentalServicesDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddScoped<INotifier, Notifier>();
        services.TryAddScoped<IUnitOfWork, UnitOfWork>();
        services.TryAddSingleton<IRedisCacheService, RedisCacheService>();
        services.TryAddSingleton<IMessageProducer, MessageProducer>();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.Scan(scan => scan
            .FromAssemblyOf<RentalService>()
            .AddClasses(classes => classes.AssignableTo(typeof(IRepository<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<IRentalService>())
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<IVehicleService>())
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<ICourierService>())
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        services.AddRabbitMQ(configuration);
    }
}
