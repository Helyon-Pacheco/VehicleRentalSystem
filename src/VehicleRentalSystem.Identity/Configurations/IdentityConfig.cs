using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VehicleRentalSystem.Identity.Extensions;
using VehicleRentalSystem.Infrastructure.Identity;

namespace VehicleRentalSystem.Identity.Configurations;

public static class IdentityConfig
{
    public static IServiceCollection AddIdentityConfig(this IServiceCollection services, IConfiguration configuration)
    {
        ConfigureDatabase(services, configuration);
        ConfigureIdentity(services);
        var appSettings = ConfigureAppSettings(services, configuration);
        ConfigureAuthentication(services, appSettings);
        return services;
    }

    private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthenticationDbContext>(options =>
            options.UseNpgsql(configuration.GetSection("DatabaseSettings:DefaultConnection").Value));
    }

    private static void ConfigureIdentity(IServiceCollection services)
    {
        services.AddDefaultIdentity<IdentityUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AuthenticationDbContext>()
            .AddErrorDescriber<IdentityMessagesEnglish>()
            .AddDefaultTokenProviders();
    }

    private static AppSettings ConfigureAppSettings(IServiceCollection services, IConfiguration configuration)
    {
        var appSettingsSection = configuration.GetSection("AppSettings");
        var appSettings = appSettingsSection.Get<AppSettings>()
            ?? throw new InvalidOperationException("AppSettings section is not configured in appsettings.json.");

        appSettings.Secret = Environment.GetEnvironmentVariable("APP_SECRET")
            ?? throw new InvalidOperationException("Environment variable APP_SECRET is not set.");

        if (string.IsNullOrWhiteSpace(appSettings.Secret) || appSettings.Secret.Length < 16)
            throw new ArgumentException("The secret key must be at least 16 characters long.");

        services.Configure<AppSettings>(options =>
        {
            options.Secret = appSettings.Secret;
            options.ExpirationHours = appSettings.ExpirationHours;
            options.Issuer = appSettings.Issuer;
            options.ValidAt = appSettings.ValidAt;
        });

        return appSettings;
    }

    private static void ConfigureAuthentication(IServiceCollection services, AppSettings appSettings)
    {
        var key = Encoding.ASCII.GetBytes(appSettings.Secret);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = appSettings.ValidAt,
                ValidIssuer = appSettings.Issuer
            };
        });
    }
}
