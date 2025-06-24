using hmt_energy_csharp.Energy;
using System.Collections.Generic;

namespace hmt_energy_csharp.Engineroom.MainSwitchboards
{
    public class VesselMainSwitchboardDto : BaseVesselEnergyDto
    {
        public IList<MainSwitchboardDto> MainSwitchboardDtos { get; set; } = new List<MainSwitchboardDto>();
    }
}