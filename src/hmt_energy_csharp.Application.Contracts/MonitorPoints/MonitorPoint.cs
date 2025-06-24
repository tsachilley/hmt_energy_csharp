using System;
using System.Collections.Generic;
using System.Text;

namespace hmt_energy_csharp.MonitorPoints
{
    public class MonitorPoint
    {
        //监测点名
        public string Name { get; set; }

        //监测状态
        public int IsChecked { get; set; } = 0;
    }
}