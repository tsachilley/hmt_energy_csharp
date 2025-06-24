using hmt_energy_csharp.Energy;

namespace hmt_energy_csharp.Engineroom.CoolingFreshWaters
{
    /**
     * 冷却淡水系统
     */

    public class CoolingFreshWaterDto : BaseEnergyDto
    {
        //低温冷却淡水泵压力 5321
        public double? LTCFWPress { get; set; }

        //中央冷却器低温淡水出口温度 5322
        public double? CCLTCFWOutTemp { get; set; }

        //1号低温冷却淡水泵出口压力 5323C1
        public int? LTCFW1Press { get; set; }

        //2号低温冷却淡水泵出口压力 5323C2
        public int? LTCFW2Press { get; set; }

        //3号低温冷却淡水泵出口压力 5323C3
        public int? LTCFW3Press { get; set; }

        //主机缸套水冷却泵出口压力 5329c
        public int? MEJWCOutPress { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;
    }
}