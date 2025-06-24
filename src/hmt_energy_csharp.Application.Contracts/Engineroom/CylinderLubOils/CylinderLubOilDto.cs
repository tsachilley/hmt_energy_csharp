using hmt_energy_csharp.Energy;

namespace hmt_energy_csharp.Engineroom.CylinderLubOils
{
    /**
     * 主机气缸滑油
     */

    public class CylinderLubOilDto : BaseEnergyDto
    {
        //主机气缸滑油进口温度 1361
        public double? MEInTemp { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;
    }
}