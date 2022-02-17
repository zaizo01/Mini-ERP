using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiPractices.Entities;

namespace WebApiPractices
{
    public class ApplicationDbContext: IdentityDbContext
    // <User>
    {
        public ApplicationDbContext(DbContextOptions options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

        public DbSet<Department> Department { get; set; }
        public DbSet<JobPosition> JobPosition { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Article> Article { get; set; }

    }
}
