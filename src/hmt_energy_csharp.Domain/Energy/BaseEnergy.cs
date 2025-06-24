using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace hmt_energy_csharp.Energy
{
    /* *
     * 能效基础类
     * */

    public class BaseEnergy : BasicAggregateRoot<long>
    {
        //采集系统设备号
        public string Number { get; set; }

        //航行信息时间
        public DateTime ReceiveDatetime { get; set; }

        //设备编号
        public string DeviceNo { get; set; }
    }
}