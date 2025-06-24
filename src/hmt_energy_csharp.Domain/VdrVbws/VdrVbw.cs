using hmt_energy_csharp.VDRs;
using System;

namespace hmt_energy_csharp.VdrVbws
{
    public class VdrVbw : VdrEntity
    {
        /// <summary>
        /// 纵向水速
        /// </summary>
        public float lngwatspd { get; set; }

        /// <summary>
        /// 横向水速
        /// </summary>
        public float tvswatspd { get; set; }

        /// <summary>
        /// 水速状态 A = data valid, V = data invalid
        /// </summary>
        public string watspdstatus { get; set; }

        /// <summary>
        /// 纵向地速
        /// </summary>
        public float lnggrdspd { get; set; }

        /// <summary>
        /// 横向地速
        /// </summary>
        public float tvsgrdspd { get; set; }

        /// <summary>
        /// 地速状态 A = data valid, V = data invalid
        /// </summary>
        public string grdspdstatus { get; set; }

        /// <summary>
        /// 船尾横向水速
        /// </summary>
        public float tvswatspdstern { get; set; }

        /// <summary>
        /// 船尾水速状态 A = data valid, V = data invalid
        /// </summary>
        public string watspdstatusstern { get; set; }

        /// <summary>
        /// 船尾横向地速
        /// </summary>
        public float tvsgrdspdstern { get; set; }

        /// <summary>
        /// 船尾地速状态 A = data valid, V = data invalid
        /// </summary>
        public string grdspdstatusstern { get; set; }

        /// <summary>
        /// 计算值 水速
        /// </summary>
        public float watspd { get; set; }

        /// <summary>
        /// 计算值 地速
        /// </summary>
        public float grdspd { get; set; }

        public VdrVbw()
        {
        }

        public VdrVbw(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.CRCCheck(sentence.Trim('$'), 2))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] strVBWInfo = strData.Split(',');
                    lngwatspd = Convert.ToSingle(strVBWInfo[1].IsNullOrWhiteSpace() ? "0" : strVBWInfo[1]);
                    tvswatspd = Convert.ToSingle(strVBWInfo[2].IsNullOrWhiteSpace() ? "0" : strVBWInfo[2]);
                    watspdstatus = strVBWInfo[3].ToString();
                    lnggrdspd = Convert.ToSingle(strVBWInfo[4].IsNullOrWhiteSpace() ? "0" : strVBWInfo[4]);
                    tvsgrdspd = Convert.ToSingle(strVBWInfo[5].IsNullOrWhiteSpace() ? "0" : strVBWInfo[5]);
                    grdspdstatus = strVBWInfo[6].ToString();
                    tvswatspdstern = Convert.ToSingle(strVBWInfo[7].IsNullOrWhiteSpace() ? "0" : strVBWInfo[7]);
                    watspdstatusstern = strVBWInfo[8].ToString();
                    tvsgrdspdstern = Convert.ToSingle(strVBWInfo[9].IsNullOrWhiteSpace() ? "0" : strVBWInfo[9]);
                    grdspdstatusstern = strVBWInfo[10].ToString();
                    watspd = lngwatspd;//TSpeed(strVBWInfo[1], strVBWInfo[2]);
                    grdspd = TSpeed(strVBWInfo[4], strVBWInfo[5]);
                }
            }
            catch (Exception)
            {
            }
        }

        private float TSpeed(string lngSpd, string tvsSpd)
        {
            float result = 0;
            double result_d = Math.Round(Math.Sqrt(Math.Pow(Convert.ToSingle(lngSpd), 2) + Math.Pow(Convert.ToSingle(tvsSpd), 2)), 4);
            result = Convert.ToSingle(result_d);
            return result;
        }
    }
}