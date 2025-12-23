using Microsoft.AspNetCore.Identity;

namespace HelpdeskModel.Models
{
    public class ApplicationUser : IdentityUser<long>
    {
        public string FullName { get; set; }
    }
}
