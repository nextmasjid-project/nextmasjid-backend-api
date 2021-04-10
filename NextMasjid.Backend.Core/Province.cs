using System;
namespace NextMasjid.Backend.Core
{
    public class Province
    {
        public int ProvinceID { get; set; }
        public string ProvinceNameAr { get; set; }
        public string ProvinceNameEn { get; set; }
        public City[] Cities { get; set; }
    }
}
