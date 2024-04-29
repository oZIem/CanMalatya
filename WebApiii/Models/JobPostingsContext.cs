using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiii.Models;

namespace WebApiii.Models
{
    public class JobPostingsContext : DbContext
    {
        public DbSet<JobPosting> JobPostings { get; set; }

        public JobPostingsContext(DbContextOptions<JobPostingsContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=(localdb)\\MSSQLLocalDB; Database=jobpostings.db;Trusted_Connection=true");
        }
    }

       
    }




