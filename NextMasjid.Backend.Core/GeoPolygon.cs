using System;
using System.Linq;

namespace NextMasjid.Backend.Core
{
    public class GeoPolygon
    {

        public GeoPolygon()
        {
            IsNegativePolygon = false;
        }

        public bool IsNegativePolygon { get; set; }


        public GeoPoint[] Points { get; set; }
        public GeoPoint BottomLeft { get { return new GeoPoint() { Lat = Points.Min(p => p.Lat), Lng = Points.Min(p => p.Lng) }; } }
        public GeoPoint TopRight { get { return new GeoPoint() { Lat = Points.Max(p => p.Lat), Lng = Points.Max(p => p.Lng) }; } }

        /// <summary>
        /// Checks of a certain point is contained in this polygon
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        public bool Contains(double lat, double lng)
        {
            var j = Points.Length - 1;
            var inside = false;
            for (int i = 0; i < Points.Length; j = i++)
            {
                var pi = Points[i];
                var pj = Points[j];
                if (((pi.Lng <= lng && lng < pj.Lng) || (pj.Lng <= lng && lng < pi.Lng)) &&
                    (lat < (pj.Lat - pi.Lat) * (lng - pi.Lng) / (pj.Lng - pi.Lng) + pi.Lat))
                    inside = !inside;
            }
            return inside;
        }

    }
}
