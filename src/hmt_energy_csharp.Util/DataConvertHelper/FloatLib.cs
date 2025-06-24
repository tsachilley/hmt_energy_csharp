using System.Text.RegularExpressions;

namespace hmt_energy_csharp.DataConvertHelper
{
    /// <summary>
    /// Float类型转换库，返回的目标类型都是float类型，即浮点型数据，也就是有符号的小数，包括无符号的小数，有符号的整数，无符号的整数
    /// </summary>
    public class FloatLib
    {
        #region 字节数组中转换成单个的float浮点型数据

        /// <summary>
        /// 将字节数组中某4个字节转换成单个的Float浮点型数据
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="start">开始位置，默认是0</param>
        /// <param name="type">字节顺序，默认是ABCD</param>
        /// <returns>Float</returns>
        public static float GetFloatFromByteArray(byte[] source, int start = 0, DataFormat type = DataFormat.ABCD)
        {
            byte[] result = ByteArrayLib.Get4ByteArray(source, start, type);
            if (result != null)
            {
                return BitConverter.ToSingle(result, 0);
            }
            else
            {
                return 0.0f;
            }
        }

        #endregion 字节数组中转换成单个的float浮点型数据

        #region 将字节数组转换成float浮点型数组

        /// <summary>
        /// 将字节数组转换成Float数组
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <param name="type">字节顺序，默认是ABCD</param>
        /// <returns>Float数组</returns>
        public static float[] GetFloatArrayFromByteArray(byte[] source, DataFormat type = DataFormat.ABCD)
        {
            float[] values = new float[source.Length / 4];
            for (int i = 0; i < source.Length / 4; i++)
            {
                values[i] = GetFloatFromByteArray(source, 4 * i, type);
            }
            return values;
        }

        #endregion 将字节数组转换成float浮点型数组

        #region 将字符串转换成浮点型数组

        /// <summary>
        /// 将Float字符串转换成单精度浮点型数组
        /// </summary>
        /// <param name="val">Float字符串</param>
        /// <returns>单精度浮点型数组</returns>
        public static float[] GetFloatArrayFromString(string val)
        {
            List<float> Result = new List<float>();
            if (val.Contains(' '))
            {
                string[] str = Regex.Split(val, "\\s+", RegexOptions.IgnoreCase);

                foreach (var item in str)
                {
                    Result.Add(Convert.ToSingle(item));
                }
            }
            else
            {
                Result.Add(Convert.ToSingle(val));
            }

            return Result.ToArray();
        }

        #endregion 将字符串转换成浮点型数组
    }
}