using System.Text.RegularExpressions;

namespace hmt_energy_csharp.DataConvertHelper
{
    /// <summary>
    /// int类型转换库，返回的目标都是int，即int32类型，也就是32位有符号整型数据，包括无符号整形数据uint
    /// </summary>
    public class IntLib
    {
        #region 将字节数组转换成单个的32位有符号整型数据

        /// <summary>
        /// 将字节数组转换成单个的32位有符号整型数据，即byte[]=》int
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="start">开始位置，默认是0</param>
        /// <param name="type">字节顺序，默认是ABCD</param>
        /// <returns>单个的32位有符号整型数据</returns>
        public static int GetIntFromByteArray(byte[] source, int start = 0, DataFormat type = DataFormat.ABCD)
        {
            return BitConverter.ToInt32(ByteArrayLib.Get4ByteArray(source, start, type), 0);
        }

        #endregion 将字节数组转换成单个的32位有符号整型数据

        #region 将字节数组转换成32位int整型数组

        /// <summary>
        /// 将字节数组转换成32位int整型数组，即32位有符号整型数组，byte[]=》int[]
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="type">字节顺序，默认ABCD</param>
        /// <returns>32位有符号整型数组</returns>
        public static int[] GetIntArrayFromByteArray(byte[] source, DataFormat type = DataFormat.ABCD)
        {
            int[] values = new int[source.Length / 4];
            for (int i = 0; i < source.Length / 4; i++)
            {
                values[i] = GetIntFromByteArray(source, 4 * i, type);
            }
            return values;
        }

        #endregion 将字节数组转换成32位int整型数组

        #region 将字符串转转成32位整型数组

        /// <summary>
        /// 将字符串转转成32位整型数组
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int[] GetIntArrayFromString(string val)
        {
            List<int> Result = new List<int>();
            if (val.Contains(' '))
            {
                string[] str = Regex.Split(val, "\\s+", RegexOptions.IgnoreCase);

                foreach (var item in str)
                {
                    Result.Add(Convert.ToInt32(item));
                }
            }
            else
            {
                Result.Add(Convert.ToInt32(val));
            }

            return Result.ToArray();
        }

        #endregion 将字符串转转成32位整型数组
    }
}