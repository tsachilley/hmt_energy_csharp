using System;
using System.Collections.Generic;
using System.Text;

namespace hmt_energy_csharp.Energy.Generators
{
    public class VesselGeneratorDto : BaseVesselEnergyDto
    {
        public IList<GeneratorDto> GeneratorDtos { get; set; } = new List<GeneratorDto>();
    }
}
