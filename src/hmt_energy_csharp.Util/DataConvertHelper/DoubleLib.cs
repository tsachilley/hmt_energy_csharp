using System.Text.RegularExpressions;

namespace hmt_energy_csharp.DataConvertHelper
{
    /// <summary>
    /// Double类型转换库，返回的目标类型都是Double类型，即双精度的浮点型数据，也就是有符号的小数，包括无符号的小数，有符号的整数，无符号的整数
    /// </summary>
    public class DoubleLib
    {
        #region 将字节数组转换成单个的Double类型

        /// <summary>
        /// 将字节数组转换成单个的Double类型
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="start">开始位置</param>
        /// <param name="type">字节顺序，默认是ABCD</param>
        /// <returns>Double类型数值</returns>
        public static double GetDoubleFromByteArray(byte[] source, int start, DataFormat type = DataFormat.ABCD)
        {
            byte[] result = ByteArrayLib.Get8ByteArray(source, start, type);
            if (result != null)
            {
                return BitConverter.ToDouble(result, 0);
            }
            else
            {
                return 0.0;
            }
        }

        #endregion 将字节数组转换成单个的Double类型

        #region 将字节数组转换成Double数组

        /// <summary>
        /// 将字节数组转换成Double数组
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="type">字节顺序，默认是ABCD</param>
        /// <returns>Double数组</returns>
        public static double[] GetDoubleArrayFromByteArray(byte[] source, DataFormat type = DataFormat.ABCD)
        {
            double[] values = new double[source.Length / 8];
            for (int i = 0; i < source.Length / 4; i++)
            {
                values[i] = GetDoubleFromByteArray(source, 8 * i, type);
            }
            return values;
        }

        #endregion 将字节数组转换成Double数组

        #region 将字符串转转成双精度浮点型数组

        /// <summary>
        /// 将Double字符串转换成双精度浮点型数组
        /// </summary>
        /// <param name="val">Double字符串</param>
        /// <returns>双精度浮点型数组</returns>
        public static double[] GetDoubleArrayFromString(string val)
        {
            List<double> Result = new List<double>();
            if (val.Contains(' '))
            {
                string[] str = Regex.Split(val, "\\s+", RegexOptions.IgnoreCase);

                foreach (var item in str)
                {
                    Result.Add(Convert.ToDouble(item));
                }
            }
            else
            {
                Result.Add(Convert.ToDouble(val));
            }

            return Result.ToArray();
        }

        #endregion 将字符串转转成双精度浮点型数组
    }
}