using hmt_energy_csharp.Energy;

namespace hmt_energy_csharp.Engineroom.CompressedAirSupplies
{
    /**
     * 主机压缩空气系统
     */

    public class CompressedAirSupplyDto : BaseEnergyDto
    {
        //主机起动空气压力 1501
        public double? MEStartPress { get; set; }

        //主机控制空气进口压力 1502
        public double? MEControlPress { get; set; }

        //主机排气阀弹簧进口空气压力 1503
        public double? ExhaustValuePress { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;
    }
}