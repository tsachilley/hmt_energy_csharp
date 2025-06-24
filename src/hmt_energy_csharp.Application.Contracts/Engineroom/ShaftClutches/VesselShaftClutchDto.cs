using hmt_energy_csharp.Energy;
using System.Collections.Generic;

namespace hmt_energy_csharp.Engineroom.ShaftClutches
{
    public class VesselShaftClutchDto : BaseVesselEnergyDto
    {
        public IList<ShaftClutchDto> ShaftClutchDtos { get; set; } = new List<ShaftClutchDto>();
    }
}