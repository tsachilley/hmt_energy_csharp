using hmt_energy_csharp.EntityFrameworkCore.MySql;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.WhiteLists
{
    public class EfCoreWhiteListRepository : EfCoreRepository<hmt_energy_csharpDbContext, WhiteList, Guid>, IWhiteListRepository
    {
        public EfCoreWhiteListRepository(IDbContextProvider<hmt_energy_csharpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}