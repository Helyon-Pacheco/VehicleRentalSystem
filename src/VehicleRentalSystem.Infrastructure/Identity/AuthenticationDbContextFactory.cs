using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace VehicleRentalSystem.Infrastructure.Identity;

public class AuthenticationDbContextFactory
{
    public static AuthenticationDbContext CreateDbContext(string[] args)
    {
        var parentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())
            ?? throw new InvalidOperationException("Could not determine the parent directory of the current working directory.");

        var basePath = parentDirectory.FullName;
        var apiProjectPath = Path.Combine(basePath, "VehicleRentalSystem.Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath)
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetSection("DatabaseSettings:DefaultConnection").Value;

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("The connection string 'DefaultConnection' was not found.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AuthenticationDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AuthenticationDbContext(optionsBuilder.Options);
    }
}
