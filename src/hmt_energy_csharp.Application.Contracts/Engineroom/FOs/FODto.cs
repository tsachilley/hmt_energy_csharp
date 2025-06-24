using hmt_energy_csharp.Energy;

namespace hmt_energy_csharp.Engineroom.FOs
{
    /**
     * 燃油系统
     */

    public class FODto : BaseEnergyDto
    {
        //主机燃油进机压力 1201
        public double? MEInPressure { get; set; }

        //主机燃油进机温度 1202
        public double? MEInTemp { get; set; }

        //主机高压油管泄漏高报警 1203
        public int? MEHPOPLeakage { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;
    }
}