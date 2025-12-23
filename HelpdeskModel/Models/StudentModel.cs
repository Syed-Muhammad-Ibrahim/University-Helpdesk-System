using HelpdeskModel.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskModel.Models
{
    public class StudentModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long CreatedBy { get; set; }

        public long? ModifiedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public ModelStatus Status { get; set; }

        public ApplicationUser User { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public Department Department { get; set; }


    }
}
