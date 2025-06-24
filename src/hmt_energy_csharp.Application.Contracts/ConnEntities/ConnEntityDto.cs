using System;
using Volo.Abp.Application.Dtos;

namespace hmt_energy_csharp.ConnEntities
{
    public class ConnEntityDto : EntityDto<int>
    {
        public DateTime? create_time { get; set; }
        public DateTime? update_time { get; set; }
        public DateTime? delete_time { get; set; }
    }
}