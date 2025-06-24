using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace hmt_energy_csharp.Sentences
{
    public interface ISentenceService : IApplicationService
    {
        Task<SentenceDto> CreateAsync(CreateSentenceDto dto);

        /// <summary>
        /// 获取时间段内数据
        /// </summary>
        /// <param name="vdrId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        Task<IEnumerable<SentenceDto>> GetListByDateVdrAsync(string vdrId, long? dateFrom, long? dateTo);

        /// <summary>
        /// 获取最新数据
        /// </summary>
        /// <param name="vdrId"></param>
        /// <returns></returns>
        Task<IEnumerable<SentenceDto>> GetTop1TimeByVdrAsync(string vdrId);

        Task<IEnumerable<SentenceDto>> GetPageListAsync(string vdrId, int pageNum, int pageCount, string sorting, string asc, long dateFrom, long dateTo);

        Task<int> GetResultCountAsync(string vdrId, long dateFrom, long dateTo);
    }
}