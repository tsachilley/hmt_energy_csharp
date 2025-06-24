using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace hmt_energy_csharp.CII.Coefficients
{
    public class CIICoefficient : BaseEntity
    {
        //船型编码
        public string ShipType { get; set; }

        //参数1
        public decimal? Coefficient1 { get; set; }

        //参数2
        public decimal? Coefficient2 { get; set; }

        //重量条件
        public string WeightCondition { get; set; }

        //值
        public decimal? WeightValue { get; set; }

        //低值
        public decimal? LowValue { get; set; }

        //包含低值
        public decimal? ContainLow { get; set; }

        //高值
        public decimal? HighValue { get; set; }

        //包含高值
        public decimal? ContainHigh { get; set; }

        //排序
        public decimal? Sort { get; set; }
    }
}