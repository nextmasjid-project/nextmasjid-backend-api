using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NextMasjid.Backend.API.Models;

namespace NextMasjid.Backend.API.Controllers
{
    [ApiController]
    [Route("province")]
    public class ProvinceController : BaseController<ProvinceController>
    {
   
        [HttpGet("{lang}")]
        public IEnumerable<ProvinceModel> Get(string lang = "ar")
        {
            List<ProvinceModel> provs = new List<ProvinceModel>();
            foreach (var prov in Provinces)
            {
                ProvinceModel pvModel = new ProvinceModel();
                pvModel.ID = prov.ProvinceID.ToString();
                pvModel.Name = lang == "ar" ? prov.ProvinceNameAr : prov.ProvinceNameEn;
                
                List<CityModel> cs = new List<CityModel>();

                foreach(var city in prov.Cities)
                {
                    cs.Add(new CityModel() { ID = city.CityID.ToString(), Name = lang == "ar"? city.CityNameAr : city.CityNameEn, Lat = city.Center.Lat, Lng = city.Center.Lng });
                }

                pvModel.Cities = cs.ToArray();

                provs.Add(pvModel);
            }
            return provs;
        }
    }
}
