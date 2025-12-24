using HelpdeskModel.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskModel.Models
{
    public class Notice
    {
        public long Id { get; set; }
        public long? CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public long? ModifiedById { get; set; }
        public ApplicationUser ModifiedBy { get; set; }
        public long? ApprovedById { get; set; }
        public ApplicationUser ApprovedBy { get; set; }
        public long? RejectedById { get; set; }
        public ApplicationUser RejectedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? RejectedAt { get; set; }
        public ModelStatus Status { get; set; }
        public bool isApproved { get; set; } = false;
        public Department Department { get; set; }
        public string Description { get; set; }

    }
}
