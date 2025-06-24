using System;
using Volo.Abp.Domain.Entities;

namespace hmt_energy_csharp.ConnEntities
{
    public class ConnEntity : BasicAggregateRoot<int>
    {
        public DateTime? create_time { get; set; } = DateTime.Now;
        public DateTime? update_time { get; set; }
        public DateTime? delete_time { get; set; }
    }
}