using System.Text.RegularExpressions;

namespace hmt_energy_csharp.DataConvertHelper
{
    /// <summary>
    /// short类型转换库，返回的目标全是short类型，即int16类型，也就是16位有符号整型数据，也包括无符号整数ushort
    /// </summary>
    public class ShortLib
    {
        #region 将字节数组中转换成16位整型

        /// <summary>
        /// 将字节数组转换成单个的16位有符号整型数据
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="start">开始位置，默认是0</param>
        /// <param name="dataFormat">字节顺序，默认是ABCD</param>
        /// <returns>单个的16位有符号整型数据</returns>
        public static short GetShortFromByteArray(byte[] source, int start = 0, DataFormat dataFormat = DataFormat.ABCD)
        {
            return BitConverter.ToInt16(ByteArrayLib.Get2ByteArray(source, start, dataFormat), 0);
        }

        #endregion 将字节数组中转换成16位整型

        #region 将字节数组转换成16位整型有符号数组

        /// <summary>
        /// 将字节数组转换成16位整型数组，即byte[]=> short[]
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="type">字节顺序，默认是ABCD</param>
        /// <returns>16位整型有符号数组</returns>
        public static short[] GetShortArrayFromByteArray(byte[] source, DataFormat type = DataFormat.ABCD)
        {
            short[] result = new short[source.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = GetShortFromByteArray(source, i * 2, type);
            }
            return result;
        }

        #endregion 将字节数组转换成16位整型有符号数组

        #region 将字符串转转成16位整型数组

        public static short[] GetShortArrayFromString(string val)
        {
            List<short> Result = new List<short>();

            if (val.Contains(' '))
            {
                string[] str = Regex.Split(val, "\\s+", RegexOptions.IgnoreCase);

                foreach (var item in str)
                {
                    Result.Add(Convert.ToInt16(item));
                }
            }
            else
            {
                Result.Add(Convert.ToInt16(val));
            }

            return Result.ToArray();
        }

        #endregion 将字符串转转成16位整型数组

        #region 设置16位整型或字节数组某个位

        public static short SetbitValueFromShort(short value, int bit, bool val, DataFormat dataFormat = DataFormat.ABCD)
        {
            byte[] bt = ByteArrayLib.GetByteArrayFromShort(value, dataFormat);

            if (bit >= 0 && bit <= 7)
            {
                bt[1] = ByteLib.SetbitValue(bt[1], bit, val);
            }
            else
            {
                bt[0] = ByteLib.SetbitValue(bt[0], bit - 8, val);
            }
            return ShortLib.GetShortFromByteArray(bt, 0, dataFormat);
        }

        public static short SetbitValueFrom2ByteArray(byte[] bt, int bit, bool val, DataFormat dataFormat = DataFormat.ABCD)
        {
            if (bit >= 0 && bit <= 7)
            {
                bt[1] = ByteLib.SetbitValue(bt[1], bit, val);
            }
            else
            {
                bt[0] = ByteLib.SetbitValue(bt[0], bit - 8, val);
            }
            return GetShortFromByteArray(bt, 0, dataFormat);
        }

        #endregion 设置16位整型或字节数组某个位
    }
}