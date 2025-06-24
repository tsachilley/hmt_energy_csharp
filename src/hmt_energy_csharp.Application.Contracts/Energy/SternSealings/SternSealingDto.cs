namespace hmt_energy_csharp.Energy.SternSealings
{
    /**
     * 艉密封
     **/

    public class SternSealingDto : BaseEnergyDto
    {
        //前轴承温度 ℃
        public decimal? FrontTEMP { get; set; }

        //后轴承温度 ℃
        public decimal? BackTEMP { get; set; }

        //后轴承温度(左) ℃
        public decimal? BackLeftTEMP { get; set; }

        //后轴承温度(右) ℃
        public decimal? BackRightTEMP { get; set; }

        //是否已上传
        public byte Uploaded { get; set; } = 0;
    }
}