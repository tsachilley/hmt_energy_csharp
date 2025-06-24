using hmt_energy_csharp.VDRs;

namespace hmt_energy_csharp.CollectionFlowmeters
{
    public class CollectionFlowmeter : VdrEntity
    {
        /// <summary>
        /// 燃料消耗-瞬时
        /// </summary>
        public string fuelconsume { get; set; }

        /// <summary>
        /// 燃料类型
        /// </summary>
        public string fueltype { get; set; }

        /// <summary>
        /// 燃料消耗-累计
        /// </summary>
        public string fuelconsumeaccumulative { get; set; }

        /// <summary>
        /// 动力设备类型
        /// </summary>
        public string devicetype { get; set; }

        /// <summary>
        /// 动力设备编号
        /// </summary>
        public string deviceno { get; set; }

        /// <summary>
        /// 每海里燃料消耗
        /// </summary>
        public string fcpernm { get; set; }

        /// <summary>
        /// 每千瓦燃料消耗
        /// </summary>
        public string fcperpow { get; set; }
    }
}