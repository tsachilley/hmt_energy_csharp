using hmt_energy_csharp.Energy;
using System.Collections.Generic;

namespace hmt_energy_csharp.Engineroom.CoolingWaters
{
    public class VesselCoolingWaterDto : BaseVesselEnergyDto
    {
        public IList<CoolingWaterDto> CoolingWaterDtos { get; set; } = new List<CoolingWaterDto>();
    }
}