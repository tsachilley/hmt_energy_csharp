using hmt_energy_csharp.Energy;
using System.Collections.Generic;

namespace hmt_energy_csharp.Engineroom.MERemoteControls
{
    public class VesselMERemoteControlDto : BaseVesselEnergyDto
    {
        public IList<MERemoteControlDto> MERemoteControlDtos { get; set; } = new List<MERemoteControlDto>();
    }
}