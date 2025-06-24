using hmt_energy_csharp.Energy;

namespace hmt_energy_csharp.Engineroom.MainGeneratorSets
{
    /**
     * 主发电机
     */

    public class MainGeneratorSetDto : BaseEnergyDto
    {
        //主发电机组缸套水进机压力 3101
        public double? DGCFWInPress { get; set; }

        //主发电机组起动空气压力 3102
        public double? DGStartAirPress { get; set; }

        //主发电机组滑油压力 3103
        public double? DGLOPress { get; set; }

        //主发电机组滑油进机温度 3104
        public double? DGLOInTemp { get; set; }

        //主发电机组缸套水出机温度 3105 3118
        public double? DGCFWOutTemp { get; set; }

        //主发电机组废气进增压器温度（1~3缸） 3106
        public double? DGEGTC1To3InTemp { get; set; }

        //主发电机组废气进增压器温度（4~6缸） 3107
        public double? DGEGTC4To6InTemp { get; set; }

        //主发电机组柴油机转速
        public int? DGEngineSpeed { get; set; }

        //主发电机组柴油机负荷
        public int? DGEngineLoad { get; set; }

        //主发电机组柴油机运行时间
        public int? DGEngineRunHour { get; set; }

        //主发电机组滑油进机低压停机 3119
        public int? DGLOInPress { get; set; }

        //主发电机组滑油进机滤器高压差 3120
        public int? DGLOFilterInPress { get; set; }

        //主发电机组控制空气低压 3121
        public int? DGControlAirPress { get; set; }

        //主发电机组增压器滑油低压 3122
        public int? DGTCLOPress { get; set; }

        //主发电机组机器运行 3125
        public int? DGEngineRunning { get; set; }

        //主发电机组U相定子绕组温度 3128
        public double? DGUTemp { get; set; }

        //主发电机组V相定子绕组温度 3129
        public double? DGVTemp { get; set; }

        //主发电机组W相定子绕组温度 3130
        public double? DGWTemp { get; set; }

        //主发电机组轴承温度 3131
        public double? DGBTDTemp { get; set; }

        //主发电机组1缸排气温度 3206
        public int? DGCyl1ExTemp { get; set; }

        //主发电机组2缸排气温度 3207
        public int? DGCyl2ExTemp { get; set; }

        //主发电机组3缸排气温度 3208
        public int? DGCyl3ExTemp { get; set; }

        //主发电机组4缸排气温度 3209
        public int? DGCyl4ExTemp { get; set; }

        //主发电机组5缸排气温度 3210
        public int? DGCyl5ExTemp { get; set; }

        //主发电机组6缸排气温度 3211
        public int? DGCyl6ExTemp { get; set; }

        //主发电机组增压器排气温度 3212
        public int? DGTCEXOutTemp { get; set; }

        //主发电机组增压空气压力 3213
        public double? DGBoostAirPress { get; set; }

        //主发电机燃油进机压力 3213A
        public double? DGFOInPress { get; set; }

        //上传云端标识
        public byte Uploaded { get; set; } = 0;
    }
}