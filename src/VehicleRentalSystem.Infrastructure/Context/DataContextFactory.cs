using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace VehicleRentalSystem.Infrastructure.Context;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var parentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())
            ?? throw new InvalidOperationException("Could not determine the parent directory of the current working directory.");

        var basePath = parentDirectory.FullName;
        var apiProjectPath = Path.Combine(basePath, "BikeRentalSystem.Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath)
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetSection("DatabaseSettings:DefaultConnection").Value;

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("The connection string 'DefaultConnection' was not found.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new DataContext(optionsBuilder.Options);
    }
}
