using Dapper;
using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.VesselInfos
{
    public class EfCoreVesselInfoRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, VesselInfo, int>, IVesselInfoRepository
    {
        private readonly ILogger<EfCoreVesselInfoRepository> _logger;

        public EfCoreVesselInfoRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider, ILogger<EfCoreVesselInfoRepository> logger) : base(dbContextProvider)
        {
            _logger = logger;
        }

        public async Task CalcProperties(string sn)
        {
            var dbset = await GetDbSetAsync();
            var lastEntity = await dbset.OrderByDescending(t => t.ReceiveDatetime).FirstOrDefaultAsync(t => t.SN == sn);
            if (lastEntity == null)
                return;
            lastEntity.Heel = lastEntity.PortDraft - lastEntity.StarBoardDraft;
            lastEntity.Trim = lastEntity.AsternDraft - lastEntity.BowDraft;
            lastEntity.Draft = (lastEntity.BowDraft + lastEntity.AsternDraft) / 2;
            if (lastEntity.MEPower != null && (lastEntity.MEHFOConsumption != null || lastEntity.MEMDOConsumption != null))
                lastEntity.MESFOC = (lastEntity.MEHFOConsumption ?? 0 + lastEntity.MEMDOConsumption ?? 0) / lastEntity.MEPower;
            if (lastEntity.DGPower != null && (lastEntity.DGHFOConsumption != null || lastEntity.DGMDOConsumption != null))
                lastEntity.DGSFOC = (lastEntity.DGHFOConsumption ?? 0 + lastEntity.DGMDOConsumption ?? 0) / lastEntity.DGPower;
            if ((lastEntity.MEPower != null || lastEntity.DGPower != null) && (lastEntity.MEHFOConsumption != null || lastEntity.MEMDOConsumption != null || lastEntity.DGHFOConsumption != null || lastEntity.DGMDOConsumption != null || lastEntity.BLRHFOConsumption != null || lastEntity.BLRMDOConsumption != null))
                lastEntity.SFOC = (lastEntity.MEHFOConsumption ?? 0 + lastEntity.MEMDOConsumption ?? 0 + lastEntity.DGHFOConsumption ?? 0 + lastEntity.DGMDOConsumption ?? 0 + lastEntity.BLRHFOConsumption ?? 0 + lastEntity.BLRMDOConsumption ?? 0) / (lastEntity.MEPower ?? 0 + lastEntity.DGPower ?? 0);
            lastEntity.Slip = (1 - (lastEntity.WaterSpeed / (lastEntity.MERpm * 5128.77) * 1852 * 1000 / 60)) * 100;
            lastEntity.MEHFOCPerNm = lastEntity.MEHFOConsumption / lastEntity.WaterSpeed;
            lastEntity.MEMDOCPerNm = lastEntity.MEMDOConsumption / lastEntity.WaterSpeed;
            if (lastEntity.WaterSpeed != null && (lastEntity.MEHFOConsumption != null || lastEntity.MEMDOConsumption != null))
                lastEntity.MEFCPerNm = (lastEntity.MEHFOConsumption ?? 0 + lastEntity.MEMDOConsumption ?? 0) / lastEntity.WaterSpeed;
            lastEntity.DGHFOCPerNm = lastEntity.DGHFOConsumption / lastEntity.WaterSpeed;
            lastEntity.DGMDOCPerNm = lastEntity.DGMDOConsumption / lastEntity.WaterSpeed;
            if (lastEntity.WaterSpeed != null && (lastEntity.DGHFOConsumption != null || lastEntity.DGMDOConsumption != null))
                lastEntity.DGFCPerNm = (lastEntity.DGHFOConsumption ?? 0 + lastEntity.DGMDOConsumption ?? 0) / lastEntity.WaterSpeed;
            lastEntity.BLRHFOCPerNm = lastEntity.BLRHFOConsumption / lastEntity.WaterSpeed;
            lastEntity.BLRMDOCPerNm = lastEntity.BLRMDOConsumption / lastEntity.WaterSpeed;
            if (lastEntity.WaterSpeed != null && (lastEntity.BLRHFOConsumption != null || lastEntity.BLRMDOConsumption != null))
                lastEntity.BLRFCPerNm = (lastEntity.BLRHFOConsumption ?? 0 + lastEntity.BLRMDOConsumption ?? 0) / lastEntity.WaterSpeed;
            if (lastEntity.WaterSpeed != null && (lastEntity.MEHFOConsumption != null || lastEntity.DGHFOConsumption != null || lastEntity.BLRHFOConsumption != null))
                lastEntity.HFOCPerNm = (lastEntity.MEHFOConsumption ?? 0 + lastEntity.DGHFOConsumption ?? 0 + lastEntity.BLRHFOConsumption ?? 0) / lastEntity.WaterSpeed;
            if (lastEntity.WaterSpeed != null && (lastEntity.MEMDOConsumption != null || lastEntity.DGMDOConsumption != null || lastEntity.BLRMDOConsumption != null))
                lastEntity.MDOCPerNm = (lastEntity.MEMDOConsumption ?? 0 + lastEntity.DGMDOConsumption ?? 0 + lastEntity.BLRMDOConsumption ?? 0) / lastEntity.WaterSpeed;
            if (lastEntity.WaterSpeed != null && (lastEntity.MEHFOConsumption != null || lastEntity.MEMDOConsumption != null || lastEntity.DGHFOConsumption != null || lastEntity.DGMDOConsumption != null || lastEntity.BLRHFOConsumption != null || lastEntity.BLRMDOConsumption != null))
                lastEntity.FCPerNm = (lastEntity.MEHFOConsumption ?? 0 + lastEntity.DGHFOConsumption ?? 0 + lastEntity.BLRHFOConsumption ?? 0 + lastEntity.MEMDOConsumption ?? 0 + lastEntity.DGMDOConsumption ?? 0 + lastEntity.BLRMDOConsumption ?? 0) / lastEntity.WaterSpeed;
            var result = dbset.Update(lastEntity);
        }

        public async Task<IQueryable<VesselInfo>> GetListChart(string number, string parameters)
        {
            var queryParams = parameters.ToJObject();
            var dbParameters = new List<DbParameter>();
            StringBuilder sbSql = new StringBuilder();

            sbSql.Append("select t0.\"Id\",t0.\"ReceiveDatetime\",t0.\"Longitude\",t0.\"Latitude\",t0.\"Course\",t0.\"MagneticVariation\",t0.\"TotalDistanceGrd\",t0.\"ResetDistanceGrd\",t0.\"TotalDistanceWat\",t0.\"ResetDistanceWat\",t0.\"WindDirection\",t0.\"WindSpeed\",t0.\"WaveHeight\",t0.\"WaveDirection\",t0.\"Temperature\",t0.\"Pressure\",t0.\"Weather\",t0.\"Visibility\",t0.\"WaterSpeed\",t0.\"GroundSpeed\",t0.\"BowDraft\",t0.\"AsternDraft\",t0.\"PortDraft\",t0.\"StarBoardDraft\",t0.\"Trim\",t0.\"Heel\",t0.\"Draft\",t0.\"Depth\",t0.\"DepthOffset\",t0.\"MESFOC\",t0.\"MEHFOConsumption\",t0.\"MEMDOConsumption\",t0.\"DGSFOC\",t0.\"DGHFOConsumption\",t0.\"DGMDOConsumption\",t0.\"BLRSFOC\",t0.\"BLRHFOConsumption\",t0.\"BLRMDOConsumption\",t0.\"Slip\",t1.\"Power\" \"MEPower\",t0.\"Torque\",t1.\"Rpm\" \"MERpm\",t0.\"Thrust\",t0.\"create_time\",t0.\"update_time\",t0.\"delete_time\",t0.\"BLGHFOCACC\",t0.\"BLGMDOCACC\",t0.\"BLRFCPerNm\",t0.\"BLRHFOCPerNm\",t0.\"BLRMDOCPerNm\",t0.\"DGFCPerNm\",t0.\"DGHFOCACC\",t0.\"DGHFOCPerNm\",t0.\"DGMDOCACC\",t0.\"DGMDOCPerNm\",t0.\"DGPower\",t0.\"FCPerNm\",t0.\"HFOCPerNm\",t0.\"MDOCPerNm\",t0.\"MEFCPerNm\",t0.\"MEHFOCACC\",t0.\"MEHFOCPerNm\",t0.\"MEMDOCACC\",t0.\"MEMDOCPerNm\",t0.\"SFOC\",t0.\"SN\",t0.\"Status\",t0.\"RtCII\",t0.\"Uploaded\",t0.\"X\",t0.\"Y\",t0.\"SeaTemperature\" from \"vesselinfo\" t0,\"energy_totalindicator\" t1 where t0.\"SN\"=t1.\"Number\" and t0.\"ReceiveDatetime\"=t1.\"ReceiveDatetime\" and NVL(t0.\"delete_time\", TO_TIMESTAMP('1970-01-01', 'yyyy-mm-dd'))=TO_TIMESTAMP('1970-01-01', 'yyyy-mm-dd')");
            if (!string.IsNullOrWhiteSpace(number))
            {
                sbSql.Append($" and t0.\"SN\"='{number}'");
                dbParameters.Add(new OracleParameter { ParameterName = ":SN", Value = number });
            }
            if (queryParams.ContainsKey("StartDate") && !string.IsNullOrWhiteSpace(queryParams["StartDate"].ToString()))
            {
                sbSql.Append($" and t0.\"ReceiveDatetime\">=TO_TIMESTAMP('{Convert.ToDateTime(queryParams["StartDate"]).ToString("yyyy-MM-dd HH:mm:ss")}','yyyy-mm-dd hh24:mi:ss')");
                dbParameters.Add(new OracleParameter { ParameterName = ":StartDate", Value = Convert.ToDateTime(queryParams["StartDate"]).ToString("yyyy-MM-dd HH:mm:ss") });
            }
            if (queryParams.ContainsKey("EndDate") && !string.IsNullOrWhiteSpace(queryParams["EndDate"].ToString()))
            {
                sbSql.Append($" and t0.\"ReceiveDatetime\"<TO_TIMESTAMP('{Convert.ToDateTime(queryParams["EndDate"]).ToString("yyyy-MM-dd HH:mm:ss")}','yyyy-mm-dd hh24:mi:ss')");
                dbParameters.Add(new OracleParameter { ParameterName = ":EndDate", Value = Convert.ToDateTime(queryParams["EndDate"]).ToString("yyyy-MM-dd HH:mm:ss") });
            }
            if (queryParams.ContainsKey("Status") && !string.IsNullOrWhiteSpace(queryParams["Status"].ToString()))
            {
                if (queryParams["Status"].ToString() == "Berthing")
                    sbSql.Append($" and t0.\"Status\"='Berthing'");
                else
                    sbSql.Append($" and (t0.\"Status\"='Manoeuvring' or t0.\"Status\"='Cruising')");
            }
            sbSql.Append($" order by t0.\"ReceiveDatetime\"");

            var dbset = await GetDbSetAsync();
            return dbset.FromSqlRaw(sbSql.ToString());
        }

        public async Task<IQueryable<VesselInfo>> GetListMap(string number, string parameters)
        {
            var queryParams = parameters.ToJObject();
            var dbParameters = new List<DbParameter>();
            StringBuilder sbSql = new StringBuilder();

            sbSql.Append("select t0.\"Id\",t0.\"ReceiveDatetime\",t0.\"Longitude\",t0.\"Latitude\",t0.\"Course\",t0.\"MagneticVariation\",t0.\"TotalDistanceGrd\",t0.\"ResetDistanceGrd\",t0.\"TotalDistanceWat\",t0.\"ResetDistanceWat\",t0.\"WindDirection\",t0.\"WindSpeed\",t0.\"WaveHeight\",t0.\"WaveDirection\",t0.\"Temperature\",t0.\"Pressure\",t0.\"Weather\",t0.\"Visibility\",t0.\"WaterSpeed\",t0.\"GroundSpeed\",t0.\"BowDraft\",t0.\"AsternDraft\",t0.\"PortDraft\",t0.\"StarBoardDraft\",t0.\"Trim\",t0.\"Heel\",t0.\"Draft\",t0.\"Depth\",t0.\"DepthOffset\",t0.\"MESFOC\",t1.\"HFOAccumulated\" AS \"MEHFOConsumption\",t0.\"MEMDOConsumption\",t0.\"DGSFOC\",t1.\"MethanolAccumulated\" AS \"DGHFOConsumption\",t0.\"DGMDOConsumption\",t0.\"BLRSFOC\",t0.\"BLRHFOConsumption\",t0.\"BLRMDOConsumption\",t0.\"Slip\",t0.\"MEPower\",t0.\"Torque\",t0.\"MERpm\",t0.\"Thrust\",t0.\"create_time\",t0.\"update_time\",t0.\"delete_time\",t0.\"BLGHFOCACC\",t0.\"BLGMDOCACC\",t0.\"BLRFCPerNm\",t0.\"BLRHFOCPerNm\",t0.\"BLRMDOCPerNm\",t0.\"DGFCPerNm\",t0.\"DGHFOCACC\",t0.\"DGHFOCPerNm\",t0.\"DGMDOCACC\",t0.\"DGMDOCPerNm\",t0.\"DGPower\",t0.\"FCPerNm\",t0.\"HFOCPerNm\",t0.\"MDOCPerNm\",t0.\"MEFCPerNm\",t0.\"MEHFOCACC\",t0.\"MEHFOCPerNm\",t0.\"MEMDOCACC\",t0.\"MEMDOCPerNm\",t0.\"SFOC\",t0.\"SN\",t0.\"Status\",t0.\"RtCII\",t0.\"Uploaded\",t0.\"X\",t0.\"Y\",t0.\"SeaTemperature\" from \"vesselinfo\" t0,\"energy_totalindicator\" t1 where NVL(\"delete_time\", TO_TIMESTAMP('1970-01-01', 'yyyy-mm-dd'))=TO_TIMESTAMP('1970-01-01', 'yyyy-mm-dd') AND t0.\"SN\"=t1.\"Number\" AND t0.\"ReceiveDatetime\"=t1.\"ReceiveDatetime\"");
            if (!string.IsNullOrWhiteSpace(number))
            {
                sbSql.Append($" and t0.\"SN\"='{number}'");
                dbParameters.Add(new OracleParameter { ParameterName = ":SN", Value = number });
            }
            if (queryParams.ContainsKey("StartDate") && !string.IsNullOrWhiteSpace(queryParams["StartDate"].ToString()))
            {
                sbSql.Append($" and t0.\"ReceiveDatetime\">=TO_TIMESTAMP('{Convert.ToDateTime(queryParams["StartDate"]).ToString("yyyy-MM-dd HH:mm:ss")}','yyyy-mm-dd hh24:mi:ss')");
                dbParameters.Add(new OracleParameter { ParameterName = ":StartDate", Value = Convert.ToDateTime(queryParams["StartDate"]).ToString("yyyy-MM-dd HH:mm:ss") });
            }
            if (queryParams.ContainsKey("EndDate") && !string.IsNullOrWhiteSpace(queryParams["EndDate"].ToString()))
            {
                sbSql.Append($" and t0.\"ReceiveDatetime\"<TO_TIMESTAMP('{Convert.ToDateTime(queryParams["EndDate"]).ToString("yyyy-MM-dd HH:mm:ss")}','yyyy-mm-dd hh24:mi:ss')");
                dbParameters.Add(new OracleParameter { ParameterName = ":EndDate", Value = Convert.ToDateTime(queryParams["EndDate"]).ToString("yyyy-MM-dd HH:mm:ss") });
            }
            sbSql.Append(" order by t0.\"ReceiveDatetime\"");
            var dbset = await GetDbSetAsync();
            return dbset.FromSqlRaw(sbSql.ToString());
        }

        public async Task<int> GetTotalCount(string number, string parameters)
        {
            var queryParams = parameters.ToJObject();

            var dbset = await GetDbSetAsync();
            return dbset.Where(t => t.delete_time == null && t.SN == number).WhereIf(queryParams.ContainsKey("StartDate") && !string.IsNullOrWhiteSpace(queryParams["StartDate"].ToString()), t => t.ReceiveDatetime >= Convert.ToDateTime(queryParams["StartDate"])).WhereIf(queryParams.ContainsKey("EndDate") && !string.IsNullOrWhiteSpace(queryParams["EndDate"].ToString()), t => t.ReceiveDatetime < Convert.ToDateTime(queryParams["EndDate"])).Count();
        }

        public async Task<IQueryable<VesselInfo>> GetListPage(string number, int pageNumber, int countPerPage, string sorting, string asc, string parameters)
        {
            var queryParams = parameters.ToJObject();
            var dbParameters = new List<DbParameter>();
            StringBuilder sbSql = new StringBuilder();

            sbSql.Append("select \"Id\",\"ReceiveDatetime\",\"Longitude\",\"Latitude\",\"Course\",\"MagneticVariation\",\"TotalDistanceGrd\",\"ResetDistanceGrd\",\"TotalDistanceWat\",\"ResetDistanceWat\",\"WindDirection\",\"WindSpeed\",\"WaveHeight\",\"WaveDirection\",\"Temperature\",\"Pressure\",\"Weather\",\"Visibility\",\"WaterSpeed\",\"GroundSpeed\",\"BowDraft\",\"AsternDraft\",\"PortDraft\",\"StarBoardDraft\",\"Trim\",\"Heel\",\"Draft\",\"Depth\",\"DepthOffset\",\"MESFOC\",\"MEHFOConsumption\",\"MEMDOConsumption\",\"DGSFOC\",\"DGHFOConsumption\",\"DGMDOConsumption\",\"BLRSFOC\",\"BLRHFOConsumption\",\"BLRMDOConsumption\",\"Slip\",\"MEPower\",\"Torque\",\"MERpm\",\"Thrust\",\"create_time\",\"update_time\",\"delete_time\",\"BLGHFOCACC\",\"BLGMDOCACC\",\"BLRFCPerNm\",\"BLRHFOCPerNm\",\"BLRMDOCPerNm\",\"DGFCPerNm\",\"DGHFOCACC\",\"DGHFOCPerNm\",\"DGMDOCACC\",\"DGMDOCPerNm\",\"DGPower\",\"FCPerNm\",\"HFOCPerNm\",\"MDOCPerNm\",\"MEFCPerNm\",\"MEHFOCACC\",\"MEHFOCPerNm\",\"MEMDOCACC\",\"MEMDOCPerNm\",\"SFOC\",\"SN\",\"Status\",\"RtCII\",\"Uploaded\",\"X\",\"Y\",\"SeaTemperature\" from \"vesselinfo\" where NVL(\"delete_time\", TO_TIMESTAMP('1970-01-01', 'yyyy-mm-dd'))=TO_TIMESTAMP('1970-01-01', 'yyyy-mm-dd')");
            if (!string.IsNullOrWhiteSpace(number))
            {
                sbSql.Append($" and \"SN\"='{number}'");
                dbParameters.Add(new OracleParameter { ParameterName = ":SN", Value = number });
            }
            if (queryParams.ContainsKey("StartDate") && !string.IsNullOrWhiteSpace(queryParams["StartDate"].ToString()))
            {
                sbSql.Append($" and \"ReceiveDatetime\">=TO_TIMESTAMP('{Convert.ToDateTime(queryParams["StartDate"]).ToString("yyyy-MM-dd HH:mm:ss")}','yyyy-mm-dd hh24:mi:ss')");
                dbParameters.Add(new OracleParameter { ParameterName = ":StartDate", Value = Convert.ToDateTime(queryParams["StartDate"]).ToString("yyyy-MM-dd HH:mm:ss") });
            }
            if (queryParams.ContainsKey("EndDate") && !string.IsNullOrWhiteSpace(queryParams["EndDate"].ToString()))
            {
                sbSql.Append($" and \"ReceiveDatetime\"<TO_TIMESTAMP('{Convert.ToDateTime(queryParams["EndDate"]).ToString("yyyy-MM-dd HH:mm:ss")}','yyyy-mm-dd hh24:mi:ss')");
                dbParameters.Add(new OracleParameter { ParameterName = ":EndDate", Value = Convert.ToDateTime(queryParams["EndDate"]).ToString("yyyy-MM-dd HH:mm:ss") });
            }
            if (!string.IsNullOrWhiteSpace(sorting))
            {
                sbSql.Append($" order by {sorting}");
                dbParameters.Add(new OracleParameter { ParameterName = ":Sorting", Value = sorting });
                if (!string.IsNullOrWhiteSpace(asc))
                {
                    sbSql.Append($" {asc}");
                    dbParameters.Add(new OracleParameter { ParameterName = ":Asc", Value = asc });
                }
            }
            sbSql.Insert(0, "select ROWNUM rn,t1.\"Id\",t1.\"ReceiveDatetime\",t1.\"Longitude\",t1.\"Latitude\",t1.\"Course\",t1.\"MagneticVariation\",t1.\"TotalDistanceGrd\",t1.\"ResetDistanceGrd\",t1.\"TotalDistanceWat\",t1.\"ResetDistanceWat\",t1.\"WindDirection\",t1.\"WindSpeed\",t1.\"WaveHeight\",t1.\"WaveDirection\",t1.\"Temperature\",t1.\"Pressure\",t1.\"Weather\",t1.\"Visibility\",t1.\"WaterSpeed\",t1.\"GroundSpeed\",t1.\"BowDraft\",t1.\"AsternDraft\",t1.\"PortDraft\",t1.\"StarBoardDraft\",t1.\"Trim\",t1.\"Heel\",t1.\"Draft\",t1.\"Depth\",t1.\"DepthOffset\",t1.\"MESFOC\",t1.\"MEHFOConsumption\",t1.\"MEMDOConsumption\",t1.\"DGSFOC\",t1.\"DGHFOConsumption\",t1.\"DGMDOConsumption\",t1.\"BLRSFOC\",t1.\"BLRHFOConsumption\",t1.\"BLRMDOConsumption\",t1.\"Slip\",t1.\"MEPower\",t1.\"Torque\",t1.\"MERpm\",t1.\"Thrust\",t1.\"create_time\",t1.\"update_time\",t1.\"delete_time\",t1.\"BLGHFOCACC\",t1.\"BLGMDOCACC\",t1.\"BLRFCPerNm\",t1.\"BLRHFOCPerNm\",t1.\"BLRMDOCPerNm\",t1.\"DGFCPerNm\",t1.\"DGHFOCACC\",t1.\"DGHFOCPerNm\",t1.\"DGMDOCACC\",t1.\"DGMDOCPerNm\",t1.\"DGPower\",t1.\"FCPerNm\",t1.\"HFOCPerNm\",t1.\"MDOCPerNm\",t1.\"MEFCPerNm\",t1.\"MEHFOCACC\",t1.\"MEHFOCPerNm\",t1.\"MEMDOCACC\",t1.\"MEMDOCPerNm\",t1.\"SFOC\",t1.\"SN\",t1.\"Status\",t1.\"RtCII\",t1.\"Uploaded\",t1.\"X\",t1.\"Y\",t1.\"SeaTemperature\" from (").Append($") t1 where ROWNUM<={pageNumber * countPerPage}");
            dbParameters.Add(new OracleParameter { ParameterName = ":EndCount", Value = pageNumber * countPerPage });
            sbSql.Insert(0, "select t2.\"Id\",t2.\"ReceiveDatetime\",t2.\"Longitude\",t2.\"Latitude\",t2.\"Course\",t2.\"MagneticVariation\",t2.\"TotalDistanceGrd\",t2.\"ResetDistanceGrd\",t2.\"TotalDistanceWat\",t2.\"ResetDistanceWat\",t2.\"WindDirection\",t2.\"WindSpeed\",t2.\"WaveHeight\",t2.\"WaveDirection\",t2.\"Temperature\",t2.\"Pressure\",t2.\"Weather\",t2.\"Visibility\",t2.\"WaterSpeed\",t2.\"GroundSpeed\",t2.\"BowDraft\",t2.\"AsternDraft\",t2.\"PortDraft\",t2.\"StarBoardDraft\",t2.\"Trim\",t2.\"Heel\",t2.\"Draft\",t2.\"Depth\",t2.\"DepthOffset\",t2.\"MESFOC\",t2.\"MEHFOConsumption\",t2.\"MEMDOConsumption\",t2.\"DGSFOC\",t2.\"DGHFOConsumption\",t2.\"DGMDOConsumption\",t2.\"BLRSFOC\",t2.\"BLRHFOConsumption\",t2.\"BLRMDOConsumption\",t2.\"Slip\",t2.\"MEPower\",t2.\"Torque\",t2.\"MERpm\",t2.\"Thrust\",t2.\"create_time\",t2.\"update_time\",t2.\"delete_time\",t2.\"BLGHFOCACC\",t2.\"BLGMDOCACC\",t2.\"BLRFCPerNm\",t2.\"BLRHFOCPerNm\",t2.\"BLRMDOCPerNm\",t2.\"DGFCPerNm\",t2.\"DGHFOCACC\",t2.\"DGHFOCPerNm\",t2.\"DGMDOCACC\",t2.\"DGMDOCPerNm\",t2.\"DGPower\",t2.\"FCPerNm\",t2.\"HFOCPerNm\",t2.\"MDOCPerNm\",t2.\"MEFCPerNm\",t2.\"MEHFOCACC\",t2.\"MEHFOCPerNm\",t2.\"MEMDOCACC\",t2.\"MEMDOCPerNm\",t2.\"SFOC\",t2.\"SN\",t2.\"Status\",t2.\"RtCII\",t2.\"Uploaded\",t2.\"X\",t2.\"Y\",t2.\"SeaTemperature\" from (").Append($") t2 where rn>{(pageNumber - 1) * countPerPage} order by rn");
            dbParameters.Add(new OracleParameter { ParameterName = ":StartCount", Value = (pageNumber - 1) * countPerPage });

            var dbset = await GetDbSetAsync();
            return dbset.FromSqlRaw(sbSql.ToString());
        }

        public async Task<VesselInfo> GetYesterdayAsync(string number, DateTimeOffset dateTimeOffset)
        {
            var dbset = await GetDbSetAsync();
            var yesterday = dbset.Where(t => t.delete_time == null && t.SN == number && t.ReceiveDatetime < Convert.ToDateTime(dateTimeOffset.AddDays(-1).ToString("yyyy-MM-dd 12:00:00"))).OrderByDescending(t => t.ReceiveDatetime).Take(1);
            return await yesterday.FirstOrDefaultAsync();
        }

        public async Task<VesselInfo> GetTodayAsync(string number, DateTimeOffset dateTimeOffset)
        {
            var dbset = await GetDbSetAsync();
            var today = dbset.Where(t => t.delete_time == null && t.SN == number && t.ReceiveDatetime < Convert.ToDateTime(dateTimeOffset.ToString("yyyy-MM-dd 12:00:00"))).OrderByDescending(t => t.ReceiveDatetime).Take(1);
            return await today.FirstOrDefaultAsync();
        }

        public async Task<VesselInfo> GetDepartureAsync(string number, DateTimeOffset dateTimeOffset)
        {
            var dbset = await GetDbSetAsync();
            var today = dbset.Where(t => t.delete_time == null && t.SN == number && t.ReceiveDatetime >= dateTimeOffset.DateTime).OrderBy(t => t.ReceiveDatetime).Take(1);
            return await today.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<object>> QueryFromSql(string sql)
        {
            var connection = await GetDbConnectionAsync();
            var needClose = false;
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    needClose = true;
                    connection.Open();
                }
                return await connection.QueryAsync(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                return null;
            }
            finally
            {
                if (needClose)
                    connection.Close();
            }
        }
    }
}