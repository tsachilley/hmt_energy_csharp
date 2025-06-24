using hmt_energy_csharp.VDRs;
using System;

namespace hmt_energy_csharp.VdrMwds
{
    public class VdrMwd : VdrEntity
    {
        /// <summary>
        /// 风向 相对于正北 true
        /// </summary>
        public float tdirection { get; set; }

        /// <summary>
        /// 风向 相对于正北 magnetic
        /// </summary>
        public float magdirection { get; set; }

        /// <summary>
        /// 风速 节
        /// </summary>
        public float knspeed { get; set; }

        /// <summary>
        /// 风速 m/s
        /// </summary>
        public float speed { get; set; }

        public VdrMwd()
        {
        }

        public VdrMwd(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.CRCCheck(sentence.Trim('$'), 2))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] strMWDInfo = strData.Split(',');
                    tdirection = Convert.ToSingle(strMWDInfo[1]);
                    magdirection = Convert.ToSingle(strMWDInfo[3]);
                    knspeed = Convert.ToSingle(strMWDInfo[5]);
                    speed = Convert.ToSingle(strMWDInfo[7]);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}