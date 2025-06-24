using hmt_energy_csharp.Energy;

namespace hmt_energy_csharp.Engineroom.ScavengeAirs
{
    /**
     * 主机扫气系统
     */

    public class ScavengeAirDto : BaseEnergyDto
    {
        //主机扫气集管温度 1601
        public double? MEReceiverTemp { get; set; }

        //主机1号缸扫气失火温度 1602
        public double? MEFBCyl1Temp { get; set; }

        //主机2号缸扫气失火温度 1603
        public double? MEFBCyl2Temp { get; set; }

        //主机3号缸扫气失火温度 1604
        public double? MEFBCyl3Temp { get; set; }

        //主机4号缸扫气失火温度 1605
        public double? MEFBCyl4Temp { get; set; }

        //主机5号缸扫气失火温度 1606
        public double? MEFBCyl5Temp { get; set; }

        //主机6号缸扫气失火温度 1607
        public double? MEFBCyl6Temp { get; set; }

        //主机扫气集管压力 1609
        public double? MEPress { get; set; }

        //主机空冷器芯子空气压降 1610
        public double? MECoolerPressDrop { get; set; }

        //主机增压器吸气温度A 1611
        public double? METCInTempA { get; set; }

        //主机增压器吸气温度B 1612
        public double? METCInTempB { get; set; }

        //机舱相对湿度 1613
        public double? ERRelativeHumidity { get; set; }

        //机舱温度 1614
        public double? ERTemp { get; set; }

        //机舱压力 1615
        public double? ERAmbientPress { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;
    }
}