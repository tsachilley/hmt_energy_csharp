namespace hmt_energy_csharp.Energy.SupplyUnits
{
    public class SupplyUnitDto : BaseEnergyDto
    {
        //运行
        public byte IsRuning { get; set; }

        //温度
        public decimal? Temperature { get; set; }

        //压力
        public decimal? Pressure { get; set; }

        //是否已上传
        public byte Uploaded { get; set; } = 0;
    }
}