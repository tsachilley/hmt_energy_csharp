using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace hmt_energy_csharp.VesselInfos
{
    public interface IVesselInfoRepository : IRepository<VesselInfo, int>
    {
        Task CalcProperties(string sn);

        Task<IQueryable<VesselInfo>> GetListPage(string number, int pageNumber, int countPerPage, string sorting, string asc, string parameters);

        Task<IQueryable<VesselInfo>> GetListMap(string number, string parameters);

        Task<int> GetTotalCount(string number, string parameters);

        Task<IQueryable<VesselInfo>> GetListChart(string number, string parameters);

        Task<VesselInfo> GetTodayAsync(string number, DateTimeOffset dateTimeOffset);

        Task<VesselInfo> GetYesterdayAsync(string number, DateTimeOffset dateTimeOffset);

        Task<VesselInfo> GetDepartureAsync(string number, DateTimeOffset dateTimeOffset);

        Task<IEnumerable<object>> QueryFromSql(string sql);

        Task<IEnumerable<T>> QueryFromSql<T>(string sql);

        Task<AllInfo> GetLatestInfosAsync(string number, DateTime receiveDatetime);

        Task<DataTable> ExecuteDataTable(string sql);
    }
}