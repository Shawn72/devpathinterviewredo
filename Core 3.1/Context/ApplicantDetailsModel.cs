using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevInterview._1.Context
{
    public class ApplicantDetailsModel
    {

        public string id { get; set; } //this is the key
        public string user_Id { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string sex { get; set; }
        public string date_of_birth { get; set; }
        public string marital_status { get; set; }
        public string pinfl_number { get; set; }
        //  public int County_Id { get; set; }
        public string county { get; set; }
        public string sub_county { get; set; }
        public string ward { get; set; }
        public string postal_address { get; set; }
        public string physical_address { get; set; }
        public string telephone_number { get; set; }
        public string email { get; set; }

        public List<SelectListItem> List_Counties = new List<SelectListItem>();

    }
}
