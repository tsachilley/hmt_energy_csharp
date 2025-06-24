using Microsoft.EntityFrameworkCore;
using System;

namespace hmt_energy_csharp.Energy.TotalIndicators
{
    /* *
     * 多传感器累积量
     * */

    [Comment("多传感器累积量")]
    [Index(nameof(Number), nameof(ReceiveDatetime), IsUnique = true, Name = "UK_TotalIndicator_NR")]
    public class TotalIndicator : BaseEnergy
    {
        //柴油瞬时消耗
        public decimal? DGO { get; set; }

        //低硫重油瞬时消耗**
        public decimal? LFO { get; set; }

        //重油瞬时消耗
        public decimal? HFO { get; set; }

        //液化石油气(丙烷)瞬时消耗
        public decimal? LPG_P { get; set; }

        //液化石油气(丁烷)瞬时消耗
        public decimal? LPG_B { get; set; }

        //液化天然气瞬时消耗
        public decimal? LNG { get; set; }

        //甲醇瞬时消耗
        public decimal? Methanol { get; set; }

        //乙醇瞬时消耗
        public decimal? Ethanol { get; set; }

        //功率
        public decimal? Power { get; set; }

        //扭矩
        public decimal? Torque { get; set; }

        //推力
        public decimal? Thrust { get; set; }

        //转速
        [Precision(10, 2)]
        public decimal? Rpm { get; set; }

        //柴油累积消耗
        [Comment("柴油累积消耗")]
        [Precision(14, 4)]
        public decimal? DGOAccumulated { get; set; }

        //低硫重油累积消耗
        [Comment("低硫重油累积消耗")]
        [Precision(14, 4)]
        public decimal? LFOAccumulated { get; set; }

        //重油累积消耗
        [Comment("重油累积消耗")]
        [Precision(14, 4)]
        public decimal? HFOAccumulated { get; set; }

        //液化石油气(丙烷)累积消耗
        [Comment("液化石油气(丙烷)累积消耗")]
        [Precision(14, 4)]
        public decimal? LPG_PAccumulated { get; set; }

        //液化石油气(丁烷)累积消耗
        [Comment("液化石油气(丁烷)累积消耗")]
        [Precision(14, 4)]
        public decimal? LPG_BAccumulated { get; set; }

        //液化天然气累积消耗
        [Comment("液化天然气累积消耗")]
        [Precision(14, 4)]
        public decimal? LNGAccumulated { get; set; }

        //甲醇累积消耗
        [Comment("甲醇累积消耗")]
        [Precision(14, 4)]
        public decimal? MethanolAccumulated { get; set; }

        //乙醇累积消耗
        [Comment("乙醇累积消耗")]
        [Precision(14, 4)]
        public decimal? EthanolAccumulated { get; set; }

        //是否已上传
        [Comment("是否已上传")]
        public byte Uploaded { get; set; } = 0;
    }
}