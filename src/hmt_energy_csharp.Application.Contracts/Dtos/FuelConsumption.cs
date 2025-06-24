using System;
using System.Collections.Generic;
using System.Text;

namespace hmt_energy_csharp.Dtos
{
    public class FuelConsumption
    {
        public string DeviceType { get; set; }
        public string FuelType { get; set; }
        public decimal Cons { get; set; }
        public string DeviceNo { get; set; }
    }
}