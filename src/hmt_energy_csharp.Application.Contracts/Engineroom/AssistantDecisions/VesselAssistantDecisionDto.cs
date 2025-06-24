using hmt_energy_csharp.Energy;
using System.Collections.Generic;

namespace hmt_energy_csharp.Engineroom.AssistantDecisions
{
    public class VesselAssistantDecisionDto : BaseVesselEnergyDto
    {
        public IList<AssistantDecisionDto> AssistantDecisionDtos { get; set; } = new List<AssistantDecisionDto>();
    }
}