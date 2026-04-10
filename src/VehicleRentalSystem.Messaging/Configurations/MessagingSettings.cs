namespace VehicleRentalSystem.Messaging.Configurations;

public class MessagingSettings
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMessagingDependencies(configuration);
        services.AddHealthCheckConfiguration(configuration);
    }
}
