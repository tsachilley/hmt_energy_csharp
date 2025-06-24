using System.Text.RegularExpressions;

namespace hmt_energy_csharp.DataConvertHelper
{
    /// <summary>
    /// long类型转换库，返回的目标都是long，即int64类型，也就是64位有符号整型数据，包括无符号整形数据ulong
    /// </summary>
    public class LongLib
    {
        #region 字节数组中转换成64位整型

        /// <summary>
        /// 将字节数组转换成单个的64位有符号整型数据，即byte[]=》long
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="start">开始位置，默认是0</param>
        /// <param name="type">字节顺序，默认是ABCD</param>
        /// <returns>单个的64位有符号整型数据</returns>
        public static long GetLongFromByteArray(byte[] source, int start, DataFormat type = DataFormat.ABCD)
        {
            return BitConverter.ToInt64(ByteArrayLib.Get8ByteArray(source, start, type), 0);
        }

        #endregion 字节数组中转换成64位整型

        #region 将字节数组中转换成64位整型数组

        /// <summary>
        /// 将字节数组转换成64位 long整型数组，即64位有符号整型数组，byte[]=》long[]
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="type">字节顺序，默认ABCD</param>
        /// <returns>64位有符号整型数组</returns>
        public static long[] GetLongArrayFromByteArray(byte[] source, DataFormat type = DataFormat.ABCD)
        {
            long[] values = new long[source.Length / 8];
            for (int i = 0; i < source.Length / 8; i++)
            {
                values[i] = GetLongFromByteArray(source, 8 * i, type);
            }
            return values;
        }

        #endregion 将字节数组中转换成64位整型数组

        #region 将字符串转转成64位整型数组

        public static long[] GetLongArrayFromString(string val)
        {
            List<long> Result = new List<long>();

            if (val.Contains(' '))
            {
                string[] str = Regex.Split(val, "\\s+", RegexOptions.IgnoreCase);

                foreach (var item in str)
                {
                    Result.Add(Convert.ToInt64(item));
                }
            }
            else
            {
                Result.Add(Convert.ToInt64(val));
            }

            return Result.ToArray();
        }

        #endregion 将字符串转转成64位整型数组
    }
}