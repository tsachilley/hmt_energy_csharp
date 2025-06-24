using System.Collections.Generic;
using System.Threading.Tasks;

namespace hmt_energy_csharp.VDRs
{
    public interface IVDRService
    {
        Task<int> AddAsync(string strSql);

        Task<int> DataAnalysisAsync(string strDataO, string strSymbol, string vdrId);

        Task<object> GetVoyageRealTimeAsync(string vdrId, float propPitch);

        Task<object> GetVoyageHistoryAsync(string vdrId, int pageNum, int pageCount, string sorting, string asc, long dateFrom, long dateTo);

        Task<object> GetVoyageHistoryChartAsync(string vdrId, int pageNum, int pageCount, string sorting, string asc, long dateFrom, long dateTo, string strParams);

        Task<object> GetVoyageHistoryMapAsync(string vdrId, int pageNum, int pageCount, string sorting, string asc, long dateFrom, long dateTo, string strParams);

        Task<int> GetVoyageHistoryCountAsync(string vdrId, long dateFrom, long dateTo);

        Task<IEnumerable<VdrEntityDto>> GetVoyageAnalyseAsync(string vdrId, long? dateFrom, long? dateTo, float? slipFrom, float? slipTo, float? draftFrom, float? draftTo, string analyseType, float propPitch);

        Task<string> RequestVoyageAnalyseAsync(string vdrId, long? dateFrom, long? dateTo, float? slipFrom, float? slipTo, float? draftFrom, float? draftTo, float? windSpdFrom, float? windSpdTo, float? speedFrom, float? speedTo, float? mefcFrom, float? mefcTo, float? powerFrom, float? powerTo, float? windDirFrom, float? windDirTo, float? rpmFrom, float? rpmTo, string analyseType, float propPitch);

        Task<List<VdrEntityDto>> RequestVoyageDistributionAsync(string vdrId, long? dateFrom, long? dateTo, float? slipFrom, float? slipTo, float? draftFrom, float? draftTo, float? windSpdFrom, float? windSpdTo, float? speedFrom, float? speedTo, float? mefcFrom, float? mefcTo, float? powerFrom, float? powerTo, float? windDirFrom, float? windDirTo, float? rpmFrom, float? rpmTo, string analyseType, float propPitch);
    }
}