using hmt_energy_csharp.VDRs;
using System;

namespace hmt_energy_csharp.VdrVlws
{
    public class VdrVlw : VdrEntity
    {
        /// <summary>
        /// 总对水里程 单位海里
        /// </summary>
        public float watdistotal { get; set; }

        /// <summary>
        /// 重置对水里程 单位海里
        /// </summary>
        public float watdisreset { get; set; }

        /// <summary>
        /// 总对地里程 单位海里
        /// </summary>
        public float grddistotal { get; set; }

        /// <summary>
        /// 重置对地里程 单位海里
        /// </summary>
        public float grddisreset { get; set; }

        public VdrVlw()
        {
        }

        public VdrVlw(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.CRCCheck(sentence.Trim('$'), 2))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] strVLWInfo = strData.Split(',');
                    watdistotal = Convert.ToSingle(strVLWInfo[1]);
                    watdisreset = Convert.ToSingle(strVLWInfo[3]);
                    grddistotal = Convert.ToSingle(strVLWInfo[5]);
                    grddisreset = Convert.ToSingle(strVLWInfo[5]);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}