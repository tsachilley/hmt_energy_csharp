using hmt_energy_csharp.Energy;
using System.Collections.Generic;

namespace hmt_energy_csharp.Engineroom.CoolingFreshWaters
{
    public class VesselCoolingFreshWaterDto : BaseVesselEnergyDto
    {
        public IList<CoolingFreshWaterDto> CoolingFreshWaterDtos { get; set; } = new List<CoolingFreshWaterDto>();
    }
}