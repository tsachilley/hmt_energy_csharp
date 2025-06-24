using hmt_energy_csharp.Energy;
using Microsoft.EntityFrameworkCore;
using System;

namespace hmt_energy_csharp.Engineroom.MERemoteControls
{
    /**
     * 主机遥控系统
     */

    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_MERemoteControl_NRD")]
    public class MERemoteControl : BaseEnergy
    {
        //主机转速 2336
        public double? MERpm { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;

        public MERemoteControl()
        {
        }

        public MERemoteControl(string sentence)
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