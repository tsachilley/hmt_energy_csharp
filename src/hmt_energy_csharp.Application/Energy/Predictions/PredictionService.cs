using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace hmt_energy_csharp.Energy.Predictions
{
    public class PredictionService : hmt_energy_csharpAppService, IPredictionService
    {
        private readonly IPredictionRepository _predictionRepository;

        public PredictionService(IPredictionRepository predictionRepository)
        {
            _predictionRepository = predictionRepository;
        }

        /// <summary>
        /// 根据采集系统序列号和时间获取对象
        /// </summary>
        /// <param name="number"></param>
        /// <param name="receviceDatetime"></param>
        /// <returns></returns>
        public async Task<PredictionDto> GetByNumberReceiveDatetimeAsync(string number, DateTime receviceDatetime)
        {
            var result = await _predictionRepository.FirstOrDefaultAsync(t => t.Number == number && t.ReceiveDatetime == receviceDatetime);
            return ObjectMapper.Map<Prediction, PredictionDto>(result);
        }
    }
}