namespace hmt_energy_csharp
{
    public static class GPSHelper
    {
        /// <summary>
        /// 经纬度小数转度分秒
        /// </summary>
        /// <param name="strLatLongValue">格式如:E 123.4234</param>
        /// <returns></returns>
        public static string TLatLong(string strLatLongValue)
        {
            string result = "";
            try
            {
                string[] tempStrs = strLatLongValue.Split(' ');
                double result_degree = Math.Floor(Convert.ToDouble(tempStrs[1]) / 100);
                double result_minute = Math.Floor(Convert.ToDouble(tempStrs[1])) - result_degree * 100;
                double result_second = Math.Round((Convert.ToDouble(tempStrs[1]) - Math.Floor(Convert.ToDouble(tempStrs[1]))) * 60, 4);
                result = tempStrs[0] + result_degree + "°" + result_minute + "′" + result_second + "″";
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "经纬度转换失败");
            }
            return result;
        }

        private static readonly double pi = Math.PI;
        private static readonly double bdPi = Math.PI * 3000.0 / 180.0;
        private static readonly double earthR = 6378245.0; //地球半长轴
        private static readonly double ee = 0.00669342162296594323; //地球扁率

        public static double distanceHav_km(pointLatLon p0, pointLatLon p1)//计算两gps点的球面距离
        {
            if (p1.Type != p0.Type)
            {
                //不同坐标系下的点无法计算距离
                return -1;
            }

            //经纬度转换成弧度
            double lat0 = radians(p0.Lat);
            double lat1 = radians(p1.Lat);
            double lon0 = radians(p0.Lon);
            double lon1 = radians(p1.Lon);

            double dlon = Math.Abs(lon0 - lon1);
            double dlat = Math.Abs(lat0 - lat1);

            double h = Math.Sin(0.5 * dlat) * Math.Sin(0.5 * dlat) + Math.Cos(lat0) * Math.Cos(lat1) * Math.Sin(0.5 * dlon) * Math.Sin(0.5 * dlon);
            double distance = 2 * earthR * Math.Asin(Math.Sqrt(h)) / 1000;

            return distance;
        }

        public static pointLatLon wgs84Tobd09(pointLatLon gpsPoint)
        {
            pointLatLon gcj02 = wgs84Togcj02(gpsPoint);
            pointLatLon bd09 = gcj02Tobd09(gcj02);

            return bd09;
        }

        public static pointLatLon bd09Towgs84(pointLatLon bdPoint)
        {
            pointLatLon gcj02 = bd09Togcj02(bdPoint);
            pointLatLon map84 = gcj02Towgs84(gcj02);

            return map84;
        }

