using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskModel.ViewModels
{
    public class ComplainViewModel
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public long DepartmentId { get; set; }
        public long? AttachmentId { get; set; }
        public IFormFile? File { get; set; }
    }
}
