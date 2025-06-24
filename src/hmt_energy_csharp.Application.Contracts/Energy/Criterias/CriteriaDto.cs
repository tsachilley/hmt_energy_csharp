using System;
using System.Collections.Generic;
using System.Text;

namespace hmt_energy_csharp.Energy.Criterias
{
    public class CriteriaDto
    {
        public double Speed { get; set; } = 18;
        public double Power { get; set; } = 10000;
        public double DGO { get; set; }
        public double LFO { get; set; }
        public double HFO { get; set; } = 1200;
        public double LPG_P { get; set; }
        public double LPG_B { get; set; }
        public double LNG { get; set; }
        public double Methanol { get; set; } = 110;
        public double Ethanol { get; set; }

        public double SFOC { get; set; } = (1200d + 110d) / 7000d;
        public double PerNm { get; set; } = (1200d + 110d) / 18d;
        public double CEmission { get; set; } = (1200d * 3.114d + 110d * 1.375d) * 1000d / (10000d * 18d);

        public double Proppitch { get; set; } = 6000;
    }
}