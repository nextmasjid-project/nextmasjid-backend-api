using System;
using System.Linq;

namespace NextMasjid.Backend.Core
{
    public class City
    {

        public int CityID { get; set; }
        public string CityNameEn { get; set; }
        public string CityNameAr { get; set; }
        public GeoPolygon[] Polygons { get; set; }
        public double Area { get; set; }
        public double Population { get; set; }
        public double Density { get; set; }
        public int RankInDensity { get; set; }
        public int RankInPopulation { get; set; }

        public Province Province { get; set; }

        public GeoPolygon[] NegativeSpaces
        {
            get
            {
                if (Polygons == null)
                    return new GeoPolygon[0];
                return Polygons.Where(p => p.IsNegativePolygon).ToArray();
            }
        }

        public GeoPoint Center
        {
            get
            {
                if (Polygons == null || Polygons.Length == 0 || Polygons[0].Points == null || Polygons[0].Points.Length == 0)
                    return null;
                return new GeoPoint() { Lat = Polygons[0].Points.Average(p => p.Lat), Lng = Polygons[0].Points.Average(p => p.Lng) };
            }
        }
    }

}
