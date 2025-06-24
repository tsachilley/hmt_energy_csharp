using System;
using System.Collections.Generic;
using System.Text;

namespace hmt_energy_csharp.Energy.PowerUnits
{
    public class VesselPowerUnitDto : BaseVesselEnergyDto
    {
        public IList<PowerUnitDto> PowerUnitDtos { get; set; } = new List<PowerUnitDto>();
    }
}
