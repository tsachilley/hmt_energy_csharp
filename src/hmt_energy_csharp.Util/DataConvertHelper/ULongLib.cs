using System.Text.RegularExpressions;

namespace hmt_energy_csharp.DataConvertHelper
{
    /// <summary>
    /// ulong类型转换库，返回的目标都是ulong，即uint64类型，也就是64位无符号整型数据
    /// </summary>
    public class ULongLib
    {
        #region 将字节数组中截取转成64位整型

        /// <summary>
        /// 将字节数组转换成单个的64位无符号整型数据，即byte[]=》ulong
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="start">开始位置，默认是0</param>
        /// <param name="type">字节顺序，默认是ABCD</param>
        /// <returns>单个的64位整型数据</returns>
        public static ulong GetULongFromByteArray(byte[] source, int start, DataFormat type = DataFormat.ABCD)
        {
            return BitConverter.ToUInt64(ByteArrayLib.Get8ByteArray(source, start, type), 0);
        }

        #endregion 将字节数组中截取转成64位整型

        #region 将字节数组中截取转成64位整型数组

        /// <summary>
        /// 将字节数组转换成64位整型数组，即64位无符号整型数组，byte[]=》ulong[]
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="type">字节顺序，默认ABCD</param>
        /// <returns>64位无符号整型数组</returns>
        public static ulong[] GetULongArrayFromByteArray(byte[] source, DataFormat type = DataFormat.ABCD)
        {
            ulong[] values = new ulong[source.Length / 8];
            for (int i = 0; i < source.Length / 8; i++)
            {
                values[i] = GetULongFromByteArray(source, 8 * i, type);
            }
            return values;
        }

        #endregion 将字节数组中截取转成64位整型数组

        #region 将字符串转转成64位整型数组

        public static ulong[] GetULongArrayFromString(string val)
        {
            List<ulong> Result = new List<ulong>();

            if (val.Contains(' '))
            {
                string[] str = Regex.Split(val, "\\s+", RegexOptions.IgnoreCase);

                foreach (var item in str)
                {
                    Result.Add(Convert.ToUInt64(item));
                }
            }
            else
            {
                Result.Add(Convert.ToUInt64(val));
            }

            return Result.ToArray();
        }

        #endregion 将字符串转转成64位整型数组
    }
}