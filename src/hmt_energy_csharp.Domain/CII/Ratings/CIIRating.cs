using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hmt_energy_csharp.CII.Ratings
{
    public class CIIRating : BaseEntity
    {
        //船型编码
        public string ShipType { get; set; }

        //等级
        public string Rating { get; set; }

        //等级值
        public decimal? RatingValue { get; set; }

        //重量条件
        public string WeightCondition { get; set; }

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