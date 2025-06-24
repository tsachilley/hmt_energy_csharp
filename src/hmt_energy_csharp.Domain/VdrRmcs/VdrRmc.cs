using hmt_energy_csharp.VDRs;
using System;

namespace hmt_energy_csharp.VdrRmcs
{
    public class VdrRmc : VdrEntity
    {
        /// <summary>
        /// 日期时间
        /// </summary>
        public string rmcdatetime { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public string latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public string longtitude { get; set; }

        /// <summary>
        /// 对地航速 单位节
        /// </summary>
        public string grdspeed { get; set; }

        /// <summary>
        /// 对地航向 单位度
        /// </summary>
        public string grdcoz { get; set; }

        /// <summary>
        /// 地磁偏差 单位度
        /// </summary>
        public string magvar { get; set; }

        public VdrRmc()
        {
        }

        public VdrRmc(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.CRCCheck(sentence.Trim('$'), 2))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] strRMCInfo = strData.Split(',');
                    if (strRMCInfo[2].ToLower() == "V")
                        //数据状态不正常
                        return;
                    rmcdatetime = strRMCInfo[9] + " " + strRMCInfo[1];
                    latitude = strRMCInfo[4].ToLower().Equals("n") ? strRMCInfo[3] : ("-" + strRMCInfo[3]);
                    longtitude = strRMCInfo[6].ToLower().Equals("e") ? strRMCInfo[5] : ("-" + strRMCInfo[5]);
                    grdspeed = strRMCInfo[7];
                    grdcoz = strRMCInfo[8];
                    //magvar = strRMCInfo[11] + " " + strRMCInfo[10];
                    magvar = Convert.ToDouble(strRMCInfo[10]) + "";
                }
            }
            catch (Exception)
            {
            }
        }
    }
}