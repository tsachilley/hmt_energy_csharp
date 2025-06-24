using hmt_energy_csharp.Energy;
using System.Collections.Generic;

namespace hmt_energy_csharp.Engineroom.CompositeBoilers
{
    public class VesselCompositeBoilerDto : BaseVesselEnergyDto
    {
        public IList<CompositeBoilerDto> CompositeBoilerDtos { get; set; } = new List<CompositeBoilerDto>();
    }
}