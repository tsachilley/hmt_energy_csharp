using hmt_energy_csharp.VDRs;
using System;

namespace hmt_energy_csharp.VdrHdgs
{
    public class VdrHdg : VdrEntity
    {
        /// <summary>
        /// Magnetic sensor heading, degrees
        /// </summary>
        public string msh { get; set; }

        /// <summary>
        /// Magnetic deviation, degrees E/W
        /// </summary>
        public string md { get; set; }

        /// <summary>
        /// Magnetic variation, degrees E/W
        /// </summary>
        public string mv { get; set; }

        public VdrHdg()
        {
        }

        public VdrHdg(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.CRCCheck(sentence.Trim('$'), 2))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] strHDGInfo = strData.Split(',');
                    msh = strHDGInfo[1];
                    md = strHDGInfo[3] + " " + strHDGInfo[2];
                    mv = strHDGInfo[5] + " " + strHDGInfo[4];
                }
            }
            catch (Exception)
            {
            }
        }
    }
}