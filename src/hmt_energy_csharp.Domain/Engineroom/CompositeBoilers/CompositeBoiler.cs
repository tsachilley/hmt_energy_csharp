using hmt_energy_csharp.Energy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace hmt_energy_csharp.Engineroom.CompositeBoilers
{
    /**
     * 燃油废气组合锅炉
     */

    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_CompositeBoiler_NRD")]
    public class CompositeBoiler : BaseEnergy
    {
        //锅炉燃烧器运行 4105
        public double? BLRBurnerRunning { get; set; }

        //锅炉使用重油 4106
        public double? BLRHFOService { get; set; }

        //锅炉使用轻油 4107
        public double? BLRDGOService { get; set; }

        //锅炉1号燃油泵运行 4118
        public double? BLRFOP1On { get; set; }

        //锅炉2号燃油泵运行 4119
        public double? BLRFOP2On { get; set; }

        //锅炉燃油温度低 4127
        public double? BLRFOTempLow { get; set; }

        //锅炉燃油压力高 4128
        public double? BLRFOPressHigh { get; set; }

        //锅炉燃油温度高 4135
        public double? BLRFOTempHigh { get; set; }

        //锅炉轻柴油温度高 4144
        public double? BLRDGOTempHigh { get; set; }

        //锅炉重油温度高 4147
        public double? BLRHFOTempHigh { get; set; }

        //1#发电机排气温度高 4148
        public double? BLRGE1EXTempHigh { get; set; }

        //2#发电机排气温度高 4149
        public double? BLRGE2EXTempHigh { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;

        public CompositeBoiler()
        {
        }

        public CompositeBoiler(string sentence)
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