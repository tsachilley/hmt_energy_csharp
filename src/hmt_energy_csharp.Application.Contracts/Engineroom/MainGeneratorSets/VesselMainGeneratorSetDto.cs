using hmt_energy_csharp.Energy;
using System.Collections.Generic;

namespace hmt_energy_csharp.Engineroom.MainGeneratorSets
{
    public class VesselMainGeneratorSetDto : BaseVesselEnergyDto
    {
        public IList<MainGeneratorSetDto> MainGeneratorSetDtos { get; set; } = new List<MainGeneratorSetDto>();
    }
}