using hmt_energy_csharp.Energy;

namespace hmt_energy_csharp.Engineroom.ShaftClutches
{
    /**
     * 推进机械轴系 & 离合器
     */

    public class ShaftClutchDto : BaseEnergyDto
    {
        //尾管后轴承温度 2101
        public double? SternAftTemp { get; set; }

        //中间轴承温度 2102
        public double? InterTemp { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;
    }
}