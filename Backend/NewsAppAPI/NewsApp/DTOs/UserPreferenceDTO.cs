using static NewsApp.Models.Enum;

namespace NewsApp.DTOs
{
    public class UserPreferenceDTO
    {
        public int UserID { get; set; }

        public Dictionary<int, Preference> preferences { get; set; }


    }

    public class UserPreferenceReturnDTO
    {
        public int ID { get; set; } 
        public int UserID { get; set; }

        public int CategoryID { get; set; }

        public Preference preference { get; set; }  


    }
}
