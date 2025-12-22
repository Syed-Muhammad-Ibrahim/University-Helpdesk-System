using Microsoft.AspNetCore.Identity;

namespace Student_Complain_Management_System.Models
{
    public class ApplicationUser : IdentityUser<long>
    {
        public string FullName { get; set; }
    }
}
