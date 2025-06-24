using hmt_energy_csharp.Energy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace hmt_energy_csharp.Engineroom.LubOilPurifyings
{
    /**
     * 滑油净化系统
     */

    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_LubOilPurifying_NRD")]
    public class LubOilPurifying : BaseEnergy
    {
        //主机滑油滤器压差高 5241
        public int? MEFilterPressHigh { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;

        public LubOilPurifying()
        {
        }

        public LubOilPurifying(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.GetBCCXorCode(sentence))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] str = strData.Split(',');
                }
            }
            catch (Exception)
            {
            }
        }
    }
}