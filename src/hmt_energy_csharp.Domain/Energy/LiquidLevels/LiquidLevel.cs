using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Entities;

namespace hmt_energy_csharp.Energy.LiquidLevels
{
    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_LiquidLevel_NRD")]
    public class LiquidLevel : BaseEnergy
    {
        //液位
        public decimal? Level { get; set; }

        //温度
        public decimal? Temperature { get; set; }

        //是否已上传
        [Comment("是否已上传")]
        public byte Uploaded { get; set; } = 0;

        public LiquidLevel()
        {
        }

        public LiquidLevel(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.GetBCCXorCode(sentence))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] str = strData.Split(',');
                    Level = Convert.ToDecimal(str[1]);
                    Temperature = Convert.ToDecimal(str[2]);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}