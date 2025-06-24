using System;
using System.Collections.Generic;
using System.Text;

namespace hmt_energy_csharp.MonitorPoints
{
    public class VesselMonitorPoints
    {
        public string Number { get; set; }

        public IList<MonitorPoint> MonitorPoints { get; set; } = new List<MonitorPoint>
        {
            new MonitorPoint { Name = "定位"},
            new MonitorPoint { Name = "风速风向"},
            new MonitorPoint { Name = "航速"},
            new MonitorPoint { Name = "水深"},
            new MonitorPoint { Name = "吃水"},
            new MonitorPoint { Name = "左轴"},
            new MonitorPoint { Name = "右轴"},
            new MonitorPoint { Name = "发电机组燃油管路1进口流量"},
            new MonitorPoint { Name = "发电机组燃油管路1出口流量"},
            new MonitorPoint { Name = "发电机组燃油管路2进口流量"},
            new MonitorPoint { Name = "发电机组燃油管路2出口流量"},
            new MonitorPoint { Name = "发电机组甲醇管路流量"},
            new MonitorPoint { Name = "发电机组1状态"},
            new MonitorPoint { Name = "发电机组2状态"},
            new MonitorPoint { Name = "发电机组3状态"},
            new MonitorPoint { Name = "发电机组4状态"},
            new MonitorPoint { Name = "锂电池组1状态"},
            new MonitorPoint { Name = "锂电池组2状态"},
            new MonitorPoint { Name = "左艉密封"},
            new MonitorPoint { Name = "右艉密封"},
            new MonitorPoint { Name = "液位遥测"},
            new MonitorPoint { Name = "供油单元"}
        };
    }
}