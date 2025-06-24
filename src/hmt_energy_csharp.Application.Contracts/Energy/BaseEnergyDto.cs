using System;
using Volo.Abp.Application.Dtos;

namespace hmt_energy_csharp.Energy
{
    public class BaseEnergyDto : EntityDto<long>
    {
        //采集系统设备号
        public string Number { get; set; }

        //航行信息时间
        public DateTime ReceiveDatetime { get; set; }

        //设备编号
        public string DeviceNo { get; set; }
    }
}