using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace hmt_energy_csharp.WhiteLists
{
    public class WhiteList : FullAuditedAggregateRoot<Guid>
    {
        public string TargetId { get; set; }
        public string TargetIp { get; set; }
    }
}