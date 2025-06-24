using hmt_energy_csharp.VDRs;
using System;

namespace hmt_energy_csharp.IEC61162SX5s
{
    public class HDWSEED : VdrEntity
    {
        /// <summary>
        /// 瞬时流量 kg/h
        /// </summary>
        public float Instantaneous { get; set; }

        /// <summary>
        /// 累积流量 kg
        /// </summary>
        public float Accumulated { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        public float Temperature { get; set; }

        /// <summary>
        /// 密度
        /// </summary>
        public float Density { get; set; }

        public HDWSEED()
        {
        }

        public HDWSEED(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.GetBCCXorCode(sentence))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] strShaft = strData.Split(',');
                    Instantaneous = Convert.ToSingle(strShaft[1]);
                    Accumulated = Convert.ToSingle(strShaft[2]);
                    Temperature = Convert.ToSingle(strShaft[3]);
                    Density = Convert.ToSingle(strShaft[4]);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}