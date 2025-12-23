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
        public DbSet<StudentModel> Students { get; set; }
    }
}
