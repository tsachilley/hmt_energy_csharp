using hmt_energy_csharp.VDRs;

namespace hmt_energy_csharp.CollectionDrafts
{
    /// <summary>
    /// 吃水数据
    /// </summary>
    public class CollectionDraft : VdrEntity
    {
        /// <summary>
        /// 船艏
        /// </summary>
        public string bow { get; set; }

        /// <summary>
        /// 船艉
        /// </summary>
        public string stern { get; set; }

        /// <summary>
        /// 左舷
        /// </summary>
        public string port { get; set; }

        /// <summary>
        /// 右舷
        /// </summary>
        public string starboard { get; set; }

        /// <summary>
        /// 纵倾
        /// </summary>
        public string trim { get; set; }

        /// <summary>
        /// 横倾
        /// </summary>
        public string heel { get; set; }

        /// <summary>
        /// 平均吃水
        /// </summary>
        public string draft { get; set; }
    }
}