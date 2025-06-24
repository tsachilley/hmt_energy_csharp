using System;
using Volo.Abp.Application.Dtos;

namespace hmt_energy_csharp.WhiteLists
{
    public class WhiteListDto : EntityDto<Guid>
    {
        public string TargetId { get; set; }

        public string TargetIp { get; set; }
    }
}