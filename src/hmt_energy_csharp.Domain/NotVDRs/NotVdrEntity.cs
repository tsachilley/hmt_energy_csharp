using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace hmt_energy_csharp.NotVDRs
{
    public class NotVdrEntity
    {
        private readonly IDictionary<string, List<float>> _dictFilter;

        //kg/h
        public float? DGInFC { get; set; }

        //kg/m3
        public float? DGInDensity { get; set; }

        //℃
        public float? DGInTemperature { get; set; }

        //kg
        public float? DGInAcc { get; set; }

        //kg/h
        public float? DGOutFC { get; set; }

        //℃
        public float? DGOutTemperature { get; set; }

        //kg/m3
        public float? DGOutDensity { get; set; }

        //kg
        public float? DGOutAcc { get; set; }

        //kg/h
        public float? DGMgoFC { get; set; }

        //℃
        public float? DGMgoTemperature { get; set; }

        //kg/m3
        public float? DGMgoDensity { get; set; }

        //kg
        public float? DGMgoAcc { get; set; }

        //kg/s
        public float? BlrFC { get; set; }

        //K 减273.15
        public float? BlrTemperature { get; set; }

        //kg/m3
        public float? BlrDensity { get; set; }

        //kg
        public float? BlrAcc { get; set; }

        //kg/s
        public float? BlrMGOFC { get; set; }

        //K 减273.15
        public float? BlrMGOTemperature { get; set; }

        //kg/m3
        public float? BlrMGODensity { get; set; }

        //kg
        public float? BlrMGOAcc { get; set; }

        //kg/s
        public float? MEFC { get; set; }

        //K 减273.15
        public float? METemperature { get; set; }

        //kg/m3
        public float? MEDensity { get; set; }

        //kg
        public float? MEAcc { get; set; }

        //KW
        public float? DG1Power { get; set; }

        //KW
        public float? DG2Power { get; set; }

        //KW
        public float? DG3Power { get; set; }

        //mm
        public float? DraftBow { get; set; }

        //mm
        public float? DraftPort { get; set; }

        //mm
        public float? DraftStarboard { get; set; }

        //mm
        public float? DraftAstern { get; set; }

        //rpm
        public float? MERpm { get; set; }

        //KNm
        public float? METorque { get; set; }

        //KW
        public float? MEPower { get; set; }

        public NotVdrEntity()
        {
        }

        public NotVdrEntity(string sentence, string vdrId, IDictionary<string, List<float>> dictFilter)
        {
            if (sentence.IsNullOrWhiteSpace())
                return;
            _dictFilter = dictFilter;

            var receiveData = StringHelper.StringToBytes(sentence);
            int CanCount = receiveData[0];
            receiveData = receiveData.Skip(1).ToArray();
            for (int i = 0; i < CanCount; i++)
            {
                //单个报文长度
                int CanLength = 1;
                string ZID = "";
                int ZTypeCount = 1;
                //帧类型
                string ZType = (receiveData[0] & 0x80) != 0 ? "扩展帧" : "标准帧";
                //帧格式
                string ZFormat = (receiveData[0] & 0x40) != 0 ? "远程帧" : "数据帧";
                //数据长度
                int DataLength = (receiveData[0] & 0xf);
                //CAN来源 0:CAN1 1:CAN2
                string CanSource = (receiveData[0] & 0x10) == 0 ? "CAN1" : "CAN2";

                CanLength += DataLength;
                switch (ZType)
                {
                    case "标准帧":
                        ZTypeCount = 2;
                        string s_str1 = receiveData[1].ToString("X2");
                        string s_str2 = receiveData[2].ToString("X2");
                        ZID = s_str1 + s_str2;
                        CanLength = CanLength + 2;
                        break;

                    case "扩展帧":
                        ZTypeCount = 4;
                        string e_str1 = receiveData[1].ToString("X2");
                        string e_str2 = receiveData[2].ToString("X2");
                        string e_str3 = receiveData[3].ToString("X2");
                        string e_str4 = receiveData[4].ToString("X2");
                        ZID = e_str1 + e_str2 + e_str3 + e_str4;
                        CanLength = CanLength + 4;
                        break;

                    default:
                        break;
                }

                var lstData = new List<string>();
                for (int j = 0; j < DataLength; j++)
                {
                    lstData.Add(receiveData[ZTypeCount + 1 + j].ToString("X2"));
                }
                switch (CanSource)
                {
                    case "CAN1":
                        //uint count = receiveCount;
                        string _zid = ZID;
                        string _zformat = ZFormat;
                        string _ztype = ZType;
                        int _dataLength = DataLength;

                        CanGetAI(_zid, lstData.ToArray(), vdrId);
                        break;

                    default:

                        break;
                }
                receiveData = receiveData.Skip(CanLength).Take(receiveData.Length - CanLength).ToArray();//去掉这个报文的内容
            }
        }

        /// <summary>
        /// 根据表格获取can数据
        /// </summary>
        /// <param name="zid">帧ID</param>
        private void CanGetAI(string zid, string[] candata, string vdrId)
        {
            dynamic tempEntity = new ExpandoObject();
            //根据帧ID获取相应的值
            if (zid == "1888C481" || zid == "1888E481" || zid == "18890481")
            {
                //主机
                switch (zid)
                {
                    case "1888C481":
                        METemperature = BytesToSingle("KROHNE", new List<string> { candata[4], candata[5], candata[6], candata[7] }); //k 减273.15
                        MEFC = Filter("MEVelocity", BytesToSingle("KROHNE", new List<string> { candata[0], candata[1], candata[2], candata[3] }), 12, vdrId); //kg/s
                        break;

                    case "1888E481":
                        MEDensity = BytesToSingle("KROHNE", new List<string> { candata[0], candata[1], candata[2], candata[3] }); //kg/m3
                        break;

                    case "18890481":
                        MEAcc = BytesToSingle("KROHNE", new List<string> { candata[0], candata[1], candata[2], candata[3] }); //kg
                        break;
                }
            }
            else if (zid == "18880461" || zid == "18882461")
            {
                //辅机进
                switch (zid)
                {
                    case "18880461":
                        DGInDensity = BytesToSingle("EMERSON", new List<string> { candata[4], candata[5], candata[6], candata[7] }); //kg/m3
                        DGInFC = Filter("DGVelocityIn", BytesToSingle("EMERSON", new List<string> { candata[0], candata[1], candata[2], candata[3] }), 12, vdrId); //kg/h
                        break;

                    case "18882461":
                        DGInAcc = BytesToSingle("EMERSON", new List<string> { candata[4], candata[5], candata[6], candata[7] }); //kg
                        DGInTemperature = BytesToSingle("EMERSON", new List<string> { candata[0], candata[1], candata[2], candata[3] }); //℃
                        break;
                }
            }
            else if (zid == "18884461" || zid == "18886461")
            {
                //辅机出
                switch (zid)
                {
                    case "18884461":
                        DGOutTemperature = BytesToSingle("EMERSON", new List<string> { candata[4], candata[5], candata[6], candata[7] }); //℃
                        DGOutFC = Filter("DGVelocityOut", BytesToSingle("EMERSON", new List<string> { candata[0], candata[1], candata[2], candata[3] }), 12, vdrId); //kg/h
                        break;

                    case "18886461":
                        DGOutAcc = BytesToSingle("EMERSON", new List<string> { candata[4], candata[5], candata[6], candata[7] }); //kg
                        DGOutDensity = BytesToSingle("EMERSON", new List<string> { candata[0], candata[1], candata[2], candata[3] }); //kg/m3
                        break;
                }
            }
            else if (zid == "18888461" || zid == "1888A461")
            {
                //辅机轻油进
                switch (zid)
                {
                    case "18888461":
                        DGMgoTemperature = BytesToSingle("EMERSON", new List<string> { candata[4], candata[5], candata[6], candata[7] }); //℃
                        DGMgoFC = Filter("DGVelocityMgoIn", BytesToSingle("EMERSON", new List<string> { candata[0], candata[1], candata[2], candata[3] }), 12, vdrId); //kg/h
                        break;

                    case "1888A461":
                        DGMgoAcc = BytesToSingle("EMERSON", new List<string> { candata[4], candata[5], candata[6], candata[7] }); //kg
                        DGMgoDensity = BytesToSingle("EMERSON", new List<string> { candata[0], candata[1], candata[2], candata[3] }); //kg/m3
                        break;
                }
            }
            else if (zid == "18880481" || zid == "18882481" || zid == "18884481")
            {
                //锅炉进
                switch (zid)
                {
                    case "18880481":
                        BlrTemperature = BytesToSingle("KROHNE", new List<string> { candata[4], candata[5], candata[6], candata[7] }); //k 减273.15
                        BlrFC = Filter("BLRVelocityIn", BytesToSingle("KROHNE", new List<string> { candata[0], candata[1], candata[2], candata[3] }), 12, vdrId); //kg/s
                        break;

                    case "18882481":
                        BlrDensity = BytesToSingle("KROHNE", new List<string> { candata[0], candata[1], candata[2], candata[3] }); //kg/m3
                        break;

                    case "18884481":
                        BlrAcc = BytesToSingle("KROHNE", new List<string> { candata[0], candata[1], candata[2], candata[3] }); //kg
                        break;
                }
            }
            else if (zid == "18886481" || zid == "18888481" || zid == "1888A481")
            {
                //锅炉轻油
                switch (zid)
                {
                    case "18886481":
                        BlrMGOTemperature = BytesToSingle("KROHNE", new List<string> { candata[4], candata[5], candata[6], candata[7] }); //k 减273.15
                        BlrMGOFC = Filter("BLRVelocityMgoIn", BytesToSingle("KROHNE", new List<string> { candata[0], candata[1], candata[2], candata[3] }), 12, vdrId); //kg/s
                        break;

                    case "18888481":
                        BlrMGODensity = BytesToSingle("KROHNE", new List<string> { candata[0], candata[1], candata[2], candata[3] }); //kg/m3
                        break;

                    case "1888A481":
                        BlrMGOAcc = BytesToSingle("KROHNE", new List<string> { candata[0], candata[1], candata[2], candata[3] }); //kg
                        break;
                }
            }
            else if (zid == "188804A1" || zid == "188824A1" || zid == "188844A1")
            {
                //轴功率仪、吃水监测
                switch (zid)
                {
                    case "188804A1":
                        DG1Power = Convert.ToInt32(candata[1] + candata[0], 16); //电机1功率 KW
                        DG2Power = Convert.ToInt32(candata[3] + candata[2], 16); //电机2功率 KW
                        DG3Power = Convert.ToInt32(candata[5] + candata[4], 16); //电机3功率 KW
                        DraftBow = Convert.ToInt32(candata[7] + candata[6], 16) / 100f; //船艏吃水 mm
                        break;

                    case "188824A1":
                        DraftPort = Convert.ToInt32(candata[1] + candata[0], 16) / 100f; //船艏吃水 mm
                        DraftStarboard = Convert.ToInt32(candata[3] + candata[2], 16) / 100f; //船艏吃水 mm
                        DraftAstern = Convert.ToInt32(candata[5] + candata[4], 16) / 100f; //船艏吃水 mm
                        MERpm = Convert.ToInt32(candata[7] + candata[6], 16); //主机转速 rpm
                        break;

                    case "188844A1":
                        METorque = Convert.ToInt32(candata[1] + candata[0], 16); //轴扭矩 KNm
                        MEPower = Convert.ToInt32(candata[3] + candata[2], 16); //主机功率 KW
                        break;
                }
            }
        }

        /// <summary>
        /// 不同流量计解析方式不同
        /// </summary>
        /// <param name="FMType">流量计种类</param>
        /// <param name="nBytes">源数据</param>
        /// <returns></returns>
        private float? BytesToSingle(string FMType, List<string> nBytes)
        {
            //值
            float value = 0;
            string[] sBytes = new string[4] { "0", "0", "0", "0" };

            switch (FMType)
            {
                case "YINUO":
                    sBytes[0] = nBytes[3];
                    sBytes[1] = nBytes[2];
                    sBytes[2] = nBytes[1];
                    sBytes[3] = nBytes[0];
                    break;

                case "KROHNE":
                    sBytes[0] = nBytes[0];
                    sBytes[1] = nBytes[1];
                    sBytes[2] = nBytes[2];
                    sBytes[3] = nBytes[3];
                    break;

                case "EMERSON":
                    sBytes[0] = nBytes[2];
                    sBytes[1] = nBytes[3];
                    sBytes[2] = nBytes[0];
                    sBytes[3] = nBytes[1];
                    break;

                case "OTHERS":
                    sBytes[0] = nBytes[0];
                    sBytes[1] = nBytes[1];
                    sBytes[2] = nBytes[2];
                    sBytes[3] = nBytes[3];
                    break;
            }
            UInt32 x = Convert.ToUInt32(sBytes[0] + sBytes[1] + sBytes[2] + sBytes[3], 16);//字符串转16进制32位无符号整数
            value = BitConverter.ToSingle(BitConverter.GetBytes(x), 0);//IEEE754 字节转换float

            return Convert.ToSingle(Math.Round(value, 4));
        }

        /// <summary>
        /// 滤波
        /// </summary>
        /// <param name="paramName">需要滤波的参数</param>
        /// <param name="tempValue">当前值</param>
        /// <param name="filterValue">阈值</param>
        /// <param name="vdrId">设备Id</param>
        /// <returns></returns>
        public float? Filter(string paramName, float? tempValueNull, float filterValue, string vdrId)
        {
            float result = 0;
            var tempValue = Convert.ToSingle(tempValueNull);

            try
            {
                List<float> lstTemp = new List<float>();
                if (_dictFilter.ContainsKey(vdrId + "_" + paramName))
                {
                    lstTemp = _dictFilter[vdrId + "_" + paramName];
                }

                if (lstTemp.Count >= filterValue)
                {
                    lstTemp.Add(tempValue);
                    lstTemp.Remove(0);
                    result = lstTemp.Average();
                }
                else
                {
                    lstTemp.Add(tempValue);
                    result = tempValue;
                }

                if (!_dictFilter.ContainsKey(vdrId + "_" + paramName))
                {
                    _dictFilter.Add(vdrId + "_" + paramName, lstTemp);
                }
            }
            catch (Exception)
            {
                //Log.Error(ex, "滤波器计算错误=>" + paramName + ":" + tempValue);
            }

            return result;
        }
    }
}