using hmt_energy_csharp.Energy;
using System.Collections.Generic;

namespace hmt_energy_csharp.Engineroom.ScavengeAirs
{
    public class VesselScavengeAirDto : BaseVesselEnergyDto
    {
        public IList<ScavengeAirDto> ScavengeAirDtos { get; set; } = new List<ScavengeAirDto>();
    }
}