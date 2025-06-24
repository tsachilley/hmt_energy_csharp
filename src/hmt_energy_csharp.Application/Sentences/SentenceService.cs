using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Sentences
{
    public class SentenceService : hmt_energy_csharpAppService, ISentenceService
    {
        private readonly ISentenceRepository _repository;
        private readonly ILogger<SentenceService> _logger;

        public SentenceService(ISentenceRepository repository, ILogger<SentenceService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<SentenceDto> CreateAsync(CreateSentenceDto dto)
        {
            var entity = new Sentence
            {
                data = dto.data,
                time = dto.time,
                vdr_id = dto.vdr_id,
                /*sentenceid = dto.sentenceid,
                isdecoded = dto.isdecoded*/
                category = dto.category
            };
            await _repository.InsertAsync(entity);
            return ObjectMapper.Map<Sentence, SentenceDto>(entity);
        }

        /// <summary>
        /// 获取时间段内数据
        /// </summary>
        /// <param name="vdrId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SentenceDto>> GetListByDateVdrAsync(string vdrId, long? dateFrom, long? dateTo)
        {
            try
            {
                var sentences = await _repository.GetListByDateVdrAsync(vdrId, dateFrom, dateTo);
                return ObjectMapper.Map<IEnumerable<Sentence>, IEnumerable<SentenceDto>>(sentences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// 获取最新数据
        /// </summary>
        /// <param name="vdrId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SentenceDto>> GetTop1TimeByVdrAsync(string vdrId)
        {
            try
            {
                var sentences = await _repository.GetTop1TimeByVdrAsync(vdrId);
                return ObjectMapper.Map<IEnumerable<Sentence>, IEnumerable<SentenceDto>>(sentences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// 获取最新数据
        /// </summary>
        /// <param name="vdrId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SentenceDto>> GetPageListAsync(string vdrId, int pageNum, int pageCount, string sorting, string asc, long dateFrom, long dateTo)
        {
            try
            {
                var sentences = await _repository.GetPageListAsync(vdrId, pageNum, pageCount, sorting, asc, dateFrom, dateTo);
                return ObjectMapper.Map<IEnumerable<Sentence>, IEnumerable<SentenceDto>>(sentences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// 获取最新数据
        /// </summary>
        /// <param name="vdrId"></param>
        /// <returns></returns>
        public async Task<int> GetResultCountAsync(string vdrId, long dateFrom, long dateTo)
        {
            try
            {
                return await _repository.GetResultCountAsync(vdrId, dateFrom, dateTo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MethodBase.GetCurrentMethod().DeclaringType.Namespace + "_" + MethodBase.GetCurrentMethod().Name);
                return 0;
            }
        }
    }
}