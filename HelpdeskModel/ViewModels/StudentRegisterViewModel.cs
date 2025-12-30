using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskModel.ViewModels
{
    public class StudentRegisterViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(40, MinimumLength = 4, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Password must contain at least one uppercase, one lowercase, and one number.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(120, ErrorMessage = "Address cannot be longer than {1} characters.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [RegularExpression(@"^(01)[0-9]{9}$", ErrorMessage = "Phone must be a valid number (Ex: 01XXXXXXXXX)")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "Please select a valid Department.")]
        public long DepartmentId { get; set; }
    }
}
