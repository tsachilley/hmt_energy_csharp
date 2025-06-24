using hmt_energy_csharp.VDRs;
using System;

namespace hmt_energy_csharp.VdrRpms
{
    public class VdrRpm : VdrEntity
    {
        /// <summary>
        /// 源 shaft/engine S/E
        /// </summary>
        public string source { get; set; }

        /// <summary>
        /// 编号 numbered from centre-line odd = starboard, even = port, 0 = single or on centre - line
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// 转速 单位rpm &quot;-&quot; = counter-clockwise
        /// </summary>
        public float speed { get; set; }

        /// <summary>
        /// 螺距 % of maximum, &quot;-&quot; = astern
        /// </summary>
        public float propellerpitch { get; set; }

        public VdrRpm()
        {
        }

        public VdrRpm(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.CRCCheck(sentence.Trim('$'), 2))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] strRPMInfo = strData.Split(',');
                    if (strRPMInfo[5].ToLower() == "V")
                        //数据状态不正常
                        return;
                    source = strRPMInfo[1];
                    number = strRPMInfo[2];
                    speed = Convert.ToSingle(strRPMInfo[3]);
                    propellerpitch = Convert.ToSingle(strRPMInfo[4]);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}