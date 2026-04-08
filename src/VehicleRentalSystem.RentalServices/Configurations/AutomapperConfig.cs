using AutoMapper;
using System.Reflection;

namespace VehicleRentalSystem.RentalServices.Configuration;

public static class AutomapperConfig
{
    public static void ConfigureAutomapper(this IServiceCollection services)
    {
        var mappingProfiles = GetMappingProfiles();
        if (mappingProfiles.Any())
        {
            services.AddAutoMapper(cfg => { }, mappingProfiles);
        }
    }

    private static Type[] GetMappingProfiles()
    {
        var assembly = Assembly.GetExecutingAssembly();
        return assembly.GetTypes()
            .Where(type => typeof(Profile).IsAssignableFrom(type) && !type.IsAbstract && type.Namespace == "VehicleRentalSystem.RentalServices.Mappers")
            .ToArray();
    }
}
