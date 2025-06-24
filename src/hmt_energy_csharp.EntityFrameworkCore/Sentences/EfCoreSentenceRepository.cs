using hmt_energy_csharp.EntityFrameworkCore.MySql;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Sentences
{
    public class EfCoreSentenceRepository : EfCoreRepository<hmt_energy_csharpDbContext, Sentence, int>, ISentenceRepository
    {
        public EfCoreSentenceRepository(IDbContextProvider<hmt_energy_csharpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        /// <summary>
        /// 获取时间段内数据
        /// </summary>
        /// <param name="vdrId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Sentence>> GetListByDateVdrAsync(string vdrId, long? dateFrom, long? dateTo)
        {
            try
            {
                var dbset = await GetDbSetAsync();
                return dbset.Where(t => t.vdr_id == vdrId && t.delete_time == null && t.time >= (dateFrom ?? 0) && t.time < (dateTo ?? long.MaxValue));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取最新数据
        /// </summary>
        /// <param name="vdrId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Sentence>> GetTop1TimeByVdrAsync(string vdrId)
        {
            try
            {
                var dbSet = await GetDbSetAsync();
                return dbSet.FromSqlInterpolated($"SELECT * FROM sentence WHERE TIME =( SELECT TIME FROM sentence WHERE TIME <( SELECT TIME FROM sentence WHERE delete_time IS NULL AND vdr_id = {vdrId} ORDER BY TIME DESC LIMIT 1 ) AND delete_time IS NULL AND vdr_id = {vdrId} ORDER BY TIME DESC LIMIT 1 )");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Sentence>> GetPageListAsync(string vdrId, int pageNum, int pageCount, string sorting, string asc, long dateFrom, long dateTo)
        {
            try
            {
                var dbSet = await GetDbSetAsync();
                if (pageNum >= 0)
                {
                    return dbSet.FromSqlRaw($"select sentence.* from (select time from sentence where delete_time is null and vdr_id='{vdrId}' {(dateFrom == long.MinValue ? "" : "and time>=" + dateFrom + " ")} {(dateTo == long.MaxValue ? "" : "and time<" + dateTo + " ")} group by time order by time {asc} LIMIT {(pageNum - 1) * pageCount},{pageCount}) t inner join sentence on t.time=sentence.time and sentence.delete_time is null and sentence.vdr_id='{vdrId}' order by sentence.time {asc}");
                }
                else
                {
                    return dbSet.FromSqlRaw($"select * from sentence where sentence.delete_time is null and sentence.vdr_id='{vdrId}' {(dateFrom == long.MinValue ? "" : "and time>=" + dateFrom)} {(dateTo == long.MaxValue ? "" : "and time<" + dateTo)} order by sentence.time {asc}");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> GetResultCountAsync(string vdrId, long dateFrom, long dateTo)
        {
            try
            {
                var dbSet = await GetDbSetAsync();
                return await dbSet.Where(t => t.time >= dateFrom && t.time < dateTo && t.vdr_id == vdrId && t.delete_time == null).Select(t => t.time).Distinct().CountAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}