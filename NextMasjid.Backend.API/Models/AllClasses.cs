using System;
namespace NextMasjid.Backend.API.Models
{
  

    public class CityModel
    {
        public string ID { get; set; }
        public string Name { get; set; }

        public double Lat { get; set; }
        public double Lng { get; set; }

    }

    public class ProvinceModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public CityModel[] Cities { get; set; }

    }

    public class MasjidModel
    {
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class ScoreModel
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
        public int Value { get; set; }
    }

    public class EditorChoiceModel
    {
        public string Notes { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }



    public class ScoreDetailedModel
    {


        public double Lat { get; set; }
        public double Lng { get; set; }

        public int Value { get; set; }

        public double MosqueDensity { get; set; }
        public double ExpectedPrayers { get; set; }
        public double PopulationDensity { get; set; }
        public double NearestMosqueDistance { get; set; }

        public string FirstNearestMasjidName { get; set; }
        public string FirstNearestMasjidDistance { get; set; }

        public string SecondNearestMasjidName { get; set; }
        public string SecondNearestMasjidDistance { get; set; }

        public string ThirdNearestMasjidName { get; set; }
        public string ThirdNearestMasjidDistance { get; set; }

    }

  
}



/*
 *  List of Services
 *      GetMosquesByBound       [input sw and ne points]    [result: arrayOf MosqueSimple[string nameEn, string nameAr, string latlng]]
 *      GetScoresByBound        [input sw and ne points]    [result: arrayOf ScoreSimple[string latlng, int value]]
 *      GetProvinceCity         [input: none]               [result: arrayOf ProvinceSimple[string provinceNameAr provinceNameEn, arrayOf CitySimple [int cityID, cityNameAr, cityNameEn, string centreLatLng]]]
 *      GetEditorChoice         [input: none]               [result: arrayOf EditorChoiceSimple [string latlng]]
 *      GetLocationScore        [input: string latlng]      [result: ScoreSimple[string latlng, int value]]
 *      GetLocationScoreDetails [input: string latlng]      [result: ScoreDetailsSimple [string lattlng, int value, int 1, int 2, int 3, int 4, arrayOf MosqueDistanceSimple[string nameEn, string nameAr, string latlng, string distanceEn, string distanceAr]]
 *      
 *      
 *      
 *   Masjids        // 1
 *   Cities         // 1
 *   Scores         // 3
 *   EditorsChoice  // 1
 *   
 */