using System;
using System.Collections.Generic;
using System.Text;

namespace hmt_energy_csharp.Energy.SternSealings
{
    public class VesselSternSealingDto : BaseVesselEnergyDto
    {
        public IList<SternSealingDto> SternSealingDtos { get; set; } = new List<SternSealingDto>();
    }
}
