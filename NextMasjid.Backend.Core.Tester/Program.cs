using System;
using System.Linq;
using NextMasjid.Backend.Core.Data;

namespace NextMasjid.Backend.Core.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Hello World - V5");
            testCSVAppend();
            return;

           // Core.Data.DataReaderWriter.WriteMasjids("/Users/waleed/Projects/NextMasjid.Backend.API/NextMasjid.Backend.API/Data/reportedMasjids_v4.csv", new Masjid[0]);

            
            //DataReaderWriter.WriteEditorChoices(new EditorChoice[] {
            //    new EditorChoice(){ EditorChoiceID = 1, Notes = "None1", Location = new GeoPoint(24.77920027112631, 46.607062293959736) },
            //    new EditorChoice(){ EditorChoiceID = 2, Notes = "None2", Location = new GeoPoint(24.76404230498755, 46.641737891746985) },
            //    new EditorChoice(){  EditorChoiceID = 3, Notes = "None3", Location = new GeoPoint(21.520461277029252, 39.21790121410377)}
            //}, "/Users/waleed/Desktop/choices_1.csv");




            var masjids = DataReaderWriter.ReadMasjids("/Users/waleed/Desktop/Masjids4.csv");
            var provinces = DataReaderWriter.ReadProvincesAndCities("/Users/waleed/Desktop/citiesdb-final3.csv");
            Console.WriteLine("Start Calculations");
            DataReaderWriter.WriteScores("/Users/waleed/Desktop/scores-all5.dat", Score.ComputeEntirePolygons(provinces.SelectMany(c => c.Cities).ToArray(), masjids));


            var scores = DataReaderWriter.ReadScores("/Users/waleed/Desktop/scores-all5.dat");
            Console.WriteLine("File has been read");

            // Console.WriteLine("XXX333");
            //return;

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            stopwatch.Start();


            var result = Score.SearchAreaFast(scores, new GeoPoint(24.771842316853565, 46.61494572619337), new GeoPoint(24.79837561510408, 46.65829022373457), 4);


            stopwatch.Stop();

            Console.WriteLine(result.Count);

            Console.WriteLine("Time is:");
            Console.WriteLine(stopwatch.ElapsedMilliseconds);


            // read file

          //  var provinces = Backend.Core.Data.DataReaderWriter.ReadProvincesAndCities("/Users/waleed/Desktop/citiesdb-final.csv");
            //foreach(var prov in provinces)
            //{
            //    Console.WriteLine(prov.ProvinceNameAr + "," + prov.ProvinceNameEn + "," + prov.ProvinceID);
            //    foreach (var city in prov.Cities)
            //    {
            //        Console.WriteLine("\t" + city.CityNameAr);
            //        var pol = city.Polygons[0];
            //        city.Polygons = new Models.GeoPolygon[] { pol, pol };
            //    }
            //}

         //   Data.DataReaderWriter.WriteProvincesAndCites("/Users/waleed/Desktop/citiesdb-final2.csv", provinces);

            //var provinces2 = Backend.Core.Data.DataReaderWriter.ReadProvincesAndCities("/Users/waleed/Desktop/citiesdb-final2.csv");

            //foreach(var city in provinces2.SelectMany(p=>p.Cities))
            //{
            //    city.Polygons = new Models.GeoPolygon[] { city.Polygons[0] };
            //}

            //Data.DataReaderWriter.WriteProvincesAndCites("/Users/waleed/Desktop/citiesdb-final3.csv", provinces2);


        }

        static void testCSVAppend()
        {
            DataReaderWriter.AppendMasjids("/Users/waleed/Projects/NextMasjid.Backend.API/NextMasjid.Backend.API/Data/reportedMasjids_v4.csv", new Masjid[] { new Masjid() { Name = "test", CityID = -1, IsGrandMosque = false, MasjidID = "00000dwds", Location = new GeoPoint(24.11,25.333) } });
        }
        static void testWebIssue()
        {
            var sw = new GeoPoint(24.755929442618097, 46.60904532301554);
            var ne = new GeoPoint(24.85378703767595, 46.79529791402198);

            // todo check for size
            // todo check for zoom level
            int step = 2;

            var diff = ne.Lat - sw.Lat;
            if (diff < 0.05)
                step = 2;
            else if (diff < 0.15)
                step = 10;
            else if (diff < 0.3)
                step = 20;
            else
                step = 50;

            Console.WriteLine(step);
            Console.WriteLine("Continue?");
            Console.ReadLine();

            var scores = Data.DataReaderWriter.ReadScores("/Users/waleed/Projects/NextMasjid.Backend.API/NextMasjid.Backend.API/Data/scores_v6.dat");

            // read first few
            for (int i = 0; i < 1000; i++)
                Console.WriteLine(scores.ElementAt(i).Key.ToString());
            var subScores = Core.Score.SearchAreaFast(scores, sw, ne, step);


         //   return subScores.Select(s => new ScoreModel() { Lat = ((double)s.Key.Item1 / 10000), Lng = ((double)s.Key.Item2 / 10000), Value = s.Value });

        }
    }
}
