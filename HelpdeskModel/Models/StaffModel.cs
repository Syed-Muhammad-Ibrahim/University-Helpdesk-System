using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskModel.Models
{
    public class StaffModel
    {
        [Required(ErrorMessage = "Staff Id is Requierd")]
        public long StudentId { get; set; }

        [Required(ErrorMessage = "Staff Name is Requierd")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Staff Adress is Requierd")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Staff Phone Number is Requierd")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Staff Department is Requierd")]
        public string Department { get; set; }
    }
}
