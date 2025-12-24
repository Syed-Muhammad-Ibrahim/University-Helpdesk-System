using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HelpdeskModel.Models;

namespace HelpdeskRepository.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,long>
    {
        public AppDbContext(DbContextOptions<AppDbContext>options) : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Complain> Complains { get; set; }
    }
}
