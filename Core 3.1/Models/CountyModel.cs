using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevInterview._1.Models
{
    public class CountyModel
    {
        public string county_id { get; set; }
        public string county_name { get; set; }
    }

    public class SubCountyModel
    {
        public int Sub_Id { get; set; }
        public string Subcounty_Name { get; set; }
        public int County_Id { get; set; }

        public List<SelectListItem> List_Counties = new List<SelectListItem>();
    }

    public class WardsModel
    {
        public int id { get; set; }
        public int subcounty_id { get; set; }
        public string ward_name { get; set; }
    }


}
