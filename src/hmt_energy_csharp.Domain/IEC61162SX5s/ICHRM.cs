using hmt_energy_csharp.VDRs;
using System;

namespace hmt_energy_csharp.IEC61162SX5s;

public class ICHRM : VdrEntity
{
    /// <summary>
    /// 横摇角度
    /// </summary>
    public double RollAngle { get; set; }

    /// <summary>
    /// 横摇周期
    /// </summary>
    public double RollPeriod { get; set; }

    /// <summary>
    /// 左舷横摇
    /// </summary>
    public double PortRollAngle { get; set; }

    /// <summary>
    /// 右舷横摇
    /// </summary>
    public double StarboardRollAngle { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string Status { get; set; }

    public ICHRM()
    {
    }

    public ICHRM(string sentence)
    {
        try
        {
            if (sentence == null)
                return;
            if (StringHelper.CRCCheck(sentence.Trim('$'), 2))
            {
                var strData = sentence[..^3];
                string[] strICHRMInfo = strData.Split(',');
                RollAngle = Convert.ToDouble(strICHRMInfo[1]);
                RollPeriod = Convert.ToDouble(strICHRMInfo[2]);
                PortRollAngle = Convert.ToDouble(strICHRMInfo[3]);
                StarboardRollAngle = Convert.ToDouble(strICHRMInfo[4]);
                Status = strICHRMInfo[5];
            }
        }
        catch (Exception)
        {
        }
    }
}