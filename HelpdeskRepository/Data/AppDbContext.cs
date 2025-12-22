using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HelpdeskModel.Models;
using Student_Complain_Management_System.Models;

namespace HelpdeskRepository.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,long>
    {
        public AppDbContext(DbContextOptions<AppDbContext>options) : base(options)
        {
        }

        
    }
}
