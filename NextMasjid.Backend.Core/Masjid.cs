using System;
namespace NextMasjid.Backend.Core
{
    public class Masjid
    {
        public string MasjidID { get; set; }
        public string Name { get; set; }
        public GeoPoint Location { get; set; }
        public int CityID { get; set; }
        public bool IsGrandMosque { get; set; }
    }
}
