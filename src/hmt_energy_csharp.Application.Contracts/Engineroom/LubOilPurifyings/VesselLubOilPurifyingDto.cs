using hmt_energy_csharp.Energy;
using System.Collections.Generic;

namespace hmt_energy_csharp.Engineroom.LubOilPurifyings
{
    public class VesselLubOilPurifyingDto : BaseVesselEnergyDto
    {
        public IList<LubOilPurifyingDto> LubOilPurifyingDtos { get; set; } = new List<LubOilPurifyingDto>();
    }
}