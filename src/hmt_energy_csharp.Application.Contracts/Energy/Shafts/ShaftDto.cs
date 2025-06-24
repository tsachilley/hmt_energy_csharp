namespace hmt_energy_csharp.Energy.Shafts
{
    /**
     * 主轴
     */

    public class ShaftDto : BaseEnergyDto
    {
        //功率 kW
        public decimal? Power { get; set; }

        //转速
        public decimal? RPM { get; set; }

        //扭矩 kNm
        public decimal? Torque { get; set; }

        //推力 kN
        public decimal? Thrust { get; set; }

        //能量
        public double? Energy { get; set; }

        //转数
        public double? Revolutions { get; set; }

        //是否已上传
        public byte Uploaded { get; set; } = 0;
    }
}