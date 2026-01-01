using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskModel.ViewModels
{
    public class DepartmentViewModel
    {
        public long Id { get; set; }

        [Required]
        [StringLength(80)]
        public string Name { get; set; }
    }
}
