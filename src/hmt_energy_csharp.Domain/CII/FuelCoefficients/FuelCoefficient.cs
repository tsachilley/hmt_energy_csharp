using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hmt_energy_csharp.CII.FuelCoefficients
{
    public class FuelCoefficient : BaseEntity
    {
        //代码
        public string Code { get; set; }

        //中文名
        public string NameCN { get; set; }

        //英文名
        public string NameEN { get; set; }

        //值
        public decimal? Value { get; set; }

        //排序
        public decimal? Sort { get; set; }
    }
}