using hmt_energy_csharp.VDRs;
using System;

namespace hmt_energy_csharp.VdrTrcs
{
    public class VdrTrc : VdrEntity
    {
        /// <summary>
        /// 推进器编号 Odd = Bow thruster Even = Stern thrusters
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// 需求转速
        /// </summary>
        public string rpmdemand { get; set; }

        /// <summary>
        /// 转速状态 P:% R:RPM V:无效
        /// </summary>
        public string rpmindicator { get; set; }

        /// <summary>
        /// 需求螺距
        /// </summary>
        public string pitchdemand { get; set; }

        /// <summary>
        /// 螺距状态 P:% D:度 V:无效
        /// </summary>
        public string pitchindicator { get; set; }

        /// <summary>
        /// 需求方位角
        /// </summary>
        public string azimuth { get; set; }

        /// <summary>
        /// 操作位置指示器
        /// </summary>
        public string oli { get; set; }

        public VdrTrc()
        {
        }

        public VdrTrc(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.CRCCheck(sentence.Trim('$'), 2))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] strTRCInfo = strData.Split(',');
                    if (strTRCInfo[8].ToLower() == "C")
                        //C是命令控制语句 不解析
                        return;
                    number = strTRCInfo[1];
                    rpmdemand = strTRCInfo[2];
                    rpmindicator = strTRCInfo[3];
                    pitchdemand = strTRCInfo[4];
                    pitchindicator = strTRCInfo[5];
                    azimuth = strTRCInfo[6];
                    oli = strTRCInfo[7];
                }
            }
            catch (Exception)
            {
            }
        }
    }
}