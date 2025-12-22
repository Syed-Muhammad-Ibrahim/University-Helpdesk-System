using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Student_Complain_Management_System.Models;

namespace Student_Complain_Management_System.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,long>
    {
        public AppDbContext(DbContextOptions<AppDbContext>options) : base(options)
        {
        }

        
    }
}
