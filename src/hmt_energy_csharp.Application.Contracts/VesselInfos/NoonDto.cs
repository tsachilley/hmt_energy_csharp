namespace hmt_energy_csharp.VesselInfos
{
    public class NoonDto
    {
        /// <summary>
        /// 航行里程
        /// </summary>
        public double Distance { get; set; } = 0;

        /// <summary>
        /// 对水航行里程
        /// </summary>
        public double DistanceWater { get; set; } = 0;

        /// <summary>
        /// 航行时长
        /// </summary>
        public double Duration { get; set; } = 0;

        /// <summary>
        /// 航速
        /// </summary>
        public double Speed { get; set; } = 0;

        /// <summary>
        /// 对水航速
        /// </summary>
        public double SpeedWater { get; set; } = 0;

        /// <summary>
        /// 主机转速
        /// </summary>
        public double MERpm { get; set; } = 0;

        /// <summary>
        /// 滑失比
        /// </summary>
        public double Slip { get; set; } = 0;

        /// <summary>
        /// 本航次已航行里程
        /// </summary>
        public double DistanceTotally { get; set; } = 0;

        /// <summary>
        /// 本航次对水已航行里程
        /// </summary>
        public double DistanceWaterTotally { get; set; } = 0;

        /// <summary>
        /// 本航次航行时长
        /// </summary>
        public double DurationTotally { get; set; } = 0;

        /// <summary>
        /// 本航次航速
        /// </summary>
        public double SpeedTotally { get; set; } = 0;

        /// <summary>
        /// 本航次对水航速
        /// </summary>
        public double SpeedWaterTotally { get; set; } = 0;

        /// <summary>
        /// 轻/柴油消耗
        /// </summary>
        public double DOConsumption { get; set; } = 0;

        /// <summary>
        /// 重油消耗
        /// </summary>
        public double FOConsumption { get; set; } = 0;

        /// <summary>
        /// 主机燃油消耗
        /// </summary>
        public double MEFuelConsumption { get; set; } = 0;

        /// <summary>
        /// 辅机燃油消耗
        /// </summary>
        public double DGFuelConsumption { get; set; } = 0;

        /// <summary>
        /// 锅炉燃油消耗
        /// </summary>
        public double BLRFuelConsumption { get; set; } = 0;

        /// <summary>
        /// 风速
        /// </summary>
        public double WindSpeed { get; set; } = 0;

        /// <summary>
        /// 风向
        /// </summary>
        public double WindDirection { get; set; } = 0;

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; } = 0;

        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude { get; set; } = 0;

        /// <summary>
        /// 航向
        /// </summary>
        public double Course { get; set; } = 0;

        /// <summary>
        /// 船艏向
        /// </summary>
        public double BowDirection { get; set; } = 0;

        /// <summary>
        /// 辅机功率
        /// </summary>
        public double AEPower { get; set; } = 0;

        /// <summary>
        /// 启用辅机数量
        /// </summary>
        public int NumberOfAE { get; set; } = 0;

        /// <summary>
        /// 水温
        /// </summary>
        public double SeaTemperature { get; set; } = 0;

        /// <summary>
        /// 浪高
        /// </summary>
        public double WaveHeight { get; set; } = 0;

        /// <summary>
        /// 浪向
        /// </summary>
        public double WaveDirection { get; set; } = 0;

        /// <summary>
        /// 天气
        /// </summary>
        public string Weather { get; set; } = string.Empty;

        /// <summary>
        /// 温度
        /// </summary>
        public double Temperature { get; set; } = 0;

        /// <summary>
        /// 气压
        /// </summary>
        public double Pressure { get; set; } = 0;

        /// <summary>
        /// 能见度
        /// </summary>
        public double Visibility { get; set; } = 0;
    }
}