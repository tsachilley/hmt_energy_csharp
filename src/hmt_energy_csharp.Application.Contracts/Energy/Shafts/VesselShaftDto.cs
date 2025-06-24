using System;
using System.Collections.Generic;
using System.Text;

namespace hmt_energy_csharp.Energy.Shafts
{
    public class VesselShaftDto : BaseVesselEnergyDto
    {
        public IList<ShaftDto> ShaftDtos { get; set; } = new List<ShaftDto>();
    }
}
