using HelpdeskModel.BusinessRules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskModel.ViewModels.UpdateViewModels
{
    public class StudentEditViewModel
    {
        public long Id { get; set; }
        public long StudentId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(40, MinimumLength = 4, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(120, ErrorMessage = "Address cannot be longer than {1} characters.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [RegularExpression(@"^(01)[0-9]{9}$", ErrorMessage = "Phone must be a valid number (Ex: 01XXXXXXXXX)")]
        public string Phone { get; set; }

        public long DepartmentId { get; set; }
        public ModelStatus Status { get; set; }
    }
}
