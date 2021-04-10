using System;
namespace NextMasjid.Backend.Core
{
    public class GeoPoint
    {
        public GeoPoint()
        {

        }
        public GeoPoint(double lat, double lng)
        {
            Lat = lat;
            Lng = lng;
        }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

}
