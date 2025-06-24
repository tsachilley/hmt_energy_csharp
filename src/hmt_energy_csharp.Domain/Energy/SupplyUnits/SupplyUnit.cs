using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Entities;

namespace hmt_energy_csharp.Energy.SupplyUnits
{
    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_SupplyUnit_NRD")]
    public class SupplyUnit : BaseEnergy
    {
        //运行
        public byte IsRuning { get; set; }

        //温度
        public decimal? Temperature { get; set; }

        //压力
        public decimal? Pressure { get; set; }

        //是否已上传
        [Comment("是否已上传")]
        public byte Uploaded { get; set; } = 0;

        public SupplyUnit()
        {
        }

        public SupplyUnit(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.GetBCCXorCode(sentence))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] str = strData.Split(',');
                    IsRuning = Convert.ToByte(str[1]);
                    Temperature = Convert.ToDecimal(str[2]);
                    Pressure = Convert.ToDecimal(str[3]);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}