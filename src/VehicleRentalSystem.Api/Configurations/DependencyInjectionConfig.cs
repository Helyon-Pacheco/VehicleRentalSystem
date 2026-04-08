using VehicleRentalSystem.Core.Notifications;
using VehicleRentalSystem.Identity.Configurations;
using VehicleRentalSystem.Messaging.Configurations;
using VehicleRentalSystem.RentalServices.Configuration;

namespace VehicleRentalSystem.Api.Configurations;

public static class DependencyInjectionConfig
{
    public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRentalServicesDependencies(configuration);
        services.AddIdentityServicesDependencies();
        services.AddMessagingDependencies(configuration);

        services.AddTransient(_ => new Notification("Default message", NotificationType.Information));
    }
}
