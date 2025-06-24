using hmt_energy_csharp.VDRs;
using System;

namespace hmt_energy_csharp.IEC61162SX5s
{
    public class SX5 : VdrEntity
    {
        /// <summary>
        /// 每分钟转速
        /// </summary>
        public float rpm { get; set; }

        /// <summary>
        /// 扭矩 kNm
        /// </summary>
        public float torque { get; set; }

        /// <summary>
        /// 推力 kN
        /// </summary>
        public float thrust { get; set; }

        /// <summary>
        /// 功率 kW
        /// </summary>
        public float power { get; set; }

        public SX5()
        {
        }

        public SX5(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.GetBCCXorCode(sentence))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] strShaft = strData.Split(',');
                    rpm = Convert.ToSingle(strShaft[1]);
                    torque = Convert.ToSingle(strShaft[2]);
                    thrust = Convert.ToSingle(strShaft[3]);
                    power = Convert.ToSingle(strShaft[4]);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}