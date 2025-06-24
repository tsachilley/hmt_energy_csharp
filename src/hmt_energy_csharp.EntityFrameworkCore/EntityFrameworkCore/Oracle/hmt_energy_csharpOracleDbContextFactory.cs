using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace hmt_energy_csharp.EntityFrameworkCore.Oracle
{
    public class hmt_energy_csharpOracleDbContextFactory : IDesignTimeDbContextFactory<hmt_energy_csharpOracleDbContext>
    {
        public hmt_energy_csharpOracleDbContext CreateDbContext(string[] args)
        {
            hmt_energy_csharpEfCoreEntityExtensionMappings.Configure();

            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<hmt_energy_csharpOracleDbContext>()
                .UseOracle(configuration.GetConnectionString("OracleDefault"));

            return new hmt_energy_csharpOracleDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../hmt_energy_csharp.DbMigrator/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}