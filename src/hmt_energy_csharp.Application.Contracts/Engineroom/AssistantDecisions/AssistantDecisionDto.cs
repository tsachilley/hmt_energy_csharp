using hmt_energy_csharp.Energy;

namespace hmt_energy_csharp.Engineroom.AssistantDecisions
{
    /**
     * 辅助决策
     */

    public class AssistantDecisionDto : BaseEnergyDto
    {
        //设备key
        public string? Key { get; set; }

        //辅助决策内容
        public string? Content { get; set; }

        //状态
        public int? State { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;
    }
}