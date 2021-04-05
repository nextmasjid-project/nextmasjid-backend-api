using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NextMasjid.Backend.Core.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

namespace NextMasjid.Backend.API.Controllers
{

    public abstract class BaseController<T> : Controller where T : BaseController<T>
    {
        private ILogger<T> _logger; 

        private IMemoryCache _memoryCache;

        private IConfiguration _configuration;

        private IWebHostEnvironment _webHostEnv;

        protected ILogger<T> Logger => _logger ??= HttpContext.RequestServices.GetService<ILogger<T>>();
        protected IMemoryCache Memory => _memoryCache ??= HttpContext.RequestServices.GetService<IMemoryCache>();
        protected IConfiguration Configuration => _configuration ??= HttpContext.RequestServices.GetService<IConfiguration>();
        protected IWebHostEnvironment WebHostEnv => _webHostEnv ??= HttpContext.RequestServices.GetService<IWebHostEnvironment>();

        public BaseController()
        {
        }

        const string masjidKey = "allMasjids";
        const string reportedMasjidKey = "allReportedMasjids";
        const string provinceKey = "allProvinces";
        const string scoreKey = "allScores";
        const string choiceKey = "allChoices";


        Core.Masjid[] masjids;
        //Core. reportedMasjids = new List<Core.Masjid>();
        Core.Province[] provinces;
        Core.EditorChoice[] choices;
        Dictionary<(int, int), int> scores;

        public Core.Masjid[] Masjids
        {
            get {
                if (Memory.TryGetValue(masjidKey, out masjids))
                    return masjids;

                var path = Configuration["masjidsPath"];

                masjids = DataReaderWriter.ReadMasjids(path);
                Memory.Set(masjidKey, masjids);
                return masjids;
            }
        }

        //protected void ReloadReportedMasjids()
        //{
        //    Memory.Set<Core.Masjid>(reportedMasjidKey, null);
        //}

        //protected void AddToReported(Core.Masjid masjid)
        //{
        //    reportedMasjids.Add(masjid);
        //}

        public Core.Masjid[] ReportedMasjids
        {
            get
            {
                var path = Configuration["reportedMasjidsPath"];
                return DataReaderWriter.ReadMasjids(path);

                //Memory.Set(reportedMasjidKey, reportedMasjids);
                //return reportedMasjids.ToArray();
            }
            
        }


        public Core.Province[] Provinces
        {
            get {
                if (Memory.TryGetValue(provinceKey, out provinces))
                    return provinces;

                var path = Configuration["provincesPath"];
                provinces = DataReaderWriter.ReadProvincesAndCities(path);
                Memory.Set(provinceKey, provinces);
                return provinces;
            }
        }

        public Dictionary<(int, int),int> Scores
        {
            get {
                if (Memory.TryGetValue(scoreKey, out scores))
                    return scores;
                var path = Configuration["scoresPath"];
                scores = DataReaderWriter.ReadScores(path);
                Memory.Set(scoreKey, scores);
                return scores;
            }
        }

        public Core.EditorChoice[] Choices
        {
            get
            {
                if (Memory.TryGetValue(provinceKey, out choices))
                    return choices;

                var path = Configuration["choicesPath"];
                choices = DataReaderWriter.ReadEditorChoices(path);
                Memory.Set(choiceKey, choices);
                return choices;
            }
        }

        private string getFullPath(string filePath)
        {
            return System.IO.Path.Combine(WebHostEnv.ContentRootPath, filePath);
        }


    }
}
