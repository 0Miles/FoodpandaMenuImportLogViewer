using System;

namespace FoodpandaMenuImportLog.Dto
{
    public class FoodpandaMenuImportLogDto
    {
        public DateTime createdAt { get; set; }
        public string message { get; set; }
        public string menuImportId { get; set; }
        public string status { get; set; }
        public string createdTime
        {
            get
            {
                return createdAt.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
}