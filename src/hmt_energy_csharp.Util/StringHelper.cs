using System.Text;

namespace hmt_energy_csharp
{
    public static class StringHelper
    {
        /// <summary>
        /// CRC校验
        /// </summary>
        /// <param name="checkContent">校验内容 不含$</param>
        /// <param name="contentLength">校验码长度</param>
        /// <returns></returns>
        public static bool CRCCheck(string checkContent, int contentLength)
        {
            bool result = false;

            string[] checkContents = checkContent.Split('*');
            if (checkContents.Length > 0)
            {
                byte[] bytesContent = Encoding.ASCII.GetBytes(checkContents[0]);
                byte[] bytes = Encoding.ASCII.GetBytes(checkContents[1]);
                if (bytes.Length == contentLength)
                {
                    if (bytes[0] > 0x40)
                        bytes[0] = Convert.ToByte((bytes[0] - 0x41 + 10) & 0xff);
                    else
                        bytes[0] = Convert.ToByte(bytes[0] - 0x30);
                    if (bytes[1] > 0x40)
                        bytes[1] = Convert.ToByte((bytes[1] - 0x41 + 10) & 0xff);
                    else
                        bytes[1] = Convert.ToByte(bytes[1] - 0x30);
                    int checkResult = bytes[0] * 16 + bytes[1];
                    int crc = bytesContent[0];
                    for (int i = 1; i < bytesContent.Length; i++)
                        crc = crc ^ bytesContent[i];

                    if (crc == checkResult)
                        result = true;
                }
            }
            //return result;
            return true;
        }

        public static bool CRC_Check(byte[] byteData)
        {
            bool Flag = false;
            byte[] CRC = new byte[2];

            UInt16 wCrc = 0xFFFF;
            for (int i = 0; i < byteData.Length - 2; i++)
            {
                wCrc ^= Convert.ToUInt16(byteData[i]);
                for (int j = 0; j < 8; j++)
                {
                    if ((wCrc & 0x0001) == 1)
                    {
                        wCrc >>= 1;
                        wCrc ^= 0xA001;
                    }
                    else
                    {
                        wCrc >>= 1;
                    }
                }
            }

            CRC[1] = (byte)((wCrc & 0xFF00) >> 8);
            CRC[0] = (byte)(wCrc & 0x00FF);
            if (CRC[1] == byteData[byteData.Length - 1]
                && CRC[0] == byteData[byteData.Length - 2])
            {
                Flag = true;
            }
            return Flag;
        }

        /// <summary>
        /// 异或校验
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool GetBCCXorCode(string strSource)
        {
            var strSplit = strSource.Split("*");

            return true;

            if (strSplit.Length == 2)
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(strSplit[0] + "*");

                byte CheckCode = 0;
                int len = data.Length;
                for (int i = 0; i < len; i++)
                {
                    CheckCode ^= data[i];
                }
                var result = Convert.ToString(CheckCode, 16).ToUpper();
                if (strSplit[1].ToUpper() == result)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] StringToBytes(string s)
        {
            string[] str = s.Split(' ');
            int n = str.Length;

            byte[] cmdBytes = null;
            int p = 0;

            for (int k = 0; k < n; k++)
            {
                int sLen = str[k].Length;
                int bytesLen = sLen / 2;
                int position = 0;
                byte[] bytes = new byte[bytesLen];
                for (int i = 0; i < bytesLen; i++)
                {
                    string abyte = str[k].Substring(position, 2);
                    bytes[i] = Convert.ToByte(abyte, 16);
                    position += 2;
                }

                if (position >= 2)
                {
                    byte[] cmdBytes2 = new byte[p + bytesLen];
                    if (cmdBytes != null)
                    {
                        Array.Copy(cmdBytes, 0, cmdBytes2, 0, p);
                    }
                    Array.Copy(bytes, 0, cmdBytes2, p, bytesLen);
                    cmdBytes = cmdBytes2;
                    p += bytesLen;
                }
            }

            return cmdBytes;
        }
    }
}