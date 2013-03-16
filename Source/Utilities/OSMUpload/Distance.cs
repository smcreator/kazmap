using System;

namespace OSMUpload
{
    static class Distance
    {
        /// <summary>  
        /// Returns the distance in kilometers of any two latitude / longitude points.  
        /// </summary>  
        static public double Calc(double pos1X, double pos1Y, double pos2X, double pos2Y)
        {
            const double R = 6371;
            var dLat = ToRadian(pos2Y - pos1Y);
            var dLon = ToRadian(pos2X - pos1X);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadian(pos1Y)) * Math.Cos(ToRadian(pos2Y)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            var d = R * c;
            return d;
        }

        //public static double Calc(double lat1, double lon1, double lat2, double lon2)
        //{
        //    double k = Math.PI / 180;
        //    lat1 *= k;
        //    lon1 *= k;
        //    lat2 *= k;
        //    lon2 *= k;
        //    return Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) +
        //        Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon1 - lon2)) * 6371;
        //}

        static double ToRadian(double val)
        {
            return (Math.PI / 180) * val;
        }
    }
}