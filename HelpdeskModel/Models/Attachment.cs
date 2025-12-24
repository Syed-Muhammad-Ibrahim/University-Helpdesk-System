using HelpdeskModel.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpdeskModel.Models
{
    public class Attachment
    {
        public long Id { get; set; }
        public long? CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public long? ModifiedById { get; set; }
        public ApplicationUser ModifiedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public ModelStatus Status { get; set; }
        public string FileNmae { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
    }
}
