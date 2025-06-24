using System.Text.RegularExpressions;

namespace hmt_energy_csharp.DataConvertHelper
{
    /// <summary>
    /// ushort转换库，返回的目标都是ushort，即uint16类型，也就是16位无符号整型数据
    /// </summary>
    public class UShortLib
    {
        #region 将字节数组转换成单个的16位无符号整型

        /// <summary>
        /// 将字节数组转换成单个整形数据ushort，即单个16位无符号整型数据，byte[]=》ushort
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="start">开始位置，默认为0</param>
        /// <param name="type">字节顺序，默认ABCD</param>
        /// <returns>单个16位无符号整型数据</returns>
        public static ushort GetUShortFromByteArray(byte[] source, int start = 0, DataFormat type = DataFormat.ABCD)
        {
            return BitConverter.ToUInt16(ByteArrayLib.Get2ByteArray(source, start, type), 0);
        }

        #endregion 将字节数组转换成单个的16位无符号整型

        #region 将字节数组转换成16位无符号整型数组

        /// <summary>
        /// 将字节数组转换成整形数组ushort[]，即16位无符号整型数组，byte[]=》ushort[]
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="type">字节顺序，默认ABCD</param>
        /// <returns>16位无符号整型数组</returns>
        public static ushort[] GetUShortArrayFromByteArray(byte[] source, DataFormat type = DataFormat.ABCD)
        {
            ushort[] result = new ushort[source.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = GetUShortFromByteArray(source, i * 2, type);
            }
            return result;
        }

        #endregion 将字节数组转换成16位无符号整型数组

        #region 将字符串转换成16位无符号整型数组

        /// <summary>
        /// 字符串转换成16位无符号整型数组
        /// </summary>
        /// <param name="val">字符串</param>
        /// <returns>ushort[]整型数组</returns>
        public static ushort[] GetUShortArrayFromString(string val)
        {
            List<ushort> Result = new List<ushort>();
            if (val.Contains(' '))
            {
                string[] str = Regex.Split(val, "\\s+", RegexOptions.IgnoreCase);
                foreach (var item in str)
                {
                    Result.Add(Convert.ToUInt16(item));
                }
            }
            else
            {
                Result.Add(Convert.ToUInt16(val));
            }
            return Result.ToArray();
        }

        #endregion 将字符串转换成16位无符号整型数组

        #region 将无符号整型某个位赋值

        /// <summary>
        /// 将无符号整型某个位赋值
        /// </summary>
        /// <param name="_Mask">操作位</param>
        /// <param name="a">操作的整数</param>
        /// <param name="flag">操作数</param>
        /// <returns>返回整数</returns>
        public static ushort SetIntegerSomeBit(int _Mask, ushort a, bool flag)
        {
            if (flag)
            {
                a = Convert.ToUInt16(a | (0x1 << _Mask));
            }
            else
            {
                a = Convert.ToUInt16(a & ~(0x1 << _Mask));
            }
            return a;
        }

        #endregion 将无符号整型某个位赋值
    }
}