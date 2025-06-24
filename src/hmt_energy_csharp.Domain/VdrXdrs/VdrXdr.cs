using hmt_energy_csharp.VDRs;
using System;
using System.Text;

namespace hmt_energy_csharp.VdrXdrs
{
    public class VdrXdr : VdrEntity
    {
        /// <summary>
        /// 传感器类型
        /// </summary>
        public string sensortype { get; set; }

        /// <summary>
        /// 传感器数值
        /// </summary>
        public string sensorvalue { get; set; }

        /// <summary>
        /// 数值单位
        /// </summary>
        public string sensorunit { get; set; }

        /// <summary>
        /// 传感器ID
        /// </summary>
        public string sensorid { get; set; }

        public VdrXdr()
        {
        }

        public VdrXdr(string sentence)
        {
            try
            {
                if (sentence == null)
                    return;
                if (StringHelper.CRCCheck(sentence.Trim('$'), 2))
                {
                    var strData = sentence.Substring(0, sentence.Length - 3);
                    string[] strXDRInfo = strData.Split(',');
                    string guid = Guid.NewGuid().ToString();
                    StringBuilder sbSql = new StringBuilder();
                    sbSql.Append("insert into vdr_xdr (create_time,sentenceid,type,sensortype,sensorvalue,sensorunit,sensorid)" +
                        $" values (SYSDATE(),'{guid}','{strXDRInfo[0].Trim('$')}','{strXDRInfo[1]}','{strXDRInfo[2]}','{strXDRInfo[3]}','{strXDRInfo[4]}')");
                    sensortype = strXDRInfo[1];
                    sensorvalue = strXDRInfo[2];
                    sensorunit = strXDRInfo[3];
                    sensorid = strXDRInfo[4];
                }
            }
            catch (Exception)
            {
            }
        }
    }
}