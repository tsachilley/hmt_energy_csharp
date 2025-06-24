using System;
using System.Collections.Generic;
using System.Text;

namespace hmt_energy_csharp.Energy.PowerUnits
{
    public class PowerUnitDto : BaseEnergyDto
    {
        //柴油瞬时消耗
        public decimal? DGO { get; set; }

        //低硫重油瞬时消耗
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

        //柴油累积消耗
        public decimal? DGOAccumulated { get; set; }

        //低硫重油累积消耗
        public decimal? LFOAccumulated { get; set; }

        //重油累积消耗
        public decimal? HFOAccumulated { get; set; }

        //液化石油气(丙烷)累积消耗
        public decimal? LPG_PAccumulated { get; set; }

        //液化石油气(丁烷)累积消耗
        public decimal? LPG_BAccumulated { get; set; }

        //液化天然气累积消耗
        public decimal? LNGAccumulated { get; set; }

        //甲醇累积消耗
        public decimal? MethanolAccumulated { get; set; }

        //乙醇累积消耗
        public decimal? EthanolAccumulated { get; set; }

        //动力单元类型:me:主机 ae:辅机 blr:锅炉
        public string DeviceType { get; set; }

        //是否已上传
        public byte Uploaded { get; set; } = 0;
    }
}