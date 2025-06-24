using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace hmt_energy_csharp.WhiteLists
{
    public interface IWhiteListAppService : IApplicationService
    {
        Task<bool> IsInWhiteListAsync(string targetId, string targetIp);

        Task<WhiteListDto> GetByTarget(string targetId, string targetIp);

        Task<WhiteListDto> GetAsync(Guid id);

        Task<PagedResultDto<WhiteListDto>> GetListAsync(PagedAndSortedResultRequestDto input);

        Task<WhiteListDto> CreateAsync(WhiteListDto input);

        Task UpdateAsync(Guid id, WhiteListDto input);

        Task DeleteAsync(Guid id);

        Task HardDeleteAsync(Guid id);
    }
}