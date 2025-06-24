using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace hmt_energy_csharp.WhiteLists
{
    public class WhiteListAppService : ApplicationService, IWhiteListAppService
    {
        private readonly ILogger<WhiteListAppService> _logger;
        private readonly IWhiteListRepository _repository;

        public WhiteListAppService(ILogger<WhiteListAppService> logger, IWhiteListRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<WhiteListDto> GetByTarget(string targetId, string targetIp)
        {
            return null;
        }

        public async Task<bool> IsInWhiteListAsync(string targetId, string targetIp)
        {
            var result = await _repository.AnyAsync(t => t.TargetIp == targetIp);
            return result;
        }

        public async Task<WhiteListDto> CreateAsync(WhiteListDto input)
        {
            WhiteList entity = new WhiteList();
            entity.TargetId = input.TargetId;
            entity.TargetIp = input.TargetIp;
            await _repository.InsertAsync(entity);
            return ObjectMapper.Map<WhiteList, WhiteListDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<WhiteListDto> GetAsync(Guid id)
        {
            var entity = await _repository.GetAsync(id);
            return ObjectMapper.Map<WhiteList, WhiteListDto>(entity);
        }

        public async Task<PagedResultDto<WhiteListDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var entities = await _repository.GetListAsync();

            var totalCount = await _repository.CountAsync();

            return new PagedResultDto<WhiteListDto>(totalCount, ObjectMapper.Map<List<WhiteList>, List<WhiteListDto>>(entities));
        }

        public async Task HardDeleteAsync(Guid id)
        {
            await _repository.HardDeleteAsync(t => t.Id == id);
        }

        public async Task UpdateAsync(Guid id, WhiteListDto input)
        {
            var entity = await _repository.GetAsync(id);
            entity.TargetId = input.TargetId;
            entity.TargetIp = input.TargetIp;

            await _repository.UpdateAsync(entity);
        }
    }
}