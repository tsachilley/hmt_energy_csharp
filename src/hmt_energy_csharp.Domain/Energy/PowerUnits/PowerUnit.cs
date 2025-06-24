using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Energy.PowerUnits
{
    /// <summary>
    /// 动力单元能耗
    /// </summary>
    [Comment("动力单元能耗")]
    [Index(nameof(DeviceType), nameof(Number), nameof(ReceiveDatetime), IsUnique = true, Name = "UK_PowerUnit_DNR")]
    public class PowerUnit : BaseEnergy
    {
        //柴油瞬时消耗
        [Comment("柴油瞬时消耗")]
        [Precision(10, 4)]
        public decimal? DGO { get; set; }

        //低硫重油瞬时消耗
        [Comment("低硫重油瞬时消耗")]
        [Precision(10, 4)]
        public decimal? LFO { get; set; }

        //重油瞬时消耗
        [Comment("重油瞬时消耗")]
        [Precision(10, 4)]
        public decimal? HFO { get; set; }

        //液化石油气(丙烷)瞬时消耗
        [Comment("液化石油气(丙烷)瞬时消耗")]
        [Precision(10, 4)]
        public decimal? LPG_P { get; set; }

        //液化石油气(丁烷)瞬时消耗
        [Comment("液化石油气(丁烷)瞬时消耗")]
        [Precision(10, 4)]
        public decimal? LPG_B { get; set; }

        //液化天然气瞬时消耗
        [Comment("液化天然气瞬时消耗")]
        [Precision(10, 4)]
        public decimal? LNG { get; set; }

        //甲醇瞬时消耗
        [Comment("甲醇瞬时消耗")]
        [Precision(10, 4)]
        public decimal? Methanol { get; set; }

        //乙醇瞬时消耗
        [Comment("乙醇瞬时消耗")]
        [Precision(10, 4)]
        public decimal? Ethanol { get; set; }

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

        //动力单元类型:me:主机 ae:辅机 blr:锅炉
        [Comment("动力单元类型:me:主机 ae:辅机 blr:锅炉")]
        public string DeviceType { get; set; }

        //是否已上传
        [Comment("是否已上传")]
        public byte Uploaded { get; set; } = 0;
    }
}