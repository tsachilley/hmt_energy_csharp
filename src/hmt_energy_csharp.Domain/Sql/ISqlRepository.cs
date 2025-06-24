using System.Data;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace hmt_energy_csharp
{
    public interface ISqlRepository : IRepository
    {
        /// <summary>
        /// 执行给定的命令
        /// </summary>
        /// <param name="sql">命令字符串</param>
        /// <param name="parameters">要应用于命令字符串的参数</param>
        /// <returns>执行命令后由数据库返回的结果</returns>
        Task<int> Execute(string sql, params object[] parameters);

        /// <summary>
        /// 执行语句返回dataset,注意参数用@p0、@p1、@p3...以此类推，要按照顺序
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">sql参数</param>
        /// <returns>dataset</returns>
        Task<DataTable> ExecuteDataTable(string sql, params object[] parameters);
    }
}