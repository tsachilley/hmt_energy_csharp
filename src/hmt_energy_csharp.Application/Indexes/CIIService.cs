using hmt_energy_csharp.CII.Coefficients;
using hmt_energy_csharp.CII.FuelCoefficients;
using hmt_energy_csharp.CII.Ratings;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hmt_energy_csharp.Indexes
{
    public class CIIService : hmt_energy_csharpAppService, ICIIService
    {
        private readonly ILogger<CIIService> _logger;
        private readonly IFuelCoefficientRepository _fuelCoefficient;
        private readonly ICIICoefficientRepository _ciiCoefficient;
        private readonly ICIIRatingRepository _ciiRating;

        public CIIService(ILogger<CIIService> logger, IFuelCoefficientRepository fuelCoefficient, ICIICoefficientRepository ciiCoefficient, ICIIRatingRepository ciiRating)
        {
            this._logger = logger;
            this._fuelCoefficient = fuelCoefficient;
            this._ciiCoefficient = ciiCoefficient;
            this._ciiRating = ciiRating;
        }

        /// <summary>
        /// 计算基于DWT的碳排放指数
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<double> CalcCDWT(string parameters)
        {
            try
            {
                var queryParams = parameters.ToJObject();
                var dwt = 0d;
                if (queryParams.ContainsKey("dwt") && !string.IsNullOrWhiteSpace(queryParams["dwt"].ToString()))
                {
                    dwt = Convert.ToDouble(queryParams["dwt"]);
                }
                var speedGround = 0d;
                if (queryParams.ContainsKey("speedGround") && !string.IsNullOrWhiteSpace(queryParams["speedGround"].ToString()))
                {
                    speedGround = Convert.ToDouble(queryParams["speedGround"]);
                }

                return (await CalcCEmission(parameters)) * 1000d / (dwt * speedGround * 1.852);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "计算基于DWT的碳排放指数错误");
            }
            return double.NaN;
        }

        /// <summary>
        /// 计算C排放量
        /// </summary>
        /// <param name="parameters">计算参数,json格式</param>
        /// <returns></returns>
        public async Task<double> CalcCEmission(string parameters)
        {
            try
            {
                var queryParams = parameters.ToJObject();
                var DGO = 0m;
                if (queryParams.ContainsKey("DGO") && !string.IsNullOrWhiteSpace(queryParams["DGO"].ToString()))
                {
                    DGO = Convert.ToDecimal(queryParams["DGO"]);
                }
                var LFO = 0m;
                if (queryParams.ContainsKey("LFO") && !string.IsNullOrWhiteSpace(queryParams["LFO"].ToString()))
                {
                    LFO = Convert.ToDecimal(queryParams["LFO"]);
                }
                var HFO = 0m;
                if (queryParams.ContainsKey("HFO") && !string.IsNullOrWhiteSpace(queryParams["HFO"].ToString()))
                {
                    HFO = Convert.ToDecimal(queryParams["HFO"]);
                }
                var LPG_P = 0m;
                if (queryParams.ContainsKey("LPG_P") && !string.IsNullOrWhiteSpace(queryParams["LPG_P"].ToString()))
                {
                    LPG_P = Convert.ToDecimal(queryParams["LPG_P"]);
                }
                var LPG_B = 0m;
                if (queryParams.ContainsKey("LPG_B") && !string.IsNullOrWhiteSpace(queryParams["LPG_B"].ToString()))
                {
                    LPG_B = Convert.ToDecimal(queryParams["LPG_B"]);
                }
                var LNG = 0m;
                if (queryParams.ContainsKey("LNG") && !string.IsNullOrWhiteSpace(queryParams["LNG"].ToString()))
                {
                    LNG = Convert.ToDecimal(queryParams["LNG"]);
                }
                var Methanol = 0m;
                if (queryParams.ContainsKey("Methanol") && !string.IsNullOrWhiteSpace(queryParams["Methanol"].ToString()))
                {
                    Methanol = Convert.ToDecimal(queryParams["Methanol"]);
                }
                var Ethanol = 0m;
                if (queryParams.ContainsKey("Ethanol") && !string.IsNullOrWhiteSpace(queryParams["Ethanol"].ToString()))
                {
                    Ethanol = Convert.ToDecimal(queryParams["Ethanol"]);
                }

                var resultCofficient = await _fuelCoefficient.GetListAsync(t => t.delete_time == null);

                var DGOValue = DGO * resultCofficient.FirstOrDefault(t => t.Code == "DGO")?.Value;
                var LFOValue = LFO * resultCofficient.FirstOrDefault(t => t.Code == "LFO")?.Value;
                var HFOValue = HFO * resultCofficient.FirstOrDefault(t => t.Code == "HFO")?.Value;
                var LPG_PValue = LPG_P * resultCofficient.FirstOrDefault(t => t.Code == "LPG_P")?.Value;
                var LPG_BValue = LPG_B * resultCofficient.FirstOrDefault(t => t.Code == "LPG_B")?.Value;
                var LNGValue = LNG * resultCofficient.FirstOrDefault(t => t.Code == "LNG")?.Value;
                var MethanolValue = Methanol * resultCofficient.FirstOrDefault(t => t.Code == "Methanol")?.Value;
                var EthanolValue = Ethanol * resultCofficient.FirstOrDefault(t => t.Code == "Ethanol")?.Value;
                return (double)((DGOValue ?? 0) + (LFOValue ?? 0) + (HFOValue ?? 0) + (LPG_PValue ?? 0) + (LPG_BValue ?? 0) + (LNGValue ?? 0) + (MethanolValue ?? 0) + (EthanolValue ?? 0));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "计算CO2Emission错误");
            }
            return double.NaN;
        }

        /// <summary>
        /// 计算CII
        /// </summary>
        /// <param name="parameters">计算参数,json格式</param>
        /// <returns></returns>
        public async Task<object> CalcCII(string parameters)
        {
            try
            {
                var CIIRef = 0.0d;
                var RCII = 0.0d;
                var CO2Emission = 0.0d;
                var ACII = 0.0d;
                var D1 = 0.0d;
                var D2 = 0.0d;
                var D3 = 0.0d;
                var D4 = 0.0d;
                var CIIRating = "";

                #region 输入参数

                var queryParams = parameters.ToJObject();
                var shipType = "";
                if (queryParams.ContainsKey("shipType") && !string.IsNullOrWhiteSpace(queryParams["shipType"].ToString()))
                {
                    shipType = queryParams["shipType"].ToString();
                }
                var dwt = 0;
                if (queryParams.ContainsKey("dwt") && !string.IsNullOrWhiteSpace(queryParams["dwt"].ToString()))
                {
                    dwt = Convert.ToInt32(queryParams["dwt"].ToString());
                }
                var gt = 0;
                if (queryParams.ContainsKey("gt") && !string.IsNullOrWhiteSpace(queryParams["gt"].ToString()))
                {
                    gt = Convert.ToInt32(queryParams["gt"].ToString());
                }
                var selectedYear = 0;
                if (queryParams.ContainsKey("year") && !string.IsNullOrWhiteSpace(queryParams["year"].ToString()))
                {
                    selectedYear = Convert.ToInt32(queryParams["year"].ToString());
                }
                var distance = 0d;
                if (queryParams.ContainsKey("distance") && !string.IsNullOrWhiteSpace(queryParams["distance"].ToString()))
                {
                    distance = Convert.ToDouble(queryParams["distance"]);
                }

                #endregion 输入参数

                var shipTypeCode = "";
                switch (shipType)
                {
                    case "1":// "bulk carrier":
                        shipTypeCode = "bulk carrier";
                        break;

                    case "2"://"gas carrier":
                        shipTypeCode = "gas carrier";
                        break;

                    case "3"://"tanker":
                        shipTypeCode = "tanker";
                        break;

                    case "4"://"container ship":
                        shipTypeCode = "container ship";
                        break;

                    case "5"://"general cargo ship":
                        shipTypeCode = "general cargo ship";
                        break;

                    case "6"://"refrigerated cargo carrier":
                        shipTypeCode = "refrigerated cargo carrier";
                        break;

                    case "7"://"combination carrier":
                        shipTypeCode = "combination carrier";
                        break;

                    case "8"://"LNG carrier":
                        shipTypeCode = "LNG carrier";
                        break;

                    case "9"://"ro-ro cargo ship(vehicle carrier)":
                        shipTypeCode = "ro-ro cargo ship(vehicle carrier)";
                        break;

                    case "10"://"ro-ro cargo ship":
                        shipTypeCode = "ro-ro cargo ship";
                        break;

                    case "11"://"ro-ro passenger ship":
                        shipTypeCode = "ro-ro passenger ship";
                        break;

                    case "12"://"high speed craft(SOLAS X)":
                        shipTypeCode = "high speed craft(SOLAS X)";
                        break;

                    case "13"://"cruise passenger ship":
                        shipTypeCode = "cruise passenger ship";
                        break;
                }
                var resultCIICoefficients = await _ciiCoefficient.GetListAsync(t => t.delete_time == null && t.ShipType == shipTypeCode);
                var resultCIIRatings = await _ciiRating.GetListAsync(t => t.delete_time == null && t.ShipType == shipTypeCode);

                #region 计算CII参考值

                foreach (var entity in resultCIICoefficients)
                {
                    if (entity.WeightCondition == "DWT")
                    {
                        if ((entity.ContainLow == 0 ? (dwt > (entity.LowValue ?? decimal.MinValue)) : (dwt >= (entity.LowValue ?? decimal.MinValue))) && (entity.ContainHigh == 0 ? (dwt < (entity.HighValue ?? decimal.MaxValue)) : (dwt <= (entity.HighValue ?? decimal.MaxValue))))
                        {
                            CIIRef = Convert.ToDouble(entity.Coefficient1) * Math.Pow(Convert.ToDouble(entity.WeightValue ?? dwt), Convert.ToDouble(entity.Coefficient2));
                        }
                    }
                    else if (entity.WeightCondition == "GT")
                    {
                        if ((entity.ContainLow == 0 ? (gt > (entity.LowValue ?? decimal.MinValue)) : (gt >= (entity.LowValue ?? decimal.MinValue))) && (entity.ContainHigh == 0 ? (gt < (entity.HighValue ?? decimal.MaxValue)) : (gt <= (entity.HighValue ?? decimal.MaxValue))))
                        {
                            CIIRef = Convert.ToDouble(entity.Coefficient1) * Math.Pow(Convert.ToDouble(entity.WeightValue ?? gt), Convert.ToDouble(entity.Coefficient2));
                        }
                    }
                }

                #endregion 计算CII参考值

                #region 计算年系数

                var yearCoeffienct = (100d - (5 + (selectedYear - 2023) * 2)) / 100d;

                #endregion 计算年系数

                #region 计算CII要求值

                RCII = CIIRef * yearCoeffienct;

                #endregion 计算CII要求值

                #region 计算CII实际值

                CO2Emission = await CalcCEmission(parameters);
                //原公式为var CIIValue = CO2Emission * 1000000d
                var CIIValue = CO2Emission * 1000d;
                switch (shipType)
                {
                    case "1":// "bulk carrier":
                    case "2"://"gas carrier":
                    case "3"://"tanker":
                    case "4"://"container ship":
                    case "5"://"general cargo ship":
                    case "6"://"refrigerated cargo carrier":
                    case "7"://"combination carrier":
                    case "8"://"LNG carrier":
                        ACII = CIIValue / dwt / distance;
                        break;

                    case "9"://"ro-ro cargo ship(vehicle carrier)":
                    case "10"://"ro-ro cargo ship":
                    case "11"://"ro-ro passenger ship":
                    case "12"://"high speed craft(SOLAS X)":
                    case "13"://"cruise passenger ship":
                        ACII = CIIValue / gt / distance;
                        break;
                }

                #endregion 计算CII实际值

                #region 获取CIIRank

                foreach (var entity in resultCIIRatings)
                {
                    if (entity.WeightCondition == "DWT")
                    {
                        if ((entity.ContainLow == 0 ? (dwt > (entity.LowValue ?? decimal.MinValue)) : (dwt >= (entity.LowValue ?? decimal.MinValue))) && (entity.ContainHigh == 0 ? (dwt < (entity.HighValue ?? decimal.MaxValue)) : (dwt <= (entity.HighValue ?? decimal.MaxValue))))
                        {
                            switch (entity.Rating)
                            {
                                case "A":
                                    D1 = (double)entity.RatingValue;
                                    break;

                                case "B":
                                    D2 = (double)entity.RatingValue;
                                    break;

                                case "C":
                                    D3 = (double)entity.RatingValue;
                                    break;

                                case "D":
                                    D4 = (double)entity.RatingValue;
                                    break;
                            }
                        }
                    }
                    else if (entity.WeightCondition == "GT")
                    {
                        if ((entity.ContainLow == 0 ? (gt > (entity.LowValue ?? decimal.MinValue)) : (gt >= (entity.LowValue ?? decimal.MinValue))) && (entity.ContainHigh == 0 ? (gt < (entity.HighValue ?? decimal.MaxValue)) : (gt <= (entity.HighValue ?? decimal.MaxValue))))
                        {
                            switch (entity.Rating)
                            {
                                case "A":
                                    D1 = (double)entity.RatingValue;
                                    break;

                                case "B":
                                    D2 = (double)entity.RatingValue;
                                    break;

                                case "C":
                                    D3 = (double)entity.RatingValue;
                                    break;

                                case "D":
                                    D4 = (double)entity.RatingValue;
                                    break;
                            }
                        }
                    }
                }
                var tempResult = ACII / RCII;
                if (tempResult < D1)
                {
                    CIIRating = "A";
                }
                else if (tempResult < D2)
                {
                    CIIRating = "B";
                }
                else if (tempResult < D3)
                {
                    CIIRating = "C";
                }
                else if (tempResult < D4)
                {
                    CIIRating = "D";
                }
                else
                {
                    CIIRating = "E";
                }

                #endregion 获取CIIRank

                var result = new
                {
                    CIIRef = Math.Round(CIIRef, 3),
                    RCII = Math.Round(RCII, 3),
                    CO2Emission = Math.Round(CO2Emission, 3),
                    ACII = Math.Round(ACII.CompareTo(double.NaN) == 0 ? 3656245 : ACII, 3),
                    D1 = D1,
                    D2 = D2,
                    D3 = D3,
                    D4 = D4,
                    CIIRating = CIIRating
                };
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "计算CII错误");
            }
            return null;
        }

        /// <summary>
        /// 计算EEOI
        /// </summary>
        /// <param name="parameters">计算参数,json格式</param>
        /// <returns></returns>
        public async Task<object> CalcEEOI(string parameters)
        {
            try
            {
                var queryParams = parameters.ToJObject();
                var cargoCarried = 0d;
                if (queryParams.ContainsKey("cargoCarried") && !string.IsNullOrWhiteSpace(queryParams["cargoCarried"].ToString()))
                {
                    cargoCarried = Convert.ToDouble(queryParams["cargoCarried"]);
                }
                var distance = 0d;
                if (queryParams.ContainsKey("distance") && !string.IsNullOrWhiteSpace(queryParams["distance"].ToString()))
                {
                    distance = Convert.ToDouble(queryParams["distance"]);
                }

                return (await CalcCEmission(parameters)) * 1000d / (cargoCarried * distance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "计算CO2Emission错误");
            }
            return double.NaN;
        }
    }
}