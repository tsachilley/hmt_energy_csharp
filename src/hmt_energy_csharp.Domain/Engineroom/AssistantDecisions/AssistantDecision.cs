using hmt_energy_csharp.Energy;
using Microsoft.EntityFrameworkCore;
using System;

namespace hmt_energy_csharp.Engineroom.AssistantDecisions
{
    /**
     * 辅助决策
     */

    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(Key), IsUnique = true, Name = "UK_AssistantDecision_NRK")]
    public class AssistantDecision : BaseEnergy
    {
        //设备key
        public string? Key { get; set; }

        //辅助决策内容
        public string? Content { get; set; }

        //状态
        public int? State { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;

        public AssistantDecision()
        {
        }

        public AssistantDecision(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.GetBCCXorCode(sentence))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] str = strData.Split(',');
                }
            }
            catch (Exception)
            {
            }
        }
    }
}