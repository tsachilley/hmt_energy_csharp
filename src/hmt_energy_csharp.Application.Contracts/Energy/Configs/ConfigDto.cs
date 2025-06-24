using System;
using Volo.Abp.Application.Dtos;

namespace hmt_energy_csharp.Energy.Configs
{
    public class ConfigDto : EntityDto<long>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int? Interval { get; set; } = 10;
        public decimal? HighLimit { get; set; }
        public decimal? HighHighLimit { get; set; }
        public byte IsDevice { get; set; } = 1;
        public byte IsEnabled { get; set; } = 1;
        public string Number { get; set; } = "NDY1273";
        public DateTime? create_time { get; set; }
        public DateTime? update_time { get; set; }
        public DateTime? delete_time { get; set; }
    }
}