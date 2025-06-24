using hmt_energy_csharp.Energy;
using System.Collections.Generic;

namespace hmt_energy_csharp.Engineroom.ExhaustGases
{
    public class VesselExhaustGasDto : BaseVesselEnergyDto
    {
        public IList<ExhaustGasDto> ExhaustGasDtos { get; set; } = new List<ExhaustGasDto>();
    }
}