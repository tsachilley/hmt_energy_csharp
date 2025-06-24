using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Entities;

namespace hmt_energy_csharp.Energy.SternSealings
{
    /**
     * 艉密封
     **/

    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_SternSealing_NRD")]
    public class SternSealing : BaseEnergy
    {
        //前轴承温度 ℃
        public decimal? FrontTEMP { get; set; }

        //后轴承温度 ℃
        public decimal? BackTEMP { get; set; }

        //后轴承温度(左) ℃
        public decimal? BackLeftTEMP { get; set; }

        //后轴承温度(右) ℃
        public decimal? BackRightTEMP { get; set; }

        //是否已上传
        [Comment("是否已上传")]
        public byte Uploaded { get; set; } = 0;

        public SternSealing()
        {
        }

        public SternSealing(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.GetBCCXorCode(sentence))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] str = strData.Split(',');
                    FrontTEMP = Convert.ToDecimal(str[1]);
                    BackTEMP = Convert.ToDecimal(str[2]);
                    BackLeftTEMP = Convert.ToDecimal(str[3]);
                    BackRightTEMP = Convert.ToDecimal(str[4]);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}