using System;
using System.Collections.Generic;
using System.Text;

namespace hmt_energy_csharp.Energy.Batteries
{
    public class VesselBatteryDto : BaseVesselEnergyDto
    {
        public IList<BatteryDto> BatteryDtos { get; set; } = new List<BatteryDto>();
    }
}
