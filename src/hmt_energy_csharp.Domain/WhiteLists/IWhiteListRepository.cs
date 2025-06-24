using System;
using Volo.Abp.Domain.Repositories;

namespace hmt_energy_csharp.WhiteLists
{
    public interface IWhiteListRepository : IRepository<WhiteList, Guid>
    {
    }
}