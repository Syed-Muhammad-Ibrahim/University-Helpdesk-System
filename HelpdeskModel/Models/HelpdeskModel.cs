using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskModel.Models
{
    public class Notification
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public ApplicationUser User { get; set; }
        public long ComplainId { get; set; }
        public Complain Complain { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
