using hmt_energy_csharp.Energy;
using System.Collections.Generic;

namespace hmt_energy_csharp.Engineroom.CoolingSeaWaters
{
    public class VesselCoolingSeaWaterDto : BaseVesselEnergyDto
    {
        public IList<CoolingSeaWaterDto> CoolingSeaWaterDtos { get; set; } = new List<CoolingSeaWaterDto>();
    }
}