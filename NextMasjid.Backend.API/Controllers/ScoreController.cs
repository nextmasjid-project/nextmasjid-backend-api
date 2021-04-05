using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NextMasjid.Backend.API.Models;
using System.Linq;

namespace NextMasjid.Backend.API.Controllers
{
    [ApiController]
    [Route("score")]
    public class ScoreController : BaseController<ScoreController>
    {

        [HttpGet("byArea/{swLat}/{swLng}/{neLat}/{neLng}/{step}")]
        public IEnumerable<ScoreModel> Get(double swLat, double swLng, double neLat, double neLng, int step)
        {

            if (step < 1)
                step = 1;

            // dual step
            var subScores90 = Core.Score.SearchAreaFast(Scores, new Core.GeoPoint(swLat, swLng), new Core.GeoPoint(neLat, neLng), 2 * step).Where(s => s.Value >= 85);
            var subScores80 = Core.Score.SearchAreaFast(Scores, new Core.GeoPoint(swLat, swLng), new Core.GeoPoint(neLat, neLng), 4 * step).Where(s => s.Value >= 70 && s.Value < 85);
            //var subScores70 = Core.Score.SearchAreaFast(Scores, new Core.GeoPoint(swLat, swLng), new Core.GeoPoint(neLat, neLng), 32).Where(s => s.Value >= 70 && s.Value < 80);
            var subScoresElse = Core.Score.SearchAreaFast(Scores, new Core.GeoPoint(swLat, swLng), new Core.GeoPoint(neLat, neLng), 32 * step).Where(s => s.Value <70 );

            var result90 = subScores90.Select(s => new ScoreModel() { Lat = ((double)s.Key.Item1 / 10000), Lng = ((double)s.Key.Item2 / 10000), Value = s.Value });
            var result80 = subScores80.Select(s => new ScoreModel() { Lat = ((double)s.Key.Item1 / 10000), Lng = ((double)s.Key.Item2 / 10000), Value = s.Value });
            //var result70 = subScores70.Select(s => new ScoreModel() { Lat = ((double)s.Key.Item1 / 10000), Lng = ((double)s.Key.Item2 / 10000), Value = s.Value });
            var resultsElse = subScoresElse.Select(s => new ScoreModel() { Lat = ((double)s.Key.Item1 / 10000), Lng = ((double)s.Key.Item2 / 10000), Value = s.Value });

            var result = new List<ScoreModel>();
            result.AddRange(result90);
            result.AddRange(result80);
            //result.AddRange(result70);
            result.AddRange(resultsElse);

            return result;

        }

        //[HttpGet("byArea/{swLat}/{swLng}/{neLat}/{neLng}/")]
        //public IEnumerable<ScoreModel> Get(double swLat, double swLng, double neLat, double neLng)
        //{

        //    var subScores = Core.Score.SearchAreaFast(Scores, new Core.GeoPoint(swLat, swLng), new Core.GeoPoint(neLat, neLng), 2).Where(s => s.Value >= 50);
            
        //    return subScores.Select(s => new ScoreModel() { Lat = ((double)s.Key.Item1 / 10000), Lng = ((double)s.Key.Item2 / 10000), Value = s.Value }); ;

        //}

        [HttpGet("byPoint/{lat}/{lng}")]
        public ScoreModel Get(double lat, double lng)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            int value = Backend.Core.Score.SearchPoint(Scores, new Core.GeoPoint(lat, lng));
            stopwatch.Stop();
            return new ScoreModel() { Lat = lat, Lng = lng, Value = value };
        }


        [HttpGet("byPointDetails/{lang}/{lat}/{lng}")]
        public ScoreDetailedModel Get(string lang, double lat, double lng)
        {
            int value = Backend.Core.Score.SearchPoint(Scores, new Core.GeoPoint(lat, lng));
            return new ScoreDetailedModel() { Lat = lat, Lng = lng, Value = value, ExpectedPrayers = 1, MosqueDensity = 1, PopulationDensity = 1, NearestMosqueDistance = 100 };
        }
    }
}
