using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace hmt_energy_csharp.Energy.Configs
{
    public class ConfigService : hmt_energy_csharpAppService, IConfigService
    {
        private readonly IConfigRepository _configRepository;

        public ConfigService(IConfigRepository configRepository)
        {
            _configRepository = configRepository;
        }

        public async Task<ConfigDto> Add(ConfigDto dto)
        {
            var config = new Config
            {
                Code = dto.Code,
                Name = dto.Name,
                Interval = dto.Interval,
                HighLimit = dto.HighLimit,
                HighHighLimit = dto.HighHighLimit,
                IsDevice = dto.IsDevice,
                IsEnabled = dto.IsEnabled,
                Number = dto.Number,
                create_time = dto.create_time
            };
            var result = await _configRepository.InsertAsync(config);
            return ObjectMapper.Map<Config, ConfigDto>(result);
        }

        public async Task Delete(int id)
        {
            await _configRepository.DeleteAsync(id);
        }

        public async Task<ConfigDto> Get(ConfigDto dto)
        {
            var result = await _configRepository.GetAsync(dto.Id);
            return ObjectMapper.Map<Config, ConfigDto>(result);
        }

        public async Task<IList<ConfigDto>> GetList(string Filter)
        {
            var queryParams = Filter.ToJObject();
            Expression<Func<Config, bool>> expression = t => true;
            if (queryParams.ContainsKey("IsDevice"))
            {
                expression.And(t => t.IsDevice == Convert.ToByte(queryParams["IsDevice"]));
            }
            if (queryParams.ContainsKey("IsEnabled"))
            {
                expression.And(t => t.IsEnabled == Convert.ToByte(queryParams["IsEnabled"]));
            }
            if (queryParams.ContainsKey("Number"))
            {
                expression.And(t => t.Number == queryParams["Number"].ToString());
            }
            var result = await _configRepository.GetListAsync(expression);
            return ObjectMapper.Map<IList<Config>, IList<ConfigDto>>(result);
        }

        public async Task<object> GetPage(int pageNum, int countPerPage, string sorting, string asc, string parameters)
        {
            var queryParams = parameters.ToJObject();
            Expression<Func<Config, bool>> expression = t => true;
            if (queryParams.ContainsKey("IsDevice"))
            {
                expression = expression.And(t => t.IsDevice == Convert.ToByte(queryParams["IsDevice"]));
            }
            if (queryParams.ContainsKey("IsEnabled"))
            {
                expression = expression.And(t => t.IsEnabled == Convert.ToByte(queryParams["IsEnabled"]));
            }
            if (queryParams.ContainsKey("Number"))
            {
                expression = expression.And(t => t.Number == queryParams["Number"].ToString());
            }
            var data = ObjectMapper.Map<IList<Config>, IList<ConfigDto>>((await _configRepository.GetListAsync(expression)).Skip((pageNum - 1) * countPerPage).Take(countPerPage).OrderByDescending(t => t.Id).ToList());
            var count = (await _configRepository.GetListAsync(expression)).Count();
            var result = new
            {
                count = count,
                data = data,
            };
            return result;
        }

        public async Task<ConfigDto> Update(int id, ConfigDto dto)
        {
            var entity = await _configRepository.GetAsync(id);
            entity.Code = dto.Code;
            entity.Name = dto.Name;
            entity.Interval = dto.Interval;
            entity.HighLimit = dto.HighLimit;
            entity.HighHighLimit = dto.HighHighLimit;
            entity.IsDevice = dto.IsDevice;
            entity.IsEnabled = dto.IsEnabled;
            entity.Number = dto.Number;
            entity.update_time = DateTime.Now;
            var result = await _configRepository.UpdateAsync(entity);
            return ObjectMapper.Map<Config, ConfigDto>(result);
        }
    }
}