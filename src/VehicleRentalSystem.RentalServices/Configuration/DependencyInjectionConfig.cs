using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VehicleRentalSystem.Core.Interfaces.Identity;
using VehicleRentalSystem.Core.Interfaces.Messaging;
using VehicleRentalSystem.Core.Interfaces.Notifications;
using VehicleRentalSystem.Core.Interfaces.Repositories;
using VehicleRentalSystem.Core.Interfaces.Services;
using VehicleRentalSystem.Core.Interfaces.UoW;
using VehicleRentalSystem.Core.Notifications;
using VehicleRentalSystem.Identity.Extensions;
using VehicleRentalSystem.Identity.Interfaces;
using VehicleRentalSystem.Identity.Services;
using VehicleRentalSystem.Infrastructure.Repositories;
using VehicleRentalSystem.Infrastructure.UoW;
using VehicleRentalSystem.Messaging.Configurations;
using VehicleRentalSystem.Messaging.Services;
using VehicleRentalSystem.RentalServices.Services;
using VehicleRentalSystem.Shared.Services;

namespace VehicleRentalSystem.RentalServices.Configuration;

public static class DependencyInjectionConfig
{
    public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        RegisterManualServices(services);

        services.Scan(scan => scan
            .FromAssemblies(
                typeof(INotifier).Assembly,
                typeof(IRepository<>).Assembly,
                typeof(BaseService).Assembly,
                typeof(IAspNetUser).Assembly,
                typeof(IMessageProducer).Assembly,
                typeof(IMessageConsumer).Assembly,
                typeof(IValidator<>).Assembly
            )
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service") ||
                                                         type.Name.EndsWith("Repository") ||
                                                         type.Name.EndsWith("UnitOfWork") ||
                                                         type.Name.EndsWith("Validation")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Producer") ||
                                                         type.Name.EndsWith("Consumer") ||
                                                         typeof(IHostedService).IsAssignableFrom(type)))
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
        );

        AddMessagingServices(services, configuration);

        services.AddTransient(provider =>
        {
            var message = "Default message";
            return new Notification(message, NotificationType.Information);
        });
    }

    private static void RegisterManualServices(IServiceCollection services)
    {
        services.TryAddScoped<INotifier, Notifier>();
        services.TryAddScoped<IUnitOfWork, UnitOfWork>();
        services.TryAddScoped<IVehicleRepository, VehicleRepository>();
        services.TryAddScoped<ICourierRepository, CourierRepository>();
        services.TryAddScoped<IRentalRepository, RentalRepository>();
        services.TryAddScoped<IVehicleNotificationRepository, VehicleNotificationRepository>();
        services.TryAddScoped<IAspNetUser, AspNetUser>();
        services.TryAddScoped<IAuthService, AuthService>();
        services.TryAddScoped<RoleManager<IdentityRole>>();
        services.AddAuthorization();
        services.TryAddSingleton<IRedisCacheService, RedisCacheService>();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.TryAddSingleton<IMessageProducer, MessageProducer>();
    }

    private static void AddMessagingServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddRabbitMQ(configuration);
    }
}
