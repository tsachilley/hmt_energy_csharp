using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using hmt_energy_csharp.Energy;

namespace hmt_energy_csharp.Engineroom.MainSwitchboards
{
    /**
     * 主配电板
     */

    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_MainSwitchboard_NRD")]
    public class MainSwitchboard : BaseEnergy
    {
        //主配电板电压高 3407A
        public double? MBVoltageHigh { get; set; }

        //主配电板电压低 3407B
        public double? MBVoltageLow { get; set; }

        //主配电板电板频率高 3408A
        public double? MBFrequencyHigh { get; set; }

        //主配电板电板频率低 3408B
        public double? MBFrequencyLow { get; set; }

        //发电机运行指示 3468
        public double? DGRunning { get; set; }

        //发电机功率 3480
        public double? DGPower { get; set; }

        //发电机电压 3483
        public double? DGVoltageL1L2 { get; set; }

        //发电机电压 3483
        public double? DGVoltageL2L3 { get; set; }

        //发电机电压 3483
        public double? DGVoltageL3L1 { get; set; }

        //发电机电流 3486
        public double? DGCurrentL1 { get; set; }

        //发电机电流 3486
        public double? DGCurrentL2 { get; set; }

        //发电机电流 3486
        public double? DGCurrentL3 { get; set; }

        //发电机频率 3489
        public double? DGFrequency { get; set; }

        //发电机功率因素 3492
        public double? DGPowerFactor { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;

        public MainSwitchboard()
        {
        }

        public MainSwitchboard(string sentence)
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