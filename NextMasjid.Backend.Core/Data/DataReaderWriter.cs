using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using NextMasjid.Backend.Core;

namespace NextMasjid.Backend.Core.Data
{
    public static class DataReaderWriter
    {
        public static void WriteScores(string path, Dictionary<(int, int), int> data)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            BinaryWriter w = new BinaryWriter(fs);

            foreach (var item in data)
            {
                
                w.Write(item.Key.Item1);
                w.Write(item.Key.Item2);
                w.Write((short)item.Value);
                
            }

            w.Flush();
            w.Close();
            fs.Close();

        }

        public static Dictionary<(int, int), int> ReadScores(string path)
        {
            Dictionary<(int, int), int> result = new Dictionary<(int, int), int>();
            //using var fs = new FileStream(path, FileMode.Open);


            //BinaryReader br = new BinaryReader(fs);

            var file = File.ReadAllBytes(path);
            using var s = new MemoryStream(file);

            using BinaryReader br = new BinaryReader(s);

            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                result.Add((br.ReadInt32(), br.ReadInt32()), br.ReadInt16());
            }

            return result;

        }

        public static EditorChoice[] ReadEditorChoices(string path)
        {
            using TextFieldParser parser = new TextFieldParser(path);

            List<EditorChoice> choices = new List<EditorChoice>();

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.ReadLine();
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();

                EditorChoice choice = new EditorChoice()
                {
                    EditorChoiceID = int.Parse(fields[0]),
                    Notes = fields[1],
                };

                var accurate = fields[2];

                accurate = accurate.Replace("$", "\"");
                accurate = accurate.Replace("#", ",");

                var point = System.Text.Json.JsonSerializer.Deserialize<GeoPoint>(accurate);

                choice.Location = point;
                choices.Add(choice);

            }
            return choices.ToArray();
        }

        public static void WriteEditorChoices(EditorChoice[] editorChoices, string path)
        {
            const string comma = ",";

            StringBuilder sb = new StringBuilder();
            //  sb.AppendLine("cityID,nameAr,nameEn,area,population,rankInDesnity,rankInPopulation,points");
            sb.AppendLine("choiceID, notes, location");

            foreach (var choice in editorChoices)
            {

                sb.Append(choice.EditorChoiceID.ToString());

                sb.Append(comma + choice.Notes);

                string s = System.Text.Json.JsonSerializer.Serialize(choice.Location);

                s = s.Replace("\"", "$");
                s = s.Replace(comma, "#");

                sb.Append(comma + s);

                sb.AppendLine();
                
            }
            System.IO.File.WriteAllText(path, sb.ToString());

        }



        public static Province[] ReadProvincesAndCities(string path)
        {
            using TextFieldParser parser = new TextFieldParser(path);

            List<City> cities = new List<City>();

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.ReadLine();
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();

                //  labels.Add(new AnnotationBox()
                City c = new City()
                {
                    CityID = int.Parse(fields[0]),
                    CityNameAr = fields[1],
                    CityNameEn = fields[2],
                    Area = double.Parse(fields[3]),
                    Population = double.Parse(fields[4]),
                    RankInDensity = int.Parse(fields[5]),
                    RankInPopulation = int.Parse(fields[6]),
                    Province = new Province() { ProvinceID = int.Parse(fields[7]), ProvinceNameAr = fields[8], ProvinceNameEn = fields[9] }
                };


                var accurate = fields[10];

                accurate = accurate.Replace("$", "\"");
                accurate = accurate.Replace("#", ",");

                var pols = System.Text.Json.JsonSerializer.Deserialize<GeoPolygon[]>(accurate);
                c.Polygons = pols;
                cities.Add(c);

            }

            List<Province> provinces = new List<Province>();
            foreach(var cx in cities.GroupBy(c=>c.Province.ProvinceID))
            {
                Province province = new Province();
                province.ProvinceID = cx.Key;
                province.ProvinceNameAr = cx.First().Province.ProvinceNameAr;
                province.ProvinceNameEn = cx.First().Province.ProvinceNameEn;
                province.Cities = cx.ToArray();
                provinces.Add(province);
            }
            return provinces.ToArray();
        }

        public static void WriteProvincesAndCites(string path, IEnumerable<Province> provinces)
        {
            const string comma = ",";

            var cities = provinces.SelectMany(p => p.Cities);

            StringBuilder sb = new StringBuilder();
          //  sb.AppendLine("cityID,nameAr,nameEn,area,population,rankInDesnity,rankInPopulation,points");
            sb.AppendLine("cityID,cityNameAr,cityNameEn,area,population,rankInDesnity,rankInPopulation,provinceID,provinceNameAr,provinceNameEn,points");

            int i = 0;
            foreach (var city in cities)
            {

                sb.Append(city.CityID.ToString());// cityID

                sb.Append(comma + city.CityNameAr);// nameAr
                sb.Append(comma + city.CityNameEn);// nameEn
                sb.Append(comma + city.Area);// area
                sb.Append(comma + city.Population);// population
                sb.Append(comma + city.RankInDensity);// rankDen
                sb.Append(comma + city.RankInPopulation);// rankPol
                sb.Append(comma + city.Province.ProvinceID);
                sb.Append(comma + city.Province.ProvinceNameAr);
                sb.Append(comma + city.Province.ProvinceNameEn);

                string s = System.Text.Json.JsonSerializer.Serialize(city.Polygons);

                s = s.Replace("\"", "$");
                s = s.Replace(comma, "#");


                sb.Append(comma + s);

                sb.AppendLine();
                i++;
            }

            System.IO.File.WriteAllText(path, sb.ToString());
        }

        public static Masjid[] ReadMasjids(string path)
        {
            using TextFieldParser parser = new TextFieldParser(path);

            List<Masjid> masjids = new List<Masjid>();

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.ReadLine();
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();

                Masjid m = new Masjid()
                {
                    MasjidID = fields[4],
                    Location = new GeoPoint() { Lat = double.Parse(fields[3]), Lng = double.Parse(fields[2]) },
                    Name = fields[0],
                    IsGrandMosque = false,
                    CityID = int.Parse(fields[5]),
                };

                masjids.Add(m);

            }
            return masjids.ToArray();
        }

        public static void WriteMasjids(string path, IEnumerable<Masjid> masjids)
        {
            const string comma = ",";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Name,Category,Lng,Lat,Code,CityID");

            foreach (var masjid in masjids)
            {

                sb.Append(masjid.Name);
                sb.Append(comma + "1");
                sb.Append(comma + masjid.Location.Lng);
                sb.Append(comma + masjid.Location.Lat);
                sb.Append(comma + masjid.MasjidID);
                sb.Append(comma + masjid.CityID);

                sb.AppendLine();
            }

            System.IO.File.WriteAllText(path, sb.ToString());
        }


        private static object appeentLock = new object();
        public static void AppendMasjids(string path, IEnumerable<Masjid> masjids)
        {
            const string comma = ",";
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine();
            foreach (var masjid in masjids)
            {

                sb.Append(masjid.Name);
                sb.Append(comma + "1");
                sb.Append(comma + masjid.Location.Lng);
                sb.Append(comma + masjid.Location.Lat);
                sb.Append(comma + masjid.MasjidID);
                sb.Append(comma + masjid.CityID);

                sb.AppendLine();
            }
            lock (appeentLock)
            {
                System.IO.File.AppendAllText(path, sb.ToString());
            }
        }



     
      
        //static List<City> KmlToGeofence(string file)
        //{
        //    var result = new List<City>();



        //    //byte[] byteArray = Encoding.ASCII.GetBytes(kmlString);
        //    //MemoryStream stream = new MemoryStream(byteArray);

        //    var doc = ((Document)((Kml)KmlFile.Load(File.OpenRead(file)).Root).Feature);

        //    var rnd = new Random();

        //    int id = 0;

        //    foreach (var docFeature in doc.Features.ToArray())
        //    {
        //        var folder = (Folder)docFeature;
        //        foreach (var f in folder.Features)
        //        {
        //            var city = new City();

        //            var placeMarker = (Placemark)f;

        //            city.CityNameEn = placeMarker.Name.Trim();


        //            if (placeMarker.Geometry is Polygon)
        //            {
        //                var polygon = (Polygon)placeMarker.Geometry;
        //                var boundary = polygon.OuterBoundary;
        //                var ring = boundary.LinearRing;

        //                List<GeoPoint> pnts = new List<GeoPoint>();
        //                foreach (var coor in ring.Coordinates)
        //                {
        //                    pnts.Add(new GeoPoint()
        //                    {
        //                        Lat = coor.Latitude,
        //                        Lng = coor.Longitude
        //                    }); ;

        //                    //city.Points.Add(new Models.Point()
        //                    //{
        //                    //    Id = rnd.Next(1, int.MaxValue),
        //                    //    Latitude = coor.Latitude,
        //                    //    Longitude = coor.Longitude
        //                    //});
        //                }

        //                city.Polygons = new GeoPolygon[] { new GeoPolygon() { Points = pnts.ToArray() } };
        //                pnts.Clear();

        //                if (!result.Exists(c => c.CityNameEn == city.CityNameEn))

        //                {
        //                    city.CityID = id++;

        //                    result.Add(city);
        //                }
        //            }
        //            else
        //            {
        //                foreach (var g in ((MultipleGeometry)placeMarker.Geometry).Geometry)
        //                {

        //                    var polygon = (Polygon)g;
        //                    var boundary = polygon.OuterBoundary;
        //                    var ring = boundary.LinearRing;

        //                    List<GeoPoint> pnts = new List<GeoPoint>();
        //                    foreach (var coor in ring.Coordinates)
        //                    {
        //                        pnts.Add(new GeoPoint()
        //                        {
        //                            Lat = coor.Latitude,
        //                            Lng = coor.Longitude
        //                        }); ;
        //                    }

        //                    city.Polygons = new GeoPolygon[] { new GeoPolygon() { Points = pnts.ToArray() } };
        //                    pnts.Clear();


        //                    if (!result.Exists(c => c.CityNameEn == city.CityNameEn))

        //                    {
        //                        city.CityID = id++;

        //                        result.Add(city);
        //                    }
        //                }
        //            }

        //        }
        //    }

        //    return result;

        //}

        //static City[] GetCities()
        //{

        //    // todo this should be done from Database! 

        //    var reesult = KmlToGeofence("/Users/waleed/Downloads/KSA.kml");
        //    reesult = reesult.Where(r => r.CityNameEn.Contains("populated")).ToList(); // since the file contains duplicate. 

        //    foreach (var city in reesult)
        //    {
        //        List<GeoPoint> points = new List<GeoPoint>();
        //        points.AddRange(city.Polygons[0].Points);

        //        if (points.First().Lat != points.Last().Lat || points.First().Lng != points.Last().Lng)
        //            points.Add(points[0]);

        //        var area = Math.Abs(points.Take(points.Count - 1)
        //           .Select((p, i) => (points[i + 1].Lat - p.Lat) * (points[i + 1].Lng + p.Lng))
        //           .Sum() / 2);

        //        city.Area = area;
        //    }

        //    return reesult.ToArray();
        //}
    }
}
