using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NextMasjid.Backend.API.Models;

namespace NextMasjid.Backend.API.Controllers
{
    [ApiController]
    [Route("masjid")]
    public class MasjidController : BaseController<MasjidController>
    {
     
        [HttpGet("{lang}/{swLat}/{swLng}/{neLat}/{neLng}")]
        public IEnumerable<MasjidModel> Get( double swLat, double swLng, double neLat, double neLng)
        {
            if (swLat == 0 || swLng == 0 || neLat == 0 || neLng == 0)
                return new MasjidModel[0];

            // todo check for max?

            var masjids = Masjids.Where(
                mm =>
                mm.Location.Lat > swLat &&
                mm.Location.Lat < neLat &&
                mm.Location.Lng > swLng &&
                mm.Location.Lng < neLng);

            // convert it to MasjidModel and return it
            return masjids.Select(m => new MasjidModel() { Lat = m.Location.Lat, Lng = m.Location.Lng, Name = m.Name });

        }

        [HttpPost("report-missing")]
        public IActionResult ReportMissingMasjid([FromForm] MasjidModel[] masjids)
        {
            var newMasjids = masjids.Select(m => new Core.Masjid() { Name = m.Name, Location = new Core.GeoPoint(m.Lat, m.Lng), CityID = -1, MasjidID = Guid.NewGuid().ToString() });
            Core.Data.DataReaderWriter.AppendMasjids(Configuration["reportedMasjidsPath"], newMasjids);
            return CreatedAtAction("GetReportedMissingMasjids", new { });
        }

        [HttpGet("reported")]
        public IEnumerable<MasjidModel> GetReportedMissingMasjids()
        {
            return ReportedMasjids.Select(m => new MasjidModel() { Lat = m.Location.Lat, Lng = m.Location.Lng, Name = m.Name });
        }

    }
}
