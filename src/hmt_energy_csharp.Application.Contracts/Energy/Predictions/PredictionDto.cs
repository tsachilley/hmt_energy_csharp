using System;

namespace hmt_energy_csharp.Energy.Predictions
{
    public class PredictionDto : BaseEnergyDto
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

        //是否已上传
        public byte Uploaded { get; set; } = 0;
    }
}