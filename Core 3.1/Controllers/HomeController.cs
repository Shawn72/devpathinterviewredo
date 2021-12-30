using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DevInterview._1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using Microsoft.AspNetCore.Http;
using DevInterview._1.Context;
using System.Security.Cryptography;

namespace DevInterview._1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private AppDBContext Context { get; }

        public HomeController(ILogger<HomeController> logger, IConfiguration config, AppDBContext _context)
        {
            _logger = logger;
            _configuration = config;
            Context = _context;
        }

        [Authorize]
        public IActionResult Index()
        {
            var userI = HttpContext.Session.GetString("UserId");
            ViewBag.Name = userI;
            if (userI == null) {
                return View("../Account/Login");
            }
            ApplicantDetailsModel subM = new ApplicantDetailsModel();
            List<CountyModel> mycounties = PopulateCountyDDL();
            var ctys = new SelectList(mycounties, "county_id", "county_name");
            subM.List_Counties = ctys.ToList();
            return View(subM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //
        public IActionResult County()
        {
            ApplicantDetailsModel subM = new ApplicantDetailsModel();
            List<CountyModel> mycounties = PopulateCountyDDL();
            var ctys = new SelectList(mycounties, "county_id", "county_name");
            subM.List_Counties = ctys.ToList();
            return View(subM);
        }

        public List<CountyModel> PopulateCountyDDL()
        {
            List<CountyModel> counties = new List<CountyModel>();

            string sqlConstr = _configuration.GetConnectionString("ConnectionStr");

            using (SqlConnection con = new SqlConnection(sqlConstr))
            {
                SqlCommand cmd = new SqlCommand("GetAllCounties", con);
                cmd.Connection = con; //initialize connection
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sd = new SqlDataAdapter(cmd);

                con.Open();

                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        counties.Add(new CountyModel
                        {
                            county_id = sdr["id"].ToString(),
                            county_name = sdr["county_name"].ToString()
                        });
                    }
                }
                con.Close();
            }
            return counties;
        }
        public List<SubCountyModel> GetAllSubcounties()
        {
            List<SubCountyModel> subcounties = new List<SubCountyModel>();
            string sqlConstr = _configuration.GetConnectionString("ConnectionStr");

            using (SqlConnection con = new SqlConnection(sqlConstr))
            {
                string query = "SELECT id, county_id, subcounty_name FROM [dbo].[Subcounties]";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con; //initialize connection
                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            subcounties.Add(new SubCountyModel
                            {
                                Sub_Id = Convert.ToInt32(sdr["id"]),
                                County_Id = Convert.ToInt32(sdr["county_id"]),
                                Subcounty_Name = sdr["subcounty_name"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return subcounties;
        }

        public List<WardsModel> GetAllWards()
        {
            List<WardsModel> wards = new List<WardsModel>();
            string sqlConstr = _configuration.GetConnectionString("ConnectionStr");

            using (SqlConnection con = new SqlConnection(sqlConstr))
            {
                string query = "SELECT id, subcounty_id, ward_name FROM [dbo].[Wards]";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con; //initialize connection
                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            wards.Add(new WardsModel
                            {
                                id = Convert.ToInt32(sdr["id"]),
                                subcounty_id = Convert.ToInt32(sdr["subcounty_id"]),
                                ward_name = sdr["ward_name"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return wards;
        }


        //acts like a small API
        public JsonResult GetSubcounties(int CountyID)
        {
            //GET STATE LIST FROM JSON OR DB AS PER YOUR REQUIREMENT

            //get Json formed from SQL data
            IEnumerable<SubCountyModel> subsList = GetAllSubcounties();

            //use the items to populate dropdownlist
            List<SelectListItem> subcountylist = new List<SelectListItem>();
            subsList.Where(w => w.County_Id.Equals(CountyID)).ToList().ForEach(f =>
            {
                subcountylist.Add(new SelectListItem()
                {
                    Text = f.Subcounty_Name,
                    Value = f.Sub_Id.ToString()
                });
            });
            return Json(subcountylist);
        }

        public JsonResult GetWards(int SubCountyID)
        {
            //GET STATE LIST FROM JSON OR DB AS PER YOUR REQUIREMENT

            //get Json formed from SQL data
            IEnumerable<WardsModel> subsList = GetAllWards();

            //use the items to populate dropdownlist
            List<SelectListItem> wardslist = new List<SelectListItem>();
            subsList.Where(w => w.subcounty_id.Equals(SubCountyID)).ToList().ForEach(f =>
            {
                wardslist.Add(new SelectListItem()
                {
                    Text = f.ward_name,
                    Value = f.id.ToString()
                });
            });
            return Json(wardslist);
        }


        [HttpPost]
        public IActionResult InsertDetails(ApplicantDetailsModel appModel)
        {
            int res = 0;
            try
            {
                //get id
                string sqlConstr = _configuration.GetConnectionString("ConnectionStr");
                var usernameP = HttpContext.Session.GetString("UserId");
                var userId = (dynamic)null;
                using (SqlConnection con = new SqlConnection(sqlConstr))
                {
                    string query = "SELECT id, UserName FROM [dbo].[AspNetUsers] Where UserName ='" + usernameP + "'";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con; //initialize connection
                        con.Open();

                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                userId = sdr["id"].ToString();
                            }
                        }
                        con.Close();
                    }
                }
                appModel.user_Id = userId;
                appModel.id = appModel.pinfl_number; //use pinfl number as primary key
                this.Context.Applicant_Details.Add(appModel);
                res = Context.SaveChanges();
            }
            catch {
                TempData["Error"] = true;
            }

            if (res > 0) {
                TempData["Success"] = true;
                ViewBag.Success = TempData["Success"];
            }
            else {
                TempData["Error"] = true; 
            }

             return View("Message");
        }
        public IActionResult Message()
        {         
            return View();
        }

        public IActionResult HouseHoldMembers() {
            return View();
        }

        [HttpPost]
        public IActionResult InsertHouseHoldDetails(HouseHoldModel appModel)
        {
            int res = 0;
            try
            {
                //get id
                string sqlConstr = _configuration.GetConnectionString("ConnectionStr");
                var usernameP = HttpContext.Session.GetString("UserId");
                var userId = (dynamic)null;
                using (SqlConnection con = new SqlConnection(sqlConstr))
                {
                    string query = "SELECT id, UserName FROM [dbo].[AspNetUsers] Where UserName ='" + usernameP + "'";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con; //initialize connection
                        con.Open();

                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                userId = sdr["id"].ToString();
                            }
                        }
                        con.Close();
                    }
                }
                var bytes = new byte[10];
                var idUnq = RandomNumberGenerator.Create();
                idUnq.GetBytes(bytes);
                uint random = BitConverter.ToUInt32(bytes, 0) % 100000000;
                appModel.user_id = userId;
                appModel.id = random.ToString(); //use pinfl number as primary key
                this.Context.Applicant_Householdmembers.Add(appModel);
                res = Context.SaveChanges();                
            }
            catch(Exception ex)
            {
                TempData["Error"] = true;
            }

            if (res > 0)
            {
                TempData["Success"] = true;
                ViewBag.Success = TempData["Success"];
            }
            else
            {
                TempData["Error"] = true;
            }

            return View("Message");
        }

        public IActionResult Education() {
            return View();
        }

        [HttpPost]
        public IActionResult InsertEducation(EducationModel appModel)
        {
            int res = 0;
            try
            {
                //get id first
                string sqlConstr = _configuration.GetConnectionString("ConnectionStr");
                var usernameP = HttpContext.Session.GetString("UserId");
                var userId = (dynamic)null;
                using (SqlConnection con = new SqlConnection(sqlConstr))
                {
                    string query = "SELECT id, UserName FROM [dbo].[AspNetUsers] Where UserName ='" + usernameP + "'";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con; //initialize connection
                        con.Open();

                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                userId = sdr["id"].ToString();
                            }
                        }
                        con.Close();
                    }
                }
                var bytes = new byte[10];
                var idUnq = RandomNumberGenerator.Create();
                idUnq.GetBytes(bytes);
                //generate unique number to use as Primary Key
                uint random = BitConverter.ToUInt32(bytes, 0) % 100000000;
                appModel.user_id = userId;
                appModel.id = random.ToString(); //use pinfl number as primary key

                this.Context.Applicant_Education.Add(appModel);
                res = Context.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["Error"] = true;
            }

            if (res > 0)
            {
                TempData["Success"] = true;
                ViewBag.Success = TempData["Success"];
            }
            else
            {
                TempData["Error"] = true;
            }

            return View("Message");
        }
    }

}
