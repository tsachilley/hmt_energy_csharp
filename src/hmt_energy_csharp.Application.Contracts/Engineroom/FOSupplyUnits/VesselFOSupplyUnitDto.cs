using hmt_energy_csharp.Energy;
using System.Collections.Generic;

namespace hmt_energy_csharp.Engineroom.FOSupplyUnits
{
    public class VesselFOSupplyUnitDto : BaseVesselEnergyDto
    {
        public IList<FOSupplyUnitDto> FOSupplyUnitDtos { get; set; } = new List<FOSupplyUnitDto>();
    }
}