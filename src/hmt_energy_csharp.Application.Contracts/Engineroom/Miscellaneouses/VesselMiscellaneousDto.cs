using hmt_energy_csharp.Energy;
using System.Collections.Generic;

namespace hmt_energy_csharp.Engineroom.Miscellaneouses
{
    public class VesselMiscellaneousDto : BaseVesselEnergyDto
    {
        public IList<MiscellaneousDto> MiscellaneousDtos { get; set; } = new List<MiscellaneousDto>();
    }
}