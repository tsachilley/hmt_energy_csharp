using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using hmt_energy_csharp.Energy;

namespace hmt_energy_csharp.Engineroom.FOSupplyUnits
{
    /**
     * 燃油供油单元
     */

    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_FOSupplyUnit_NRD")]
    public class FOSupplyUnit : BaseEnergy
    {
        //主机&辅机燃油单元三通阀CV阀位指示HFO 4502
        public int? HFOService { get; set; }

        //主机&辅机燃油单元三通阀CV阀位指示MDO 4502A
        public int? DGOService { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;

        public FOSupplyUnit()
        {
        }

        public FOSupplyUnit(string sentence)
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