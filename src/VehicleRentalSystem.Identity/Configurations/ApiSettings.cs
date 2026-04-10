using Asp.Versioning.ApiExplorer;
using System.Text.Json.Serialization;

namespace VehicleRentalSystem.Identity.Configurations;

public class ApiSettings
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        configuration = BuildConfiguration(environment);
        services.AddSingleton(configuration);

        ConfigureControllers(services);
        ConfigureAdditionalServices(services, configuration);
    }

    private static IConfiguration BuildConfiguration(IWebHostEnvironment environment)
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    private static void ConfigureControllers(IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
    }

    private static void ConfigureAdditionalServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityServicesDependencies();
        services.AddIdentityConfig(configuration);
        services.AddSwaggerConfig();
        services.AddHealthCheckConfiguration(configuration);
        services.AddHttpContextAccessor();
        services.AddAuthorization();
        services.AddAuthentication();
    }

    public void ConfigurePipeline(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwaggerConfig(provider);
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseHealthCheckConfiguration();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
