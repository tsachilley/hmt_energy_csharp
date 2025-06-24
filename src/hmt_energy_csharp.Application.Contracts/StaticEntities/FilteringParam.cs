using System;
using System.Collections.Generic;
using System.Text;

namespace hmt_energy_csharp.StaticEntities
{
    public class FilteringParam
    {
        public string Number { get; set; }
        public string DeviceNo { get; set; }
        public IList<decimal> Values { get; set; } = new List<decimal>();
    }
}