using hmt_energy_csharp.VDRs;
using System;

namespace hmt_energy_csharp.VdrDpts
{
    public class VdrDpt : VdrEntity
    {
        /// <summary>
        /// Water depth relative to the transducer 单位 米
        /// </summary>
        public float depth { get; set; }

        /// <summary>
        /// Offset from transducer 单位 米
        /// </summary>
        public float offset { get; set; }

        /// <summary>
        /// Maximum range scale in use
        /// </summary>
        public float mrs { get; set; }

        public VdrDpt()
        {
        }

        public VdrDpt(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.CRCCheck(sentence.Trim('$'), 2))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] strDPTInfo = strData.Split(',');
                    depth = Convert.ToSingle(strDPTInfo[1]);
                    offset = Convert.ToSingle(strDPTInfo[2]);
                    mrs = Convert.ToSingle(strDPTInfo[3]);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}