using Asp.Versioning.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json.Serialization;
using VehicleRentalSystem.Infrastructure.Context;
using VehicleRentalSystem.Shared.Configurations;

namespace VehicleRentalSystem.RentalServices.Configuration;

public class ApiSettings
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        configuration = BuildConfiguration(environment);
        services.AddSingleton(configuration);

        ConfigureDatabase(services, configuration);
        ConfigureRedis(services, configuration);
        ConfigureAzureBlobStorage(services, configuration);
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

    private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("DatabaseSettings:DefaultConnection").Value;
        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(connectionString));
    }

    private static void ConfigureRedis(IServiceCollection services, IConfiguration configuration)
    {
        var redisHost = configuration["RedisSettings:Host"];
        var redisPort = configuration["RedisSettings:Port"];

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configurationOptions = new ConfigurationOptions
            {
                EndPoints = { $"{redisHost}:{redisPort}" },
                AbortOnConnectFail = false
            };
            return ConnectionMultiplexer.Connect(configurationOptions);
        });
    }

    private static void ConfigureAzureBlobStorage(IServiceCollection services, IConfiguration configuration)
    {
        var azureConnectionString = Environment.GetEnvironmentVariable("AZURE_CONNECTION_STRING");
        var containerName = configuration["AzureBlobStorageSettings:ContainerName"]
            ?? throw new InvalidOperationException("AzureBlobStorageSettings:ContainerName is not configured.");

        services.Configure<AzureBlobStorageSettings>(options =>
        {
            options.ConnectionString = azureConnectionString ?? string.Empty;
            options.ContainerName = containerName;
        });
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
        services.ConfigureAutomapper();
        services.AddDependencyInjection(configuration);
        services.AddApiVersioningConfiguration();
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
