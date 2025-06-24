using System;
using System.Collections.Generic;
using System.Text;

namespace hmt_energy_csharp.Energy.Flowmeters
{
    public class VesselFlowmeterDto : BaseVesselEnergyDto
    {
        public IList<FlowmeterDto> FlowmeterDtos { get; set; } = new List<FlowmeterDto>();
    }
}
