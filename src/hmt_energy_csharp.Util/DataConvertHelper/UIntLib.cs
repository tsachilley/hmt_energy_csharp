using System.Text.RegularExpressions;

namespace hmt_energy_csharp.DataConvertHelper
{
    /// <summary>
    /// UInt类型转换库，返回的目标都是uint，即uint32类型，也就是32位无符号整型数据
    /// </summary>
    public class UIntLib
    {
        #region 字节数组中截取转成单个的32位无符号整型

        /// <summary>
        /// 将字节数组中转换成单个的32位无符号整型，即byte[]=》uint
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="start">开始位置，默认是0</param>
        /// <param name="type">字节顺序，默认是ABCD</param>
        /// <returns>单个的32位无符号整型</returns>
        public static uint GetUIntFromByteArray(byte[] source, int start = 0, DataFormat type = DataFormat.ABCD)
        {
            return BitConverter.ToUInt32(ByteArrayLib.Get4ByteArray(source, start, type), 0);
        }

        #endregion 字节数组中截取转成单个的32位无符号整型

        #region 将字节数组中截取转成32位无符号整型数组

        /// <summary>
        /// 将字节数组中转换成32位无符号整型数组，即byte[]=》uint[]
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="type">字节顺序，默认是ABCD</param>
        /// <returns>32位无符号整型数组</returns>
        public static uint[] GetUIntArrayFromByteArray(byte[] source, DataFormat type = DataFormat.ABCD)
        {
            uint[] values = new uint[source.Length / 4];
            for (int i = 0; i < source.Length / 4; i++)
            {
                values[i] = GetUIntFromByteArray(source, 4 * i, type);
            }
            return values;
        }

        #endregion 将字节数组中截取转成32位无符号整型数组

        #region 将字符串转转成32位无符号整型数组

        public static uint[] GetUIntArrayFromString(string val)
        {
            List<uint> Result = new List<uint>();
            if (val.Contains(' '))
            {
                string[] str = Regex.Split(val, "\\s+", RegexOptions.IgnoreCase);

                foreach (var item in str)
                {
                    Result.Add(Convert.ToUInt32(item));
                }
            }
            else
            {
                Result.Add(Convert.ToUInt32(val));
            }

            return Result.ToArray();
        }

        #endregion 将字符串转转成32位无符号整型数组
    }
}