using hmt_energy_csharp.EntityFrameworkCore.MySql;
using hmt_energy_csharp.NotVDRs;
using hmt_energy_csharp.VdrDpts;
using hmt_energy_csharp.VdrGnss;
using hmt_energy_csharp.VdrMwds;
using hmt_energy_csharp.VdrRpms;
using hmt_energy_csharp.VdrVbws;
using hmt_energy_csharp.VdrVlws;
using hmt_energy_csharp.VdrVtgs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.VDRs
{
    public class EfCoreVDRRepository : EfCoreRepository<hmt_energy_csharpDbContext, VdrEntity, int>, IVDRRepository
    {
        private readonly IDbContextProvider<hmt_energy_csharpDbContext> _dbContextProvider;
        private readonly ILogger<EfCoreVDRRepository> _logger;

        public EfCoreVDRRepository(IDbContextProvider<hmt_energy_csharpDbContext> dbContextProvider, ILogger<EfCoreVDRRepository> logger) : base(dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
            _logger = logger;
        }

        public async Task<int> RunTransactionAsync(string[] strSqls)
        {
            var result = 0;
            IDbContextTransaction transaction = null;
            try
            {
                using (var _context = await _dbContextProvider.GetDbContextAsync())
                {
                    transaction = await _context.Database.BeginTransactionAsync();
                    foreach (var strSql in strSqls)
                    {
                        result = await _context.Database.ExecuteSqlRawAsync($"{strSql}");
                        if (result == 0)
                            break;
                    }
                    if (result > 0)
                        transaction.Commit();
                    else
                        transaction.Rollback();
                    return result;
                }
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                return 0;
            }
        }

        public async Task<int> AddAsync(string strSql)
        {
            var result = 0;
            try
            {
                var _context = await _dbContextProvider.GetDbContextAsync();
                result = await _context.Database.ExecuteSqlRawAsync(strSql);
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        /// <summary>
        /// 获取实时数据所有值
        /// </summary>
        /// <returns></returns>
        public async Task<object> GetRealTimeAsync(string vdrId)
        {
            try
            {
                var _context = await _dbContextProvider.GetDbContextAsync();
                var parameters = new List<DbParameter>
                {
                    new MySqlParameter { ParameterName = "?vdrId", Value = vdrId }
                };

                #region VDR数据 可以考虑定制化

                var vdr_dpt = _context.VdrDpts.FromSqlRaw(@"SELECT
	                    vdr_dpt.*
                    FROM
	                    vdr_dpt,
	                    sentence
                    WHERE
	                    vdr_dpt.sentenceid = sentence.sentenceid
	                    AND sentence.vdr_id = ?vdrId
	                    AND sentence.delete_time IS NULL
	                    AND vdr_dpt.delete_time IS NULL
	                    AND TIMESTAMPDIFF(
		                    MINUTE,
		                    STR_TO_DATE( sentence.TIME, '%Y-%m-%d %H:%i:%s' ),
	                    SYSDATE())<15
                    ORDER BY
	                    sentence.TIME DESC,
	                    vdr_dpt.Id DESC
	                    LIMIT 1", parameters.ToArray());
                var vdr_gga = _context.VdrGgas.FromSqlRaw(@"SELECT
	                    vdr_gga.*
                    FROM
	                    vdr_gga,
	                    sentence
                    WHERE
	                    vdr_gga.sentenceid = sentence.sentenceid
	                    AND sentence.vdr_id = ?vdrId
	                    AND sentence.delete_time IS NULL
	                    AND vdr_gga.delete_time IS NULL
	                    AND TIMESTAMPDIFF(
		                    MINUTE,
		                    STR_TO_DATE( sentence.TIME, '%Y-%m-%d %H:%i:%s' ),
	                    SYSDATE())<15
                    ORDER BY
	                    sentence.TIME DESC,
	                    vdr_gga.Id DESC
	                    LIMIT 1", parameters.ToArray());
                var vdr_gns = _context.VdrGnss.FromSqlRaw(@"SELECT
	                    vdr_gns.*
                    FROM
	                    vdr_gns,
	                    sentence
                    WHERE
	                    vdr_gns.sentenceid = sentence.sentenceid
	                    AND sentence.vdr_id = ?vdrId
	                    AND sentence.delete_time IS NULL
	                    AND vdr_gns.delete_time IS NULL
	                    AND TIMESTAMPDIFF(
		                    MINUTE,
		                    STR_TO_DATE( sentence.TIME, '%Y-%m-%d %H:%i:%s' ),
	                    SYSDATE())<15
                    ORDER BY
	                    sentence.TIME DESC,
	                    vdr_gns.Id DESC
	                    LIMIT 1", parameters.ToArray());
                var vdr_hdg = _context.VdrHdgs.FromSqlRaw(@"SELECT
	                    vdr_hdg.*
                    FROM
	                    vdr_hdg,
	                    sentence
                    WHERE
	                    vdr_hdg.sentenceid = sentence.sentenceid
	                    AND sentence.vdr_id = ?vdrId
	                    AND sentence.delete_time IS NULL
	                    AND vdr_hdg.delete_time IS NULL
	                    AND TIMESTAMPDIFF(
		                    MINUTE,
		                    STR_TO_DATE( sentence.TIME, '%Y-%m-%d %H:%i:%s' ),
	                    SYSDATE())<15
                    ORDER BY
	                    sentence.TIME DESC,
	                    vdr_hdg.Id DESC
	                    LIMIT 1", parameters.ToArray());
                var vdr_mwd = _context.VdrMwds.FromSqlRaw(@"SELECT
	                    vdr_mwd.*
                    FROM
	                    vdr_mwd,
	                    sentence
                    WHERE
	                    vdr_mwd.sentenceid = sentence.sentenceid
	                    AND sentence.vdr_id = ?vdrId
	                    AND sentence.delete_time IS NULL
	                    AND vdr_mwd.delete_time IS NULL
	                    AND TIMESTAMPDIFF(
		                    MINUTE,
		                    STR_TO_DATE( sentence.TIME, '%Y-%m-%d %H:%i:%s' ),
	                    SYSDATE())<15
                    ORDER BY
	                    sentence.TIME DESC,
	                    vdr_mwd.Id DESC
	                    LIMIT 1", parameters.ToArray());
                var vdr_mwv = _context.VdrMwvs.FromSqlRaw(@"SELECT
	                    vdr_mwv.*
                    FROM
	                    vdr_mwv,
	                    sentence
                    WHERE
	                    vdr_mwv.sentenceid = sentence.sentenceid
	                    AND sentence.vdr_id = ?vdrId
	                    AND sentence.delete_time IS NULL
	                    AND vdr_mwv.delete_time IS NULL
	                    AND TIMESTAMPDIFF(
		                    MINUTE,
		                    STR_TO_DATE( sentence.TIME, '%Y-%m-%d %H:%i:%s' ),
	                    SYSDATE())<15
                    ORDER BY
	                    sentence.TIME DESC,
	                    vdr_mwv.Id DESC
	                    LIMIT 1", parameters.ToArray());
                var vdr_prc = _context.VdrPrcs.FromSqlRaw(@"SELECT
	                    vdr_prc.*
                    FROM
	                    vdr_prc,
	                    sentence
                    WHERE
	                    vdr_prc.sentenceid = sentence.sentenceid
	                    AND sentence.vdr_id = ?vdrId
	                    AND sentence.delete_time IS NULL
	                    AND vdr_prc.delete_time IS NULL
	                    AND TIMESTAMPDIFF(
		                    MINUTE,
		                    STR_TO_DATE( sentence.TIME, '%Y-%m-%d %H:%i:%s' ),
	                    SYSDATE())<15
                    ORDER BY
	                    sentence.TIME DESC,
	                    vdr_prc.Id DESC
	                    LIMIT 1", parameters.ToArray());
                var vdr_rmc = _context.VdrRmcs.FromSqlRaw(@"SELECT
	                    vdr_rmc.*
                    FROM
	                    vdr_rmc,
	                    sentence
                    WHERE
	                    vdr_rmc.sentenceid = sentence.sentenceid
	                    AND sentence.vdr_id = ?vdrId
	                    AND sentence.delete_time IS NULL
	                    AND vdr_rmc.delete_time IS NULL
	                    AND TIMESTAMPDIFF(
		                    MINUTE,
		                    STR_TO_DATE( sentence.TIME, '%Y-%m-%d %H:%i:%s' ),
	                    SYSDATE())<15
                    ORDER BY
	                    sentence.TIME DESC,
	                    vdr_rmc.Id DESC
	                    LIMIT 1", parameters.ToArray());
                var vdr_rpm = _context.VdrRpms.FromSqlRaw(@"SELECT
	                    vdr_rpm.*
                    FROM
	                    vdr_rpm,
	                    sentence
                    WHERE
	                    vdr_rpm.sentenceid = sentence.sentenceid
	                    AND sentence.vdr_id = ?vdrId
	                    AND sentence.delete_time IS NULL
	                    AND vdr_rpm.delete_time IS NULL
	                    AND TIMESTAMPDIFF(
		                    MINUTE,
		                    STR_TO_DATE( sentence.TIME, '%Y-%m-%d %H:%i:%s' ),
	                    SYSDATE())<15
                    ORDER BY
	                    sentence.TIME DESC,
	                    vdr_rpm.Id DESC
	                    LIMIT 1", parameters.ToArray());
                var vdr_trc = _context.VdrTrcs.FromSqlRaw(@"SELECT
	                    vdr_trc.*
                    FROM
	                    vdr_trc,
	                    sentence
                    WHERE
	                    vdr_trc.sentenceid = sentence.sentenceid
	                    AND sentence.vdr_id = ?vdrId
	                    AND sentence.delete_time IS NULL
	                    AND vdr_trc.delete_time IS NULL
	                    AND TIMESTAMPDIFF(
		                    MINUTE,
		                    STR_TO_DATE( sentence.TIME, '%Y-%m-%d %H:%i:%s' ),
	                    SYSDATE())<15
                    ORDER BY
	                    sentence.TIME DESC,
	                    vdr_trc.Id DESC
	                    LIMIT 1", parameters.ToArray());
                var vdr_trd = _context.VdrTrds.FromSqlRaw(@"SELECT
	                    vdr_trd.*
                    FROM
	                    vdr_trd,
	                    sentence
                    WHERE
	                    vdr_trd.sentenceid = sentence.sentenceid
	                    AND sentence.vdr_id = ?vdrId
	                    AND sentence.delete_time IS NULL
	                    AND vdr_trd.delete_time IS NULL
	                    AND TIMESTAMPDIFF(
		                    MINUTE,
		                    STR_TO_DATE( sentence.TIME, '%Y-%m-%d %H:%i:%s' ),
	                    SYSDATE())<15
                    ORDER BY
	                    sentence.TIME DESC,
	                    vdr_trd.Id DESC
	                    LIMIT 1", parameters.ToArray());
                var vdr_vbw = _context.VdrVbws.FromSqlRaw(@"SELECT
	                    vdr_vbw.*
                    FROM
	                    vdr_vbw,
	                    sentence
                    WHERE
	                    vdr_vbw.sentenceid = sentence.sentenceid
	                    AND sentence.vdr_id = ?vdrId
	                    AND sentence.delete_time IS NULL
	                    AND vdr_vbw.delete_time IS NULL
	                    AND TIMESTAMPDIFF(
		                    MINUTE,
		                    STR_TO_DATE( sentence.TIME, '%Y-%m-%d %H:%i:%s' ),
	                    SYSDATE())<15
                    ORDER BY
	                    sentence.TIME DESC,
	                    vdr_vbw.Id DESC
	                    LIMIT 1", parameters.ToArray());
                var vdr_vlw = _context.VdrVlws.FromSqlRaw(@"SELECT
	                    vdr_vlw.*
                    FROM
	                    vdr_vlw,
	                    sentence
                    WHERE
	                    vdr_vlw.sentenceid = sentence.sentenceid
	                    AND sentence.vdr_id = ?vdrId
	                    AND sentence.delete_time IS NULL
	                    AND vdr_vlw.delete_time IS NULL
	                    AND TIMESTAMPDIFF(
		                    MINUTE,
		                    STR_TO_DATE( sentence.TIME, '%Y-%m-%d %H:%i:%s' ),
	                    SYSDATE())<15
                    ORDER BY
	                    sentence.TIME DESC,
	                    vdr_vlw.Id DESC
	                    LIMIT 1", parameters.ToArray());
                var vdr_vtg = _context.VdrVtgs.FromSqlRaw(@"SELECT
	                    vdr_vtg.*
                    FROM
	                    vdr_vtg,
	                    sentence
                    WHERE
	                    vdr_vtg.sentenceid = sentence.sentenceid
	                    AND sentence.vdr_id = ?vdrId
	                    AND sentence.delete_time IS NULL
	                    AND vdr_vtg.delete_time IS NULL
	                    AND TIMESTAMPDIFF(
		                    MINUTE,
		                    STR_TO_DATE( sentence.TIME, '%Y-%m-%d %H:%i:%s' ),
	                    SYSDATE())<15
                    ORDER BY
	                    sentence.TIME DESC,
	                    vdr_vtg.Id DESC
	                    LIMIT 1", parameters.ToArray());
                var vdr_xdr = _context.VdrXdrs.FromSqlRaw(@"SELECT
	                    vdr_xdr.*
                    FROM
	                    vdr_xdr,
	                    sentence
                    WHERE
	                    vdr_xdr.sentenceid = sentence.sentenceid
	                    AND sentence.vdr_id = ?vdrId
	                    AND sentence.delete_time IS NULL
	                    AND vdr_xdr.delete_time IS NULL
	                    AND TIMESTAMPDIFF(
		                    MINUTE,
		                    STR_TO_DATE( sentence.TIME, '%Y-%m-%d %H:%i:%s' ),
	                    SYSDATE())<15
                    ORDER BY
	                    sentence.TIME DESC,
	                    vdr_xdr.Id DESC
	                    LIMIT 1", parameters.ToArray());

                #endregion VDR数据 可以考虑定制化

                var result = new
                {
                    vdr_dpt = vdr_dpt,
                    vdr_gga = vdr_gga,
                    vdr_gns = vdr_gns,
                    vdr_hdg = vdr_hdg,
                    vdr_mwd = vdr_mwd,
                    vdr_mwv = vdr_mwv,
                    vdr_prc = vdr_prc,
                    vdr_rmc = vdr_rmc,
                    vdr_rpm = vdr_rpm,
                    vdr_trc = vdr_trc,
                    vdr_trd = vdr_trd,
                    vdr_vbw = vdr_vbw,
                    vdr_vlw = vdr_vlw,
                    vdr_vtg = vdr_vtg,
                    vdr_xdr = vdr_xdr
                };
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取历史数据
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <param name="sorting"></param>
        /// <returns></returns>
        public async Task<object> GetHistoryAsync(string vdrId, int pageNum, int pageCount, string sorting, string asc, string dateFrom, string dateTo)
        {
            try
            {
                var _context = await _dbContextProvider.GetDbContextAsync();

                var parameters = new List<DbParameter>
                {
                    new MySqlParameter { ParameterName = "?vdrId", Value = vdrId }
                };

                StringBuilder sbSql = new StringBuilder();

                #region VDR采集数据 可以考虑定制化

                sbSql.Append(@"SELECT
					t2.`time`,
					(
					SELECT
						depth
					FROM
						vdr_dpt,
						sentence
					WHERE
						vdr_dpt.sentenceid = sentence.sentenceid
						AND vdr_dpt.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_dpt_depth,
					(
					SELECT
						`offset`
					FROM
						vdr_dpt,
						sentence
					WHERE
						vdr_dpt.sentenceid = sentence.sentenceid
						AND vdr_dpt.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_dpt_offset,
					(
					SELECT
						longtitude
					FROM
						vdr_gns,
						sentence
					WHERE
						vdr_gns.sentenceid = sentence.sentenceid
						AND vdr_gns.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_gns_longtitude,
					(
					SELECT
						latitude
					FROM
						vdr_gns,
						sentence
					WHERE
						vdr_gns.sentenceid = sentence.sentenceid
						AND vdr_gns.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_gns_latitude,
					(
					SELECT
						satnum
					FROM
						vdr_gns,
						sentence
					WHERE
						vdr_gns.sentenceid = sentence.sentenceid
						AND vdr_gns.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_gns_satnum,
					(
					SELECT
						antennaaltitude
					FROM
						vdr_gns,
						sentence
					WHERE
						vdr_gns.sentenceid = sentence.sentenceid
						AND vdr_gns.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_gns_antennaaltitude,
					(
					SELECT
						tdirection
					FROM
						vdr_mwd,
						sentence
					WHERE
						vdr_mwd.sentenceid = sentence.sentenceid
						AND vdr_mwd.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_mwd_tdirection,
					(
					SELECT
						magdirection
					FROM
						vdr_mwd,
						sentence
					WHERE
						vdr_mwd.sentenceid = sentence.sentenceid
						AND vdr_mwd.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_mwd_magdirection,
					(
					SELECT
						knspeed
					FROM
						vdr_mwd,
						sentence
					WHERE
						vdr_mwd.sentenceid = sentence.sentenceid
						AND vdr_mwd.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_mwd_knspeed,
					(
					SELECT
						speed
					FROM
						vdr_mwd,
						sentence
					WHERE
						vdr_mwd.sentenceid = sentence.sentenceid
						AND vdr_mwd.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_mwd_speed,
					(
					SELECT
						source
					FROM
						vdr_rpm,
						sentence
					WHERE
						vdr_rpm.sentenceid = sentence.sentenceid
						AND vdr_rpm.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_rpm_source,
					(
					SELECT
						number
					FROM
						vdr_rpm,
						sentence
					WHERE
						vdr_rpm.sentenceid = sentence.sentenceid
						AND vdr_rpm.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_rpm_number,
					(
					SELECT
						speed
					FROM
						vdr_rpm,
						sentence
					WHERE
						vdr_rpm.sentenceid = sentence.sentenceid
						AND vdr_rpm.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_rpm_speed,
					(
					SELECT
						propellerpitch
					FROM
						vdr_rpm,
						sentence
					WHERE
						vdr_rpm.sentenceid = sentence.sentenceid
						AND vdr_rpm.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_rpm_propellerpitch,
					(
					SELECT
						watspd
					FROM
						vdr_vbw,
						sentence
					WHERE
						vdr_vbw.sentenceid = sentence.sentenceid
						AND vdr_vbw.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_vbw_watspd,
					(
					SELECT
						grdspd
					FROM
						vdr_vbw,
						sentence
					WHERE
						vdr_vbw.sentenceid = sentence.sentenceid
						AND vdr_vbw.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_vbw_grdspd,
					(
					SELECT
						grdcoztrue
					FROM
						vdr_vtg,
						sentence
					WHERE
						vdr_vtg.sentenceid = sentence.sentenceid
						AND vdr_vtg.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_vtg_grdcoztrue,
					(
					SELECT
						grdcozmag
					FROM
						vdr_vtg,
						sentence
					WHERE
						vdr_vtg.sentenceid = sentence.sentenceid
						AND vdr_vtg.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_vtg_grdcozmag,
					(
					SELECT
						grdspdknot
					FROM
						vdr_vtg,
						sentence
					WHERE
						vdr_vtg.sentenceid = sentence.sentenceid
						AND vdr_vtg.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_vtg_grdspdknot,
					(
					SELECT
						grdspdkm
					FROM
						vdr_vtg,
						sentence
					WHERE
						vdr_vtg.sentenceid = sentence.sentenceid
						AND vdr_vtg.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_vtg_grdspdkm,
					(
					SELECT
						watdistotal
					FROM
						vdr_vlw,
						sentence
					WHERE
						vdr_vlw.sentenceid = sentence.sentenceid
						AND vdr_vlw.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_vlw_watdistotal,
					(
					SELECT
						watdisreset
					FROM
						vdr_vlw,
						sentence
					WHERE
						vdr_vlw.sentenceid = sentence.sentenceid
						AND vdr_vlw.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_vlw_watdisreset,
					(
					SELECT
						grddistotal
					FROM
						vdr_vlw,
						sentence
					WHERE
						vdr_vlw.sentenceid = sentence.sentenceid
						AND vdr_vlw.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_vlw_grddistotal,
					(
					SELECT
						grddisreset
					FROM
						vdr_vlw,
						sentence
					WHERE
						vdr_vlw.sentenceid = sentence.sentenceid
						AND vdr_vlw.delete_time IS NULL
						AND sentence.delete_time IS NULL
						AND sentence.`time` = t2.`time`
					) history_vlw_grddisreset
				FROM
					sentence t2
				WHERE
					t2.delete_time IS NULL
					AND t2.`vdr_id` = ?vdrId ");

                #endregion VDR采集数据 可以考虑定制化

                if (!string.IsNullOrWhiteSpace(dateFrom))
                {
                    sbSql.Append("AND t2.`time`>=?from ");
                    parameters.Add(new MySqlParameter { ParameterName = "?from", Value = dateFrom });
                }
                if (!string.IsNullOrWhiteSpace(dateTo))
                {
                    sbSql.Append("AND t2.`time`<?to ");
                    parameters.Add(new MySqlParameter { ParameterName = "?to", Value = dateTo });
                }
                sbSql.Append(@"GROUP BY
					t2.`time`
				ORDER BY ");
                sorting = string.IsNullOrWhiteSpace(sorting) ? "time" : sorting;
                asc = asc.Equals("desc") ? "desc" : asc.Equals("asc") ? "asc" : "desc";
                sbSql.Append($"t2.`{sorting}` {asc}");
                var historyData = _context.vdrTotalEntities.FromSqlRaw(sbSql.ToString(), parameters.ToArray()).PageBy((pageNum - 1) * pageCount, pageCount);

                #region 获取总结果数 historyDataCount

                StringBuilder sbSqlCount = new StringBuilder();
                sbSqlCount.Append(@"SELECT
						t2.`time`
					FROM
						sentence t2
					WHERE
						t2.delete_time IS NULL
						AND t2.`vdr_id` = ?vdrId ");
                if (!string.IsNullOrWhiteSpace(dateFrom))
                {
                    sbSqlCount.Append("AND t2.`time`>?from ");
                    parameters.Add(new MySqlParameter { ParameterName = "?from", Value = dateFrom });
                }
                if (!string.IsNullOrWhiteSpace(dateTo))
                {
                    sbSqlCount.Append("AND t2.`time`<?to ");
                    parameters.Add(new MySqlParameter { ParameterName = "?to", Value = dateTo });
                }
                sbSqlCount.Append(@"GROUP BY
					t2.`time`");
                var historyDataCount = _context.vdrTotalEntities.FromSqlRaw(sbSqlCount.ToString(), parameters.ToArray()).Count();

                #endregion 获取总结果数 historyDataCount

                var historyResult = new
                {
                    pagenum = pageNum,
                    pagecount = pageCount,
                    sorting = sorting,
                    resultcount = historyDataCount,
                    data = historyData
                };
                return historyResult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

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
        public async Task<IEnumerable<VdrTotalEntity>> GetAnalyseAsync(string vdrId, long? dateFrom, long? dateTo, float? slipFrom, float? slipTo, float? draftFrom, float? draftTo, string analyseType, float propPitch)
        {
            try
            {
                var _context = await _dbContextProvider.GetDbContextAsync();
                //        var parameters = new List<DbParameter>
                //        {
                //            new MySqlParameter { ParameterName = "?vdrId", Value = vdrId }
                //        };

                //        StringBuilder sbSql = new StringBuilder();
                //        sbSql.Append(@"SELECT * FROM sentence
                //WHERE delete_time IS NUll ");
                //        sbSql.Append("AND vdr_id=?vdrId ");
                //        parameters.Add(new MySqlParameter { ParameterName = "?vdrId", Value = vdrId });
                //        switch (analyseType)
                //        {
                //            case "speed":
                //                if (dateFrom != null)
                //                {
                //                    sbSql.Append("AND `time`>=?dateFrom ");
                //                    parameters.Add(new MySqlParameter { ParameterName = "?dateFrom", Value = dateFrom });
                //                }
                //                if (dateTo != null)
                //                {
                //                    sbSql.Append("AND `time`<?dateTo ");
                //                    parameters.Add(new MySqlParameter { ParameterName = "?dateTo", Value = dateTo });
                //                }
                //                break;
                //        }
                //        var sentenceEntities = _context.Sentences.FromSqlRaw(sbSql.ToString(), parameters.ToArray());
                var sentenceEntities = _context.Sentences.Where(t => t.delete_time == null && t.vdr_id == vdrId && t.time >= (dateFrom ?? 0) && t.time <= (dateTo ?? 0));
                IList<VdrTotalEntity> resultEntities = new List<VdrTotalEntity>();
                var lstCurrentDatetime = new List<long>();
                foreach (var entity in sentenceEntities)
                {
                    if (lstCurrentDatetime.Contains(entity.time))
                        continue;
                    else
                    {
                        try
                        {
                            lstCurrentDatetime.Add(entity.time);
                            var SameTimeSet = sentenceEntities.Where(t => t.time == entity.time);
                            VdrVbw vdrVbw = new VdrVbw((await SameTimeSet.FirstOrDefaultAsync(t => t.category == "vbw"))?.data);

                            NotVdrEntity notVdrEntity = new NotVdrEntity((await SameTimeSet.FirstOrDefaultAsync(t => t.category == "notvdr")).data, vdrId, StaticVoyageData.DictFilter);

                            SameTimeSet.First(t => t.category == "dpt");

                            var slip = (1 - (vdrVbw.watspd / (notVdrEntity.MERpm * propPitch) * 1852f * 1000f / 60f)) * 100f;
                            if (slip < slipFrom || slip >= slipTo || slip == null)
                                continue;
                            var draft = (notVdrEntity.DraftBow + notVdrEntity.DraftAstern) / 2f;
                            if (draft < draftFrom || draft >= draftTo || draft == null)
                                continue;

                            VdrDpt vdrDpt = new VdrDpt((await SameTimeSet.FirstOrDefaultAsync(t => t.category == "dpt"))?.data);
                            VdrGns vdrGns = new VdrGns((await SameTimeSet.FirstOrDefaultAsync(t => t.category == "gns"))?.data);
                            VdrMwd vdrMwd = new VdrMwd((await SameTimeSet.FirstOrDefaultAsync(t => t.category == "mwd"))?.data);
                            VdrRpm vdrRpm = new VdrRpm((await SameTimeSet.FirstOrDefaultAsync(t => t.category == "rpm"))?.data);
                            VdrVtg vdrVtg = new VdrVtg((await SameTimeSet.FirstOrDefaultAsync(t => t.category == "vtg"))?.data);
                            VdrVlw vdrVlw = new VdrVlw((await SameTimeSet.FirstOrDefaultAsync(t => t.category == "vlw"))?.data);

                            resultEntities.Add(new VdrTotalEntity
                            {
                                time = DateTimeOffset.FromUnixTimeSeconds(entity.time).LocalDateTime,
                                history_dpt_depth = vdrDpt.depth,
                                history_dpt_offset = vdrDpt.offset,
                                history_gns_longtitude = vdrGns.longtitude,
                                history_gns_latitude = vdrGns.latitude,
                                history_gns_satnum = vdrGns.satnum,
                                history_gns_antennaaltitude = vdrGns.antennaaltitude,
                                history_mwd_tdirection = vdrMwd.tdirection,
                                history_mwd_magdirection = vdrMwd.magdirection,
                                history_mwd_knspeed = vdrMwd.knspeed,
                                history_mwd_speed = vdrMwd.speed,
                                history_rpm_source = vdrRpm.source,
                                history_rpm_number = vdrRpm.number,
                                history_rpm_speed = vdrRpm.speed,
                                history_rpm_propellerpitch = vdrRpm.propellerpitch,
                                history_vbw_watspd = vdrVbw.watspd,
                                history_vbw_grdspd = vdrVbw.grdspd,
                                history_vtg_grdcoztrue = vdrVtg.grdcoztrue,
                                history_vtg_grdcozmag = vdrVtg.grdcozmag,
                                history_vtg_grdspdknot = vdrVtg.grdspdknot,
                                history_vtg_grdspdkm = vdrVtg.grdspdkm,
                                history_vlw_watdistotal = vdrVlw.watdistotal,
                                history_vlw_watdisreset = vdrVlw.watdisreset,
                                history_vlw_grddistotal = vdrVlw.grddistotal,
                                history_vlw_grddisreset = vdrVlw.grddisreset,
                                history_draft_trim = notVdrEntity.DraftAstern - notVdrEntity.DraftBow,
                                history_draft_heel = notVdrEntity.DraftPort - notVdrEntity.DraftStarboard,
                                history_draft_draft = draft,
                                history_power_rpm = notVdrEntity.MERpm,
                                history_power_power = notVdrEntity.MEPower,
                                history_power_slip = slip,
                                history_flowmeter_me_fcpernm = notVdrEntity.MEFC / vdrVbw.watspd,
                                history_flowmeter_me_fcperpow = notVdrEntity.MEFC / notVdrEntity.MEPower,
                                history_flowmeter_fcpernm = (notVdrEntity.MEFC + notVdrEntity.DGInFC - notVdrEntity.DGOutFC + notVdrEntity.DGMgoFC + notVdrEntity.BlrFC + notVdrEntity.BlrMGOFC) / vdrVbw.watspd,
                                history_flowmeter_fcperpow = (notVdrEntity.MEFC + notVdrEntity.DGInFC - notVdrEntity.DGOutFC + notVdrEntity.DGMgoFC + notVdrEntity.BlrFC + notVdrEntity.BlrMGOFC) / (notVdrEntity.MEPower + notVdrEntity.DG1Power + notVdrEntity.DG2Power + notVdrEntity.DG3Power)
                            });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                            continue;
                        }
                    }
                }

                return resultEntities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }
    }
}