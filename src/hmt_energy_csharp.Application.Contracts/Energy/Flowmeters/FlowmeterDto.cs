using System;

namespace hmt_energy_csharp.Energy.Flowmeters
{
    /**
     * 流量计
     **/

    public class FlowmeterDto : BaseEnergyDto
    {
        //瞬时消耗 kg/h
        public decimal? ConsAct { get; set; }

        //累计消耗 kg/h
        public decimal? ConsAcc { get; set; }

        //温度 ℃
        public decimal? Temperature { get; set; }

        //密度 kg/m3
        public decimal? Density { get; set; }

        //耗能单元类型 主机、辅机、锅炉等
        public string DeviceType { get; set; }

        //耗能类型 重油、轻油、甲醇等
        public string FuelType { get; set; }

        //是否已上传
        public byte Uploaded { get; set; } = 0;
    }
}