        public static pointLatLon bd09Togcj02(pointLatLon bdPoint)
        {
            double x = bdPoint.Lon - 0.0065, y = bdPoint.Lat - 0.006;
            double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * bdPi);
            double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * bdPi);
            double gg_lon = z * Math.Cos(theta);
            double gg_lat = z * Math.Sin(theta);

            return new pointLatLon(gg_lat, gg_lon, gpsType.gcj02);
        }

        public static pointLatLon gcj02Tobd09(pointLatLon Gpoint)
        {
            double x = Gpoint.Lon, y = Gpoint.Lat;
            double z = Math.Sqrt(x * x + y * y) + 0.00002 * Math.Sin(y * bdPi);
            double theta = Math.Atan2(y, x) + 0.000003 * Math.Cos(x * bdPi);
            double bd_lon = z * Math.Cos(theta) + 0.0065;
            double bd_lat = z * Math.Sin(theta) + 0.006;

            return new pointLatLon(bd_lat, bd_lon, gpsType.bd09);
        }

        public static pointLatLon gcj02Towgs84(pointLatLon Gpoint)
        {
            pointLatLon gps = transform(Gpoint);
            double lontitude = Gpoint.Lon * 2 - gps.Lon;
            double latitude = Gpoint.Lat * 2 - gps.Lat;

            return new pointLatLon(latitude, lontitude, gpsType.wgs84);
        }

        public static pointLatLon wgs84Togcj02(pointLatLon Gpoint)
        {
            //if (outOfChina(Gpoint.Lat, Gpoint.Lon))
            //{
            //    return new pointLatLon(0, 0, gpsType.gcj02);
            //}
            double dLat = transformLat(Gpoint.Lon - 105.0, Gpoint.Lat - 35.0);
            double dLon = transformLon(Gpoint.Lon - 105.0, Gpoint.Lat - 35.0);
            double radLat = Gpoint.Lat / 180.0 * pi;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((earthR * (1 - ee)) / (magic * sqrtMagic) * pi);
            dLon = (dLon * 180.0) / (earthR / sqrtMagic * Math.Cos(radLat) * pi);
            double mgLat = Gpoint.Lat + dLat;
            double mgLon = Gpoint.Lon + dLon;

            return new pointLatLon(mgLat, mgLon, gpsType.gcj02);
        }

        private static pointLatLon transform(pointLatLon Gpoint)
        {
            if (outOfChina(Gpoint.Lat, Gpoint.Lon))
            {
                return new pointLatLon(Gpoint.Lat, Gpoint.Lon);
            }
            double dLat = transformLat(Gpoint.Lon - 105.0, Gpoint.Lat - 35.0);
            double dLon = transformLon(Gpoint.Lon - 105.0, Gpoint.Lat - 35.0);
            double radLat = Gpoint.Lat / 180.0 * pi;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((earthR * (1 - ee)) / (magic * sqrtMagic) * pi);
            dLon = (dLon * 180.0) / (earthR / sqrtMagic * Math.Cos(radLat) * pi);
            double mgLat = Gpoint.Lat + dLat;
            double mgLon = Gpoint.Lon + dLon;
            return new pointLatLon(mgLat, mgLon);
        }

        private static double transformLat(double x, double y)
        {
            double ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y
                    + 0.2 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(y * pi) + 40.0 * Math.Sin(y / 3.0 * pi)) * 2.0 / 3.0;
            ret += (160.0 * Math.Sin(y / 12.0 * pi) + 320 * Math.Sin(y * pi / 30.0)) * 2.0 / 3.0;
            return ret;
        }

        private static double transformLon(double x, double y)
        {
            double ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1
                    * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(x * pi) + 40.0 * Math.Sin(x / 3.0 * pi)) * 2.0 / 3.0;
            ret += (150.0 * Math.Sin(x / 12.0 * pi) + 300.0 * Math.Sin(x / 30.0
                    * pi)) * 2.0 / 3.0;
            return ret;
        }

        private static Boolean outOfChina(double lat, double lon)
        {
            if (lon < 72.004 || lon > 137.8347)
                return true;
            if (lat < 0.8293 || lat > 55.8271)
                return true;
            return false;
        }

        private static double radians(double degree)
        {
            return pi * degree / 180;
        }

        public static double Get84(double d, string type)
        {
            double resul = 0d;
            double temp = Math.Abs(d);
            temp = Math.Floor(temp / 100d) + (temp / 100d - Math.Floor(temp / 100)) * 100d / 60d;
            resul = (d > 0 ? 1 : -1) * temp;
            return resul;
        }

        public static pointLatLon GetBdFrom84(pointLatLon gpsPoint)
        {
            gpsPoint.Lat = Get84(gpsPoint.Lat, "lat");
            gpsPoint.Lon = Get84(gpsPoint.Lon, "lon");
            /*var temp = Gps84_To_bd09(new PointLatLng(Math.Abs(gpsPoint.Lat), Math.Abs(gpsPoint.Lng)));
            gpsPoint.Lat = (gpsPoint.Lat > 0 ? 1 : -1) * temp.Lat;
            gpsPoint.Lng = (gpsPoint.Lng > 0 ? 1 : -1) * temp.Lng;*/
            return gpsPoint.toBd09();
        }

        public static pointLatLon GetWGS84(pointLatLon gpsPoint)
        {
            gpsPoint.Lat = Get84(gpsPoint.Lat, "lat");
            gpsPoint.Lon = Get84(gpsPoint.Lon, "lon");
            return gpsPoint;
        }
    }

    /*public class PointLatLng
    {
        public PointLatLng(double lat, double lng)
        {
            Lat = lat;
            Lng = lng;
        }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }*/

    public enum gpsType
    {
        wgs84 = 84,
        gcj02 = 2,
        bd09 = 9,
        unknown = 0
    }

    public class pointLatLon
    {
        public pointLatLon(double lat, double lon, gpsType type = gpsType.wgs84)
        {
            Lat = lat;
            Lon = lon;
            Type = type;
        }

        public string gpsStr()
        {
            return "Lat,Lon:" + Lat.ToString() + ", " + Lon.ToString() + ", gpsType:" + Type.ToString();
        }

        public pointLatLon toGcj02()
        {
            if (Type == gpsType.bd09)
                return GPSHelper.bd09Togcj02(this);
            if (Type == gpsType.gcj02)
                return this;
            if (Type == gpsType.wgs84)
                return GPSHelper.wgs84Togcj02(this);

            return new pointLatLon(0, 0, gpsType.gcj02);
        }

        public pointLatLon toWgs84()
        {
            if (Type == gpsType.bd09)
                return GPSHelper.bd09Towgs84(this);
            if (Type == gpsType.gcj02)
                return GPSHelper.gcj02Towgs84(this);
            if (Type == gpsType.wgs84)
                return this;

            return new pointLatLon(0, 0, gpsType.wgs84);
        }

        public pointLatLon toBd09()
        {
            if (Type == gpsType.bd09)
                return GPSHelper.wgs84Tobd09(this);
            if (Type == gpsType.gcj02)
                return GPSHelper.gcj02Tobd09(this);
            if (Type == gpsType.wgs84)
                return this;

            return new pointLatLon(0, 0, gpsType.bd09);
        }

        public double Lat { get; set; }
        public double Lon { get; set; }
        public gpsType Type { get; set; }
    }
}