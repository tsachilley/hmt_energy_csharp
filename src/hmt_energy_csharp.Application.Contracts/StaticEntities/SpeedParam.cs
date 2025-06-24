using System;
using System.Collections.Generic;
using System.Text;

namespace hmt_energy_csharp.StaticEntities
{
    public class SpeedParam
    {
        public string Number { get; set; }
        public IList<double> Values { get; set; } = new List<double>();
    }
}