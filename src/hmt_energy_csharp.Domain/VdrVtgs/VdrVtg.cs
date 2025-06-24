using hmt_energy_csharp.VDRs;
using System;

namespace hmt_energy_csharp.VdrVtgs
{
    public class VdrVtg : VdrEntity
    {
        /// <summary>
        /// 航向(真实)
        /// </summary>
        public float grdcoztrue { get; set; }

        /// <summary>
        /// 航向(地磁)
        /// </summary>
        public float grdcozmag { get; set; }

        /// <summary>
        /// 航速(节)
        /// </summary>
        public float grdspdknot { get; set; }

        /// <summary>
        /// 航速(千米)
        /// </summary>
        public float grdspdkm { get; set; }

        public VdrVtg()
        {
        }

        public VdrVtg(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.CRCCheck(sentence.Trim('$'), 2))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] strVTGInfo = strData.Split(',');
                    if (strVTGInfo[9].ToUpper() == "N")
                    {
                        return;
                    }
                    grdcoztrue = Convert.ToSingle(strVTGInfo[1].IsNullOrWhiteSpace() ? "0" : strVTGInfo[1]);
                    grdcozmag = Convert.ToSingle(strVTGInfo[3].IsNullOrWhiteSpace() ? "0" : strVTGInfo[3]);
                    grdspdknot = Convert.ToSingle(strVTGInfo[5].IsNullOrWhiteSpace() ? "0" : strVTGInfo[5]);
                    grdspdkm = Convert.ToSingle(strVTGInfo[7].IsNullOrWhiteSpace() ? "0" : strVTGInfo[7]);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}