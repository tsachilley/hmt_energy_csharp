using hmt_energy_csharp.Energy;

namespace hmt_energy_csharp.Engineroom.MERemoteControls
{
    /**
     * 主机遥控系统
     */

    public class MERemoteControlDto : BaseEnergyDto
    {
        //主机转速 2336
        public double? MERpm { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;
    }
}