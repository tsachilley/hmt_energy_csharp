using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace hmt_energy_csharp.Sentences
{
    public interface ISentenceRepository : IRepository<Sentence, int>
    {
        /// <summary>
        /// 获取时间段内数据
        /// </summary>
        /// <param name="vdrId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        Task<IEnumerable<Sentence>> GetListByDateVdrAsync(string vdrId, long? dateFrom, long? dateTo);

        /// <summary>
        /// 获取最新数据
        /// </summary>
        /// <param name="vdrId"></param>
        /// <returns></returns>
        Task<IEnumerable<Sentence>> GetTop1TimeByVdrAsync(string vdrId);

        Task<IEnumerable<Sentence>> GetPageListAsync(string vdrId, int pageNum, int pageCount, string sorting, string asc, long dateFrom, long dateTo);

        Task<int> GetResultCountAsync(string vdrId, long dateFrom, long dateTo);
    }
}