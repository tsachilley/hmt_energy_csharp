using hmt_energy_csharp.VDRs;
using System;

namespace hmt_energy_csharp.VdrPrcs
{
    public class VdrPrc : VdrEntity
    {
        /// <summary>
        /// 操纵杆需求
        /// </summary>
        public string leverdemand { get; set; }

        /// <summary>
        /// 操纵杆状态
        /// </summary>
        public string leverstatus { get; set; }

        /// <summary>
        /// 转速需求
        /// </summary>
        public string rpmdemand { get; set; }

        /// <summary>
        /// 转速状态
        /// </summary>
        public string rpmstatus { get; set; }

        /// <summary>
        /// 螺距需求
        /// </summary>
        public string pitchdemand { get; set; }

        /// <summary>
        /// 螺距状态
        /// </summary>
        public string pitchstatus { get; set; }

        /// <summary>
        /// 动力系统编号
        /// </summary>
        public string number { get; set; }

        public VdrPrc()
        {
        }

        public VdrPrc(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.CRCCheck(sentence.Trim('$'), 2))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] strPRCInfo = strData.Split(',');
                    leverdemand = strPRCInfo[1];
                    leverstatus = strPRCInfo[2];
                    rpmdemand = strPRCInfo[3];
                    rpmstatus = strPRCInfo[4];
                    pitchdemand = strPRCInfo[5];
                    pitchstatus = strPRCInfo[6];
                    number = strPRCInfo[8];
                }
            }
            catch (Exception)
            {
            }
        }
    }
}