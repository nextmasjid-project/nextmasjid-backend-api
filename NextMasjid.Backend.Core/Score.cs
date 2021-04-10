using System;
using System.Collections.Generic;
using System.Linq;
using NextMasjid.Backend.Core;
using NextMasjid.Backend.Core.Util;

namespace NextMasjid.Backend.Core
{
    public static class Score
    {

        private const double minDistance = 200;
        private const double maxDistance = 800;

        public static Dictionary<(int, int), int> SearchArea(Dictionary<(int, int), int> data, GeoPoint bottomLeft, GeoPoint topRight)
        {

            int swLat = (int)(bottomLeft.Lat * 10000);
            int swLng = (int)(bottomLeft.Lng * 10000);
            int neLat = (int)(topRight.Lat * 10000);
            int neLng = (int)(topRight.Lng * 10000);


            return data.Where(x =>
                          x.Key.Item1 > swLat &&
                          x.Key.Item1 < neLat &&
                          x.Key.Item2 > swLng &&
                          x.Key.Item2 < neLng).ToDictionary(p => p.Key, p => p.Value);
        }



        public static Dictionary<(int, int), int> SearchAreaHeatmap(Dictionary<(int, int), int> data, GeoPoint bottomLeft, GeoPoint topRight, int step = 2)
        {
            int swLat = (int)(bottomLeft.Lat * 10000);
            int swLng = (int)(bottomLeft.Lng * 10000);
            int neLat = (int)(topRight.Lat * 10000);
            int neLng = (int)(topRight.Lng * 10000);


            if (swLat % 2 == 1)
                swLat += 1;
            if (swLng % 2 == 1)
                swLng += 1;
            if (neLat % 2 == 1)
                neLat += 1;
            if (neLng % 2 == 1)
                neLng += 1;

            // if point is > 90, step is the same
            // for ever 10, add +2


            Dictionary<(int, int), int> result = new Dictionary<(int, int), int>();

            for (int lt = swLat; lt < neLat; lt += step)
            {
                for (int ln = swLng; ln < neLng; ln += step)
                {
                    var tempstep = 0;
                    if (data.TryGetValue((lt, ln), out int value))
                    {
                        result.TryAdd((lt, ln), value);
                        tempstep = step * (100 - value) / 10;
                        if (tempstep % 2 == 1)
                            tempstep++;
                     //   lt += tempstep;
                        ln += tempstep;
                    }
                }
            }
            return result;


        }

        public static Dictionary<(int, int), int> SearchAreaFast(Dictionary<(int, int), int> data, GeoPoint bottomLeft, GeoPoint topRight, int step = 2)
        {
            int swLat = (int)(bottomLeft.Lat * 10000);
            int swLng = (int)(bottomLeft.Lng * 10000);
            int neLat = (int)(topRight.Lat * 10000);
            int neLng = (int)(topRight.Lng * 10000);


            if (swLat % 2 == 1)
                swLat += 1;
            if (swLng % 2 == 1)
                swLng += 1;
            if (neLat % 2 == 1)
                neLat += 1;
            if (neLng % 2 == 1)
                neLng += 1;
                

            Dictionary<(int, int), int> result = new Dictionary<(int, int), int>();

            for (int lt = swLat; lt<neLat; lt += step )
            {
                for(int ln = swLng; ln < neLng; ln += step)
                {
                    if (data.TryGetValue((lt, ln), out int value))
                        result.TryAdd((lt, ln), value);
                }
            }
            return result;

       
        }

        public static int SearchPoint(Dictionary<(int, int), int> data, GeoPoint point)
        {
            var lat = (int)(point.Lat * 10000);
            var lng = (int)(point.Lng * 10000); 

            int count = 0;
            int total = 0;

            for(int dlat = lat - 3; dlat < lat + 3; dlat += 1)
            {
                for (int dlng = lng - 3; dlng < lng + 3; dlng += 1)
                {
                    if (data.TryGetValue((dlat, dlng),out int val))
                    {
                        total += val;
                        count += 1;
                    }
                }
            }

            if (count == 0)
                return -1;

            return total / count;

            
        }


