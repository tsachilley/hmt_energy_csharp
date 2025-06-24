using hmt_energy_csharp.Energy;
using System.Collections.Generic;

namespace hmt_energy_csharp.Engineroom.LubOils
{
    public class VesselLubOilDto : BaseVesselEnergyDto
    {
        public IList<LubOilDto> LubOilDtos { get; set; } = new List<LubOilDto>();
    }
}