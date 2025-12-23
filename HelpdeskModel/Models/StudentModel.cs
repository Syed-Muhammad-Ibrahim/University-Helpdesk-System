using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskModel.Models
{
    public class StudentModel
    {
        [Required(ErrorMessage ="Student Id is Requierd")]
        public long Id { get; set; }

        [Required(ErrorMessage = "Student Name is Requierd")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Student Adress is Requierd")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Student Phone Number is Requierd")]
        [Phone(ErrorMessage ="Invalid Phone Number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Student Section is Requierd")]
        public string Section { get; set; }

        [Required(ErrorMessage = "Student Program is Requierd")]
        public string Program { get; set; }

        [Required(ErrorMessage = "Student Department is Requierd")]
        public string Department {  get; set; }

        public Date


    }
}
