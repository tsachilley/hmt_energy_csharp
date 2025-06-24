using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace hmt_energy_csharp.EntityFrameworkCore.MySql;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */

public class hmt_energy_csharpDbContextFactory : IDesignTimeDbContextFactory<hmt_energy_csharpDbContext>
{
    public hmt_energy_csharpDbContext CreateDbContext(string[] args)
    {
        hmt_energy_csharpEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<hmt_energy_csharpDbContext>()
            .UseMySql(configuration.GetConnectionString("Default"), MySqlServerVersion.LatestSupportedServerVersion);

        return new hmt_energy_csharpDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../hmt_energy_csharp.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}