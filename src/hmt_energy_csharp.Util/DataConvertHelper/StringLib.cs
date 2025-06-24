using System.Text;

namespace hmt_energy_csharp.DataConvertHelper
{
    /// <summary>
    /// 字符串转换库，返回的目标类型都是字符串
    /// </summary>
    public class StringLib
    {
        #region 将字节数组转换成字符串

        /// <summary>
        /// 将字节数组转换成字符串
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">固定长度</param>
        /// <returns>字符串</returns>
        public static string GetStringFromByteArrayByBitConvert(byte[] source, int start, int count)
        {
            return BitConverter.ToString(source, start, count);
        }

        #endregion 将字节数组转换成字符串

        #region 将字节数组转换成指定编码格式的字符串

        /// <summary>
        ///  将字节数组转换成指定编码格式的字符串
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">固定长度</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>字符串</returns>
        public static string GetStringFromByteArray(byte[] source, int start, int count, Encoding encoding)
        {
            return encoding.GetString(ByteArrayLib.GetByteArray(source, start, count));
        }

        #endregion 将字节数组转换成指定编码格式的字符串

        #region 将字节数组转换成ASCII格式的字符串

        /// <summary>
        ///  将字节数组转换成ASCII格式的字符串
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">固定长度</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>ASCII格式的字符串</returns>
        public static string GetStringFromByteArray(byte[] source, int start, int count)
        {
            return Encoding.ASCII.GetString(ByteArrayLib.GetByteArray(source, start, count));
        }

        #endregion 将字节数组转换成ASCII格式的字符串

        #region 将字节数组转换成带有空格的16进制字符串

        /// <summary>
        /// 将字节数组转换成带16进制字符串
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="start">开始位置</param>
        /// <param name="count">固定长度</param>
        /// <param name="segment">空格</param>
        /// <returns>16进制字符串 </returns>
        public static string GetHexStringFromByteArray(byte[] source, int start, int count, char segment = ' ')
        {
            byte[] b = ByteArrayLib.GetByteArray(source, start, count);
            StringBuilder sb = new StringBuilder();
            if (b.Length > 0)
            {
                foreach (var item in b)
                {
                    if (segment == 0) sb.Append(string.Format("{0:X2}", item));
                    else sb.Append(string.Format("{0:X2}{1}", item, segment));
                }
            }
            if (segment != 0 && sb.Length > 1 && sb[sb.Length - 1] == segment)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将字节数组转换成带有空格的16进制字符串
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="segment">空格</param>
        /// <returns>16进制字符串 </returns>
        public static string GetHexStringFromByteArray(byte[] source, char segment = ' ')
        {
            return GetHexStringFromByteArray(source, 0, source.Length, segment);
        }

        #endregion 将字节数组转换成带有空格的16进制字符串

        #region 将字节数组转换成西门子字符串

        public static string GetSiemensStringFromByteArray(byte[] source, int start, int length)
        {
            byte[] b = ByteArrayLib.GetByteArray(source, start, length + 2);

            int valid = b[1];
            if (valid > 0)
            {
                return Encoding.GetEncoding("GBK").GetString(ByteArrayLib.GetByteArray(b, 2, valid));
            }
            else
            {
                return "empty";
            }
        }

        #endregion 将字节数组转换成西门子字符串
    }
}