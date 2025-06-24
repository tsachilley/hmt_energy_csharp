using System;
using Volo.Abp.Application.Dtos;

namespace hmt_energy_csharp.VesselInfos
{
    public class VesselInfoDto : EntityDto<int>
    {
        //船id
        public string SN { get; set; }

        //时间
        public DateTime ReceiveDatetime { get; set; }

        //经度
        public double? Longitude { get; set; }

        //纬度
        public double? Latitude { get; set; }

        //航向
        public double? Course { get; set; }

        //航向 磁差···
        public double? MagneticVariation { get; set; }

        //总里程 对地
        public double? TotalDistanceGrd { get; set; }

        //重置后总里程 对地
        public double? ResetDistanceGrd { get; set; }

        //总里程 对水
        public double? TotalDistanceWat { get; set; }

        //重置后总里程 对水
        public double? ResetDistanceWat { get; set; }

        //风向
        public double? WindDirection { get; set; }

        //风速
        public double? WindSpeed { get; set; }

        //浪高
        public double? WaveHeight { get; set; }

        //浪向
        public double? WaveDirection { get; set; }

        //气温
        public double? Temperature { get; set; }

        //气压
        public double? Pressure { get; set; }

        //天气
        public string Weather { get; set; }

        //能见度
        public double? Visibility { get; set; }

        //对水航速
        public double? WaterSpeed { get; set; }

        //对地航速
        public double? GroundSpeed { get; set; }

        //船艏吃水
        public double? BowDraft { get; set; }

        //船艉吃水
        public double? AsternDraft { get; set; }

        //左舷吃水
        public double? PortDraft { get; set; }

        //右舷吃水
        public double? StarBoardDraft { get; set; }

        //纵倾
        public double? Trim { get; set; }

        //横倾
        public double? Heel { get; set; }

        //吃水
        public double? Draft { get; set; }

        //水深
        public double? Depth { get; set; }

        //水深偏移
        public double? DepthOffset { get; set; }

        //主机能效
        public double? MESFOC { get; set; }

        //主机重油油耗
        public double? MEHFOConsumption { get; set; }

        //主机轻油油耗
        public double? MEMDOConsumption { get; set; }

        //辅机能效
        public double? DGSFOC { get; set; }

        //辅机重油油耗
        public double? DGHFOConsumption { get; set; }

        //辅机轻油油耗
        public double? DGMDOConsumption { get; set; }

        //锅炉能效
        public double? BLRSFOC { get; set; }

        //锅炉重油油耗
        public double? BLRHFOConsumption { get; set; }

        //锅炉轻油油耗
        public double? BLRMDOConsumption { get; set; }

        //滑失比
        public double? Slip { get; set; }

        //功率
        public double? MEPower { get; set; }

        //扭矩
        public double? Torque { get; set; }

        //转速
        public double? MERpm { get; set; }

        //推力
        public double? Thrust { get; set; }

        //主机重油累计消耗
        public double? MEHFOCACC { get; set; }

        //主机轻油累计消耗
        public double? MEMDOCACC { get; set; }

        //辅机重油累计消耗
        public double? DGHFOCACC { get; set; }

        //辅机轻油累计消耗
        public double? DGMDOCACC { get; set; }

        //锅炉重油累计消耗
        public double? BLGHFOCACC { get; set; }

        //锅炉轻油累计消耗
        public double? BLGMDOCACC { get; set; }

        //辅机功率
        public double? DGPower { get; set; }

        //主机每海里重油消耗
        public double? MEHFOCPerNm { get; set; }

        //主机每海里轻油消耗
        public double? MEMDOCPerNm { get; set; }

        //主机每海里油耗
        public double? MEFCPerNm { get; set; }

        //辅机每海里重油消耗
        public double? DGHFOCPerNm { get; set; }

        //辅机每海里轻油消耗
        public double? DGMDOCPerNm { get; set; }

        //辅机每海里油耗
        public double? DGFCPerNm { get; set; }

        //锅炉每海里重油消耗
        public double? BLRHFOCPerNm { get; set; }

        //锅炉每海里轻油消耗
        public double? BLRMDOCPerNm { get; set; }

        //锅炉每海里油耗
        public double? BLRFCPerNm { get; set; }

        //全船能效
        public double? SFOC { get; set; }

        //全船每海里重油消耗
        public double? HFOCPerNm { get; set; }

        //全船每海里轻油消耗
        public double? MDOCPerNm { get; set; }

        //全船每海里油耗
        public double? FCPerNm { get; set; }

        //航行状态
        public string Status { get; set; }

        //实时碳排放指标
        public double? RtCII { get; set; }

        //是否已上传
        public byte Uploaded { get; set; } = 0;

        //唯一码
        public Guid GUID { get; set; } = Guid.NewGuid();

        //横倾角
        public double? X { get; set; }

        //纵倾角
        public double? Y { get; set; }

        public DateTime create_time { get; set; }
        public DateTime? update_time { get; set; }
        public DateTime? delete_time { get; set; }

        public double? SeaTemperature { get; set; }
    }
}