using hmt_energy_csharp.Energy;
using System.Collections.Generic;

namespace hmt_energy_csharp.Engineroom.FOs
{
    public class VesselFODto : BaseVesselEnergyDto
    {
        public IList<FODto> FODtos { get; set; } = new List<FODto>();
    }
}