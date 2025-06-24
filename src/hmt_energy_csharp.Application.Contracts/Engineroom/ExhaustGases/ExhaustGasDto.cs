using hmt_energy_csharp.Energy;

namespace hmt_energy_csharp.Engineroom.ExhaustGases
{
    /**
     * 排气系统
     */

    public class ExhaustGasDto : BaseEnergyDto
    {
        //主机增压器进口排气温度 1701
        public double? METCInTemp { get; set; }

        //主机1号缸排气阀后排气温度/平均偏差 1702
        public double? MECyl1AfterTemp { get; set; }

        //主机2号缸排气阀后排气温度/平均偏差 1703
        public double? MECyl2AfterTemp { get; set; }

        //主机3号缸排气阀后排气温度/平均偏差 1704
        public double? MECyl3AfterTemp { get; set; }

        //主机4号缸排气阀后排气温度/平均偏差 1705
        public double? MECyl4AfterTemp { get; set; }

        //主机5号缸排气阀后排气温度/平均偏差 1706
        public double? MECyl5AfterTemp { get; set; }

        //主机6号缸排气阀后排气温度/平均偏差 1707
        public double? MECyl6AfterTemp { get; set; }

        //主机1号缸排气阀后排气温度/平均偏差 1702-DEV
        public double? MECyl1AfterTempDev { get; set; }

        //主机2号缸排气阀后排气温度/平均偏差 1703-DEV
        public double? MECyl2AfterTempDev { get; set; }

        //主机3号缸排气阀后排气温度/平均偏差 1704-DEV
        public double? MECyl3AfterTempDev { get; set; }

        //主机4号缸排气阀后排气温度/平均偏差 1705-DEV
        public double? MECyl4AfterTempDev { get; set; }

        //主机5号缸排气阀后排气温度/平均偏差 1706-DEV
        public double? MECyl5AfterTempDev { get; set; }

        //主机6号缸排气阀后排气温度/平均偏差 1707-DEV
        public double? MECyl6AfterTempDev { get; set; }

        //主机增压器出口排气温度 1708
        public double? METCOutTemp { get; set; }

        //主机排气集管压力 1709
        public double? MEReceiverPress { get; set; }

        //主机涡轮背压 1710
        public double? METurbBackPress { get; set; }

        //空冷器前扫气温度 1711
        public double? MEACInTemp { get; set; }

        //空冷器后扫气温度 1712
        public double? MEACOutTemp { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;
    }
}