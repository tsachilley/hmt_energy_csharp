using hmt_energy_csharp.Energy;

namespace hmt_energy_csharp.Engineroom.CoolingSeaWaters
{
    /**
     * 冷却海水系统
     */

    public class CoolingSeaWaterDto : BaseEnergyDto
    {
        //冷却海水泵出口压力 5311
        public double? CSWOutPress { get; set; }

        //冷却海水泵出口温度 5312
        public double? CSWOutTemp { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;
    }
}