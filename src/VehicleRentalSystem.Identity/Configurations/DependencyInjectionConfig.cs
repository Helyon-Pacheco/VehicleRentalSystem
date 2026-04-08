using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VehicleRentalSystem.Core.Interfaces.Identity;
using VehicleRentalSystem.Core.Interfaces.Notifications;
using VehicleRentalSystem.Core.Notifications;
using VehicleRentalSystem.Identity.Extensions;
using VehicleRentalSystem.Identity.Interfaces;
using VehicleRentalSystem.Identity.Services;

namespace VehicleRentalSystem.Identity.Configurations;

public static class DependencyInjectionConfig
{
    public static void AddIdentityServicesDependencies(this IServiceCollection services)
    {
        services.TryAddScoped<INotifier, Notifier>();
        services.TryAddScoped<IAspNetUser, AspNetUser>();
        services.TryAddScoped<RoleManager<IdentityRole>>();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddAuthorization();

        services.Scan(scan => scan
            .FromAssemblyOf<AuthService>()
            .AddClasses(classes => classes.AssignableTo<IAuthService>())
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );
    }
}