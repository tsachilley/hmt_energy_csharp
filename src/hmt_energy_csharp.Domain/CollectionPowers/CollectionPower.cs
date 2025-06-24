using hmt_energy_csharp.VDRs;

namespace hmt_energy_csharp.CollectionPowers
{
    /// <summary>
    /// 轴功率仪数据集
    /// </summary>
    public class CollectionPower : VdrEntity
    {
        /// <summary>
        /// 转速
        /// </summary>
        public string rpm { get; set; }

        /// <summary>
        /// 扭矩
        /// </summary>
        public string torque { get; set; }

        /// <summary>
        /// 功率
        /// </summary>
        public string power { get; set; }

        /// <summary>
        /// 滑失比
        /// </summary>
        public string slip { get; set; }
    }
}