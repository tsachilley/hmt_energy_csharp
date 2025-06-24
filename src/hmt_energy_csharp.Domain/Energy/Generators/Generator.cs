using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Entities;

namespace hmt_energy_csharp.Energy.Generators
{
    /**
     * 发电机
     **/

    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_Generator_NRD")]
    public class Generator : BaseEnergy
    {
        //运行
        public byte IsRuning { get; set; }

        //转速
        public decimal? RPM { get; set; }

        //启动空气进机压力
        public decimal? StartPressure { get; set; }

        //控制空气进机压力
        public decimal? ControlPressure { get; set; }

        //扫气空气进机压力
        public decimal? ScavengingPressure { get; set; }

        //滑油进口压力
        public decimal? LubePressure { get; set; }

        //滑油进口温度
        public decimal? LubeTEMP { get; set; }

        //燃油进口压力
        public decimal? FuelPressure { get; set; }

        //燃油进口温度
        public decimal? FuelTEMP { get; set; }

        //高温淡水进机压力
        public decimal? FreshWaterPressure { get; set; }

        //高温淡水进口温度
        public decimal? FreshWaterTEMPIn { get; set; }

        //高温淡水出口温度
        public decimal? FreshWaterTEMPOut { get; set; }

        //低温冷却水进机压力
        public decimal? CoolingWaterPressure { get; set; }

        //低温冷却水进口温度
        public decimal? CoolingWaterTEMPIn { get; set; }

        //低温冷却水出口温度
        public decimal? CoolingWaterTEMPOut { get; set; }

        //1号气缸排气温度
        public decimal? CylinderTEMP1 { get; set; }

        //2号气缸排气温度
        public decimal? CylinderTEMP2 { get; set; }

        //3号气缸排气温度
        public decimal? CylinderTEMP3 { get; set; }

        //4号气缸排气温度
        public decimal? CylinderTEMP4 { get; set; }

        //5号气缸排气温度
        public decimal? CylinderTEMP5 { get; set; }

        //6号气缸排气温度
        public decimal? CylinderTEMP6 { get; set; }

        //增压器废气进口温度
        public decimal? SuperchargerTEMPIn { get; set; }

        //增压器废气出口温度
        public decimal? SuperchargerTEMPOut { get; set; }

        //扫气温度
        public decimal? ScavengingTEMP { get; set; }

        //轴承温度
        public decimal? BearingTEMP { get; set; }

        //前轴承温度
        public decimal? BearingTEMPFront { get; set; }

        //后轴承温度
        public decimal? BearingTEMPBack { get; set; }

        //功率
        public decimal? Power { get; set; }

        //绕组温度L1
        public decimal? WindingTEMPL1 { get; set; }

        //绕组温度L2
        public decimal? WindingTEMPL2 { get; set; }

        //绕组温度L3
        public decimal? WindingTEMPL3 { get; set; }

        //电压L1-L2
        public decimal? VoltageL1L2 { get; set; }

        //电压L2-L3
        public decimal? VoltageL2L3 { get; set; }

        //电压L1-L3
        public decimal? VoltageL1L3 { get; set; }

        //频率L1
        public decimal? FrequencyL1 { get; set; }

        //频率L2
        public decimal? FrequencyL2 { get; set; }

        //频率L3
        public decimal? FrequencyL3 { get; set; }

        //电流L1
        public decimal? CurrentL1 { get; set; }

        //电流L2
        public decimal? CurrentL2 { get; set; }

        //电流L3
        public decimal? CurrentL3 { get; set; }

        //无功功率
        public decimal? ReactivePower { get; set; }

        //功率因素
        public decimal? PowerFactor { get; set; }

        //负载比例
        public decimal? LoadRatio { get; set; }

        //是否已上传
        [Comment("是否已上传")]
        public byte Uploaded { get; set; } = 0;

        public Generator()
        {
        }

        public Generator(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.GetBCCXorCode(sentence))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] str = strData.Split(',');
                    IsRuning = Convert.ToByte(str[1]);
                    RPM = Convert.ToDecimal(str[2]);
                    StartPressure = Convert.ToDecimal(str[3]);
                    ControlPressure = Convert.ToDecimal(str[4]);
                    ScavengingPressure = Convert.ToDecimal(str[5]);
                    LubePressure = Convert.ToDecimal(str[6]);
                    LubeTEMP = Convert.ToDecimal(str[7]);
                    FuelPressure = Convert.ToDecimal(str[8]);
                    FuelTEMP = Convert.ToDecimal(str[9]);
                    FreshWaterPressure = Convert.ToDecimal(str[10]);
                    FreshWaterTEMPIn = Convert.ToDecimal(str[11]);
                    FreshWaterTEMPOut = Convert.ToDecimal(str[12]);
                    CoolingWaterPressure = Convert.ToDecimal(str[13]);
                    CoolingWaterTEMPIn = Convert.ToDecimal(str[14]);
                    CoolingWaterTEMPOut = Convert.ToDecimal(str[15]);
                    CylinderTEMP1 = Convert.ToDecimal(str[16]);
                    CylinderTEMP2 = Convert.ToDecimal(str[17]);
                    CylinderTEMP3 = Convert.ToDecimal(str[18]);
                    CylinderTEMP4 = Convert.ToDecimal(str[19]);
                    CylinderTEMP5 = Convert.ToDecimal(str[20]);
                    CylinderTEMP6 = Convert.ToDecimal(str[21]);
                    SuperchargerTEMPIn = Convert.ToDecimal(str[22]);
                    SuperchargerTEMPOut = Convert.ToDecimal(str[23]);
                    ScavengingTEMP = Convert.ToDecimal(str[24]);
                    BearingTEMP = Convert.ToDecimal(str[25]);
                    BearingTEMPFront = Convert.ToDecimal(str[26]);
                    BearingTEMPBack = Convert.ToDecimal(str[27]);
                    Power = Convert.ToDecimal(str[28]);
                    WindingTEMPL1 = Convert.ToDecimal(str[29]);
                    WindingTEMPL2 = Convert.ToDecimal(str[30]);
                    WindingTEMPL3 = Convert.ToDecimal(str[31]);
                    VoltageL1L2 = Convert.ToDecimal(str[32]);
                    VoltageL2L3 = Convert.ToDecimal(str[33]);
                    VoltageL1L3 = Convert.ToDecimal(str[34]);
                    FrequencyL1 = Convert.ToDecimal(str[35]);
                    FrequencyL2 = Convert.ToDecimal(str[36]);
                    FrequencyL3 = Convert.ToDecimal(str[37]);
                    CurrentL1 = Convert.ToDecimal(str[38]);
                    CurrentL2 = Convert.ToDecimal(str[39]);
                    CurrentL3 = Convert.ToDecimal(str[40]);
                    ReactivePower = Convert.ToDecimal(str[41]);
                    PowerFactor = Convert.ToDecimal(str[42]);
                    LoadRatio = Convert.ToDecimal(str[43]);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}