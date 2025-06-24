using System;
using System.Collections.Generic;
using System.Text;

namespace hmt_energy_csharp.Energy.SupplyUnits
{
    public class VesselSupplyUnitDto : BaseVesselEnergyDto
    {
        public IList<SupplyUnitDto> SupplyUnitDtos { get; set; } = new List<SupplyUnitDto>();
    }
}
