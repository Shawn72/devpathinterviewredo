using DevInterview._1.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevInterview._1.Models
{
    public class AppDBContext : IdentityDbContext
    {
        private readonly DbContextOptions _options;

        public AppDBContext(DbContextOptions options): base(options)
        {
            _options = options; 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ApplicantDetailsModel> Applicant_Details { get; set; }
        public DbSet<UserDetailsModel> User_Details { get; set; }
        public DbSet<HouseHoldModel> Applicant_Householdmembers { get; set; }
        public DbSet<EducationModel> Applicant_Education { get; set; }

    }
}
