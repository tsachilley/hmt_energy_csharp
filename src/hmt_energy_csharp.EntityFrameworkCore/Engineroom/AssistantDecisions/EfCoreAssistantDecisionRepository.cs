using hmt_energy_csharp.EntityFrameworkCore.Oracle;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace hmt_energy_csharp.Engineroom.AssistantDecisions
{
    public class EfCoreAssistantDecisionRepository : EfCoreRepository<hmt_energy_csharpOracleDbContext, AssistantDecision, long>, IAssistantDecisionRepository
    {
        public EfCoreAssistantDecisionRepository(IDbContextProvider<hmt_energy_csharpOracleDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}