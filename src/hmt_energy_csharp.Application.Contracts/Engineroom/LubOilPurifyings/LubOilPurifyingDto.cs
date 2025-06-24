using hmt_energy_csharp.Energy;

namespace hmt_energy_csharp.Engineroom.LubOilPurifyings
{
    /**
     * 滑油净化系统
     */

    public class LubOilPurifyingDto : BaseEnergyDto
    {
        //主机滑油滤器压差高 5241
        public int? MEFilterPressHigh { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;
    }
}