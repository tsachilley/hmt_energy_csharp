using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.VDRs
{
    public interface IVDRRepository
    {
        Task<int> RunTransactionAsync(string[] strSqls);

        Task<int> AddAsync(string strSql);

        /// <summary>
        /// 获取实时数据所有值
        /// </summary>
        /// <returns></returns>
        Task<object> GetRealTimeAsync(string vdrId);

        /// <summary>
        /// 获取历史数据
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <param name="sorting"></param>
        /// <returns></returns>
        Task<object> GetHistoryAsync(string vdrId, int pageNum, int pageCount, string sorting, string asc, string dateFrom, string dateTo);

        /// <summary>
        /// 获取分析数据
        /// </summary>
        /// <param name="vdrId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="slipFrom"></param>
        /// <param name="slipTo"></param>
        /// <param name="draftFrom"></param>
        /// <param name="draftTo"></param>
        /// <param name="analyseType"></param>
        /// <param name="propPitch"></param>
        /// <returns></returns>
        Task<IEnumerable<VdrTotalEntity>> GetAnalyseAsync(string vdrId, long? dateFrom, long? dateTo, float? slipFrom, float? slipTo, float? draftFrom, float? draftTo, string analyseType, float propPitch);
    }
}