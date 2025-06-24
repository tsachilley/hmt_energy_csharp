using hmt_energy_csharp.Energy;

namespace hmt_energy_csharp.Engineroom.FOSupplyUnits
{
    /**
     * 燃油供油单元
     */

    public class FOSupplyUnitDto : BaseEnergyDto
    {
        //主机&辅机燃油单元三通阀CV阀位指示HFO 4502
        public int? HFOService { get; set; }

        //主机&辅机燃油单元三通阀CV阀位指示MDO 4502A
        public int? DGOService { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;
    }
}