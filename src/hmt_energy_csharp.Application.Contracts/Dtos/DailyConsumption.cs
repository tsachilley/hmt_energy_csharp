using System;

namespace hmt_energy_csharp.Dtos
{
    public class DailyConsumption
    {
        public string Number { get; set; } = string.Empty;
        public double DGOAcc { get; set; } = 0;
        public double LFOAcc { get; set; } = 0;
        public double HFOAcc { get; set; } = 0;
        public double LPG_PAcc { get; set; } = 0;
        public double LPG_BAcc { get; set; } = 0;
        public double LNGAcc { get; set; } = 0;
        public double MethanolAcc { get; set; } = 0;
        public double EthanolAcc { get; set; } = 0;

        public double MEHFOAcc { get; set; } = 0;
        public double AEHFOAcc { get; set; } = 0;
        public double BLRHFOAcc { get; set; } = 0;
        public double MEDGOAcc { get; set; } = 0;
        public double AEDGOAcc { get; set; } = 0;
        public double BLRDGOAcc { get; set; } = 0;

        public DateTime Today { get; set; } = DateTime.MinValue;
    }
}