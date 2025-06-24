using System;
using System.Collections.Generic;
using System.Text;

namespace hmt_energy_csharp.Energy.LiquidLevels
{
    public class VesselLiquidLevelDto : BaseVesselEnergyDto
    {
        public IList<LiquidLevelDto> LiquidLevelDtos { get; set; } = new List<LiquidLevelDto>();
    }
}
