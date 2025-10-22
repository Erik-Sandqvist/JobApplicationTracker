using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using JobApplicationTrackerV2.Models;
using JobApplicationTrackerV2.Data;

namespace JobApplicationTrackerV2.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Lägg till JobApplications-tabellen
        public DbSet<JobApplication> JobApplications { get; set; }
    }
}