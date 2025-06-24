using hmt_energy_csharp.Energy;
using Microsoft.EntityFrameworkCore;
using System;

namespace hmt_energy_csharp.Engineroom.CoolingWaters
{
    /**
     * 主机冷却水系统
     */

    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_CoolingWater_NRD")]
    public class CoolingWater : BaseEnergy
    {
        //主机缸套冷却水进口压力 1401
        public double? MEJacketInPress { get; set; }

        //主机缸套冷却水压差 1402
        public double? MEPressDrop { get; set; }

        //主机缸套冷却水出口压力 1403
        public double? MEOutPress { get; set; }

        //主机缸盖和排气阀冷却水压差 1403A
        public double? MEJacketPressDrop { get; set; }

        //主机缸套冷却水进口温度 1404
        public double? MEInTemp { get; set; }

        //主机1号缸缸套冷却水出口温度 1405
        public double? MEJacketCyl1OutTemp { get; set; }

        //主机2号缸缸套冷却水出口温度 1406
        public double? MEJacketCyl2OutTemp { get; set; }

        //主机3号缸缸套冷却水出口温度 1407
        public double? MEJacketCyl3OutTemp { get; set; }

        //主机4号缸缸套冷却水出口温度 1408
        public double? MEJacketCyl4OutTemp { get; set; }

        //主机5号缸缸套冷却水出口温度 1409
        public double? MEJacketCyl5OutTemp { get; set; }

        //主机6号缸缸套冷却水出口温度 1410
        public double? MEJacketCyl6OutTemp { get; set; }

        //主机1号缸缸盖冷却水出口温度 1411
        public double? MECCCyl1OutTemp { get; set; }

        //主机2号缸缸盖冷却水出口温度 1412
        public double? MECCCyl2OutTemp { get; set; }

        //主机3号缸缸盖冷却水出口温度 1413
        public double? MECCCyl3OutTemp { get; set; }

        //主机4号缸缸盖冷却水出口温度 1414
        public double? MECCCyl4OutTemp { get; set; }

        //主机5号缸缸盖冷却水出口温度 1415
        public double? MECCCyl5OutTemp { get; set; }

        //主机6号缸缸盖冷却水出口温度 1416
        public double? MECCCyl6OutTemp { get; set; }

        //主机空冷器冷却水进口压力 1418
        public double? MEACInPress { get; set; }

        //主机空冷器冷却水进口温度 1419
        public double? MEACInTemp { get; set; }

        //主机空冷器冷却水进口温度 1420
        public double? MEACOutTemp { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;

        public CoolingWater()
        {
        }

        public CoolingWater(string sentence)
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