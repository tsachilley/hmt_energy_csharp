using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Entities;

namespace hmt_energy_csharp.Energy.Flowmeters
{
    /**
     * 流量计
     **/

    [Index(nameof(Number), nameof(ReceiveDatetime), nameof(DeviceNo), IsUnique = true, Name = "UK_Flowmeter_NRD")]
    public class Flowmeter : BaseEnergy
    {
        //瞬时消耗 kg/h
        public decimal? ConsAct { get; set; }

        //累计消耗 kg/h
        public decimal? ConsAcc { get; set; }

        //温度 ℃
        public decimal? Temperature { get; set; }

        //密度 kg/m3
        public decimal? Density { get; set; }

        //耗能单元类型 主机、辅机、锅炉等
        public string DeviceType { get; set; }

        //耗能类型 重油、轻油、甲醇等
        public string FuelType { get; set; }

        //是否已上传
        [Comment("是否已上传")]
        public byte Uploaded { get; set; } = 0;

        public Flowmeter()
        {
        }

        public Flowmeter(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.GetBCCXorCode(sentence))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] str = strData.Split(',');
                    ConsAct = Convert.ToDecimal(str[1]);
                    ConsAcc = Convert.ToDecimal(str[2]);
                    Temperature = Convert.ToDecimal(str[3]);
                    Density = Convert.ToDecimal(str[4]);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}