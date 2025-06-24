using System;
using System.Collections.Generic;
using System.Text;

namespace hmt_energy_csharp.Dtos
{
    public class EnergyDistribution
    {
        /// <summary>
        /// 总能源输入
        /// </summary>
        public decimal Et { get; set; } = 0;

        /// <summary>
        /// 主机
        /// </summary>
        public decimal Emei { get; set; } = 0;

        /// <summary>
        /// 其他损失
        /// </summary>
        public decimal Emeol { get; set; } = 0;

        /// <summary>
        /// 排烟
        /// </summary>
        public decimal Emes { get; set; } = 0;

        /// <summary>
        /// 发电机
        /// </summary>
        public decimal Eaei { get; set; } = 0;

        /// <summary>
        /// 冷却损失
        /// </summary>
        public decimal Emec { get; set; } = 0;

        /// <summary>
        /// 排烟损失
        /// </summary>
        public decimal Epssl { get; set; } = 0;

        /// <summary>
        /// 传动系统
        /// </summary>
        public decimal Emeo { get; set; } = 0;

        /// <summary>
        /// 船舶电网
        /// </summary>
        public decimal Eaeeni { get; set; } = 0;

        /// <summary>
        /// 电站其它损失
        /// </summary>
        public decimal Eael { get; set; } = 0;

        /// <summary>
        /// 冷却损失
        /// </summary>
        public decimal Eaedc { get; set; } = 0;

        /// <summary>
        /// 螺旋桨
        /// </summary>
        public decimal Emetse { get; set; } = 0;

        /// <summary>
        /// 传动损失
        /// </summary>
        public decimal Emetsl { get; set; } = 0;

        /// <summary>
        /// 电力负荷
        /// </summary>
        public decimal Eaeepli { get; set; } = 0;

        /// <summary>
        /// 电网损失
        /// </summary>
        public decimal Eaeenl { get; set; } = 0;

        /// <summary>
        /// 推进损失
        /// </summary>
        public decimal Emepel { get; set; } = 0;

        /// <summary>
        /// 负载损失
        /// </summary>
        public decimal Eaeepll { get; set; } = 0;

        /// <summary>
        /// 主机T/C回收
        /// </summary>
        public decimal Emetc { get; set; } = 0;

        /// <summary>
        /// 推进做功
        /// </summary>
        public decimal Wmepee { get; set; } = 0;

        /// <summary>
        /// 发电机T/C回收
        /// </summary>
        public decimal Eaetc { get; set; } = 0;

        /// <summary>
        /// 日用设备
        /// </summary>
        public decimal Waee { get; set; } = 0;

        /// <summary>
        /// 锂电池充电
        /// </summary>
        public decimal Waech { get; set; } = 0;

        /// <summary>
        /// 全船有效利用总能量
        /// </summary>
        public decimal Eo { get; set; } = 0;
    }
}
