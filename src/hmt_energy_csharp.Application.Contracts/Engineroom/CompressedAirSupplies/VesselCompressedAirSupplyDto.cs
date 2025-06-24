using hmt_energy_csharp.Energy;
using System.Collections.Generic;

namespace hmt_energy_csharp.Engineroom.CompressedAirSupplies
{
    public class VesselCompressedAirSupplyDto : BaseVesselEnergyDto
    {
        public IList<CompressedAirSupplyDto> CompressedAirSupplyDtos { get; set; } = new List<CompressedAirSupplyDto>();
    }
}