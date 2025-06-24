using System;
using System.Collections.Generic;
using System.Text;

namespace hmt_energy_csharp.Devices
{
    public class MonitoredDevice
    {
        //设备序列号
        public string Number { get; set; }

        //通讯设备通讯时间
        public IDictionary<string, DateTime> Devices { get; set; }
    }
}