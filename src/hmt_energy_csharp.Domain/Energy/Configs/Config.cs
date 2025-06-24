using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace hmt_energy_csharp.Energy.Configs
{
    public class Config : BasicAggregateRoot<long>
    {
        /// <summary>
        /// 代码
        /// </summary>
        [Column(TypeName = "varchar2")]
        [MaxLength(30)]
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Column(TypeName = "nvarchar2")]
        [MaxLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// 采样间隔 单位：s
        /// </summary>
        public int? Interval { get; set; } = 10;

        /// <summary>
        /// 高限值
        /// </summary>
        [Precision(12, 4)]
        public decimal? HighLimit { get; set; }

        /// <summary>
        /// 高高限值
        /// </summary>
        [Precision(12, 4)]
        public decimal? HighHighLimit { get; set; }

        /// <summary>
        /// 是否设备
        /// </summary>
        public byte IsDevice { get; set; } = 1;

        /// <summary>
        /// 是否启用
        /// </summary>
        public byte IsEnabled { get; set; } = 1;

        /// <summary>
        /// 相当于船id
        /// </summary>
        [Column(TypeName = "varchar2")]
        [MaxLength(30)]
        public string Number { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? create_time { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? update_time { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? delete_time { get; set; }
    }
}