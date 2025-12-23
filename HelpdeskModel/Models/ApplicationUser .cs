using Microsoft.AspNetCore.Identity;

namespace HelpdeskModel.Models
{
    public class ApplicationUser : IdentityUser<long>
    {
        public string FullName { get; set; }

        public string? StudentId { get; set; }
        public string? StaffId { get; set; }
    }
}
