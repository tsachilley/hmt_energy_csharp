using hmt_energy_csharp.Energy;

namespace hmt_energy_csharp.Engineroom.LubOils
{
    /**
     * 滑油系统
     */

    public class LubOilDto : BaseEnergyDto
    {
        //主机增压器滑油进机压力 1301 1327
        public double? METCInPress { get; set; }

        //主机推力轴承块温度 1302
        public double? METBSTemp { get; set; }

        //主机主轴承和推力块滑油进口压力 1304
        public double? MEMBTBInPress { get; set; }

        //主机活塞冷却油进口压力 1307
        public double? MEPistonCOInPress { get; set; }

        //主机滑油进口温度 1308
        public double? MEInTemp { get; set; }

        //主机#1缸活塞冷却油出口温度 1309
        public double? MECYL1PistonCOOutTemp { get; set; }

        //主机#2缸活塞冷却油出口温度 1310
        public double? MECYL2PistonCOOutTemp { get; set; }

        //主机#3缸活塞冷却油出口温度 1311
        public double? MECYL3PistonCOOutTemp { get; set; }

        //主机#4缸活塞冷却油出口温度 1312
        public double? MECYL4PistonCOOutTemp { get; set; }

        //主机#5缸活塞冷却油出口温度 1313
        public double? MECYL5PistonCOOutTemp { get; set; }

        //主机#6缸活塞冷却油出口温度 1314
        public double? MECYL6PistonCOOutTemp { get; set; }

        //主机#1缸活塞冷却油出口无流量 1315
        public int? MECYL1PistonCOOutNoFlow { get; set; }

        //主机#2缸活塞冷却油出口无流量 1316
        public int? MECYL2PistonCOOutNoFlow { get; set; }

        //主机#3缸活塞冷却油出口无流量 1317
        public int? MECYL3PistonCOOutNoFlow { get; set; }

        //主机#4缸活塞冷却油出口无流量 1318
        public int? MECYL4PistonCOOutNoFlow { get; set; }

        //主机#5缸活塞冷却油出口无流量 1319
        public int? MECYL5PistonCOOutNoFlow { get; set; }

        //主机#6缸活塞冷却油出口无流量 1320
        public int? MECYL6PistonCOOutNoFlow { get; set; }

        //主机增压器滑油出口温度 1321
        public double? METCOutTemp { get; set; }

        //主机滑油中水分高预报警 1322
        public int? MEWaterHigh { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;
    }
}