using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace hmt_energy_csharp.Energy.Configs
{
    public interface IConfigService : IApplicationService
    {
        Task<ConfigDto> Add(ConfigDto dto);

        Task<ConfigDto> Update(int id, ConfigDto dto);

        Task Delete(int id);

        Task<ConfigDto> Get(ConfigDto dto);

        Task<IList<ConfigDto>> GetList(string Filter);

        Task<object> GetPage(int pageNum, int countPerPage, string sorting, string asc, string parameters);
    }
}