using hmt_energy_csharp.Energy;
using System.Collections.Generic;

namespace hmt_energy_csharp.Engineroom.CylinderLubOils
{
    public class VesselCylinderLubOilDto : BaseVesselEnergyDto
    {
        public IList<CylinderLubOilDto> CylinderLubOilDtos { get; set; } = new List<CylinderLubOilDto>();
    }
}