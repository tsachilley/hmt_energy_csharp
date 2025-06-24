using hmt_energy_csharp.VDRs;
using System;

namespace hmt_energy_csharp.IEC61162SX5s;

/// <summary>
/// 电子倾斜仪
/// </summary>
public class PFEC : VdrEntity
{
    /// <summary>
    /// 横向倾角
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// 纵向倾角
    /// </summary>
    public double Y { get; set; }

    public PFEC()
    {
    }

    public PFEC(string sentence)
    {
        try
        {
            if (sentence == null)
                return;
            if (StringHelper.CRCCheck(sentence.Trim('$'), 2))
            {
                var strData = sentence[..^3];
                string[] strPFECInfo = strData.Split(',');
                X = Convert.ToDouble(strPFECInfo[3]);
                Y = Convert.ToDouble(strPFECInfo[4]);
            }
        }
        catch (Exception)
        {
        }
    }
}