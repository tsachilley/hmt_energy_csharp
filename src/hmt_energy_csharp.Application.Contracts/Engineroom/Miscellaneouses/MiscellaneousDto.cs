using hmt_energy_csharp.Energy;

namespace hmt_energy_csharp.Engineroom.Miscellaneouses
{
    /**
     * 其他
     */

    public class MiscellaneousDto : BaseEnergyDto
    {
        //主机曲轴箱油雾浓度高 1831 1832
        public int? MECCOMHigh { get; set; }

        //主机轴向振动高 1834 1835
        public double? MEAxialVibration { get; set; }

        //主机负荷 1850
        public double? MELoad { get; set; }

        //主机增压器转速 1853
        public double? METCSpeed { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;
    }
}