        public static Dictionary<(int, int), int> ComputeEntirePolygons(City[] cities, Masjid[] masjids, double step = 0.0002)
        {

            System.Diagnostics.Stopwatch polygonStopwatch = new System.Diagnostics.Stopwatch();

            System.Collections.Concurrent.ConcurrentDictionary<(int, int), int> result = new System.Collections.Concurrent.ConcurrentDictionary<(int, int), int>();

            int polygonCompleted = 0;

            // foreach polygon in different thread
            System.Threading.Tasks.Parallel.ForEach(cities.OrderByDescending(cx => cx.Area), c =>
            {
                var polygon = c.Polygons.Where(p => p.IsNegativePolygon == false).FirstOrDefault();
                if (polygon == null)
                {
                    Console.WriteLine(c.CityID + "has no active polygon!");
                    return;
                }

                // get the masjids in this polygon [so we only search in short list instead of all 80000]
                var existingMosques = masjids.Where(m => m.CityID == c.CityID).ToArray();

                //existingMosques =  existingMosques.Where(em => polygon.Contains(em.Location.Lat, em.Location.Lng)).ToArray();
                // todo we can also remove the mosques outside of polygon. but maybe the data is correct already

                // just in case there is a polygon without mosques
                if (existingMosques.Length == 0)
                    existingMosques = new Masjid[] { new Masjid() { CityID = -1, Name = "non", Location = new GeoPoint() { Lat = 1, Lng = 1 } } };

                // get them as points
                var mosquesAsPoints = existingMosques.Select(m => m.Location).ToArray();

                // convert to meteres
                mosquesAsPoints = UtilMath.AllPointsInMeters(mosquesAsPoints);

                // get density rank for the city [since it's the same for each polygon]
                int densityRank = 5;// c.RankInDensity; todo


                // get population rank for the city [since it's the same for each polygon]
                int populationRank = c.RankInPopulation;

                for (double lt = polygon.BottomLeft.Lat; lt < polygon.TopRight.Lat; lt += step)
                {
                    for (double ln = polygon.BottomLeft.Lng; ln < polygon.TopRight.Lng; ln += step)
                    {
                        // if  exsist in polygon, do
                        if (polygon.Contains(lt, ln))
                        {
                            (double, double) xl = UtilMath.ConvertToMetersLatLng(lt, ln);
                            int value = CalculateDistanceScoreeFast(mosquesAsPoints, xl.Item1, xl.Item2);
                            if (value > 0)
                            {
                                value = value + densityRank + populationRank + 3; // todo fixed distance to border

                                int _lt = (int)(lt * 10000);
                                int _ln = (int)(ln * 10000);

                                if (_lt % 2 == 1)
                                    _lt += 1;
                                if (_ln % 2 == 1)
                                    _ln += 1;

                                result.TryAdd((_lt, _ln), value);
                               // Console.WriteLine(_lt + "," + _ln);
                            }
                            // else: if value is zero then no need to calculate it
                        }
                        else
                        {
                            ln += 0.001; // faster skip
                        }
                    }
                }

                polygonStopwatch.Stop();
                Console.WriteLine("City: " + c.CityNameEn + " with: " + existingMosques.Length + " in: " + polygonStopwatch.ElapsedMilliseconds + ", poly # : " + ++polygonCompleted);
                polygonStopwatch.Restart();
            });

            return result.ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        static int CalculateDistanceScoreeFast(GeoPoint[] points, double lt, double ln)
        {

            var first = points.First();
            double nearestMosqueDistance = 100000000;
            double d;
            GeoPoint nearestMosque = first;

            for (int i = 0; i < points.Length; i++)
            {
                d = UtilMath.CalculateDistanceFast(lt, ln, points[i].Lat, points[i].Lng);
                if (d < nearestMosqueDistance)
                {
                    nearestMosqueDistance = d;
                    nearestMosque = points[i];
                }
            }

            nearestMosqueDistance = Math.Sqrt(UtilMath.CalculateDistanceFast(lt, ln, nearestMosque.Lat, nearestMosque.Lng));

            if (nearestMosqueDistance < minDistance)
                return 0;

            if (nearestMosqueDistance > maxDistance)
                return 85;

            return (int)((nearestMosqueDistance / maxDistance) * 100d * 0.85d);

        }
    }
}
