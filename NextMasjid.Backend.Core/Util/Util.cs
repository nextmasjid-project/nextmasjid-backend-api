using System;
using System.Collections.Generic;
using System.Linq;
using NextMasjid.Backend.Core;

namespace NextMasjid.Backend.Core.Util
{
  

    public static class UtilMath
    {
        const double EARTH_RADIUS = 6371009;

        public static GeoPoint[] AllPointsInMeters(GeoPoint[] points)
        {
            List<GeoPoint> pointsInMeters = new List<GeoPoint>();

            foreach (var point in points)
            {
                pointsInMeters.Add(UtilMath.ConvertToMetersPoint(point));

            }
            return pointsInMeters.ToArray();
        }

        public static GeoPoint ConvertToMetersPoint(GeoPoint geoPoint)
        {
            double math = 0.918653362575817; // math.cos 27.2 (Cancer)
            var p = new GeoPoint() { Lat = geoPoint.Lat * 111111, Lng = geoPoint.Lng * 40075000 * math / 360 };
            return p;

        }
        public static (double, double) ConvertToMetersLatLng(double lt, double ln)
        {
            double math = 0.918653362575817; // math.cos 27.2 (Cancer)
            return (lt * 111111, ln * 40075000 * math / 360);
        }



        static double ToRadians(double input)
        {
            return input / 180.0 * Math.PI;
        }

        public static double ComputeSignedArea(GeoPoint[] path)
        {
            return Math.Abs(ComputeSignedArea(path, EARTH_RADIUS));
        }

        static double ComputeSignedArea(GeoPoint[] path, double radius)
        {
            int size = path.Length;
            if (size < 3) { return 0; }
            double total = 0;
            var prev = path[size - 1];
            double prevTanLat = Math.Tan((Math.PI / 2 - ToRadians(prev.Lat)) / 2);
            double prevLng = ToRadians(prev.Lng);

            foreach (var point in path)
            {
                double tanLat = Math.Tan((Math.PI / 2 - ToRadians(point.Lat)) / 2);
                double lng = ToRadians(point.Lng);
                total += PolarTriangleArea(tanLat, lng, prevTanLat, prevLng);
                prevTanLat = tanLat;
                prevLng = lng;
            }
            return total * (radius * radius);
        }

        static double PolarTriangleArea(double tan1, double lng1, double tan2, double lng2)
        {
            double deltaLng = lng1 - lng2;
            double t = tan1 * tan2;
            return 2 * Math.Atan2(t * Math.Sin(deltaLng), 1 + t * Math.Cos(deltaLng));
        }

        public static double CalculateDistanceFast(double lt1, double ln1, double lt2, double ln2)
        {
            var x = lt2 - lt1;
            var y = ln2 - ln1;
            return (x * x) + (y * y);
        }

        const double rad = (Math.PI / 180.0);
        public static double CalculateDistanceAccurate(double lt1, double ln1, double lt2, double ln2)
        {
            var d1 = lt1 * rad;
            var num1 = ln1 * rad;
            var d2 = lt2 * rad;
            var num2 = ln2 * rad - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            var r = 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
            return r;
        }

    }
}
