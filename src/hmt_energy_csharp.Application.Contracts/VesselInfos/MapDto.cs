using System;
using Volo.Abp.Application.Dtos;

namespace hmt_energy_csharp.VesselInfos
{
    public class MapDto : EntityDto<int>
    {
        //船id
        public string SN { get; set; }

        //时间
        public DateTime ReceiveDatetime { get; set; }

        //经度
        public double? Longitude { get; set; }

        //纬度
        public double? Latitude { get; set; }

        //主机能效
        public double? MESFOC { get; set; }

        //对水航速
        public double? WaterSpeed { get; set; }

        //主机重油油耗
        public double? MEHFOConsumption { get; set; }

        //辅机重油油耗
        public double? DGHFOConsumption { get; set; }

        //锅炉重油油耗
        public double? BLRHFOConsumption { get; set; }

        //全船能效
        public double? SFOC { get; set; }

        //全船每海里油耗
        public double? FCPerNm { get; set; }

        //锅炉重油累计消耗
        public double? BLGHFOCACC { get; set; }

        //锅炉轻油累计消耗
        public double? BLGMDOCACC { get; set; }

        //辅机重油累计消耗
        public double? DGHFOCACC { get; set; }

        //辅机轻油累计消耗
        public double? DGMDOCACC { get; set; }

        //主机重油累计消耗
        public double? MEHFOCACC { get; set; }

        //主机轻油累计消耗
        public double? MEMDOCACC { get; set; }
    }
}