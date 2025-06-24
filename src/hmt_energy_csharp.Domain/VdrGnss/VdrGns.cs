using hmt_energy_csharp.VDRs;
using System;

namespace hmt_energy_csharp.VdrGnss
{
    public class VdrGns : VdrEntity
    {
        /// <summary>
        /// GNS时间
        /// </summary>
        public string gnsdatetime { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public string latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public string longtitude { get; set; }

        /// <summary>
        /// 卫星数量
        /// </summary>
        public string satnum { get; set; }

        /// <summary>
        /// HDOP
        /// </summary>
        public string hdop { get; set; }

        /// <summary>
        /// 海拔
        /// </summary>
        public string antennaaltitude { get; set; }

        /// <summary>
        /// Geoidal separation
        /// </summary>
        public string geoidalseparation { get; set; }

        public VdrGns()
        {
        }

        public VdrGns(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.CRCCheck(sentence.Trim('$'), 2))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] strGNSInfo = strData.Split(',');
                    if (strGNSInfo[13].ToUpper() == "N")
                    {
                        return;
                    }
                    gnsdatetime = strGNSInfo[1];
                    //latitude = GPSHelper.TLatLong(strGNSInfo[3] + " " + strGNSInfo[2]);
                    latitude = strGNSInfo[3].ToLower().Equals("n") ? strGNSInfo[2] : ("-" + strGNSInfo[2]);
                    //longtitude = GPSHelper.TLatLong(strGNSInfo[5] + " " + strGNSInfo[4]);
                    longtitude = strGNSInfo[5].ToLower().Equals("e") ? strGNSInfo[4] : ("-" + strGNSInfo[4]);
                    satnum = strGNSInfo[7];
                    hdop = strGNSInfo[8];
                    antennaaltitude = strGNSInfo[9];
                    geoidalseparation = strGNSInfo[10];
                }
            }
            catch (Exception)
            {
            }
        }
    }
}