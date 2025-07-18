﻿namespace hmt_energy_csharp.DataConvertHelper
{
    /// <summary>
    /// 位转换库
    /// </summary>
    public class BitLib
    {
        #region 根据字节及数组获取指定位

        /// <summary>
        /// 返回指定字节的指定位
        /// </summary>
        /// <param name="b">字节</param>
        /// <param name="offset">指定位（0-7）</param>
        /// <returns>布尔结果</returns>
        public static bool GetBitFromByte(byte b, int offset)
        {
            if (offset >= 0 && offset <= 7)
            {
                return (b & (int)Math.Pow(2, offset)) != 0;
            }
            return false;
        }

        /// <summary>
        /// 返回字节数组中的某个字节某个位
        /// </summary>
        /// <param name="b">字节数组</param>
        /// <param name="index">字节索引</param>
        /// <param name="offset">指定位（0-7）</param>
        /// <returns>布尔结果</returns>
        public static bool GetBitFromByteArray(byte[] b, int index, int offset)
        {
            byte[] res = ByteArrayLib.GetByteArray(b, index, 1);

            return GetBitFromByte(res[0], offset);
        }

        #endregion 根据字节及数组获取指定位

        #region 根据两个字节及数组获取指定位

        /// <summary>
        /// 返回两个字节的指定位
        /// </summary>
        /// <param name="b">两个字节</param>
        /// <param name="offset">指定位（0-15）</param>
        /// <param name="reverse">字节顺序</param>
        /// <returns>布尔结果</returns>
        public static bool GetBitFrom2Byte(byte[] b, int offset, bool reverse = false)
        {
            byte low = reverse ? b[0] : b[1];
            byte high = reverse ? b[1] : b[0];

            if (offset >= 0 && offset <= 7)
            {
                return GetBitFromByte(low, offset);
            }
            else if (offset >= 8 && offset <= 15)
            {
                return GetBitFromByte(high, offset - 8);
            }
            else
            {
                throw new Exception("索引必须为0-15之间");
            }
        }

        /// <summary>
        /// 返回字节数组中某2个字节的指定位
        /// </summary>
        /// <param name="b">字节数组</param>
        /// <param name="index">字节索引</param>
        /// <param name="offset">指定位（0-15）</param>
        /// <param name="reverse">字节顺序</param>
        /// <returns>布尔结果</returns>
        public static bool GetBitFrom2ByteArray(byte[] b, int index, int offset, bool reverse = false)
        {
            byte[] res = ByteArrayLib.GetByteArray(b, index, 2);

            return GetBitFrom2Byte(res, offset, reverse);
        }

        #endregion 根据两个字节及数组获取指定位

        #region 根据一个Short或UShort返回指定位

        /// <summary>
        /// 根据一个Short返回指定位
        /// </summary>
        /// <param name="val">short数值</param>
        /// <param name="offset">指定位（0-15）</param>
        /// <param name="reverse">字节顺序</param>
        /// <returns>布尔结果</returns>
        public static bool GetBitFromShort(short val, int offset, bool reverse = false)
        {
            return GetBitFrom2Byte(BitConverter.GetBytes(val), offset, reverse);
        }

        /// <summary>
        /// 根据一个UShort返回指定位
        /// </summary>
        /// <param name="val">ushort数值</param>
        /// <param name="offset">指定位（0-15）</param>
        /// <param name="reverse">字节顺序</param>
        /// <returns>布尔结果</returns>
        public static bool GetBitFromUShort(ushort val, int offset, bool reverse = false)
        {
            return GetBitFrom2Byte(BitConverter.GetBytes(val), offset, reverse);
        }

        #endregion 根据一个Short或UShort返回指定位

        #region 根据字节及数组返回布尔数组

        /// <summary>
        /// 将一个字节转换成布尔数组
        /// </summary>
        /// <param name="b">字节</param>
        /// <param name="reverse">位顺序</param>
        /// <returns>布尔数组</returns>
        public static bool[] GetBitArrayFromByte(byte b, bool reverse = false)
        {
            bool[] array = new bool[8];

            if (reverse)
            {
                for (int i = 7; i >= 0; i--)
                { //对于byte的每bit进行判定
                    array[i] = (b & 1) == 1;   //判定byte的最后一位是否为1，若为1，则是true；否则是false
                    b = (byte)(b >> 1);       //将byte右移一位
                }
            }
            else
            {
                for (int i = 0; i <= 7; i++)
                { //对于byte的每bit进行判定
                    array[i] = (b & 1) == 1;   //判定byte的最后一位是否为1，若为1，则是true；否则是false
                    b = (byte)(b >> 1);       //将byte右移一位
                }
            }
            return array;
        }

        /// <summary>
        /// 将一个字节数组转换成布尔数组
        /// </summary>
        /// <param name="b">字节数组</param>
        /// <param name="reverse">位顺序</param>
        /// <returns>布尔数组</returns>
        public static bool[] GetBitArrayFromByteArray(byte[] b, bool reverse = false)
        {
            List<bool> res = new List<bool>();

            foreach (var item in b)
            {
                res.AddRange(GetBitArrayFromByte(item, reverse));
            }
            return res.ToArray();
        }

        #endregion 根据字节及数组返回布尔数组
    }
}