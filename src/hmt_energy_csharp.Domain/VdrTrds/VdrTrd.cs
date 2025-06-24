using hmt_energy_csharp.VDRs;
using System;

namespace hmt_energy_csharp.VdrTrds
{
    public class VdrTrd : VdrEntity
    {
        /// <summary>
        /// 推进器编号 Odd = Bow thruster Even = Stern thrusters
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// 应答(实际)转速
        /// </summary>
        public string rpmresponse { get; set; }

        /// <summary>
        /// 转速状态 P:% R:RPM V:无效
        /// </summary>
        public string rpmindicator { get; set; }

        /// <summary>
        /// 应答(实际)螺距
        /// </summary>
        public string pitchresponse { get; set; }

        /// <summary>
        /// 螺距状态 P:% D:度 V:无效
        /// </summary>
        public string pitchindicator { get; set; }

        /// <summary>
        /// 应答(实际)方位角
        /// </summary>
        public string azimuth { get; set; }

        public VdrTrd()
        {
        }

        public VdrTrd(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.CRCCheck(sentence.Trim('$'), 2))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] strTRDInfo = strData.Split(',');
                    number = strTRDInfo[1];
                    rpmresponse = strTRDInfo[2];
                    rpmindicator = strTRDInfo[3];
                    pitchresponse = strTRDInfo[4];
                    pitchindicator = strTRDInfo[5];
                    azimuth = strTRDInfo[6];
                }
            }
            catch (Exception)
            {
            }
        }
    }
}