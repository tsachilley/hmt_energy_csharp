using System;

namespace hmt_energy_csharp.Energy.LiquidLevels
{
    public class LiquidLevelDto : BaseEnergyDto
    {
        //液位
        public decimal? Level { get; set; }

        //温度
        public decimal? Temperature { get; set; }

        //是否已上传
        public byte Uploaded { get; set; } = 0;
    }
}