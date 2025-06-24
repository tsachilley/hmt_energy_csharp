using hmt_energy_csharp.VDRs;
using System;

namespace hmt_energy_csharp.VdrGgas
{
    public class VdrGga : VdrEntity
    {
        /// <summary>
        /// 纬度
        /// </summary>
        public string latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public string longitude { get; set; }

        /// <summary>
        /// 使用卫星数量
        /// </summary>
        public string satnum { get; set; }

        public VdrGga()
        {
        }

        public VdrGga(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.CRCCheck(sentence.Trim('$'), 2))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] strGGAInfo = strData.Split(',');
                    latitude = strGGAInfo[3].ToLower().Equals("n") ? strGGAInfo[2] : ("-" + strGGAInfo[2]);
                    longitude = strGGAInfo[5].ToLower().Equals("e") ? strGGAInfo[4] : ("-" + strGGAInfo[4]);
                    satnum = strGGAInfo[7];
                }
            }
            catch (Exception)
            {
            }
        }
    }
}