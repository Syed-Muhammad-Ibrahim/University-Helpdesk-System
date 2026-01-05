using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskModel.ViewModels
{
    public class NoticeViewModel
    {
        public long Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public long DepartmentId { get; set; }

        public long? AttachmentId { get; set; }
    }
}
