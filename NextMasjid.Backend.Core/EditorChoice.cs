using System;
namespace NextMasjid.Backend.Core
{
    public class EditorChoice
    {
        public EditorChoice()
        {
       
        }

        public int EditorChoiceID { get; set; }
        public string Notes { get; set; }
        public GeoPoint Location { get; set; }
    }
}
