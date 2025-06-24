using hmt_energy_csharp.VDRs;
using System;

namespace hmt_energy_csharp.VdrMwvs
{
    public class VdrMwv : VdrEntity
    {
        /// <summary>
        /// 风向
        /// </summary>
        public float angle { get; set; }

        /// <summary>
        /// 模式 R:相对值 T:理论值
        /// </summary>
        public string reference { get; set; }

        /// <summary>
        /// 风速
        /// </summary>
        public float speed { get; set; }

        /// <summary>
        /// 风速单位 K:km/h M:m/s N:节
        /// </summary>
        public string unit { get; set; }

        public VdrMwv()
        {
        }

        public VdrMwv(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.CRCCheck(sentence.Trim('$'), 2))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] strMWVInfo = strData.Split(',');
                    if (strMWVInfo[5].ToUpper() == "V")
                    {
                        return;
                    }
                    angle = Convert.ToSingle(strMWVInfo[1]);
                    reference = strMWVInfo[2];
                    speed = Convert.ToSingle(strMWVInfo[3]);
                    unit = strMWVInfo[4];
                }
            }
            catch (Exception)
            {
            }
        }
    }
}