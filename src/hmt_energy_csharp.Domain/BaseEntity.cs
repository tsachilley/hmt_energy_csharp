using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace hmt_energy_csharp
{
    public class BaseEntity : BasicAggregateRoot<long>
    {
        //创建时间
        public DateTime? create_time { get; set; }

        //更新时间
        public DateTime? update_time { get; set; }

        //删除时间
        public DateTime? delete_time { get; set; }
    }
}