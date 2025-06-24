using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using hmt_energy_csharp.Extension;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Sql
{
    public class SqlRepository : ISqlRepository
    {
        private readonly IDbContextProvider<hmt_energy_csharpOracleDbContext> _dbContextProvider;

        public SqlRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public async Task<int> Execute(string sql, params object[] parameters)
        {
            var context = await _dbContextProvider.GetDbContextAsync();
            return await context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public async Task<DataTable> ExecuteDataTable(string sql, params object[] parameters)
        {
            var context = await _dbContextProvider.GetDbContextAsync();
            return await context.Database.SqlQuery(sql, parameters);
        }
    }
}