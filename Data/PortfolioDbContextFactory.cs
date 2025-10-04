using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AvikstromPortfolio.Data
{
    /// <summary>
    /// Provides a design-time DbContext factory so EF Core tools
    /// can create a PortfolioDbContext.
    /// </summary>
    public class PortfolioDbContextFactory : IDesignTimeDbContextFactory<PortfolioDbContext>
    {
        public PortfolioDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PortfolioDbContext>();

            optionsBuilder
                .UseNpgsql(config.GetConnectionString("DefaultConnection"))
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine,
                [
                    DbLoggerCategory.Database.Command.Name,
                    DbLoggerCategory.Infrastructure.Name
                ], LogLevel.Information);

            return new PortfolioDbContext(optionsBuilder.Options);
        }
    }
}
