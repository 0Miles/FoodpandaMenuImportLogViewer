using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodpandaMenuImportLog.Model
{
    public class SettingModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string chinCode { get; set; }
        public string vendorId { get; set; }
        public int limit { get; set; }
    }
}
