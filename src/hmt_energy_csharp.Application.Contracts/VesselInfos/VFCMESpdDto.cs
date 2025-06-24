using System;

namespace hmt_energy_csharp.VesselInfos
{
    public class VFCMESpdDto
    {
        //时间
        public DateTime ReceiveDatetime { get; set; }

        //对水航速
        public double? WaterSpeed { get; set; }

        //全船能效
        public double? SFOC { get; set; }

        //全船每海里油耗
        public double? FCPerNm { get; set; }
    }
}