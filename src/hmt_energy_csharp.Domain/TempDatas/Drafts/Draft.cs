using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace hmt_energy_csharp.TempDatas.Drafts
{
    public class Draft
    {
        /// <summary>
        /// 船艏
        /// </summary>
        public decimal Bow { get; set; }

        /// <summary>
        /// 船艉
        /// </summary>
        public decimal Astern { get; set; }

        /// <summary>
        /// 左舷
        /// </summary>
        public decimal Port { get; set; }

        /// <summary>
        /// 右舷
        /// </summary>
        public decimal StartBoard { get; set; }

        public Draft()
        {
        }

        public Draft(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.GetBCCXorCode(sentence))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] strShaft = strData.Split(',');
                    Bow = Convert.ToDecimal(strShaft[1]);
                    Astern = Convert.ToDecimal(strShaft[2]);
                    Port = Convert.ToDecimal(strShaft[3]);
                    StartBoard = Convert.ToDecimal(strShaft[4]);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}